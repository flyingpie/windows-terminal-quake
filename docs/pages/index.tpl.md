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

{{TEMPLATE__SETTINGS}}

## :material-excavator: Building From Source

TODO

## :material-pencil-ruler: Architecture

TODO