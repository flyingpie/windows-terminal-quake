#!/usr/bin/env bash

docker image build -t nsis .

docker run -v $(pwd):/build nsis installer.nsis
