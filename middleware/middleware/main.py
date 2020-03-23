"""
.. module:: main
   :synopsis: All endpoints of the middleware component are defined here
.. moduleauthor::  Daniel Hislop, Swetank Poddar


"""

from math import sin, cos, sqrt, atan2, radians
from datetime import datetime
from time import time
import json
import os
from fastapi import FastAPI, HTTPException
from starlette.requests import Request
import requests


from app.resdiary_token import refresh_token
from app.models import RequestStructure, ResponseStructure

app = FastAPI()

RESDIARY_BOOKING_BASE_URL = "https://www.resdiary.com/Restaurant/"

RESDIARY_API_BASE_URL = "https://api.rdbranch.com/api/ConsumerApi/v1/"

DEFAULT_PARTY_SIZE = 2

DENY_REQUEST_THRESHOLD = 3

connections = {}

cuisine_types = {}

restaurant_objects = {}


def get_header():
    """
        **Get Authorisation Token for Request Header**

        This function checks if the environment variable authorisation token exists.
        If it does not then a call is made to the refresh token function in resdiary_token.py.
        The authorisation token is then added to the headers for later use.

    :return: Request Headers

    """

    if "RESDIARYAUTH" not in os.environ:
        print("Generating the first token")
        refresh_token()

    headers = {
        'Content-Type': "application/json",
        'Authorization': "Bearer %s" % (os.environ['RESDIARYAUTH'])
    }

    return headers


@app.post('/request/', response_model=ResponseStructure)
async def microsite_request(request_info: RequestStructure, request: Request):
    """
        **Determine whether request should be served or not**

        The timestamp of the current request is checked against the last timestamp for the same ip in the connection dictionary.
        If elapsed time between the last request and current request is within a threshold of 3 seconds, the request will not be served.
        Otherwise, we update the timestamp in the dictionary to the current timestamp.
        We then extract the desired info from the request, and call the process_request function.

    :param: request_info
    :param: request

    """
    ip = request.client.host
    received_at = time()
    if ip in connections and received_at - connections.get(ip) < DENY_REQUEST_THRESHOLD:
        raise HTTPException(status_code=429, detail="Please wait before requesting again.")
    connections.update({ip: received_at})

    microsites = request_info.microsites
    u_lat = radians(request_info.latitude)
    u_lon = radians(request_info.longitude)

    return await process_request(microsites, u_lat, u_lon)


async def process_request(microsites, u_lat, u_lon):
    """
        **Processes request to determine restuarant closest to user**

        We iterate over the list of microsites, calling the get_restaurant_object function with the current microsite as parameter.
        This will obtain the restaurant information from ResDiary's API. We then determine the distance between the user
        and the current restaurant by making a call to the distance_to_user function. After iterating through the list
        and comparing the current distance with the saved closest distance, we will have obtained the closest restaurant.
        If we only have one microsite, that restaurant is the cloesst.
        If the distance between a user and restaurant meets a threshold of <50 metres then we set that restaurant to the closest and stop iterating through the list.

        After obtaining the closest restaurant we make a call to the get_slots_with_promos function to obtain timeslots and promotions.
        We then add these timeslots to the restaurant object and return the object to the user.

    :param microsites: List of microsites
    :type microsites: list
    :param u_lat: User Latitude
    :type u_lat: float
    :param u_lon: User Longitude
    :type u_lon: float

    :return: Closest restaurant to the user
    :rtype: Restaurant Object

    """
    number_of_microsites = len(microsites)

    closest = None
    closest_microsite = None

    for index, microsite in enumerate(microsites):

        restaurant = await get_restaurant_object(microsite)

        r_lat = radians(restaurant['address']['latitude'])
        r_lon = radians(restaurant['address']['longitude'])

        distance = distance_to_user(r_lat, r_lon, u_lat, u_lon)

        if (number_of_microsites == 1 or distance < 50):
            closest = restaurant
            closest_microsite = microsite
            break

        if index == 0:
            best_distance = distance
            closest = restaurant
            closest_microsite = microsite
        elif distance < best_distance:
            best_distance = distance
            closest = restaurant
            closest_microsite = microsite

    # Update the slots promotions of the closest restaurant
    closest['slots'] = get_slots_with_promos(closest_microsite)

    return closest


# Haversine formula sourced from
# https://stackoverflow.com/questions/19412462/getting-distance-between-two-points-based-on-latitude-longitude
def distance_to_user(r_lat, r_lon, u_lat, u_lon):
    """
        **Calculates distance between user and restaurant**

        Uses the Haversine formula for great-circle distance to calculate the distance between the user and restaurant.
        Further information can be found at:
        `Python Implementation <https://stackoverflow.com/questions/19412462/getting-distance-between-two-points-based-on-latitude-longitude>`_.
        `Wikipedia <https://en.wikipedia.org/wiki/Haversine_formula>`_.

    :param r_lat: Restaurant Latitude
    :type r_lat: float
    :param r_lon: Restaurant Longitude
    :type r_lon: float
    :param u_lat: User Latitude
    :type u_lat: float
    :param u_lon: Restaurant Longitude
    :type u_lon: float
    :return: Distance between user and restaurant
    :rtype: float

    """
    radius = 6371000
    d_lat = r_lat - u_lat
    d_lon = r_lon - u_lon

    a = sin(d_lat / 2) ** 2 + cos(r_lat) * cos(u_lat) * sin(d_lon / 2) ** 2
    c = 2 * atan2(sqrt(a), sqrt(1 - a))
    distance = radius * c
    return distance


def get_data_about_restauarant(microsite):
    """
        **Gets restaurant summary information from ResDiary API**

        Defines a url endpoint of the ResDiary API.
        Makes a call to the process_resdiary_request function with the request method and previously defined url as parameters.
        The process_residary_request will return the response from the API.

    :param microsite: Single restaurant microsite
    :type microsites: str
    :return: Restaurant Summary Information
    :rtype: dict

    """
    url = RESDIARY_API_BASE_URL + "Restaurant/" + microsite

    response = process_resdiary_request("GET", url)

    return json.loads(response)


def get_booking_url(microsite):
    """
        **Get Booking URL for restaurant**

        Creates the booking URL for a provided restaurant. API endpoint requires that a party size is specified, this is
        currently defaulted to 2.

    :param microsite: Single restaurant microsite
    :type microsite: float
    :return: Booking URL
    :rtype: HttpUrl

    """
    return RESDIARY_BOOKING_BASE_URL + microsite + '/Book/Customer?covers=' + str(
        DEFAULT_PARTY_SIZE) + '&bookingDateTime='


def refresh_cuisines():
    """
        **Gets cuisine types from ResDiary API**

        Defines a URL endpoint of the ResDiary API.
        Makes a call to the process_resdiary_request function with the request method and previously defined url as parameters.
        The process_residary_request will return the response from the API.
        Iterate over the list of cuisine types and add them to the cuisine dictionary. Mapping an integer to the cuisine name.

    """
    url = RESDIARY_API_BASE_URL + "CuisineTypes"

    response = process_resdiary_request("GET", url)
    cuisine_data = json.loads(response)
    for cuisine in cuisine_data['Values']:
        cuisine_types.update({cuisine['ConstantValue']: cuisine['Name']})


def get_cuisine_types(cuisines_required):
    """
        **Gets names of cuisine types for restaurant**

        This function takes a list of integers which correspond to names of cuisine types. It accesses the cuisines_types
        dictionary using the list of integers as keys. If the keys are found then the appropriate values are appended to
        the cuisines list. If there is a key error then we call the refresh_cuisines function to update our dictionary,
        and access the updated dictionary for cuisine types as previous.

    :param cuisines_required: List of integers which map to cuisine types
    :type cuisines_required: list
    :return: Cuisine Types of Restaurant
    :rtype: list

    """
    cuisines = []
    try:
        cuisines = [cuisine_types[cuisine_id] for cuisine_id in cuisines_required]
    except KeyError:
        # New cuisine refresh the cuisine list
        refresh_cuisines()
        cuisines = [cuisine_types[cuisine_id] for cuisine_id in cuisines_required]

    return cuisines


def get_slots_with_promos(microsite):
    """
        **Gets the next 3 available timeslots for a restaurant and the promotions available**

        Perform a call to the timeslots and promotions endpoint of the ResDiary API.
        We access the JSON returned from the API and obtain all available promotions.
        We slice to obtain the first 3 timeslots, we split on the character ("."). Timeslots are in ISO 8601 format.
        We then obtain the names of the promotions available at each timeslot. This is done by accessing the promotions
        dictionary, where the integer value of the promotion at a timeslot is a key.


    :param microsite: Single restaurant microsite
    :type microsite: str
    :return: Next 3 available timeslots with promotions at those timeslots
    :rtype: list of dicts

    """
    timeslots = []

    visit_time = str(datetime.now()).replace(" ", "T").split(".", 1)[0]

    url = RESDIARY_API_BASE_URL + "Restaurant/" + microsite + "/AvailabilitySearch"

    payload = {'VisitDate': visit_time, 'PartySize': DEFAULT_PARTY_SIZE}

    response = process_resdiary_request("POST", url, payload)

    slots = json.loads(response)

    promotions = {}
    for promotion in slots['Promotions']:
        promotions[promotion['Id']] = promotion['Name']

    for slot in slots['TimeSlots'][:3]:
        current_slot = {"time": slot['TimeSlot'].split(".")[0], "promotions": []}

        for active_promotion in slot['AvailablePromotions']:
            current_slot['promotions'].append(promotions[active_promotion['Id']])

        timeslots.append(current_slot)

    return timeslots


def create_restaurant_object(microsite):
    """
        **Creates Restauarant Object to be sent to user**

        Function takes a provided microsite and creates the restauarant object from it.
        Appropriate call are made to obtain summary information, cuisine types and booking_url.

    :param microsite: Single restaurant microsite
    :return: Restauraunt Object without timeslots
    :rtype: Restaurant Object

    """
    response = get_data_about_restauarant(microsite)
    cuisines = get_cuisine_types(response['CuisineTypes'])
    booking_url = get_booking_url(microsite)

    restaurant_info = {}

    restaurant_info['microsite'] = microsite
    restaurant_info['name'] = response['Name']

    restaurant_info['logoUrl'] = response['MainImage']['Url']

    restaurant_info['pricePoint'] = response['PricePoint']

    restaurant_info['cuisineTypes'] = cuisines

    restaurant_info['reviews'] = {}
    restaurant_info['reviews']['average'] = response['ReviewSummary']['AverageReviewScore']
    restaurant_info['reviews']['count'] = response['ReviewSummary']['NumberOfReviews']

    if len(response['Menus']) > 0:
        restaurant_info['menuUrl'] = response['Menus'][0]['StorageUrl']
    else:
        restaurant_info['menuUrl'] = None

    restaurant_info['address'] = {}

    restaurant_info['address']['latitude'] = response['Address']['Latitude']
    restaurant_info['address']['longitude'] = response['Address']['Longitude']

    restaurant_info['bookingUrl'] = booking_url

    return restaurant_info


async def get_restaurant_object(microsite):
    """
        **Gets the restaurant object without timeslots**

        If the restaurant object for the provided microsite already exists, as we have cached data, then we return that
        restaurant object. Otherwise, we make a call to the crate_restaurant_object function with the microsite as parameter
        to create the object. We return this newly created restaurant object.

        NB there are no timeslots in this object here. These are added to the object in the process_request function. The
        process_request function is the function which will return a response to the user.

    :param microsite: Single restaurant microsite
    :return: Restauraunt Object without timeslots
    :rtype: Restaurant Object

    """
    if microsite not in restaurant_objects:
        restaurant_objects[microsite] = create_restaurant_object(microsite)

    return restaurant_objects[microsite]


def purge_data():
    """
        **Removes all cached data**

        Function will be called every 24 hours, resulting in the deletion of any cached data. Dictionaries are set to empty.

    """
    global connections
    global cuisine_types
    global restaurant_objects

    print("Removing cached restaurant/connections/cuisinies")

    connections = {}

    cuisine_types = {}

    restaurant_objects = {}


def process_resdiary_request(connection_type, url, payload={}):
    """
        **Performs calls to ResDiary API**

        When function is called it will perform a call to ResDiaryAPI. Function is provided the request method, url and payload.
        401 status code indicates an unauthorised request, indicating that the token present in the headers has expired.
        If a 401 status code is encountered, then refresh_token function is called, and the request performed again.

        If a 200 status code is encountered after a 401 status code then we know the authorisation token had expired.
        This token expires every 24 hours, hence a call will be made to remove cached data by calling purge_data function.

        If we exit the while loop, then we know the authorisation token is now valid. So if we encounter a status code
        that isn't 200 then something has gone wrong and we raise an HTTP exception.

    :param connection_type: request method "GET" or "POST"
    :param url: Url of API to call
    :param payload: Data to be delivered in the request
    :return: response from API
    :rtype: str

    """
    payload = json.dumps(payload)
    response = requests.request(connection_type, url, headers=get_header(), data=payload)

    # Keep on trying untill the response code is not 401
    while response.status_code == 401:

        # Refresh the token
        refresh_token()

        # Try feetching data again
        response = requests.request(connection_type, url, headers=get_header(), data=payload)
        # A new token is send after every 24 hours. 
        # If a request was invalid and is valid now
        # it means it's been atleast 24 hours.
        # In this case, the middleware should
        # remove all the cached data
        # (i.e. restraunt, connections and cuisine types)
        if response.status_code == 200:
            purge_data()

    if response.status_code != 200:
        print(response.text)
        raise HTTPException(status_code=response.status_code, detail="Something went wrong")

    return response.text
