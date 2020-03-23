Getting Started
###############

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. 
See deployment for notes on how to deploy the project on a live system.
There also exists usage notes for interacting with the system as an end user.

Prerequisites
*************

Unity Version: unity3d:2019.2.11f1 or greater

Vuforia Version: 8.6.7 or greater

Python Version: 3.7 or greater

Docker: Latest Version

Installation
************

A step by step series of examples that tell you how to get a development environment running. Clone repository to a directory of your choice. 

The following command will install all required modules.

Ensure you are in (docs) directory and run the shell script:

``installRequirements.sh``

Development Usage
*****************

To create a locally hosted middleware we run the uvicorn web server. (Ensure you are in the middleware/middleware directory). 

``uvicorn main:app --reload``

Requests can be sent via postman to "http://127.0.0.1:8000/request" (using the request schema in wiki). 

Alternatively, requests can be sent through the app. If sending through the app to a locally hosted middleware you must change the following in (app/Assets/Scripts/ResDiary.cs)

Live deployment IP

``using (UnityWebRequest www = new UnityWebRequest("http://138.68.119.131/request/", "POST"))``

To Local Host IP

``using (UnityWebRequest www = new UnityWebRequest("http://127.0.0.1:8000/request/", "POST"))``

The uploader can be run through the following command, in (root directory) this checks for new restaurants and logos, and uploads them.

``python uploader``

**NB For communication between the middleware and API to work, there must be an authorisation token present in the header of each request. 
The middleware will automatically generate this token provided you possess valid login details for**
`ResDiary API <https://login.rdbranch.com/Security/Login?ReturnUrl=%2fSecurity%2fLogin%2fRedirectToDefault>`_ **and add them to the .env file in the root folder.**


Deployment
**********

The middleware and uploader components are deployed in a droplet (London based) on DigitalOcean.
Each service runs in a seperate alpine based docker container, if anything goes wrong then scripts will be restarted. 
The uploader is automatically executed everyday at 00:00 as a cron job (using busybox). Docker maintains a log of all incoming requests.

To create a live deployment ensure you are in the root directory and perform the following command:

``docker-compose up``

If any changes have been made then an updated live deployment can be achieved through the following commands:

``docker-compose up --build``

Usage
*****

The app can be obtained by downloading the release apk, this is all that is needed for usage in a live deployment environment.
The app will scan a logo, get the name and metadata attached to that logo then send all the data over to the deployed middleware which 
will handle communicating with the ResDiary API, and return all the restaurant data to the app. The app will then display this data to the user.