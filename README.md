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

## Vertical Screen Coverage
The vertical space that is taken up by the terminal window can be configured through "VerticalScreenCoverage".

```jsonc
{
  // How far the terminal should come down, in percentage (eg. 50 = half way, 100 = full screen)
  "VerticalScreenCoverage": 60
}
```

![Transparency](https://files.flyingpie.nl/windows-terminal-quake/vertical-coverage.png)

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

![Transparency](https://files.flyingpie.nl/windows-terminal-quake/horizontal-coverage.png)
