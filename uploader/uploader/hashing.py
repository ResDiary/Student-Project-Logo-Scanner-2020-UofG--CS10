import json
import os

from PIL import Image


# average image hashing function gotten from
# https://stackoverflow.com/questions/49689550/simple-hash-of-pil-image
def create_image_hashes(path, new_path):
    """
        **Creates a file of average image hashes given a directory**

        This function creates a file of restaurant microsite names and their
        average hash.

    :param path: directory to scan for images
    :type path: str
    :param new_path: location of where to write hashes to
    :type new_path: str
    """
    hashes = {}
    directory = os.getcwd() + path
    for filename in os.listdir(directory):
        image = Image.open(directory + filename)
        image = image.resize((10, 10),
                             Image.ANTIALIAS)  # Resize image for processing
        image = image.convert("L")  # Convert image to greyscale
        pixel_data = list(image.getdata())
        average_pixel = sum(pixel_data) / len(pixel_data)

        # For each pixel, if larger than the average pixel set as 1,
        # if smaller set as 0
        bits = "".join(
            ["1" if (px >= average_pixel) else "0" for px in pixel_data])

        # Create a hexadecimal representation of the bits
        hex_representation = str(hex(int(bits, 2)))[2:][::-1].upper()
        hashes[hex_representation] = filename.split(".")[0]
    with open(os.getcwd() + new_path, "w+") as file:
        file.write(json.dumps(hashes))


# average image hashing function gotten from
# https://stackoverflow.com/questions/49689550/simple-hash-of-pil-image
def hash_image(image):
    """
        **Calculates the average hash of a single image**

        This function creates an average hashing of an image. It resizes the
        image, converts it to greyscale then for each pixel, if its values are
        larger than the average pixel, give it a "1", else give it a "0". This
        is then converted into a hexadecimal representation.

    :param image: image to be hashed
    :type image: str
    :return: the average hash of that image
    :rtype: str
    """
    image = image.resize((10, 10),
                         Image.ANTIALIAS)  # Resize image for processing
    image = image.convert("L")  # Convert image to greyscale
    pixel_data = list(image.getdata())
    average_pixel = sum(pixel_data) / len(pixel_data)

    # For each pixel, if larger than the average pixel set as 1,
    # if smaller set as 0
    bits = "".join(
        ["1" if (px >= average_pixel) else "0" for px in pixel_data])

    # Create a hexadecimal representation of the bits
    hex_representation = str(hex(int(bits, 2)))[2:][::-1].upper()
    return hex_representation
