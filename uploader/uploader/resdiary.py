import json
import os

import requests
from PIL import Image

from .hashing import hash_image


def get_token():
    """
        **Generates a token for authenticating communication with the API**

        This function requests a token from the RESDiary API for authenticating
        further communication with it.

    :return: a token for authenticating communicating with the API
    :rtype: str
    """
    url = "https://api.rdbranch.com/api/Jwt/Token"

    payload = json.dumps({
        "Username": os.environ['RESDIARY_EMAIL'],
        "Password": os.environ['RESDIARY_PASS']
    })

    headers = {
        'Content-Type': 'application/json'
    }

    response = requests.request("POST", url, headers=headers, data=payload)
    token = response.text
    return token


def get_restaurants(headers):
    """
        **Returns a list of all available restaurants from the API**

        This function makes a request to the RESDiary API for a list of all
        currently available restaurants, and returns the response as a list.

    :param headers: headers for communication with the API
    :type headers: dict
    :return: a list of all available restaurants
    :rtype: list
    """
    url = "https://api.rdbranch.com/api/ConsumerApi/v1/Restaurants"
    response = requests.request("GET", url, headers=headers)
    restaurants = json.loads(response.text)
    return restaurants


def get_images(restaurants, headers):
    """
        **Returns a dictionary of restaurants and their URLS**

        This function loops through the list of restaurants and queries the
        API for the URL of their logos, and saves them into a dictionary with
        the restaurant microsite name as the key and the logo URL as the value.

    :param restaurants: a list of restaurants to query with
    :type restaurants: list
    :param headers: headers for communication with the API
    :type headers: dict
    :return: A dictionary of restaurant microsite names and their logo URLS
    :rtype: dict
    """
    img_dict = {}
    for i in restaurants:
        url = "https://api.rdbranch.com/api/ConsumerApi/v1/Restaurant/" + i
        response = requests.request("GET", url, headers=headers)
        result = json.loads(response.text)
        image = result["MainImage"]["Url"]
        img_dict[i] = image
    return img_dict


def save_images(img_dict, directory):
    """
        **Saves images to a directory**

        This function saves images to a specified directory. If two images are
        the same, the duplicate image is deleted and their names are combined
        as the title for the one image.

    :param img_dict: a dictionary of restaurant microsite names and URLS
    :type img_dict: dict
    :param directory: the directory to save the images in
    :type directory: str
    """
    path = os.getcwd()
    path += directory
    if not os.path.exists(path):
        os.mkdir(path)
    hashes = {}
    for key, value in img_dict.items():
        if value[-5:] == ".jpeg":
            file = key + value[-5:]
        else:
            file = key + value[-4:]  # ".jpg"
        img = requests.get(value).content
        image_file = os.path.join(path, file)
        with open(image_file, "wb") as file:
            file.write(img)

        # Checks if there is a duplicate image in the directory,
        # if so combine the names into one image and delete the other
        image_hash = hash_image(Image.open(image_file))
        if image_hash in hashes.values():
            old_name = next(
                key for key, value in hashes.items() if value == image_hash)
            new_name = image_file[:-4] + "," + old_name + ".jpg"
            hashes[file] = hashes.pop(old_name)
            os.remove(os.path.join(path, old_name) + ".jpg")
            os.rename(image_file, new_name)
        else:
            hashes[key] = image_hash
