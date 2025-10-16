#!/bin/bash

docker run \
	-it \
	--rm \
	-v $(pwd):/data \
	--workdir /data \
	mcr.microsoft.com/dotnet/sdk:9.0 \
	./build.sh