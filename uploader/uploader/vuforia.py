import base64
import io
import json
import os

from .file_functions import convert, rename_images, save_restaurant_names
from .hashing import create_image_hashes
from .resdiary import get_restaurants, get_images, \
    save_images


def upload(path, hashes, vws_client):
    """
        **Uploads images to the vuforia database**

        This function uploads images to the vuforia database, attaching the
        list of restaurant microsite names that match this image to the image
        as metadata.

    :param path: directory of images to upload
    :type path: str
    :param hashes: dictionary of hashes and restaurant microsite names
    :type hashes: dict
    :param vws_client: instance for communication with the vuforia web services
    :type vws_client: VWS object
    """
    directory = os.getcwd() + path

    for filename in os.listdir(directory):
        image_file = directory + filename
        with open(image_file, "rb") as my_image_file:
            my_image = io.BytesIO(my_image_file.read())
        metadata = base64.b64encode(bytes(hashes[filename],
                                          'utf-8'))  # set the metadata to
        # the list of microsite names corresponding to the logo

        target_id = vws_client.add_target(
            name=filename,
            width=1,
            image=my_image,
            active_flag=True,
            application_metadata=str(metadata, 'utf-8'),
        )
        print("Uploading", filename, "...")
        vws_client.wait_for_target_processed(target_id=target_id)
        # matching_targets = cloud_reco_client.query(image=my_image)
        print("Successfully uploaded")


def delete(logo, vws_client):
    """
        **Deletes a logo from the vuforia database**

        This function looks for a logo on the vuforia database, and if found,
        deletes it from the database.

    :param logo: the logo to be deleted
    :type logo: str
    :param vws_client: instance for communication with the vuforia web services
    :type vws_client: VWS object
    """
    list_of_targets = vws_client.list_targets()
    for target in list_of_targets:
        summary = vws_client.get_target_record(target)
        if summary.name == logo:
            print("Deleting", summary.name, "...")
            vws_client.delete_target(summary.target_id)
            print("Successfully deleted")


def update_metadata(branches, hashes, vws_client):
    """
        **Updates the metadata of a logo on the vuforia database**

        This function searches for an image on the vuforia database, and
        updates the metadata to include the new restaurant micrsosite names.

    :param branches: a list of logos that need their metadata updated
    :type branches: list
    :param hashes: a dictionary of hashes and restaurant microsite names
    :type hashes: dict
    :param vws_client: instance for communication with the vuforia web services
    :type vws_client: VWS object
    """
    list_of_targets = vws_client.list_targets()
    for branch in branches:
        for target in list_of_targets:
            summary = vws_client.get_target_record(target)
            if summary.name == branch:
                metadata = base64.b64encode(bytes(hashes[branch], 'utf-8'))
                target_id = vws_client.update_target(
                    target_id=summary.target_id,
                    # name=filename,
                    # width=1,
                    # image=my_image,
                    # active_flag=True,
                    application_metadata=str(metadata, 'utf-8'),
                )
                print("Updating", branch, "...")
                vws_client.wait_for_target_processed(
                    target_id=summary.target_id)
                print("Successfully updated")


def initialise(vws_client, headers):
    """
        **Initialises the Vuforia database**

        This function populates the database if it is empty. The database is
        filled with logos with their corresponding microsite names attached
        in a list as metadata.

    :param vws_client: instance for communication with the vuforia web services
    :type vws_client: VWS object
    :param headers: headers for communication with API
    :type headers: dict
    :return:
    """
    restaurants = get_restaurants(headers)
    save_restaurant_names(restaurants)
    images = get_images(restaurants, headers)
    path = "/logos/"  # directory to save logos to
    save_images(images, path)
    create_image_hashes(path, "/imagehashes.txt")
    convert(path)
    rename_images(path, "imagehashes.txt")
    with open("imagehashes.txt", 'r') as file:
        hashes = json.load(file)
    upload(path, hashes, vws_client)
