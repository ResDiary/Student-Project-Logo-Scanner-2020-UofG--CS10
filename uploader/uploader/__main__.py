import json
import os
import shutil

from vws import VWS, CloudRecoService

from uploader.checkers import check_for_new_restaurants, check_for_new_logos, \
    check_for_new_branches
from uploader.file_functions import move_images, rename_images, convert, \
    save_restaurant_names
from uploader.hashing import create_image_hashes
from uploader.resdiary import get_token, get_images, save_images
from uploader.vuforia import upload, delete, update_metadata, initialise

if __name__ == '__main__':
    print("Starting Execution")
    TOKEN = get_token()  # Token for authenticating communication with
    # RESDiary API
    BEARER = "Bearer " + TOKEN[1:-1]
    HEADERS = {
        'Authorization': BEARER,
        'User-Agent': "PostmanRuntime/7.19.0",
        'Accept': "*/*",
        'Cache-Control': "no-cache",
        'Postman-Token': "1796ff04-4471-4439-91f1-da48e474fac0,"
                         "709e6f06-6060-4669-80ca-a15e3ed2bb68",
        'Host': "api.rdbranch.com",
        'Accept-Encoding': "gzip, deflate",
        'Connection': "keep-alive",
        'cache-control': "no-cache"
    }

    SERVER_ACCESS_KEY = os.environ['UPLOADER_SERVER_ACCESS_KEY']
    SERVER_SECRET_KEY = os.environ['UPLOADER_SERVER_SECRET_KEY']
    CLIENT_ACCESS_KEY = os.environ['UPLOADER_CLIENT_ACCESS_KEY']
    CLIENT_SECRET_KEY = os.environ['UPLOADER_CLIENT_SECRET_KEY']

    VWS_CLIENT = VWS(
        server_access_key=SERVER_ACCESS_KEY,
        server_secret_key=SERVER_SECRET_KEY,
    )
    CLOUD_RECO_CLIENT = CloudRecoService(
        client_access_key=CLIENT_ACCESS_KEY,
        client_secret_key=CLIENT_SECRET_KEY,
    )

    # checks if the database is empty: if it is, perform the initialisation
    LIST_OF_TARGETS = VWS_CLIENT.list_targets()
    if not LIST_OF_TARGETS:
        shutil.rmtree("/logos/")
        os.remove("existing_restaurants.txt")
        os.remove("imagehashes.txt")
        print("Database is empty, performing initialisation")
        initialise(VWS_CLIENT, HEADERS)
    else:
        # Find the new restaurants
        FILE = 'existing_restaurants.txt'
        NEW_RESTAURANTS_LIST = check_for_new_restaurants(HEADERS, FILE)

        # Get and save the new restaurants to the /new/ directory
        NEW_IMAGES = get_images(NEW_RESTAURANTS_LIST, HEADERS)
        path = "/new/"
        save_images(NEW_IMAGES, path)

        # Find logos that have changed
        CHANGED_LOGOS, OLD_LOGOS = check_for_new_logos(
            "/newLogoChecks/", "/new_imagehashes.txt", HEADERS,
            "/existing_restaurants.txt",
            "/imagehashes.txt")
        # Delete the old logos from the vuforia database
        for logo in OLD_LOGOS:
            delete(logo, VWS_CLIENT)

        # Save the updated logos to the /updated/ directory
        path = "/updated/"
        CHANGED_IMAGES = get_images(CHANGED_LOGOS, HEADERS)
        save_images(CHANGED_IMAGES, path)

        # Upload the new logos to the vuforia database with the correct metadata
        with open("imagehashes.txt", "r") as file:
            hashes = json.load(file)
        convert(path)
        upload(path, hashes, VWS_CLIENT)
        move_images(os.getcwd() + path, os.getcwd() + "/logos/")

        # Find new restaurants that are part of a branch
        NEW_BRANCHES = check_for_new_branches("/new/", "imagehashes.txt")

        # Update the metadata of the logos with a new branch restaurant added
        with open("imagehashes.txt", 'r') as file:
            hashes = json.load(file)
        update_metadata(NEW_BRANCHES, hashes, VWS_CLIENT)

        save_restaurant_names(NEW_RESTAURANTS_LIST)

        # Upload the new images
        create_image_hashes("/new/", "/new_hashes.txt")
        path = "/new/"
        with open("new_hashes.txt", "r") as file:
            NEW_HASHES = json.load(file)
        convert(path)
        rename_images(path, "new_hashes.txt")
        upload(path, NEW_HASHES, VWS_CLIENT)
        # Move the new images into the current /logos/ directory that stores
        # current logos
        move_images(os.getcwd() + path, os.getcwd() + "/logos/")
        with open("imagehashes.txt", "r") as file:
            hashes = json.load(file)
        hashes.update(NEW_HASHES)
        with open("imagehashes.txt", "w") as file:
            file.write(json.dumps(hashes))
        os.remove("new_hashes.txt")

    print("Execution over")
