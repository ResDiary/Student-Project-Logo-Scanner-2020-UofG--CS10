import json
import os
import requests


def refresh_token():
    """
        **Generates an authorisation token for communication with ResDiaryAPI**

        Performs a call to the authorisation token ResDiary API endpoint using the environment variables username and password.
        A token will be obtained in the API response.
        This token is then set as an environment variable.

    :return: Authorisation token environment variable

    """
    print("Requesting a fresh token")
    url = "https://api.rdbranch.com/api/Jwt/Token"

    credentials = json.dumps({
        "Username": os.environ['RESDIARY_EMAIL'],
        "Password": os.environ['RESDIARY_PASS']
    })

    headers = {
        'Content-Type': 'application/json'
    }

    response = requests.post(url, headers=headers, data=credentials)

    os.environ['RESDIARYAUTH'] = response.text.strip('"')
