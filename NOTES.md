# WinGet release
1. Build using "src/build.ps1"
2. Test new version: ```.\Tools\SandboxTest.ps1 .\manifests\f\flyingpie\windows-terminal-quake\prerelease\2.0.0.10\```

# TODO
- Code analysis warnings
- Logging
- Comments
- Options docs + schema
- Logs may not be flushed properly on app exit, dropping some entries.

# KWin Client/Window object
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