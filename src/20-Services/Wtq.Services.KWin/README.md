# Wtq.Services.KWin

## Codegen
```bash
dotnet tool update -g Tmds.DBus.Tool
dotnet dbus codegen --protocol-api --bus session --service org.kde.KWin
```

## API Docs
https://develop.kde.org/docs/plasma/kwin/api/

Example of a KWin "Window" object:
```json
{
	"objectName": "",
	"bufferGeometry": {
		"x": -99,
		"y": 0,
		"width": 100,
		"height": 100,
		"left": -99,
		"right": 1,
		"top": 0,
		"bottom": 100
	},
	"clientGeometry": {
		"x": -99,
		"y": 0,
		"width": 100,
		"height": 100,
		"left": -99,
		"right": 1,
		"top": 0,
		"bottom": 100
	},
	"pos": {
		"x": 0,
		"y": 99
	},
	"size": {
		"width": 1,
		"height": 1
	},
	"x": 0,
	"y": 99,
	"width": 1,
	"height": 1,
	"opacity": 0,
	"output": {
		"objectName": "",
		"geometry": {
			"x": 0,
			"y": 0,
			"width": 1920,
			"height": 1080,
			"left": 0,
			"right": 1919,
			"top": 0,
			"bottom": 1079
		},
		"devicePixelRatio": 1,
		"name": "eDP-1",
		"manufacturer": "BOE",
		"model": "eDP-1-0x093E",
		"serialNumber": ""
	},
	"rect": {
		"x": 0,
		"y": 0,
		"width": 1,
		"height": 1,
		"left": 0,
		"right": 1,
		"top": 0,
		"bottom": 1
	},
	"resourceName": "xwaylandvideobridge",
	"resourceClass": "xwaylandvideobridge",
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
	"skipsCloseAnimation": false,
	"popupWindow": false,
	"outline": false,
	"internalId": "{6766b73f-091d-4adc-8a2d-3cbf8073eeb4}",
	"pid": 1976,
	"stackingOrder": 1,
	"fullScreen": false,
	"fullScreenable": true,
	"active": false,
	"desktops": [
		{
			"objectName": "",
			"id": "84c0483c-dfeb-4e67-9b49-42542177c9f8",
			"x11DesktopNumber": 1,
			"name": "Desktop 1"
		}
	],
	"onAllDesktops": false,
	"activities": [],
	"skipTaskbar": true,
	"skipPager": true,
	"skipSwitcher": true,
	"closeable": true,
	"icon": "",
	"keepAbove": false,
	"keepBelow": false,
	"shadeable": false,
	"shade": false,
	"minimizable": true,
	"minimized": false,
	"iconGeometry": {
		"x": 0,
		"y": 0,
		"width": 0,
		"height": 0,
		"left": 0,
		"right": 0,
		"top": 0,
		"bottom": 0
	},
	"specialWindow": false,
	"demandsAttention": false,
	"caption": "Wayland to X Recording bridge — Xwayland Video Bridge",
	"captionNormal": "Wayland to X Recording bridge — Xwayland Video Bridge",
	"minSize": {
		"width": 0,
		"height": 0
	},
	"maxSize": {
		"width": 2147483647,
		"height": 2147483647
	},
	"wantsInput": true,
	"transient": false,
	"transientFor": null,
	"modal": false,
	"frameGeometry": {
		"x": 0,
		"y": 99,
		"width": 1,
		"height": 1,
		"left": 0,
		"right": 1,
		"top": 99,
		"bottom": 100
	},
	"move": false,
	"resize": false,
	"decorationHasAlpha": false,
	"noBorder": true,
	"providesContextHelp": false,
	"maximizable": true,
	"maximizeMode": 0,
	"moveable": true,
	"moveableAcrossScreens": true,
	"resizeable": true,
	"desktopFileName": "org.kde.xwaylandvideobridge",
	"hasApplicationMenu": false,
	"applicationMenuActive": false,
	"unresponsive": false,
	"colorScheme": "kdeglobals",
	"layer": 2,
	"hidden": false,
	"tile": null,
	"inputMethod": false
}
```