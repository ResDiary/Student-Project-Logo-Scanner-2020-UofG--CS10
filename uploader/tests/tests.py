import json
import os
import unittest
import time
from shutil import copyfile

import requests
from PIL import Image
from vws import VWS, CloudRecoService

from uploader.checkers import check_for_new_restaurants, check_for_new_logos, \
    check_for_new_branches
from uploader.file_functions import move_images, convert, rename_images, \
    save_restaurant_names
from uploader.hashing import hash_image, create_image_hashes
from uploader.resdiary import get_token, get_restaurants
from uploader.vuforia import upload, delete, update_metadata

os.chdir(os.path.dirname(__file__))

# Tests that token generation from API returns a token
class TestTokenGeneration(unittest.TestCase):

    def test_token_generation(self):
        TOKEN = get_token()
        self.assertIsInstance(TOKEN, str)


TOKEN = get_token()
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


# Tests further communication with the RESDiary API
class TestCommunication(unittest.TestCase):

    # Tests that get_restaurants returns a list of restaurants
    def test_get_restaurants(self):
        restaurant_list = get_restaurants(HEADERS)
        self.assertIsInstance(restaurant_list, list)

    # Testing querying the API for a restaurant returns the correct restaurant
    def test_get_restaurant(self):
        url = "https://api.rdbranch.com/api/ConsumerApi/v1/Restaurant" \
              "/CairncrossCafe"
        response = requests.request("GET", url, headers=HEADERS)
        result = json.loads(response.text)
        self.assertEqual("Cairncross Cafe", result["Name"])

    # Tests that querying the API for an image returns the correct URL
    def test_get_image_url(self):
        url = "https://api.rdbranch.com/api/ConsumerApi" \
              "/v1/Restaurant/CairncrossCafe"
        response = requests.request("GET", url, headers=HEADERS)
        result = json.loads(response.text)
        image = result["MainImage"]["Url"]
        self.assertEqual(image,
                         "https://resdiarybrancheuw.blob.core.windows.net"
                         "/uploads/euw/1763/images/842/img1042.jpg")


# Tests interaction with the vuforia database
class TestVuforia(unittest.TestCase):
    SERVER_ACCESS_KEY = 'a7b8477b4827fbf0e3585e9389fdc4645b517984'
    SERVER_SECRET_KEY = '5bf1a6c4a2f7f6485c4b8fc93c062a435fdd2a7b'
    CLIENT_ACCESS_KEY = '060b5c2d963821b6b4c42fa256952618667a27b4'
    CLIENT_SECRET_KEY = '2874665060e3b707896e661b9a0ef2333b113341'

    # Tests that uploading an image is successful
    def test_upload(self):
        VWS_CLIENT = VWS(
            server_access_key=self.SERVER_ACCESS_KEY,
            server_secret_key=self.SERVER_SECRET_KEY,
        )
        CLOUD_RECO_CLIENT = CloudRecoService(
            client_access_key=self.CLIENT_ACCESS_KEY,
            client_secret_key=self.CLIENT_SECRET_KEY,
        )

        # make sure to delete any images on the database before beginning
        # testing
        delete("FCF0C10608000C0136C830F3F", VWS_CLIENT)

        hashes = {"FCF0C10608000C0136C830F3F": "CairncrossCafe"}
        upload("/testing_basic/", hashes, VWS_CLIENT)
        try:
            a = VWS_CLIENT.list_targets()
            summary = VWS_CLIENT.get_target_record(a[0])
            self.assertEqual(summary.name, "FCF0C10608000C0136C830F3F")
        except:
            time.sleep(10)
            a = VWS_CLIENT.list_targets()
            summary = VWS_CLIENT.get_target_record(a[0])
            self.assertEqual(summary.name, "FCF0C10608000C0136C830F3F")

    def test_uqdate_metadata(self):
        VWS_CLIENT = VWS(
            server_access_key=self.SERVER_ACCESS_KEY,
            server_secret_key=self.SERVER_SECRET_KEY,
        )
        hashes = {"FCF0C10608000C0136C830F3F": "CairncrossCafe, Test"}
        update_metadata(["FCF0C10608000C0136C830F3F"], hashes, VWS_CLIENT)
        a = VWS_CLIENT.list_targets()
        summary = VWS_CLIENT.get_target_record(a[0])
        self.assertEqual(summary.name, "FCF0C10608000C0136C830F3F")

    # Tests that deleteing an image is successful
    def test_vuforia_delete(self):
        VWS_CLIENT = VWS(
            server_access_key=self.SERVER_ACCESS_KEY,
            server_secret_key=self.SERVER_SECRET_KEY,
        )
        delete("FCF0C10608000C0136C830F3F", VWS_CLIENT)
        a = VWS_CLIENT.list_targets()
        self.assertFalse(a)


# Tests the check_new_restaurants function
class TestNewRestaurants(unittest.TestCase):

    def setUp(self):
        path = os.getcwd() + "/testing2/"
        os.mkdir(path)

        # setup existing_restaurants file for check_for_new_restaurants test
        existing_restaurants = ["BrookeHouse", "EatatWintons", "Gilmorehill",
                                "JamesWattWineBar", "KelvinhaughKitchens",
                                "MuranoStreetSocial", "QueenMargaretClub",
                                "Union", "WolfsonsontheGreen"]
        with open(path + "test_existing_restaurants.txt", "w") as file:
            for restaurant in existing_restaurants:
                file.write("%s\n" % restaurant)

    def test_check_new_restaurants(self):
        file = os.getcwd() + "/testing2/test_existing_restaurants.txt"
        new_restaurants = check_for_new_restaurants(HEADERS, file)
        self.assertEqual(new_restaurants[0], "CairncrossCafe")

    def tearDown(self):
        path = os.getcwd() + "/testing2/"
        os.remove(path + "test_existing_restaurants.txt")
        os.rmdir(path)


# Tests the check_for_new_logos function
class TestNewLogos(unittest.TestCase):

    def setUp(self):
        path = os.getcwd() + "/testing_advanced2/"
        os.mkdir(path)

        # setup image_hashes
        restaurants = ["CairncrossCafe", "BrookeHouse", "EatatWintons",
                       "Gilmorehill", "JamesWattWineBar",
                       "KelvinhaughKitchens", "MuranoStreetSocial",
                       "QueenMargaretClub", "Union", "WolfsonsontheGreen"]
        with open(path + "test_restaurantsfile.txt", "w") as file:
            for restaurant in restaurants:
                file.write("%s\n" % restaurant)
        hashes = {"FF916E8936E8157FFFFFF9FFE": "Gilmorehill",
                  "030E1CF0F3CF8F3CF0F3870C": "QueenMargaretClub",
                  "aaaaaa": "CairncrossCafe",
                  "FFF11F0A105040210CF0F1CFC": "EatatWintons",
                  "FFFFFFF702000007FDFFFFFFF": "BrookeHouse",
                  "000E1CF8F7EF1F3CF0F384": "WolfsonsontheGreen",
                  "F8F0C10608020C1176C933FFF": "JamesWattWineBar",
                  "000F3CF063870C0870E103": "Union",
                  "FAE38B1E6A334C0970C187CF1": "MuranoStreetSocial,"
                                               "KelvinhaughKitchens"}
        with open(path + "test_imagehashes.txt", "w") as file:
            file.write(json.dumps(hashes))

    def test_check_for_new_logos(self):
        path = "/testing_advanced2/test/"
        changed_logos, old_logos = check_for_new_logos(path,
                                                       "/testing_advanced2/newhashes.txt",
                                                       HEADERS,
                                                       "/testing_advanced2/test_restaurantsfile.txt",
                                                       "/testing_advanced2/test_imagehashes.txt")

        # self.assertEqual(changed_logos[0], "e7899918008181e7")
        self.assertEqual(changed_logos[0], "FCF0C10608000C0136C830F3F")

    def tearDown(self):
        path = os.getcwd() + "/testing_advanced2/"
        os.remove(path + "test_restaurantsfile.txt")
        os.remove(path + "test_imagehashes.txt")
        os.rmdir(path)


class TestNewBranches(unittest.TestCase):

    def setUp(self):
        hashes = {"FCF0C10608000C0136C830F3F": "CairncrossCafe"}
        with open(os.getcwd() + "/test_branch_hash.txt", "w") as file:
            file.write(json.dumps(hashes))
        os.mkdir(os.getcwd() + "/test_branches/")
        copyfile(
            os.getcwd() + "/testing_basic/FCF0C10608000C0136C830F3F",
            os.getcwd() + "/test_branches/test_branch_image.jpeg")

    def test_check_for_new_branches(self):
        new_branches = check_for_new_branches("/test_branches/",
                                              os.getcwd() +
                                              "/test_branch_hash.txt")
        self.assertEqual(new_branches[0], "FCF0C10608000C0136C830F3F")

    def tearDown(self):
        os.remove(os.getcwd() + "/test_branch_hash.txt")
        os.rmdir(os.getcwd() + "/test_branches")


# Tests the average image hashing functions
class TestImageHashing(unittest.TestCase):

    def test_hash_image(self):
        image_hash = hash_image(Image.open(
            os.getcwd() + "/testing_basic/FCF0C10608000C0136C830F3F"))
        self.assertEqual(image_hash, "FCF0C10608000C0136C830F3F")

    def test_create_image_hashes(self):
        create_image_hashes("/testing_basic/", "/test_hashes.txt")
        with open("test_hashes.txt", "r") as file:
            hashes = json.load(file)
        self.assertEqual(hashes["FCF0C10608000C0136C830F3F"],
                         "FCF0C10608000C0136C830F3F")
        os.remove(os.getcwd() + "/test_hashes.txt")


# Tests the convert file function
class TestConvert(unittest.TestCase):

    def setUp(self):
        os.mkdir(os.getcwd() + "/testing_convert/")
        copyfile(os.getcwd() + "/testing_advanced/testpng.png",
                 os.getcwd() + "/testing_convert/test_image.png")

    def test_convert(self):
        convert("/testing_convert/")
        for file in os.listdir(os.getcwd() + "/testing_convert/"):
            print(file)
            self.assertTrue(file.endswith(".jpg"))

    def tearDown(self):
        os.remove(os.getcwd() + "/testing_convert/test_image.jpg")
        os.rmdir(os.getcwd() + "/testing_convert")


# Tests the save_restaurant_names function
class TestSaveRestaurantNames(unittest.TestCase):

    def test_save_restaurant_names(self):
        save_restaurant_names(["TEST"])
        restaurants = []
        with open(os.getcwd() + "/existing_restaurants.txt", "r") as file:
            for line in file:
                if line.strip():
                    restaurants.append(line.rstrip('\n'))
        self.assertTrue(restaurants[-1], "TEST")

    def tearDown(self):
        with open(os.getcwd() + "/existing_restaurants.txt", "r+") as file:
            lines = file.readlines()
            print(lines)
            lines = lines[:-1]
            print(lines)
        with open(os.getcwd() + "/existing_restaurants.txt", "w") as file:
            for line in lines:
                file.write(line)


# Tests the move_images file function
class TestMoveImages(unittest.TestCase):
    def setUp(self):
        os.mkdir(os.getcwd() + "/test_move_1/")
        os.mkdir(os.getcwd() + "/test_move_2/")
        copyfile(os.getcwd() + "/testing_advanced/testpng.png",
                 os.getcwd() + "/test_move_1/test_image.png")

    def test_move_images(self):
        move_images(os.getcwd() + "/test_move_1/",
                    os.getcwd() + "/test_move_2/")
        self.assertTrue(
            len(os.listdir(os.getcwd() + "/test_move_2/")) == 1)

    def tearDown(self):
        os.remove(os.getcwd() + "/test_move_2/test_image.png")
        os.rmdir(os.getcwd() + "/test_move_2")


class TestRenameImages(unittest.TestCase):

    def setUp(self):
        os.mkdir(os.getcwd() + "/test_rename/")
        copyfile(os.getcwd() + "/testing_advanced/testpng.png",
                 os.getcwd() + "/test_rename/test_image.png")
        hashes = {"AAAAA": "test_image"}
        with open(os.getcwd() + "/test_hash_rename.txt", "w") as file:
            file.write(json.dumps(hashes))

    def test_rename_images(self):
        rename_images("/test_rename/",
                      os.getcwd() + "/test_hash_rename.txt")
        self.assertTrue(
            os.path.exists(os.getcwd() + "/test_rename/AAAAA"))

    def tearDown(self):
        os.remove(os.getcwd() + "/test_rename/AAAAA")
        os.remove(os.getcwd() + "/test_hash_rename.txt")
        os.rmdir(os.getcwd() + "/test_rename")

# class TestUpdater(unittest.TestCase):
#     SERVER_ACCESS_KEY = '678c24bc2a239c0b271e8a7231080f106a04710d'
#     SERVER_SECRET_KEY = '924c0ae67b45666b8c491b89fa29ddcacb2f23a5'
#
#     def test_updater(self):
#         VWS_CLIENT = VWS(
#             server_access_key=self.SERVER_ACCESS_KEY,
#             server_secret_key=self.SERVER_SECRET_KEY,
#         )
#         try:
#             a = find_new_restaurants(HEADERS)
#             find_new_branches(HEADERS, VWS_CLIENT)
#             new_logos(HEADERS, "/newLogoChecks/", VWS_CLIENT)
#             self.assertTrue(1 == 1)
#         except:
#             self.assertTrue(1 == 0)
