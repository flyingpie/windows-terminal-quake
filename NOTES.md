# Notes

## WinGet release
1. Build using ```src/build.ps1```
2. Test new version: ```./Tools/SandboxTest.ps1 ./manifests/f/flyingpie/windows-terminal-quake/2.0.16```

## TODO
- Code analysis warnings
- Logging
- Comments
- Options docs + schema
- Logs may not be flushed properly on app exit, dropping some entries.
- Light up the toc active element
- webkitgtk 4.1 dependency
- Uitgebreidere example wtq.jsonc, met meer opties (en alles uitgecomment?)
- Log levels tweaken, naar een stuk minder "info"
- Ctrl+C doesn't kill always process properly

## SharpHook
The current default hotkey registration system uses a method where an invisible WinForms Window is created, and we then hook into its message pump to listen for hotkey presses, even when WTQ does not have focus (implemented here: https://github.com/flyingpie/windows-terminal-quake/blob/master/src/20-Services/Wtq.Services.WinForms/Native/HotkeyManager.cs).

Though it seems that the "Windows"/"Meta"/"Super" key doesn't show up/isn't supported.

The MSDN docs suggest it should be (https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey#parameters), but the "Windows" key seems to be captured by the OS it self or something, and hence hotkeys with said modifier don't actually trigger WTQ's hotkey system.

One way of dealing with this, is through the SharpHook library (https://github.com/TolikPylypchuk/SharpHook), which does actually enable using the "Windows" key as a modifier, but now we sometimes have issues with windows being brought to the foreground (?).

I'm not sure why, but that's where I'm at.

## KWin Client/Window object
Example of a "window" object in KWin 6 ("client" in KWin 5):
```json
{
	"objectName": "",
	"bufferGeometry": {
		"x": 0,
		"y": 26,
		"width": 1920,
		"height": 1022,
		"left": 0,
		"right": 1920,
		"top": 26,
		"bottom": 1048
	},
	"pos": {
		"x": 0,
		"y": 0
	},
	"size": {
		"width": 1920,
		"height": 1048
	},
	"x": 0,
	"y": 0,
	"width": 1920,
	"height": 1048,
	"opacity": 1,
	"screen": 0,
	"rect": {
		"x": 0,
		"y": 0,
		"width": 1920,
		"height": 1048,
		"left": 0,
		"right": 1920,
		"top": 0,
		"bottom": 1048
	},
	"resourceName": "",
	"resourceClass": "org.wezfurlong.wezterm",
	"windowRole": "",
	"desktopWindow": false,
	"dock": false,
	"toolbar": false,
	"menu": false,
	"normalWindow": true,
	"dialog": false,
	"splash": false,
	"utility": false,
	"dropdownMenu": false,
	"popupMenu": false,
	"tooltip": false,
	"notification": false,
	"criticalNotification": false,
	"appletPopup": false,
	"onScreenDisplay": false,
	"comboBox": false,
	"dndIcon": false,
	"windowType": 0,
	"managed": true,
	"deleted": false,
	"shaped": false,
	"skipsCloseAnimation": false,
	"popupWindow": false,
	"outline": false,
	"internalId": "{5af91b1c-1d02-4d12-8c43-9419e7f79b1e}",
	"pid": 279423,
	"stackingOrder": 7,
	"fullScreen": false,
	"fullScreenable": true,
	"active": false,
	"desktop": 1,
	"onAllDesktops": false,
	"activities": [
		"96241575-7027-4ddd-8e73-8d3fc2cae272"
	],
	"x11DesktopIds": [
		1
	],
	"skipTaskbar": false,
	"skipPager": false,
	"skipSwitcher": false,
	"closeable": true,
	"icon": "",
	"keepAbove": false,
	"keepBelow": false,
	"shadeable": false,
	"shade": false,
	"minimizable": true,
	"minimized": false,
	"iconGeometry": {
		"x": 155,
		"y": 1048,
		"width": 40,
		"height": 32,
		"left": 155,
		"right": 195,
		"top": 1048,
		"bottom": 1080
	},
	"specialWindow": false,
	"demandsAttention": false,
	"caption": "wezterm",
	"minSize": {
		"width": 150,
		"height": 150
	},
	"maxSize": {
		"width": 2147483647,
		"height": 2147483647
	},
	"wantsInput": true,
	"transient": false,
	"transientFor": null,
	"modal": false,
	"geometry": {
		"x": 0,
		"y": 0,
		"width": 1920,
		"height": 1048,
		"left": 0,
		"right": 1920,
		"top": 0,
		"bottom": 1048
	},
	"frameGeometry": {
		"x": 0,
		"y": 0,
		"width": 1920,
		"height": 1048,
		"left": 0,
		"right": 1920,
		"top": 0,
		"bottom": 1048
	},
	"move": false,
	"resize": false,
	"decorationHasAlpha": true,
	"noBorder": false,
	"providesContextHelp": false,
	"maximizable": true,
	"moveable": true,
	"moveableAcrossScreens": true,
	"resizeable": true,
	"desktopFileName": "org.wezfurlong.wezterm",
	"hasApplicationMenu": false,
	"applicationMenuActive": false,
	"unresponsive": false,
	"colorScheme": "kdeglobals",
	"layer": 2,
	"hidden": false,
	"tile": null
}
```

## Bugs
- Crash when not able to start Spotify (or any other app for that matter)
	- No indication what happened either
- Given settings like "HorizontalScreenCoverage" and "HorizontalAlign", an app that cannot be sized small enough can overlap onto another screen
- Bashing the toggle hotkey when the app is still starting can spawn multiple instances of said app

* Since I use fish as my shell, I couldn't execute the one liner command since fish errors out due to different syntax.

* I then switch to bash and execute the command. The command errors out with this:
curl: (23) Failure writing output to destination, passed 1378 returned 1349
Despite the error, it claims it has been successfully installed since the last command is an echo and doesn't check if things actually have gone right.

- With those fails, I manually downloaded the script, made it executable, then ran it.
It made the wtq folder and the desktop application, but then it errored out since I don't have bsdtar installed. Then ended up with curl: (23) Failure writing output to destination, passed 16375 returned 0.
	-> Updated build to spit out .tar.gz instead of zip
	-> Updated Linux install script to use tar instead of bsdtar

= With that, I downloaded the zip file itself containing WTQ and installed it where it should be. I chmod'ed the executable and ran the desktop shortcut.

* It ran! Now I can set up my stuff. I go over to examples and find my console (Ptyxis) already has an example. I click 'Add' and I don't get any feedback that it did anything. I guess that part is still a WIP. So I set up the thing myself.

* I set my hotkey to my usual terminal hotkey—an extra key on my keyboard which the GUI saw as 'F15' (had to disable the shortcut in KDE it in order for the GUI to register it, and KDE calls it 'Launch (3)').

* I set the filename to "Ptyxis", changed it to "ptyxis" due to case sensitivity, then found my existing Ptyxis terminal disappearing. Seems like as soon as you enter the filename, it snatches the window and does something with it. I haven't even saved any settings yet... are the changes hotloaded despite there being a save button? Continuing on, I set attach mode to "Find or Start" (which is the default setting).

* I enable "Always on top" and save my progress from here. I'm done setting up for now. I click 'save' and I get the warning that the settings file has changed and be aware that saving anything here will overwrite it. I think this warning is for when I edit the file itself rather than save the settings through the GUI...

* I then press my shortcut key and... nothing. I look into settings and find out that it had removed my shortcut! I added it back, saved, and watched as it remove the shortcut again. I right clicked WTQ's tray icon and opened up the settings file. Turns out it's set to 'CTRL + Q', which was the default when generating the new entry.

* I change the file manually to "F15" and the warning that there has been a change to the settings popped up again. I might be right that saving the settings in GUI makes the GUI thinks someone changed it on the outside.

* Changed it to "F15" in the GUI once more and it didn't save, clearing the key portion of the settings file.

* I changed the keybind to a random key, 2, then saved the settings. Now the GUI is just a blank screen with blobs of color on the side. Closing the window and opening it does not fix it. I've uploaded the logs here, but I couldn't find if it had logged the error.

* Quitted WTQ and rebooted it. My terminal is opened and it disappears again. Guess that means that portion of the program is working. I manually change the key in the file to "F15" again. I press it and still, nothing happens.

* I change the keybind to 'CTRL + ALT + T' and it properly saves the keybind. That one doesn't work. I changed the keybind to 'CTRL + L' and the terminal pops up in Quake mode! It became 'CTRL + None' and I changed it to 'CTRL + I', saved, then back to 'CTRL + L'. Now I'm just having problems at this point and both 'CTRL + L' and "CTRL + I' are both valid keybinds to open the terminal. The GUI says there's no shortcut assigned and the setting file says there's none either.

* I guess everything is done at this point. Pressing either of the keybinds opens and closed the console and I've achieved what I wanted. Very rocky... but it works!

## Ideas
- Pie menu
	- Can detach apps as well
	- Can add items that start an app that is not necessarily WTQ-managed
	- App icons?

# Links

## KWin Api

KWin Source
https://invent.kde.org/search?search=shortcut&nav_source=navbar&project_id=2612&group_id=1568&search_code=true&repository_ref=master

KWin API 5.x
https://kdocs.rabbitictranslator.com/appium/public/docs/plasma/kwin/api/

KWin API 6.x
https://develop.kde.org/docs/plasma/kwin/api

KWin Script That Messes With Screens
https://github.com/Hegemonia123/restoreToScreen/blob/master/contents/code/main.js

## Qt Key Sequences
https://doc.qt.io/qt-6/qt.html#Modifier-enum
https://doc.qt.io/qt-6/qkeycombination.html
https://doc.qt.io/qt-6/qt.html#Key-enum
https://doc.qt.io/qt-6/qkeysequence.html#QKeySequence-3

## Tray Icons
https://github.com/AvaloniaUI/Avalonia/pull/6560/files

# Config

```json
{
	"$schema": "./wtq.schema.2.json",

	"Apps": [
		{
			"Name":				"WezTerm",
			"HotKeys":			[{ "Modifiers": "Control", "Key": "D1" }],
			"FileName":			"D:/syncthing/apps/wezterm/wezterm-gui.exe",
			"Arguments":		"--config \"prefer_egl=true\"",
			"Opacity":			95
		},
		{
			"Name":				"QDir",
			"HotKeys":			[{ "Modifiers": "Control", "Key": "D2" }],
			"FileName":			"D:/syncthing/apps/qdir/Q-Dir_x64.exe"
		},
		{
			"Name":				"ProcessHacker",
			"HotKeys":			[{ "Modifiers": "Control", "Key": "D3" }],
			"FileName":			"D:/syncthing/apps/process-hacker/ProcessHacker.exe"
		},
		{
			"Name":				"KeePassXC",
			"HotKeys":			[{ "Modifiers": "Control", "Key": "D4" }],
			"FileName":			"D:/syncthing/apps/keepassxc/KeePassXC.exe",
			"Arguments":		"--allow-screencapture",
			"ProcessName":		"KeePassXC"
		},
		{
			"Name":				"Manual",
			"AttachMode":		"Manual",
			"HotKeys":			[{ "Modifiers": "Control", "Key": "D5" }]
		}
	],

	"HotKeys": [{ "Modifiers": "Control", "Key": "Q" }],

	"PreferMonitor": "WithCursor",

	"MonitorIndex": 0
}
```

```
actionBottomLeft: 0
actionLeft: 0

Screens
=======
Active screen follows mouse:  yes
Number of Screens: 3

Screen 0:
---------
Name: eDP-1
Enabled: 1
Geometry: 0,360,1920x1080
Scale: 1
Refresh Rate: 60003
Adaptive Sync: incapable
Screen 1:
---------
Name: DP-2
Enabled: 1
Geometry: 1920,0,2560x1440
Scale: 1
Refresh Rate: 59951
Adaptive Sync: incapable
Screen 2:
---------
Name: HDMI-A-1
Enabled: 1
Geometry: 4480,0,2560x1440
Scale: 1
Refresh Rate: 59951
Adaptive Sync: incapable

Compositing
===========
Compositing is active
```