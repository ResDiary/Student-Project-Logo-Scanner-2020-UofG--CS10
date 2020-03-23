Testing
#######

Here you will find the commands for executing tests for the middleware and uploader components.

Middleware
**********

Creation of a single python file named "tests.py" containing individual test functions. 
These test functions have been implemented using pytest in conjunction with coverage. 
Coverage generates a report following test execution, readable in the terminal or in html format, of code coverage.
Test functions can be executed using either pytest or coverage through the following commands: (in middleware directory)

``pytest tests/tests.py``

``coverage run -m pytest tests/tests.py``

Or alternatively run the shell script:

``initiateMiddlewareTest.sh``

The generated coverage html report can be viewed by opening the "index.html" file in the (middleware/htmlcov directory) in a web browser.

Uploader
********

Creation of a single python file names "test.py" containing individual test functions. 
These test functions have been implemented using unittest in conjunction with coverage. 
Test functions can be executed using either of the following commands: (in uploader directory)

``python -m unittest tests/tests.py``

``coverage run --source=. -m unittest tests/test.py``

Or alternatively run the shell script:

``startUploaderTest.sh``

The generated coverage html report can be viewed by opening the "index.html" file in the (uploader/htmlcov directory) in a web browser.