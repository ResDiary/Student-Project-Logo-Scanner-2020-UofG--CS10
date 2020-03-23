import json
import os
import shutil

from PIL import Image


def move_images(source_path, dest_path):
    """
        **Function for moving logos to a new directory**

        This function is a helper function for moving the contents of one
        directory to another specified pre-existing directory.

    :param source_path: current location of the logos
    :type source_path: str
    :param dest_path: new destination of the logos
    :type dest_path: str
    """
    logos = os.listdir(source_path)
    for logo in logos:
        try:
            os.remove(dest_path + logo)
        except OSError:
            pass
        shutil.move(source_path + logo, dest_path)
    os.rmdir(source_path)


def convert(path):
    """
        **Converts png files in a directory to jpg**

        This function scans a directory for png images. If one is found, it is
        converted to jpg and saved as a new file, with the old png file being
        removed.

    :param path: directory to be scanned
    :type path: str
    """
    directory = os.getcwd() + path
    for filename in os.listdir(directory):
        if filename.endswith(".png"):
            old_image = Image.open(directory + filename)
            rgb_im = old_image.convert('RGB')
            new_name = filename[:-4] + ".jpg"
            rgb_im.save(directory + new_name)
            os.remove(directory + filename)


def rename_images(path, hash_file):
    """
        **Renames images to their average hash value**

        This function scans a directory of images, and renames them to their
        corresponding average hash located in the hash_file.

    :param path: directory of images to rename
    :type path: str
    :param hash_file: hash file containing hashes mapped to names
    :type hash_file: str
    """
    directory = os.getcwd() + path
    with open(hash_file, 'r') as file:
        hashes = json.load(file)
    for filename in os.listdir(directory):
        name = filename.split(".")[0]
        image_hash = (list(hashes.keys())[list(hashes.values()).index(name)])
        os.rename((directory + filename), (directory + image_hash))


def save_restaurant_names(restaurants):
    """
        **Saves a file containing the current restaurant names**

        This function takes a list of restaurant microsite names and saves
        them to a text file.

    :param restaurants: a list of restaurant microsite names
    :type restaurants: list
    """
    path = os.getcwd() + "/existing_restaurants.txt"
    with open(path, "a+") as file:
        for restaurant in restaurants:
            file.write("%s\n" % restaurant)
