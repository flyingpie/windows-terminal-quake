#!/bin/bash

docker run \
	-it \
	--rm \
	--user $(id -u):$(id -g) \
	-v $(pwd):/data \
	--workdir /data \
	mcr.microsoft.com/dotnet/sdk:10.0 \
	./build.sh