# Changelog

## [vFuture]
- Feature: GUI - Suspend key presses when the hotkey field has focus (for Windows).

## [vNext]

## [v2.0.16] / 2025-xx-xx
- Refactor: Docs - Updated documentation page.
- Feature: User events - Run command when an event is fired (such as when an app toggles on or off).
- Refactor: Windows - Reworked how window handles are found. Before, a process was looked up, and its main window could be attached to. Now, the search process starts at the list of available windows, regardless of process.
- Refactor: Docs - Unified much of the documentation around settings, to use the same (comment based) data source across the GUI, the documentation page, and the JSON schema.
- Feature: Apps can now be matched on their window title (see "WindowTitle" property in app settings).
- Feature: GUI - Make JSON settings visible on "Settings (JSON)" page. Cannot be saved yet.
- Feature: New hotkey registration subsystem through the use of [SharpHook](https://github.com/TolikPylypchuk/SharpHook), which allows binding the "Windows" modifier on Windows.
- Feature: Feature flags, for scary features (such as the new window capture mentioned earlier, and the new hotkey registration through SharpHook).
- Bugfix: Scoop - Removed "persist"-property from the Scoop manifest, since settings are loaded from the same location regardless of installation method

## [v2.0.15] / 2025-03-21
- Bugfix: HideOnFocusLost broke

## [v2.0.14] / 2025-03-17
- Bugfix: GUI - Disabled the external changes notification for now, as we don't have a straight-forward way of reliably detecting this.
- Bugfix: GUI - Don't hide the sidebar (that was a feature, but it doesn't work for this use case).
- Bugfix: GUI - Properly reload app data when switching from one app to the other.
- Bugfix: Save/Load - Allow hotkeys without a modifier and/or key. Would cause these to be removed altogether on save.
- Feature: Added "wtqc" for Windows, which runs WTQ as a command line app. Useful for troubleshooting cases where the entire app won't start.
- Feature: GUI - Example apps now work.
- Feature: Make logs visible in the GUI.
- Feature: Suspend key presses when the hotkey field has focus (currently Linux only).
- Refactor: Always turn on console log, unless running non-interactively on Linux (in which case the journal would get spammed).
- Refactor: GUI - Simplified the whole thing, pull docs from xml doc.
- Refactor: KWin - Reworked when and how shortcuts are (un)registered with KWin, to be more reliably and robust, especially in combination with the GUI.
- Refactor: Option cascading, from app-specific to global, to default. Makes use of the settings easier in the rest of the app and integrates nicer with the GUI.
- Refactor: Windows - Changed taskbar icon visibility toggling method from messing with EXSTYLE properties to ITaskbarList.

## [v2.0.13] / 2025-02-07
- Bugfix: [169](https://github.com/flyingpie/windows-terminal-quake/issues/169), missing entry from the "Keys" list, for some specific keyboard layouts.
- Bugfix: Issue where the WinGet release wouldn't work straight away, due to working directories being weird because of the shim WinGet creates.
- Added banner to the GUI, to highlight that it's still in preview.

## [v2.0.12] / 2025-02-06
- Generate a JSON schema file on WTQ start, next to wherever **wtq.jsonc** is (e.g. **wtq.schema.json**). This provides autocompletion and validation in supported editors (such as VS Code).
- Initial work on a graphical UI. It can be accessed through the tray icon.
- Updated tray icon to fit in with other icons better.
- Automatically restore app window positions, when they've moved due to e.g. (dis)connecting a monitor, or when the shell restarts.
- Simpler reset of location & size when WTQ exits. Windows are now centered onto the screen with the cursor with 90% of the screen size. Previous method of moving back to where WTQ found the window was less reliable, as said location & size could sometimes be problematic (like really small or off screen).
- Disable console log by default. Can be turned on using environment variable **"WTQ_LOG_TO_CONSOLE"="TRUE"**.
- Handle cases where we can't find an off-screen location when toggling, by falling back to instantly toggling into- and out of a black hole, instead of animating anywhere.

## [v2.0.11] / 2024-11-07
- Support for KDE Plasma 5 & 6 (KWin, currently Wayland only).
- Reworked how apps are started (mostly when WTQ starts), should fix a lot of cases where multiple instances of an app (such as Windows Terminal) are started (fixes [145](https://github.com/flyingpie/windows-terminal-quake/issues/145)).
- Added **WindowTitleOverride**, which attempts to set the title of a window. Useful for "tagging" a window as being managed by WTQ, so they can be filtered out in other applications (see [#144](https://github.com/flyingpie/windows-terminal-quake/issues/144)).
```jsonc
{
  "Apps": [
    {
      "Name": "Terminal",
      "WindowTitleOverride": "The New Window Title"
    },
    ...
  ]
}
```

- Added other **off-screen locations**, to support vertically-stacked monitors (see [#146](https://github.com/flyingpie/windows-terminal-quake/issues/146)).

WTQ will look for an empty space outside the screen, starting above, then below, then to the left, then to the right. Whichever location that does not overlap with a screen wins, in order.

The order can be changed using the "OffScreenLocations" setting:
```jsonc
{
  "Apps": [
    {
      // Per app:
      "Name": "Terminal",
      "OffScreenLocations": ["Above", "Below", "Left", "Right"]
    },
    ...
  ],

  // ..or globally:
  "OffScreenLocations": ["Above", "Below", "Left", "Right"]
}
```

## [v2.0.10] / 2024-07-16
- Correctly handle multiple monitor setup, where apps that got toggled off lingered around at the top of the screen (fixes [#133](https://github.com/flyingpie/windows-terminal-quake/issues/133)).

- Added options to control the animation speed and style: 
  - **AnimationDurationMs** How long the animation takes.
  - **AnimationDurationMsWhenSwitchingApps** How long the animation takes, when switching between WTQ-attached apps.
  - **AnimationTargetFps** How often to attempt to move an application window per second.
  - **AnimationTypeToggleOn** Easing used for toggling on an app.
  - **AnimationTypeToggleOff** Easing used for toggling off an app.

- Reworked how apps are tracked

Previously, a background worker was constantly looking for configured apps, and starting processes for ones that weren't running (by default, unless the **AttachMode** was set to something other than **FindOrCreate**).

Now, finding and creating app processes only happens when WTQ starts (configurable) and when a hotkey is pressed.

As a result, an app that is manually closed is no longer automatically restarted. And perhaps more relevant, when starting an app doesn't succeed, it no longer results in an infinite retry loop.

This was especially problematic when starting a conhost-hosted app (such as PowerShell), while Windows Terminal was configured to be the default console host. In such cases, Windows Terminal would take over PowerShell, the conhost process would exit, and WTQ assumed the process failed to start.

- Added **AlwaysOnTop**, which forces an app to always be above other windows, even if it doesn't have focus.

## [v2.0.9] / 2024-05-20
- When an app is toggled up, it retains its original screen (fixes [#64](https://github.com/flyingpie/windows-terminal-quake/issues/64)).
- Reworked how windows are tracked and attached to, should fix a lot of "Main window handle not available yet"-issues.
- Added support for multiple configuration file locations (see docs for more info).
- Moved default log location to temp directory.
- Added "Open logs" button to tray icon.
- Added "HideOnFocusLost", which controls whether an app should be toggled out if another app gains focus.

Globally, for all apps:
```json
{
  "HideOnFocusLost": true | false
}
```

Or per app:
```json
{
  "Apps": [
    {
      "Name": "Terminal",
      "HideOnFocusLost": true | false
    },
    ...
  ]
}
```

## [v2.0.8] / 2024-04-28
- Automated Scoop and WinGet manifest generation.
- Automated GitHub release creation.

## [v2.0.7] / 2024-04-19
- Added opacity;
- Toggleable task bar icon visibility.

![image](https://github.com/flyingpie/windows-terminal-quake/assets/1295673/ee95e8bd-b6b2-4c48-a680-60e36a4398e1)

Globally, for all apps:
```json
{
  "Opacity": 0-100,
  "TaskBarIconVisibility": "AlwaysHidden | AlwaysVisible"
}
```

Or per app:
```json
{
  "Apps": [
    {
      "Name": "Terminal",

      "Opacity": 0-100,
      "TaskBarIconVisibility": "AlwaysHidden | AlwaysVisible"
    },
    ...
  ]
}
```

## [v2.0.6] / 2024-04-1
- Added sizing.

![wtq-sizing-01](https://github.com/flyingpie/windows-terminal-quake/assets/1295673/0f0a8f81-b0d5-4256-a15d-ac384e6386a1)

Globally, for all apps:
```json
{
  // Horizontal screen coverage, as a percentage (defaults to 95).
  "HorizontalScreenCoverage": 95,

  // Vertical screen coverage, as a percentage (defaults to 95).
  "VerticalScreenCoverage": 95,

  // How much room to leave between the top of the terminal and the top of the screen, in pixels.
  "VerticalOffset": 0
}
```

Or per app:
```json
{
  "Apps": [
    {
      "Name": "Terminal",

      // Horizontal screen coverage, as a percentage (defaults to 95).
      "HorizontalScreenCoverage": 95,

      // Vertical screen coverage as a percentage (defaults to 95).
      "VerticalScreenCoverage": 95,
    
      // How much room to leave between the top of the terminal and the top of the screen, in pixels.
      "VerticalOffset": 0
    },
    ...
  ]
}
```

## [v2.0.5] / 2024-03-17
Initial support for auto-starting apps.

**BREAKING CHANGES**
The configuration file has been simplified.

Old syntax:
```json
"Apps": [
  {
    "Name": "Terminal",
    "Hotkeys": [ { "Modifiers": "Control", "Key": "D1" } ],
    "FindExisting": {
      "ProcessName": "WindowsTerminal"
    },
    "StartNew": {
      "FileName": "wt"
    }
  }
]
```

The "**ProcessName**"-property is optional for processes where they are the same.

New syntax:
```json
"Apps": [
  {
    "Name": "Terminal",
    "Hotkeys": [{ "Modifiers": "Control", "Key": "D1" }],
    "FileName": "wt",
    "ProcessName": "WindowsTerminal"
  }
]
```

#### Auto-starting apps to toggle
Currently exploring the direction where an app has an "AttachMode", which dictates how WTQ grabs an app:

- **FindOrStart** (default)
Looks for an existing process as specified by the "FileName"- and/or "ProcessName"-properties. If no process was found, a new one will be started using the value under the "FileName"-property.
- **Find**
Just looks for existing processes as specified by the "FileName"- and/or "ProcessName"-properties. No new processes will be started (previous version behavior, where you always had to manually start the app).
- **Start** (very experimental)
Always starts a new process, specifically to be used by WTQ. Meant for apps that have multiple open instances. Initially meant for (among other things) browsers, but these turn out to be more complicated. This will be documented later.
- **Manual**
Attaches whatever app has focus, when the hotkey is pressed. Keeps the app attached until WTQ is closed.

The mode can be specified per app (note that "FindOrStart" is the default:
```json
"Apps": [
  {
    "Name": "Terminal",
    "AttachMode": "Find", // Only attach to process that is already running, don't auto-start one.
    "Hotkeys": [{ "Modifiers": "Control", "Key": "D1" }],
    "FileName": "wt",
    "ProcessName": "WindowsTerminal"
  }
]
```

## [v2.0.4] / 2024-02-13
- The ["PreferMonitor" and "MonitorIndex"](https://wtq.flyingpie.nl/v2/settings/prefer-monitor/) settings, to control what monitor an app toggles on.

The setting is available at the root config level, and can be overridden per application.

```json
{
  "PreferMonitor": "WithCursor", // WithCursor | Primary | AtIndex
  "MonitorIndex": 0,

  "Apps": [
    {
      "PreferMonitor": "WithCursor", // WithCursor | Primary | AtIndex
      "MonitorIndex": 0,
    }
  ]
}
```

## [v2.0.3] / 2024-02-11
- WinGet release preparation.

## [v2.0.2] / 2024-02-01
- Re-introduced tray icon, app runs in the background now
- Toggle out on focus lost (not configurable yet)
- Alternative method for toggling focus back to previous app, that does not change app state, reduces flickering

Feel free to join the [v2 discussion](https://github.com/flyingpie/windows-terminal-quake/discussions/119).

**The config file has changed slightly, see included example**
Includes example configuration (windows-terminal-quake.jsonc):
- Ctrl + 1: Windows Terminal
- Ctrl + 2: [Q-Dir](http://q-dir.com/)
- Ctrl + 3: [Process Hacker](https://processhacker.sourceforge.io/)
- Ctrl + 4: Spotify

- Ctrl + Q: Most recent app

Comes in 2 flavors:
**Self Contained**: Runs without any prerequisites, but is massive.
**Requires Net8**: You'll need to download the [.Net 8 runtime first](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-8.0.1-windows-x64-installer).

## [v2.0.1] / 2024-01-22
The next major version of Windows-Terminal-Quake, with support for multiple apps, including terminals other than Windows Terminal.

<video autoplay loop width="800">
	<source src="https://github.com/flyingpie/windows-terminal-quake/assets/1295673/1bedd582-833a-4b7a-895a-f9d4de6d8ba7" />
</video>

Feel free to join the [v2 discussion](https://github.com/flyingpie/windows-terminal-quake/discussions/119).

Includes example configuration (windows-terminal-quake.jsonc):
- Ctrl + 1: Windows Terminal
- Ctrl + 2: [Q-Dir](http://q-dir.com/)
- Ctrl + 3: [Process Hacker](https://processhacker.sourceforge.io/)
- Ctrl + 4: Spotify

- Ctrl + Q: Most recent app

Comes in 2 flavors:
**Self Contained**: Runs without any prerequisites, but is massive.
**Requires Net8**: You'll need to download the [.Net 8 runtime first](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-8.0.1-windows-x64-installer).