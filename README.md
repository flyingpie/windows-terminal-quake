# WTQ v2 Development

![example workflow](https://github.com/flyingpie/windows-terminal-quake/actions/workflows/continuous.yml/badge.svg)

Windows Terminal Quake v2 is underway, featuring support for other terminals, other apps in general, and multiple apps simultaneously.

Join the [discussion](https://github.com/flyingpie/windows-terminal-quake/discussions/119) to give us feedback, or take a look at a [prerelease](https://github.com/flyingpie/windows-terminal-quake/releases/tag/v2.0.0-pre5).

# windows-terminal-quake [![Build Status](https://dev.azure.com/marco0738/windows-terminal-quake/_apis/build/status/flyingpie.windows-terminal-quake?branchName=master)](https://dev.azure.com/marco0738/windows-terminal-quake/_build/latest?definitionId=2&branchName=master)
Companion program for the new Windows Terminal that enables Quake-style drop down

![Preview](./assets/example.gif)

- Runs alongside the new [Windows Terminal](https://github.com/microsoft/terminal)
- Toggle using CTRL+~ or CTRL+Q (configurable, see below)
- Shows up on the screen where the mouse is (eg. multi-monitor and multi-workspace)
- Transparency
- Configurable as fullscreen, or partial screen

## Usage
There are a couple of options:

- Download the latest release from the [releases page](https://github.com/flyingpie/windows-terminal-quake/releases).
- Clone/download the source and run **build.ps1** (uses [Cakebuild](https://cakebuild.net/)).
- Clone/download the source and build using Visual Studio.
- Via [scoop](https://scoop.sh): `scoop install https://raw.githubusercontent.com/flyingpie/windows-terminal-quake/master/scoop/windows-terminal-quake.json`

See [the documentation](https://flyingpie.github.io/windows-terminal-quake) for more settings and information.
