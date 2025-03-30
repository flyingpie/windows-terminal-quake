# About

**Welcome to the WTQ docs!**

WTQ runs in the background, and enables sliding applications on- :material-arrow-down: and off :material-arrow-up: the screen in Quake style.

Supports:

- Windows **10** and **11**
- KDE Plasma **5** & **6** (Wayland only)

See [Installation](#installation) to get started.

---

Here's an example where WTQ runs on **Windows 10**, toggling [Windows Terminal](), [Double Commander]() and [Process Hacker]().
<video controls loop autoplay>
<source src="/assets/video/wtq-v2.mp4" />
</video>

And here's one on **KDE Plasma 6**, toggling [WezTerm](), [Dolphin]() [System Monitor]() and [Spotify]().
<video controls loop autoplay>
<source src="/assets/video/wtq-v2-linux.mp4" />
</video>

!!! note "Why "Quake" Style"
	The game [Quake (by id Software)](https://en.wikipedia.org/wiki/Quake_(video_game)) is generally considered the game that popularized toggling of the console onto the screen, by sliding it from the top.
	See [this video](https://www.youtube.com/watch?v=sDrDK7BigEc) for an example of what that looked like.

!!! note "The "WTQ" Name"
	WTQ initially started as a companion app for Microsoft's thrilling sequel to the classic command prompt; [Windows Terminal](https://github.com/microsoft/terminal).
	Fit that with Quake-style toggling, and you get **"Windows Terminal Quake"**.

	Much later, support for other terminals was added, and support for toggling apps that were not terminals at all.

	So now I refer to it as **"WTQ"**, not having the balls to straight up rename it.

## :material-download: Installation 

### Windows - Scoop

```shell
scoop install https://raw.githubusercontent.com/flyingpie/windows-terminal-quake/master/scoop/wtq-latest.json
```
A shortcut is then available named "**WTQ - Windows Terminal Quake**", or you can just run "**wtq**" from a command line or **Win+R**.

### Windows - WinGet

```shell
winget install windows-terminal-quake
```
You can then call "**wtq**" from the command line.

After having done that at least once, a shortcut will appear in the start menu, called "**WTQ - Main Window**".

![image](https://github.com/user-attachments/assets/aebaf70c-76d3-4d51-9c28-1f6a7ad4b78f)

### Windows - Manual

See [the latest release](https://github.com/flyingpie/windows-terminal-quake/releases/latest), and pick a zip.

### Linux - Manual

See the [~/linux/install-or-upgrade-wtq.sh script](https://github.com/flyingpie/windows-terminal-quake/blob/master/linux/install-or-upgrade-wtq.sh) that downloads the latest version of WTQ, installs it to ```~/.local/share/wtq```, and creates a ```wtq.desktop``` file.

As a 1-liner:
```shell
bash <(curl -s https://raw.githubusercontent.com/flyingpie/windows-terminal-quake/refs/heads/master/linux/install-or-upgrade-wtq.sh)
```

And the [~/linux/uninstall-wtq.sh uninstall script](https://github.com/flyingpie/windows-terminal-quake/blob/master/linux/uninstall-wtq.sh).
```shell
bash <(curl -s https://raw.githubusercontent.com/flyingpie/windows-terminal-quake/refs/heads/master/linux/uninstall-wtq.sh)
```

!!! note
	The WTQ configuration is not removed by this script. These are usually located at ```~/.config/wtq```, also see [Settings](#settings).

### Linux - Flatpak

!!! danger "TODO"

## :material-lightbulb: App examples

!!! danger "TODO"

## :material-cog: Settings

Settings are stored in a JSON file. The file can use the extension ```.json```, ```.jsonc``` or ```.json5```. Using ```.jsonc``` or ```.json5``` are supported so that editors like VSCode automatically switch to "JSON with Comments".

The file can be in several places, to support different use cases and preferences.

!!! note "Where's My Settings File?"
	You can quickly open either the settings file, or the directory that contains the settings file by clicking the tray icon -> **Open Settings File**, or **Open Settings Directory**.

	Additionally, it's also displayed in the GUI.

These locations are considered, in this order:

1. Environment variable named ```WTQ_CONFIG_FILE``` exists and points to an existing file (regardless of filename or extension)
2. Next to the WTQ executable
	- :fontawesome-brands-windows: ```wtq.exe```
	- :fontawesome-brands-linux: ```wtq```
3. In ```$XDG_CONFIG_HOME```, if defined (following to the [XDG spec](https://specifications.freedesktop.org/basedir-spec/latest/))
	- :fontawesome-brands-windows: ```C:\users\username\.config\wtq.json```
	- :fontawesome-brands-linux: ```/home/username/.config/wtq.json```
4. In **~/.config**
	- :fontawesome-brands-windows: ```C:\users\username\.config\wtq.json```
	- :fontawesome-brands-linux: ```/home/username/.config/wtq.json```
5. In user home
	- :fontawesome-brands-windows: ```C:\users\username\wtq.json```
	- :fontawesome-brands-linux: ```/home/username/wtq.json```
6. In user home, as a dot file
	- :fontawesome-brands-windows: ```C:\users\username\.wtq.json```
	- :fontawesome-brands-linux: ```/home/username/.wtq.json```
7. :fontawesome-brands-windows: In app data
	- :fontawesome-brands-windows: ```C:\users\username\AppData\Roaming\wtq\wtq.json```
	- :fontawesome-brands-linux: ```/home/username/.config/wtq.json```

### Locations

#### On Windows

1. Environment variable named ```WTQ_CONFIG_FILE``` exists and points to an existing file (regardless of filename or extension)
2. Next to the WTQ executable
	- :fontawesome-brands-windows: ```wtq.exe```
	- :fontawesome-brands-linux: ```wtq```
3. In ```$XDG_CONFIG_HOME```, if defined (following to the [XDG spec](https://specifications.freedesktop.org/basedir-spec/latest/))
	- :fontawesome-brands-windows: ```C:\users\username\.config\wtq.json```
	- :fontawesome-brands-linux: ```/home/username/.config/wtq.json```
4. In **~/.config**
	- :fontawesome-brands-windows: ```C:\users\username\.config\wtq.json```
	- :fontawesome-brands-linux: ```/home/username/.config/wtq.json```
5. In user home
	- :fontawesome-brands-windows: ```C:\users\username\wtq.json```
	- :fontawesome-brands-linux: ```/home/username/wtq.json```
6. In user home, as a dot file
	- :fontawesome-brands-windows: ```C:\users\username\.wtq.json```
	- :fontawesome-brands-linux: ```/home/username/.wtq.json```
7. :fontawesome-brands-windows: In app data
	- :fontawesome-brands-windows: ```C:\users\username\AppData\Roaming\wtq\wtq.json```
	- :fontawesome-brands-linux: ```/home/username/.config/wtq.json```

!!! danger "TODO"
	TODO: Detail how settings file is found.

!!! danger "TODO"
	TODO: Mention wtq.schema.json

!!! danger "TODO"
	TODO: Mention GUI

### :material-earth: Global options
Defines WTQ-wide options, including the list of configured apps.
#### General
##### Apps

Applications to enable Quake-style dropdown for.

!!! note
	See the GUI and the <a href="#app-examples">docs</a> for some examples.

<div>
```json
{
	"Apps": [
		{ "Name": "App 1" },
		{ "Name": "App 2" },
		// ...
	]
}
```
</div>

---

##### Hotkeys

Global hotkeys, that toggle either the first, or the most recently toggled app.<br />
Optional.

<remarks />

<div>
```json
{
	"Hotkeys": [
		{ "Modifiers": "Control", "Key": "Q" }
	]
}
```
</div>

---

##### Show UI on start

Whether to show the GUI when WTQ is started.

Defaults to ```False```

<remarks />

Globally:
```json
{
	"ShowUiOnStart": "False",
	// ...
}
```


---

#### Animation
##### Animation target FPS

How many frames per second the animation should be.<br />
Note that this may not be hit if moving windows takes too long, hence "target" fps.<br />
Must be between 5 and 120, to prevent issues that can arise with values that are too low or too high.

Defaults to ```40```

<remarks />

Globally:
```json
{
	"AnimationTargetFps": "40",
	// ...
}
```


---


### :material-application-outline: App options
Defines the options for a single toggleable app (eg. Windows Terminal, some other terminal, a file browser, etc.).
#### App
##### Name

**REQUIRED**

A logical name for the app, used to identify it across config reloads.<br />
Appears in logs.

<remarks />

<div>
```json
{
	"Name": "Terminal",
	// ...
}
```
</div>

---

##### Hotkeys

One or more keyboard shortcuts that toggle in- and out this particular app.

<remarks />

For a single app:
```json
{
	"Apps": [
		{
			"Hotkeys": "",
			// ...
		}
	]
}
```


---

#### Process
##### Filename

**REQUIRED**

The <strong>filename</strong> to use when starting a new process for the app.<br />
E.g. <strong>notepad</strong>, <strong>dolphin</strong>, etc.

!!! note
	See the "Examples" page in the GUI for, well, examples.

For a single app:
```json
{
	"Apps": [
		{
			"FileName": "wt",
			// ...
		}
	]
}
```


---

##### Process name

Apps sometimes have <strong>process names</strong> different from their <strong>filenames</strong>.
This field can be used to look for the process name in such cases. Windows Terminal is an
example, with filename <strong>wt</strong>, and process name <strong>WindowsTerminal</strong>.

<remarks />

<div>
```json
{
	// Using with Windows Terminal requires both "Filename" and "ProcessName".
	"Apps": {
		"Filename": "wt",
		"ProcessName": "WindowsTerminal"
	}
}
```
</div>

---

##### Arguments

Command-line arguments that should be passed to the app when it's started.<br />
Note that this only applies when using an <see cref="T:Wtq.Configuration.AttachMode" /> that starts the app.

<remarks />

For a single app:
```json
{
	"Apps": [
		{
			"Arguments": "",
			// ...
		}
	]
}
```


---

##### Argument list

<div />

<remarks />

For a single app:
```json
{
	"Apps": [
		{
			"ArgumentList": "",
			// ...
		}
	]
}
```


---

##### Window title

<div />

<remarks />

For a single app:
```json
{
	"Apps": [
		{
			"WindowTitle": "Mozilla Firefox - WhatsApp",
			// ...
		}
	]
}
```


---

#### Behavior
##### Window title override

Attempt to set the window title to a specific value.

<remarks />

For a single app:
```json
{
	"Apps": [
		{
			"WindowTitleOverride": "",
			// ...
		}
	]
}
```


---


### :material-cogs: Shared options
Options that are both in global WtqOptions and per-app WtqAppOptions.
#### Process
##### Attach mode

How WTQ should get to an instance of a running app.<br />
I.e. whether to start an app instance if one cannot be found.

Defaults to ```FindOrStart```

- **Find** <div>
            Only look for existing app instances (but don't create one).
            </div>

- **Find or start** <div>
            Look for an existing app instance, create one if one does not exist yet.
            </div>

- **Manual** <div>
            Attach to whatever app is in the foreground when pressing an assigned hotkey.
            </div>

<remarks />

Globally:
```json
{
	"AttachMode": "Find | FindOrStart | Manual",
	// ...
}
```
For a single app:
```json
{
	"Apps": [
		{
			"AttachMode": "Find | FindOrStart | Manual",
			// ...
		}
	]
}
```


---

#### Behavior
##### Always on top

Whether the app should always be on top of other windows, regardless of whether it has focus.

Defaults to ```False```

<remarks />

Globally:
```json
{
	"AlwaysOnTop": "False",
	// ...
}
```
For a single app:
```json
{
	"Apps": [
		{
			"AlwaysOnTop": "False",
			// ...
		}
	]
}
```


---

##### Hide on focus lost

Whether the app should be toggled off when another app gets focus.

Defaults to ```Always```

- **Always** <div>
            Toggle off the app when focus is lost.
            </div>

- **Never** <div>
            Do not toggle off the app when focus is lost.
            </div>

<remarks />

Globally:
```json
{
	"HideOnFocusLost": "Always | Never",
	// ...
}
```
For a single app:
```json
{
	"Apps": [
		{
			"HideOnFocusLost": "Always | Never",
			// ...
		}
	]
}
```


---

##### Taskbar icon visibility

When to show the app window icon on the taskbar.

Defaults to ```AlwaysHidden```

- **Always hidden** <div>
            <strong>Never</strong> show the taskbar icon.
            </div>

- **Always visible** <div>
            <strong>Always</strong> show the taskbar icon (note that this can look a bit weird when the app is toggled off).
            </div>

- **When the app is visible** <div>
            Only show the taskbar icon when the app is toggled <strong>on</strong>.
            </div>

<remarks />

Globally:
```json
{
	"TaskbarIconVisibility": "AlwaysHidden | AlwaysVisible | WhenAppVisible",
	// ...
}
```
For a single app:
```json
{
	"Apps": [
		{
			"TaskbarIconVisibility": "AlwaysHidden | AlwaysVisible | WhenAppVisible",
			// ...
		}
	]
}
```


---

##### Opacity

Make the window see-through (applies to the entire window, including the title bar).<br />
0 (invisible) - 100 (opaque).

Defaults to ```100```

<remarks />

Globally:
```json
{
	"Opacity": "100",
	// ...
}
```
For a single app:
```json
{
	"Apps": [
		{
			"Opacity": "100",
			// ...
		}
	]
}
```


---

#### Position
##### Horizontal screen coverage

Horizontal screen coverage, as a percentage.

Defaults to ```95```

<remarks />

Globally:
```json
{
	"HorizontalScreenCoverage": "95",
	// ...
}
```
For a single app:
```json
{
	"Apps": [
		{
			"HorizontalScreenCoverage": "95",
			// ...
		}
	]
}
```


---

##### Horizontal align

Where to position an app on the chosen monitor, horizontally.

Defaults to ```Center```

- **Center** <div>
            Center of the screen.
            </div>

- **Left** <div>
            Left of the screen.
            </div>

- **Right** <div>
            Right of the screen.
            </div>

<remarks />

Globally:
```json
{
	"HorizontalAlign": "Center | Left | Right",
	// ...
}
```
For a single app:
```json
{
	"Apps": [
		{
			"HorizontalAlign": "Center | Left | Right",
			// ...
		}
	]
}
```


---

##### Vertical screen coverage

Vertical screen coverage as a percentage (0-100).

Defaults to ```95```

<remarks />

Globally:
```json
{
	"VerticalScreenCoverage": "95",
	// ...
}
```
For a single app:
```json
{
	"Apps": [
		{
			"VerticalScreenCoverage": "95",
			// ...
		}
	]
}
```


---

##### Vertical offset

How much room to leave between the top of the app window and the top of the screen, in pixels.

Defaults to ```0```

<remarks />

Globally:
```json
{
	"VerticalOffset": "0",
	// ...
}
```
For a single app:
```json
{
	"Apps": [
		{
			"VerticalOffset": "0",
			// ...
		}
	]
}
```


---

##### Off-screen locations

When moving an app off the screen, WTQ looks for an empty space to move the window to.<br />
Depending on your monitor setup, this may be above the screen, but switches to below if another monitor exists there.<br />
By default, WTQ looks for empty space in this order: Above, Below, Left, Right.

<remarks />

Globally:
```json
{
	"OffScreenLocations": "",
	// ...
}
```
For a single app:
```json
{
	"Apps": [
		{
			"OffScreenLocations": "",
			// ...
		}
	]
}
```


---

#### Monitor
##### Prefer monitor

Which monitor to preferably drop the app.

Defaults to ```WithCursor```

- **With cursor** <div>
            The monitor where the mouse cursor is currently at.
            </div>

- **At index** <div>
            The monitor at the index as specified by <strong>MonitorIndex</strong> (0-based).
            </div>

- **Primary** <div>
            The monitor considered <strong>primary</strong> by the OS.
            </div>

<remarks />

Globally:
```json
{
	"PreferMonitor": "WithCursor | AtIndex | Primary",
	// ...
}
```
For a single app:
```json
{
	"Apps": [
		{
			"PreferMonitor": "WithCursor | AtIndex | Primary",
			// ...
		}
	]
}
```


---

##### Monitor index

If <strong>PreferMonitor</strong> is set to <strong>AtIndex</strong>, this setting determines what monitor to choose.<br />
Zero based, e.g. 0, 1, etc.

Defaults to ```0```

<remarks />

Globally:
```json
{
	"MonitorIndex": "0",
	// ...
}
```
For a single app:
```json
{
	"Apps": [
		{
			"MonitorIndex": "0",
			// ...
		}
	]
}
```


---

#### Animation
##### Animation duration

How long the animation should take, in milliseconds.

Defaults to ```250```

<remarks />

Globally:
```json
{
	"AnimationDurationMs": "250",
	// ...
}
```
For a single app:
```json
{
	"Apps": [
		{
			"AnimationDurationMs": "250",
			// ...
		}
	]
}
```


---

##### Animation type (toggle ON)

The animation type to use when toggling on an application.

Defaults to ```EaseOutQuart```

- **Linear** <div />

- **Ease in+out (sine)** <div />

- **Ease in back** <div />

- **Ease in cubic** <div />

- **Ease in quadratic** <div />

- **Ease out back** <div />

- **Ease out cubic** <div />

- **Ease out quadratic** <div />

<remarks />

Globally:
```json
{
	"AnimationTypeToggleOn": "Linear | EaseInOutSine | EaseInBack | EaseInCubic | EaseInQuart | EaseOutBack | EaseOutCubic | EaseOutQuart",
	// ...
}
```
For a single app:
```json
{
	"Apps": [
		{
			"AnimationTypeToggleOn": "Linear | EaseInOutSine | EaseInBack | EaseInCubic | EaseInQuart | EaseOutBack | EaseOutCubic | EaseOutQuart",
			// ...
		}
	]
}
```


---

##### Animation type (toggle OFF)

The animation type to use when toggling off an application.

Defaults to ```EaseInQuart```

- **Linear** <div />

- **Ease in+out (sine)** <div />

- **Ease in back** <div />

- **Ease in cubic** <div />

- **Ease in quadratic** <div />

- **Ease out back** <div />

- **Ease out cubic** <div />

- **Ease out quadratic** <div />

<remarks />

Globally:
```json
{
	"AnimationTypeToggleOff": "Linear | EaseInOutSine | EaseInBack | EaseInCubic | EaseInQuart | EaseOutBack | EaseOutCubic | EaseOutQuart",
	// ...
}
```
For a single app:
```json
{
	"Apps": [
		{
			"AnimationTypeToggleOff": "Linear | EaseInOutSine | EaseInBack | EaseInCubic | EaseInQuart | EaseOutBack | EaseOutCubic | EaseOutQuart",
			// ...
		}
	]
}
```


---




## :material-excavator: Building From Source

!!! danger "TODO"

## :material-pencil-ruler: Architecture

!!! danger "TODO"

``` mermaid
graph LR
  A[Start] --> B{Error?};
  B -->|Yes| C[Hmm...];
  C --> D[Debug];
  D --> B;
  B ---->|No| E[Yay!];
```
