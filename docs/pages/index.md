# About

Welcome to the WTQ docs!

WTQ runs in the background, and allows sliding applications on- and off the screen in Quake style.

Supports:

- Windows 10 and 11;
- KDE Plasma 5 & 6 (Wayland only).

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

### Windows - Manual

### Windows - Scoop

### Windows - WinGet

### Linux - Manual

### Linux - Flatpak

## :material-lightbulb: App examples

TODO

## :material-cog: Settings

Settings are stored in a ```.json``` file, which can be in various locations, depending mostly on preference.

!!! danger
	TODO: Detail how settings file is found.

``` mermaid
graph LR
  A[Start] --> B{Error?};
  B -->|Yes| C[Hmm...];
  C --> D[Debug];
  D --> B;
  B ---->|No| E[Yay!];
```

!!! danger
	TODO: Mention wtq.schema.json

!!! danger
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

<remarks />

<div />

---

#### Animation
##### Animation target FPS

How many frames per second the animation should be.<br />
Note that this may not be hit if moving windows takes too long, hence "target" fps.<br />
Must be between 5 and 120, to prevent issues that can arise with values that are too low or too high.<br />

<remarks />

<div />

---


### :material-application-outline: App options
Defines the options for a single toggleable app (eg. Windows Terminal, some other terminal, a file browser, etc.).
#### App
##### Name

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

<div />

---

#### Process
##### Filename

The <strong>filename</strong> to use when starting a new process for the app.<br />
E.g. <strong>notepad</strong>, <strong>dolphin</strong>, etc.

!!! note
	See the "Examples" page in the GUI for, well, examples.

<div />

---

##### Process name

Apps sometimes have <Emph>process names</Emph> different from their <Emph>filenames</Emph>.
This field can be used to look for the process name in such cases. Windows Terminal is an
example, with filename <Emph>wt</Emph>, and process name <Emph>WindowsTerminal</Emph>.

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

<div />

---

##### Argument list

<div />

<remarks />

<div />

---

##### Window title

<div />

<remarks />

<div />

---

#### Behavior
##### Window title override

Attempt to set the window title to a specific value.

<remarks />

<div />

---


### :material-cogs: Shared options
Options that are both in global WtqOptions and per-app WtqAppOptions.
#### Process
##### Attach mode

How WTQ should get to an instance of a running app.<br />
I.e. whether to start an app instance if one cannot be found.

<remarks />

<div />

---

#### Behavior
##### Always on top

Whether the app should always be on top of other windows, regardless of whether it has focus.

<remarks />

<div />

---

##### Hide on focus lost

Whether the app should be toggled off when another app gets focus.

<remarks />

<div />

---

##### Taskbar icon visibility

When to show the app window icon on the taskbar.

<remarks />

<div />

---

##### Opacity

Make the window see-through (applies to the entire window, including the title bar).<br />
0 (invisible) - 100 (opaque).

<remarks />

<div />

---

#### Position
##### Horizontal screen coverage

Horizontal screen coverage, as a percentage.

<remarks />

<div />

---

##### Horizontal align

Where to position an app on the chosen monitor, horizontally.

<remarks />

<div />

---

##### Vertical screen coverage

Vertical screen coverage as a percentage (0-100).

<remarks />

<div />

---

##### Vertical offset

How much room to leave between the top of the app window and the top of the screen, in pixels.

<remarks />

<div />

---

##### Off-screen locations

When moving an app off the screen, WTQ looks for an empty space to move the window to.<br />
Depending on your monitor setup, this may be above the screen, but switches to below if another monitor exists there.<br />
By default, WTQ looks for empty space in this order: Above, Below, Left, Right.

<remarks />

<div />

---

#### Monitor
##### Prefer monitor

Which monitor to preferably drop the app.

<remarks />

<div />

---

##### Monitor index

If <strong>PreferMonitor</strong> is set to <strong>AtIndex</strong>, this setting determines what monitor to choose.<br />
Zero based, e.g. 0, 1, etc.

<remarks />

<div />

---

#### Animation
##### Animation duration

How long the animation should take, in milliseconds.

<remarks />

<div />

---

##### Animation type (toggle ON)

The animation type to use when toggling on an application.

<remarks />

<div />

---

##### Animation type (toggle OFF)

The animation type to use when toggling off an application.

<remarks />

<div />

---




## :material-excavator: Building From Source

TODO

## :material-pencil-ruler: Architecture

TODO