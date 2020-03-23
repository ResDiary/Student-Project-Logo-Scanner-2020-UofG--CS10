[![pipeline status](https://stgit.dcs.gla.ac.uk/tp3-2019-cs10/cs10-main/badges/master/pipeline.svg)](https://stgit.dcs.gla.ac.uk/tp3-2019-cs10/cs10-main/commits/master)
[![coverage report](https://stgit.dcs.gla.ac.uk/tp3-2019-cs10/cs10-main/badges/master/coverage.svg)](https://stgit.dcs.gla.ac.uk/tp3-2019-cs10/cs10-main/commits/master)
[![Middleware status](https://img.shields.io/uptimerobot/status/m784450588-8eec7f38cc8668128172b558?label=Middleware%20Status&style=flat)](https://stats.uptimerobot.com/JEXZ0U8q4A)

# Resdiary (CS10 Team Project)

ResDiary is a flat-fee, online reservation system, designed to help hospitality operators run a smooth service. Through ResDiary, restaurants are able to manage bookings more easily, be it through online reservations or processing online payments. ResDiary require a mobile application that can recognise a restaurant logo and in near real-time, display pertinent information about the restaurant to the user. The goal of the application is to increase the dicoverability of ResDiary's restaurants. 

This repository contains the files developed by CS10 for ResDiary.

## Libraries Used
| Component | Licenses |
| ------ | ------ |
| App  | [![Vuforia  - License - Agreement](https://img.shields.io/badge/Vuforia-custom%20license-green)](https://developer.vuforia.com/legal/vuforia-developer-agreement) |
| Middleware | [![PyPI - License - FastAPI](https://img.shields.io/pypi/l/fastapi?label=FastAPI&style=plastic)](https://fastapi.tiangolo.com/) [![PyPI - License - pydantic](https://img.shields.io/pypi/l/pydantic?label=pydantic&style=plastic)](https://pydantic-docs.helpmanual.io/) [![PyPI - License](https://img.shields.io/pypi/l/starlette?label=Starlette&style=plastic)](https://www.starlette.io/) [![PyPI - License - requests](https://img.shields.io/pypi/l/requests?label=requests&style=plastic)](https://requests.readthedocs.io/en/master/) |
| Uploader |  [![PyPI - License - VWS](https://img.shields.io/pypi/l/vws-python?label=Vuforia%20Web%20Services%20%28VWS%29)](https://vws-python.readthedocs.io/en/latest/) [![PyPI - License - PIL](https://img.shields.io/pypi/l/pil?label=PIL)]() [![PyPI - License - requests](https://img.shields.io/pypi/l/requests?label=requests&style=plastic)](https://requests.readthedocs.io/en/master/) | 
---


## Internal Documentation

| Component | Made with  | Download Latest Internal Documentation |
| ------ | ------ | ------ |
| App | [![made-with-doxygen](https://img.shields.io/badge/Made%20with-Doxygen-1f425f.svg)](http://www.doxygen.nl/) | [Download](https://stgit.dcs.gla.ac.uk/tp3-2019-cs10/cs10-main/-/jobs/artifacts/master/download?job=docs-app) |
| Middleware and Uploader | [![made-with-sphinx-doc](https://img.shields.io/badge/Made%20with-Sphinx-1f425f.svg)](https://www.sphinx-doc.org/) | [Download](https://stgit.dcs.gla.ac.uk/tp3-2019-cs10/cs10-main/-/jobs/artifacts/master/download?job=docs-middleware-uploader) |


Documentation of the app component has been created using doxygen. HTML docs can be generated (in docs directory) through the following command:

```
doxygen
```

The generated documentation can be viewed by opening the "index.html" file in the (docs/html directory) in a web browser.

Documentation of middleware and uploader components have been created using sphinx. HTML docs can be generated (in docs directory) through the following command:

```
make html
```

The generated documentation can be viewed by opening the "index.html" file in the (docs/build/html directory) in a web browser.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

There also exists usage notes for interacting with the system as an end user.

### Prerequisites

Unity Version: unity3d:2019.2.11f1 or greater

Vuforia Version: 8.6.7 or greater

Python Version: 3.7 or greater

Docker: Latest Version

### Installation
A step by step series of examples that tell you how to get a development environment running. Clone repository to a directory of your choice. 

The following command will install all required modules.

Ensure you are in (docs) directory and run the shell script:
```
installRequirements.sh
```

Next we run the uvicorn web server. (Ensure you are in the middleware/middleware directory). This creates a locally hosted middleware.

```
uvicorn main:app --reload
```

Requests can be sent via postman to "http://127.0.0.1:8000/request" (using the request schema in wiki), or through the app. If sending through the app to a locally hosted middleware you must change the following in (app/Assets/Scripts/ResDiary.cs)

Live deployment IP

```
using (UnityWebRequest www = new UnityWebRequest("http://138.68.119.131/request/", "POST"))
```

to

Local Host IP

```
using (UnityWebRequest www = new UnityWebRequest("http://127.0.0.1:8000/request/", "POST"))
```

The logo updater can be run through the following command, in (root directory) this checks for new restaurants and logos, and uploads them. 

```
python uploader
```

**NB For communication between the middleware and API to work, there must be an authorisation token present in the header of each request. The middleware will automatically generate this token provided you possess valid login details for [ResDiary API](https://login.rdbranch.com/Security/Login?ReturnUrl=%2fSecurity%2fLogin%2fRedirectToDefault) and add them to the .env file in the root folder.**

## Running Tests

A series of automated tests can be indivudally run to test each component of the system.

### App

Two types of tests are run through Unity’s testing solution, Unity Test Runner, PlayMode and EditorMode tests. PlayMode tests are tests which require a scene to be built and running in order to complete the test. EditorMode tests are tests that can be run on the scripts independently.

The tests can be run through in the command line through the following commands (in app directory) (where unity.exe is the path to unity installation):

```
(unity.exe) - projectPath ... -runTests -testPlatform playmode -logfile test-result -batchmode
```

```
(unity.exe) - projectPath ... -runTests -testPlatform editormode -logfile test-result -batchmode
```

Alternatively, these tests can be run in unity itself after opening the project. 

### Middleware 

Creation of a single python file named "tests.py" containing individual test functions. These test functions have been implemented using pytest in conjunction with coverage. Coverage generates a report following test execution, readable in the terminal or in html format, of code coverage.
Test functions can be executed using either pytest or coverage through the following commands: (in middleware directory)

```
pytest tests/tests.py
```

```
coverage run -m pytest tests/tests.py
```

Or alternatively run the shell script:

```
initiateMiddlewareTest.sh
```

The generated coverage html report can be viewed by opening the "index.html" file in the (middleware/htmlcov directory) in a web browser.


### Vuforia Upload Script

Creation of a single python file names "test.py" containing individual test functions. These test functions have been implemented using unittest in conjunction with coverage. Test functions can be executed using either of the following commands: (in uploader directory)

```
python -m unittest tests/tests.py
```

```
coverage run --source=. -m unittest tests/test.py
```

Or alternatively run the shell script:

```
startUploaderTest.sh
```

The generated coverage html report can be viewed by opening the "index.html" file in the (uploader/htmlcov directory) in a web browser.

## Style Guide

Project has been developed in accordance with [pep8](https://www.python.org/dev/peps/pep-0008/) style guide.

## Deployment

The middleware and uploader components are deployed in a droplet (London based) on DigitalOcean. Each service runs in a seperate alpine based docker container, if anything goes wrong then scripts will be restarted. The Updater is automatically executed everyday at 00:00 as a cron job (using busybox). Docker maintains a log of all incoming requests. 

To create a live deployment ensure you are in the root directory and perform the following command:

```
docker-compose up
```

If any changes have been made then an updated live deployment can be achieved through the following commands:

```
docker-compose up --build
```

## Usage

The app can be obtained by downloading the release apk, this is all that is needed for usage in a live deployment environment. The app will scan a logo, get the name and metadata attached to that logo then send all the data over to the deployed middleware which will handle communicating with the ResDiary API, and return all the restaurant data to the app. The app will then display this data to the user.

## Built With

Unity - Development of mobile application

Vuforia - Image Detection framework 

FastAPI and pyDantic - Devlopment of middleware component

VWS-Python - Devlopment of uploader component

## Authors

Andrew Finlayson - 2327372f

Daniel Hislop - 2317990h

Mateusz Malek-Podjaski - 2323841m

Rishi Vinod - 2331751v

Swetank Poddar - 2424088p

## Contact Us

Andrew Finlayson - 2327372f@student.gla.ac.uk

Daniel Hislop - 2317990h@student.gla.ac.uk

Mateusz Malek-Podjaski - 2323841m@student.gla.ac.uk

Rishi Vinod - 2331751v@student.gla.ac.uk

Swetank Poddar - 2424088p@student.gla.ac.uk

## License

[GNU General Public License v3.0](https://choosealicense.com/licenses/gpl-3.0/)

## Acknowledgements

[Vuforia in Unity Documentations](https://library.vuforia.com/articles/Training/getting-started-with-vuforia-in-unity.html)

[FastAPI Documentation](https://fastapi.tiangolo.com/)

[Middleware Distance Function](https://stackoverflow.com/questions/19412462/getting-distance-between-two-points-based-on-latitude-longitude)

[Vuforia Web Service for Python Documentation](https://vws-python.readthedocs.io/en/latest/)

[Image Hashing](https://stackoverflow.com/questions/49689550/simple-hash-of-pil-image)

[ResDiary API Documentation](https://login.rdbranch.com/Security/Login?ReturnUrl=%2fSecurity%2fLogin%2fRedirectToDefault)

[How to make a README](https://medium.com/@meakaakka/a-beginners-guide-to-writing-a-kickass-readme-7ac01da88ab3)

---

Dissertation for the project is present in the [dissertation repository](https://stgit.dcs.gla.ac.uk/tp3-2019-cs10/cs10-dissertation).
