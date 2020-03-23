#!/usr/bin/env bash

echo "Installing requirements"

pip install -r uploader/requirements.txt

echo "Generating coverage report"
coverage run --source=. -m unittest tests/tests.py
EXIT_CODE=$?

echo "REPORT"

coverage report -m

echo "Creating coverage report in a nice html format!"

coverage html

echo $EXIT_CODE
exit $EXIT_CODE
