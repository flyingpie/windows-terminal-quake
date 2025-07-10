#!/bin/bash

flatpak build-bundle \
	$FLATPAK_REPO \
	"$FLATPAK_APP.flatpak" \
	$FLATPAK_APP