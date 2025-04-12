#!/bin/bash

python -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt
mkdocs serve