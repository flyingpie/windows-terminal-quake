# Overview

Companion program for the new Windows Terminal that enables Quake-style drop down and some additional features.

![img](../assets/img/main.gif)

!!! note "WTQ v1"
    This is the documentation for Windows Terminal Quake v1. It is currently more feature-complete than v2, but as of yet only supports a single application at a time.

    Click [here](../index.md) to see the docs for the in-development v2.

[![Build Status](https://dev.azure.com/marco0738/windows-terminal-quake/_apis/build/status/flyingpie.windows-terminal-quake?branchName=master)](https://dev.azure.com/marco0738/windows-terminal-quake/_build/latest?definitionId=2&branchName=master)

## Features

- Runs alongside the new [Windows Terminal](https://github.com/microsoft/terminal)
- Toggle using CTRL+~ or CTRL+Q (configurable, see [hotkeys](settings/hotkeys.md))
- Shows up on the screen where the mouse is (eg. multi-monitor and multi-workspace)
- Transparency
- Configurable as fullscreen, or partial screen

## Settings
Since v0.4, the app supports a JSON settings file.
The file can be placed at either (in order):

- File called "windows-terminal-quake.json", next to the app .exe
- "C:\\Users\\(username)\\windows-terminal-quake.json"

WTQ also supports the extensions **.jsonc** and **.json5**, which can be useful for automatic syntax highlighting support in editors.

Changing the file automatically results in the app reloading the settings.

<span class="by">Settings feature Suggested by [Mike F](https://github.com/mikef-nl).</span>
