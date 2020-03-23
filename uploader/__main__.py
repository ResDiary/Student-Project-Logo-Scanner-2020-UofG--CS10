import json
import os

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

    # Returns a list of new_restaurants
    #
    # Takes as input parameters: headers - headers for communicating with API
    def find_new_restaurants(headers):

        file = 'existing_restaurants.txt'
        new_restaurants_list = check_for_new_restaurants(headers, file)
        new_images = get_images(new_restaurants_list, headers)
        path = "/new/"
        save_images(new_images, path)
        return new_restaurants_list

    # updating the new logos
    def new_logos(headers, path, vws_client):

        changed_logos, old_logos = check_for_new_logos(
            path, "/new_imagehashes.txt", headers, "/existing_restaurants.txt",
            "/imagehashes.txt")
        for logo in old_logos:
            delete(logo, vws_client)
        path = "/updated/"
        changed_images = get_images(changed_logos, headers)
        save_images(changed_images, path)
        with open("imagehashes.txt", "r") as file:
            hashes = json.load(file)
        convert(path)
        upload(path, hashes, vws_client)
        move_images(os.getcwd() + path, os.getcwd() + "/updated/")

    # updating the branches
    def find_new_branches(path, vws_client):

        new_branches = check_for_new_branches(path, "imagehashes.txt")
        with open("imagehashes.txt", 'r') as file:
            hashes = json.load(file)
        update_metadata(new_branches, hashes, vws_client)

    def upload_new_logos(path, vws_client):

        # uploading the new images
        create_image_hashes("/new/", "/new_hashes.txt")
        with open("new_hashes.txt", "r") as file:
            new_hashes = json.load(file)
        convert(path)
        rename_images(path, "new_hashes.txt")
        upload(path, new_hashes, vws_client)
        move_images(os.getcwd() + path, os.getcwd() + "/logos/")
        with open("imagehashes.txt", "r") as file:
            hashes = json.load(file)
        hashes.update(new_hashes)
        with open("imagehashes.txt", "w") as file:
            file.write(json.dumps(hashes))
        os.remove("new_hashes.txt")

    LIST_OF_TARGETS = VWS_CLIENT.list_targets()
    if not LIST_OF_TARGETS:
        print("Database is empty, performing initialisation")
        initialise(VWS_CLIENT, HEADERS)
    else:
        NEW_RESTAURANTS = find_new_restaurants(HEADERS)
        new_logos(HEADERS, "/newLogoChecks/", VWS_CLIENT)
        find_new_branches("/new/", VWS_CLIENT)
        save_restaurant_names(NEW_RESTAURANTS)
        upload_new_logos("/new/", VWS_CLIENT)

    print("Execution over")
