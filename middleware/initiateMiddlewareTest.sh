#!/usr/bin/env bash


echo "Installing requirements"

pip install -r ./middleware/requirements.txt

echo "Starting tests"

coverage run -m pytest tests/tests.py

EXIT_CODE=$?

echo "REPORT"

coverage report -m

echo "Creating coverage report in a nice html format!"

coverage html

exit $EXIT_CODE