#!/bin/bash

flatpak uninstall -y nl.flyingpie.wtq

flatpak-builder \
	--force-clean \
	--install \
	--repo=.flatpak-repo \
	--state-dir=.flatpak-state-dir \
	--user \
	.flatpak-build-dir \
	nl.flyingpie.wtq.yml
