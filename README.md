
# windows-terminal-quake
Companion program for the new Windows Terminal that enables Quake-style drop down

![Preview](https://files.flyingpie.nl/windows-terminal-quake/main.gif)

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

## Settings
Since v0.4, the app supports a JSON settings file.
The file can be placed at either "C:\\Users\\(username)\\windows-terminal-quake.json", or next to the app .exe.

Changing the file automatically results in the app reloading the settings.

Suggested by [Mike F](https://github.com/mikef-nl)

- [Hot Keys](#hot-keys)
- [Toggle Duration](#toggle-duration)
- [Transparency](#transparency)
- [Vertical Screen Coverage](#vertical-screen-coverage)
- [Vertical Offset](#vertical-offset)
- [Horizontal Screen Coverage](#horizontal-screen-coverage)
- [Horizontal Align](#horizontal-align)
- [Hide On Focus Lost](#hide-on-focus-lost)

## Hot Keys
Multiple hot keys are supported, with an optional modifier.

```jsonc
{
  // The keys that can be used to toggle the terminal
  "HotKeys": [

    // Tilde, without modifiers
    {
      "Key": "OemTilde"
    },

    // Ctrl + Q
    {
      "Modifiers": "Control",
      "Key": "Q"
    }

  ]
}
```

## Toggle Duration
How long it should take for the terminal to come down or go back up.
This is an estimate, since because of the way the toggling works, some slow downs can happen.

Setting this to 0 makes the toggle instant.

```jsonc
{
  // How long the toggle up/down takes in milliseconds
  "ToggleDurationMs": 150
}
```

Suggested by [Mike F](https://github.com/mikef-nl)

## Transparency
The terminal window can be made transparent through the "Opacity"-setting.
Note that this controls the transparency of the entire window, including the title bar.

```jsonc
{
  // Make the window see-through (applies to the entire window, including the title bar)
  // 0 (invisible) - 100 (opaque)
  "Opacity": 80
}
```

![Transparency](https://files.flyingpie.nl/windows-terminal-quake/transparency.png)

Suggested by [ukWaqas](https://github.com/ukWaqas)

## Vertical Screen Coverage
The vertical space that is taken up by the terminal window can be configured through "VerticalScreenCoverage".

```jsonc
{
  // How far the terminal should come down, in percentage (eg. 50 = half way, 100 = full screen)
  "VerticalScreenCoverage": 60
}
```

![Vertical Screen Coverage](https://files.flyingpie.nl/windows-terminal-quake/vertical-coverage.png)

Suggested by [ukWaqas](https://github.com/ukWaqas)

## Vertical Offset
How much room to leave between the top of the terminal and the top of the screen.

```jsonc
{
  // How much room to leave between the top of the terminal and the top of the screen
  "VerticalOffset": 50
}
```

![Vertical Offset](https://user-images.githubusercontent.com/1295673/95657244-b9894f80-0b13-11eb-97d5-3f984f4de912.png)

Suggested by [Neil Santos](https://github.com/nlsantos)

## Horizontal Screen Coverage
Should you want the terminal to not take the entire width of the screen, take a look at the "HorizontalScreenCoverage" setting.

```jsonc
{
  // How much horizontal space the terminal should use. When this is below 100, the terminal is centered.
  // This can also be above 100, which can be useful to compensate for the window not always taking a 100% of the screen width,
  // due to the terminal being bound to column widths (eg. try 100.5 or 101).
  "HorizontalScreenCoverage": 80
}
```

![Horizontal Coverage](https://files.flyingpie.nl/windows-terminal-quake/horizontal-coverage.png)

Suggested by [baslas](https://github.com/baslas)

## Horizontal Align
When "HorizontalScreenCoverage" is below 100, this setting determines where the terminal is place horizontally.

```jsonc
{
  // When "HorizontalScreenCoverage" is below 100, this setting determines whether the terminal is place horizontally.
  // "Center", "Left" or "Right".
  "HorizontalAlign": "Center"
}
```

![Left](https://user-images.githubusercontent.com/1295673/95656847-467ed980-0b11-11eb-87a4-2bff809c30d0.png)

![Center](https://user-images.githubusercontent.com/1295673/95656889-8645c100-0b11-11eb-8310-c829f41e76bc.png)

![Right](https://user-images.githubusercontent.com/1295673/95656866-6d3d1000-0b11-11eb-9680-cb67d1c5cc6c.png)

Suggested by [Rafael Pereira](https://github.com/bsides)

## Hide On Focus Lost
When clicking or alt-tabbing away to another app, the terminal will automatically (and instantly) hide.

Defaults to "true".

```jsonc
{
  // When clicking or alt-tabbing away to another app, the terminal will automatically (and instantly) hide.
  "HideOnFocusLost": true
}
```

Suggested by [Douglas Lara](https://github.com/douglara)
