"use strict";

console.log("Loading up stuff!! v1");

// Command loop ////////////////////////////////////////////
let wtq = {};

wtq.onGotCommand = (cmdInfoStr) => {
	console.log(`CALLBACK, a:${cmdInfoStr}`);

	try {
		const cmdInfo = JSON.parse(cmdInfoStr);
		console.log(`RESPONSE MESSAGE TYPE: ${cmdInfo.type}`);

		// TODO: Check if session ends.
		// TODO: Handle command

		const cmd = cmds[cmdInfo.type];

		if (typeof cmd === "function") {
			cmd(cmdInfo);
		} else {
			console.log(`Unknown command '${cmdInfo.type}'`);
		}
	} catch (ex) {
		console.log(`OH NOES! Anyway ${ex}`);
	}

	wtq.getNextCommand();
};

wtq.getNextCommand = () => {
	console.log(`GET_NEXT_COMMAND`);

	callDBus(
		"wtq.svc",						// Service
		"/wtq/kwin",					// Path
		"wtq.kwin",						// Interface
		"GetNextCommand",				// Method
		"arg 1",						// Argument 1
		"arg 2",						// Argument 2
		"arg 3",						// Argument 3
		wtq.onGotCommand						// Response callback
	);
};

wtq.getNextCommand();
////////////////////////////////////////////////////////////

// Commands ////////////////////////////////////////////////
let cmds = {};

cmds["BRING_WINDOW_TO_FOREGROUND"] = (cmdInfo) => {

};

cmds["GET_CURSOR_POS"] = (cmdInfo) => {

	console.log(`Get cursor pos! ${JSON.stringify(cmdInfo.params)}`);

	wtq.sendResponse(cmdInfo, workspace.cursorPos);
};

cmds["GET_WINDOW_LIST"] = (cmdInfo) => {
	wtq.sendResponse(
		cmdInfo,
		{
			windows: kwin
				.getWindows()
				.map(w => {
					return {
						internalId: w.internalId,
						resourceClass: w.resourceClass,
						resourceName: w.resourceName
					};
				}),

		}); // TODO: Map to smaller object.
};

cmds["MOVE_WINDOW"] = (cmdInfo) => {

	const w = kwin.getWindowByResourceClass(cmdInfo.params.resourceClass);

	if (!w) {
		console.log("No window found with resource class");
		return;
	}

	// Note that it's important to set the entire "frameGeometry" object in one go, otherwise separate properties may become readonly,
	// allowing us to eg. only set the width, and not the height, or vice versa.
	// Not sure if this is a bug, but it took a bunch of time to figure out.
	w.frameGeometry = {
		x: cmdInfo.params.x,
		y: cmdInfo.params.y,
		width: cmdInfo.params.width,
		height: cmdInfo.params.rectheight,
	};
};

cmds["REGISTER_HOT_KEY"] = (cmdInfo) => {
	registerShortcut(
		cmdInfo.name,
		cmdInfo.title,
		cmdInfo.sequence,
		() => {
			console.log("BLEH! Fire shortcut '{{kwinSequence}}'");
			callDBus("wtq.svc", "/wtq/kwin", "wtq.kwin", "OnPressShortcut", "{{mod}}", "{{key}}");
			console.log("BLEH! /Fire shortcut '{{kwinSequence}}'");
		});
}

cmds["SET_WINDOW_ALWAYS_ON_TOP"] = (cmdInfo) => {

};

cmds["SET_WINDOW_OPACITY"] = (cmdInfo) => {

};

cmds["SET_WINDOW_TASKBAR_ICON_VISIBLE"] = (cmdInfo) => {

};

cmds["SET_WINDOW_VISIBLE"] = (cmdInfo) => {

};
////////////////////////////////////////////////////////////

// KWin Helper Functions ///////////////////////////////////
let kwin = {};

kwin.getWindows = () => {
	
	// KWin5
	if (typeof workspace.clientList === "function") {
		console.log("Fetching window list using 'workspace.clientList()' (KWin5)");
		return workspace.clientList();
	}
	
	// KWin6
	if (typeof workspace.windowList === "function") {
		console.log("Fetching window list using 'workspace.windowList()' (KWin6)");
		return workspace.windowList();
	}
	
	console.log("Could not find function to fetch windows, unsupported version of KWin perhaps?");
};

// TODO: Use ByInternalId instead of ByResourceClass.
// TODO: Use shorter syntax.
kwin.getWindowByInternalId = (internalId) => {
	for (const w in kwin.getWindows()) {
		if (w.internalId === internalId) {
			return w;
		}
	}
};

// TODO: Use shorter syntax.
kwin.getWindowByResourceClass = (resourceClass) => {
	for (const w in kwin.getWindows()) {
		if (w.resourceClass === resourceClass) {
			return w;
		}
	}
};
////////////////////////////////////////////////////////////

// WTQ /////////////////////////////////////////////////////
//let wtq = {};

wtq.log = () => {

};

wtq.sendResponse = (cmdInfo, params) => {
	callDBus(
		"wtq.svc",
		"/wtq/kwin",
		"wtq.kwin",
		"SendResponse",
		JSON.stringify({
			responderId: cmdInfo.responderId,
			params: params,
		}));
};
////////////////////////////////////////////////////////////

// Logging /////////////////////////////////////////////////

// TODO: Try to send stuff to wtq
let log = {};

log.error = () => {
	
};

log.info = () => {

};
////////////////////////////////////////////////////////////
