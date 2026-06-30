FROM debian:12

RUN apt-get update -y \
	&& DEBIAN_FRONTEND=noninteractive apt-get install --no-install-recommends -y nsis \
	&& apt-get clean \
	&& rm -rf /var/lib/apt/lists/*

WORKDIR /build

ENTRYPOINT [ "makensis", "-V4" ]