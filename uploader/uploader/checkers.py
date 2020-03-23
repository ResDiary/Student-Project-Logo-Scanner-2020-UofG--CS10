import json
import os
import shutil

from PIL import Image

from .hashing import create_image_hashes, hash_image
from .resdiary import get_restaurants, get_images, save_images


def check_for_new_restaurants(headers, write_file):
    """
        **Returns a list of new restaurants**

        This function queries the API for all available restaurants, and
        compares it against the previously available restaurants. It then
        returns the new restaurants in the form of a list.

    :param headers: headers for communication with API
    :type headers: dict
    :param write_file: existing_files.txt file for comparison
    :type write_file: str
    :return: list of new restaurants
    :rtype: list
    """

    restaurants = get_restaurants(headers)
    existing_restaurants = []
    with open(write_file, 'r') as file:
        for line in file:
            existing_restaurants.append(line.rstrip('\n'))
    new_restaurants = list(
        set(restaurants).difference(set(existing_restaurants)))
    print("Found new restaurants: ", new_restaurants)
    return new_restaurants


def check_for_new_logos(path, new_hashes_path, headers, restaurant_file,
                        hash_file):
    """
        **Returns a list of restaurants on the database with new logos**

        This function checks for restaurants that have changed their logo by
        comparing the average hash of the image currently given by the API to
        the one stored in the hash_file. It returns a list of changed
        images and a list of the old images.

    :param path: location to save possible new logos
    :type path: str
    :param new_hashes_path: location to save hashes of logos to compare
    :type new_hashes_path: str
    :param headers: headers for communication with API
    :type headers: dict
    :param restaurant_file: existing restaurants file
    :type restaurant_file: str
    :param hash_file: existing hashes file
    :type hash_file: str
    :return: list of changed images, and a list of old images
    :rtype: list, list
    """

    existing_restaurants = []
    with open(os.getcwd() + restaurant_file, 'r') as file:
        for line in file:
            if line.strip():
                existing_restaurants.append(line.rstrip('\n'))
    images = get_images(existing_restaurants, headers)
    save_images(images, path)
    create_image_hashes(path, new_hashes_path)

    with open(os.getcwd() + new_hashes_path, 'r') as file:
        new_hashes = json.load(file)
    with open(os.getcwd() + hash_file, 'r') as file:
        old_hashes = json.load(file)

    changed_images = []
    old_images = []
    for key in old_hashes.keys():
        if key not in new_hashes.keys():
            new_key = list(new_hashes.keys())[
                list(new_hashes.values()).index(old_hashes[key])]
            changed_images.append(new_key)
            old_images.append(key)

    hash_file = os.getcwd() + hash_file
    new_hashes_path = os.getcwd() + new_hashes_path
    os.remove(hash_file)
    os.rename(new_hashes_path, hash_file)
    shutil.rmtree(os.getcwd() + path)
    print("Found changed logos: ", changed_images)
    return changed_images, old_images


def check_for_new_branches(path, hash_file):
    """
        **Returns a list of new branches**

        This function checks for new restaurants that share a logo with
        existing restaurants, and are thus part of a branch with those
        restaurants. It returns a list of the new branches and updates the
        hash_file to be correct and updated.

    :param path: location of logos to be checked
    :type path: str
    :param hash_file: hash file used to compare and update
    :type hash_file: str
    :return: a list of the new branches
    :rtype: list
    """

    new_branches = []
    with open(hash_file, 'r') as file:
        hashes = json.load(file)

    directory = os.getcwd() + path
    for filename in os.listdir(directory):
        image_hash = hash_image(Image.open(directory + filename))
        if image_hash in hashes:
            hashes[image_hash] += filename
            new_branches.append(image_hash)
            os.remove(directory + filename)
    with open(hash_file, 'w') as file:
        file.write(json.dumps(hashes))
    print("Found new branches: ", new_branches)
    return new_branches
