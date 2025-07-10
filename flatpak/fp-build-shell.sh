#!/bin/bash

source ./vars.sh

flatpak-builder \
	--run \
	$FLATPAK_BUILD_DIR \
	$FLATPAK_APP_YML \
	sh