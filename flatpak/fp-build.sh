#!/bin/bash

flatpak-builder \
	--force-clean \
	--repo=./flatpak-repo ./flatpak-build \
	nl.flyingpie.wtq.yml
