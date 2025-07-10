#!/bin/bash

source ./vars.sh

flatpak uninstall -y $FLATPAK_APP

flatpak-builder \
	--force-clean \
	--install \
	--repo=$FLATPAK_REPO \
	--state-dir=$FLATPAK_STATE_DIR \
	--user \
	$FLATPAK_BUILD_DIR \
	$FLATPAK_APP_YML