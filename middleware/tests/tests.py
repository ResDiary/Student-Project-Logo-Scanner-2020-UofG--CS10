import sys
import os


sys.path.insert(0, os.path.abspath("../middleware/middleware"))

from main import app
from starlette.testclient import TestClient
import requests
import json
import datetime
import time

client = TestClient(app)

from main import get_header
from app.resdiary_token import refresh_token

headers = get_header()

# -----------------------------------------------------------------------------------------------------------------------
# Section tests behaviour of calls to ResDiary API

# Performs a GET request to RESDiary API, Method is allowed so expecting status code 200
def test_SummaryCall_GET():
    url = "https://api.rdbranch.com/api/ConsumerApi/v1/Restaurant/CairncrossCafe"
    response = requests.get(url, headers=headers)
    assert response.status_code == 200


# Perform a POST request to RESDIary API. Method not allowed, expecting status code 405
def test_SummaryCall_POST():
    url = "https://api.rdbranch.com/api/ConsumerApi/v1/Restaurant/CairncrossCafe"
    response = requests.post(url, headers=headers)
    assert response.status_code == 405


# Tests that expected restaurant name exists in returned JSON
def test_SummaryCall_restaurant():
    url = "https://api.rdbranch.com/api/ConsumerApi/v1/Restaurant/CairncrossCafe"
    response = requests.get(url, headers=headers)
    restaurant = json.loads(response.text)
    assert restaurant['Name'] == "Cairncross Cafe"


# Sends restaurant name that doesn't exist in API. Expecting 404 status code
def test_SummaryCall_bad_restaurant():
    url = "https://api.rdbranch.com/api/ConsumerApi/v1/Restaurant/CairnCaf"
    response = requests.get(url, headers=headers)
    assert response.status_code == 404


# Perform a request using promotions call. Tests that key 'TimeSlots' exists in the response
def test_promotions_call():
    url = "https://api.rdbranch.com/api/ConsumerApi/v1/Restaurant/CairncrossCafe/AvailabilitySearch"
    visit_time = str(datetime.time()).replace(" ", "T").split(".", 1)[0]
    payload = {'VisitDate': visit_time, 'PartySize': 3}
    response = (requests.post(url, headers=headers, data=json.dumps(payload)))
    response = json.loads(response.text)
    assert 'TimeSlots' in response


# -----------------------------------------------------------------------------------------------------------------------


# Section tests the behaviour of the middleware based on request structure

# Sends a request with request structure conforming to middleware's expectation
def test_request_structure_complete():
    time.sleep(3)
    response = client.post("/request/",
                           json={"microsites": ["CairncrossCafe"], "latitude": 55.8651844, "longitude": -4.2876513})
    assert response.status_code == 200


# Sends request with incorrect microsite data type. str instead of list
def test_request_structure_microsite_type():
    response = client.post("/request/",
                           json={"microsites": "CairncrossCafe", "latitude": 55.8651844, "longitude": -4.2876513})
    assert response.status_code == 422


# Send request with incorrect latitude data type, str instead of float
def test_request_structure_latitude_type():
    response = client.post("/request/",
                           json={"microsites": ["CairncrossCafe"], "latitude": "test", "longitude": -4.2876513})
    assert response.status_code == 422


# Send request with incorrrect longitude data type, str instead of float
def test_request_structure_longitude_type():
    response = client.post("/request/",
                           json={"microsites": ["CairncrossCafe"], "latitude": 55.8651844, "longitude": "test"})
    assert response.status_code == 422


# Send request with missing field
def test_request_structure_missing_field():
    response = client.post("/request/", json={"latitude": 55.8651844, "longitude": -4.2876513})
    assert response.status_code == 422


# Send request with extra field
def test_request_structure_added_field():
    time.sleep(3)
    response = client.post("/request/",
                           json={"microsites": ["CairncrossCafe"], "latitude": 55.8651844, "longitude": -4.2876513,
                                 "extra": "testing"})
    assert response.status_code == 200


# Sends a request with request structure conforming to middleware's expectation
# with a microsite name that doesn't exist in API
def test_bad_microsite():
    time.sleep(3)
    response = client.post("/request/", json={"microsites": ["Cafe"], "latitude": 55.8651844, "longitude": -4.2876513})
    assert response.status_code == 404


# Sends GET request to middleware
def test_bad_request_method():
    response = client.get("/request/",
                          json={"microsites": ["CairncrossCafe"], "latitude": 55.8651844, "longitude": -4.2876513})
    assert response.status_code == 405


# Sends 3 requests in quick succession to determine how middleware handles spam requests
# Middleware calculates time between current request and last request from same IP,
# request is not served if it falls within threshold, returning 429 status code
def test_fast_posting():
    time.sleep(3)
    json = {"microsites": ["Union"], "latitude": 55.8651844, "longitude": -4.2876513}
    response = client.post("/request/", json=json)
    assert response.status_code == 200
    response = client.post("/request/", json=json)
    assert response.status_code == 429
    response = client.post("/request/", json=json)
    assert response.status_code == 429


# ----------------------------------------------------------------------------------------------------------------------

# Section tests the response time of calls based on number of microsites,
# allowing us to see how the middleware can effectively deal with branching

# Sends two request, with a single microsite each time
# Behaviour of middleware is dependant upon number of microsites, not the actual microsite itself
# Each request should take roughly the same time, ie both fall within threshold
def test_single_microsite():
    response_union = client.post("/request/",
                                 json={"microsites": ["Union"], "latitude": 55.8651844, "longitude": -4.2876513})
    response_cairn = client.post("/request/",
                                 json={"microsites": ["CairncrossCafe"], "latitude": 55.8651844,
                                       "longitude": -4.2876513})
    assert response_union.elapsed.total_seconds() < 0.7 and response_cairn.elapsed.total_seconds() < 0.7


# Known microsite "CairncrossCafe" is the closest, and meets the distance threshold of < 50 metres
# Middleware should be able to return restaurant immeditaely if distance threshold met, without
# needing to iterate through list of microsites.
# First response should be faster than second, as closest restaurant in last position of list in first request, whereas
# it is 2nd position of list in 2nd request
def test_distance_threshold():
    client.post("/request/",
                json={"microsites": ["GilmoreHill", "MuranoStreetSocial"],
                      "latitude": 55.8651844,
                      "longitude": -4.2876513})
    start = time.time()
    client.post("/request/",
                json={"microsites": ["Union", "KelvinhaughKitchens", "MuranoStreetSocial",
                                     "GilmoreHill", "CairncrossCafe"], "latitude": 55.8651844,
                      "longitude": -4.2876513})
    time.sleep(3)
    total = time.time() - start
    threshold_start = time.time()
    client.post("/request/",
                json={"microsites": ["CairncrossCafe", "Union",
                                     "KelvinhaughKitchens", "MuranoStreetSocial",
                                     "GilmoreHill"], "latitude": 55.8651844,
                      "longitude": -4.2876513})
    threshold_total = time.time() - threshold_start
    assert threshold_total < total


# -----------------------------------------------------------------------------------------------------------------------
# Section tests the restaurant object returned to the client following a correctly formatted request

# Set up for further tests
# Sends a request with request structure conforming to middleware's expectation
# Middleware returns JSON to client
def response_object_setup():
    response = client.post("/request/",
                           json={"microsites": ["GilmoreHill"], "latitude": 55.8651844,
                                 "longitude": -4.2876513})
    response = json.loads(response.text)
    return response


# Restaurant object is set to the JSON object returned by the middleware
restaurant_object = response_object_setup()


# Checks whether key 'name' exists in object
def test_response_object_name():
    assert 'name' in restaurant_object


# Checks whether key 'logoUrl' exists in object
def test_response_object_logo_url():
    assert 'logoUrl' in restaurant_object


# Checks whether key 'bookingUrl exists in object
def test_response_object_booking_url():
    assert 'bookingUrl' in restaurant_object


# Checks whether key 'menuUrl' exists in object
def test_response_object_menu():
    assert 'menuUrl' in restaurant_object


# Checks whether key 'cuisineTypes' exists in object
def test_response_object_cuisine_types():
    assert 'cuisineTypes' in restaurant_object


# Checks whether key 'reviews' exists in object
def test_response_object_reviews():
    assert 'reviews' in restaurant_object


# Checks whether key 'average' exists in object
def test_response_object_average_review():
    reviews = restaurant_object['reviews']
    assert 'average' in reviews


# Checks whether key 'count' exists in object
def test_response_object_count_reviews():
    reviews = restaurant_object['reviews']
    assert 'count' in reviews


# Checks whether key 'address' exists in object
def test_response_object_address():
    assert 'address' in restaurant_object


# Checks whether key 'latitude' exists in object
def test_response_object_latitude():
    address = restaurant_object['address']
    assert 'latitude' in address


# Checks whether key 'longitude' exists in object
def test_response_object_longitude():
    address = restaurant_object['address']
    assert 'longitude' in address


# Checks whether key 'slots' exists in object
def test_response_object_slots():
    assert 'slots' in restaurant_object
    # Returns boolean indicating whether dictionary is empty
    return bool(restaurant_object['slots'])


dictbool = test_response_object_slots()


# Checks whether key 'time' exists in object
def test_response_object_time():
    # If dictionary not empty, pick first index and assert key exists
    if dictbool:
        slots = restaurant_object['slots'][0]
        assert 'time' in slots
    # Otherwise assert dicitonary is empty
    else:
        assert dictbool == False


# Checks whether key 'promotions' exists in object
def test_response_object_promotions():
    # If dictionary not empty, pick first index and assert key exists
    if dictbool:
        slots = restaurant_object['slots'][0]
        assert 'promotions' in slots
    # Otherwise assert dicitonary is empty
    else:
        assert dictbool == False

# -----------------------------------------------------------------------------------------------------------------------
