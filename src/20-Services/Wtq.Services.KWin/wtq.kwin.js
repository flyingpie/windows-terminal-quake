"use strict";

let cmds = {};
let kwin = {};
let log = {};
let wtq = {};

// Logging /////////////////////////////////////////////////

log.log = (level, msg) => {
	// console.log(`${new Date().toISOString()} [${level}] ${msg}`);
	wtq.log(level, msg);
};

log.error		= (msg) => log.log("ERR", msg);
log.info		= (msg) => log.log("INF", msg);
log.warning		= (msg)	=> log.log("WRN", msg);

////////////////////////////////////////////////////////////

// Utils ///////////////////////////////////////////////////

let utils = {};
utils.mapRect = (rect) => {
	return {
		x: Math.round(rect.x),
		y: Math.round(rect.y),
		width: Math.round(rect.width),
		height: Math.round(rect.height),

		top: Math.round(rect.top),
		bottom: Math.round(rect.bottom),
		left: Math.round(rect.left),
		right: Math.round(rect.right),
	};
};

utils.mapWindow = (w) => {
	return {
		caption: w.caption,
		desktopFileName: w.desktopFileName,
		frameGeometry: utils.mapRect(w.frameGeometry),
		hidden: w.hidden,
		internalId: w.internalId,
		keepAbove: w.keepAbove,
		layer: w.layer,
		minimized: w.minimized,
		resourceClass: w.resourceClass,
		resourceName: w.resourceName,
		skipPager: w.skipPager,
		skipSwitcher: w.skipSwitcher,
		skipTaskbar: w.skipTaskbar,
	};
};

////////////////////////////////////////////////////////////

// KWin Helper Functions ///////////////////////////////////

kwin.getWindows = () => {

	// KWin5
	if (typeof workspace.clientList === "function") {
		log.info("Fetching window list using 'workspace.clientList()' (KWin5)");
		return workspace.clientList();
	}

	// KWin6
	if (typeof workspace.windowList === "function") {
		log.info("Fetching window list using 'workspace.windowList()' (KWin6)");
		return workspace.windowList();
	}

	log.warning("Could not find function to fetch windows, unsupported version of KWin perhaps?");
};

kwin.getWindowByInternalId = (internalId) => {
	for (const w of kwin.getWindows()) {
		// "internalId" is an object, so convert to string first.
		// Looks like this:
		// {ec94dfb2-f5fb-4485-bf9d-49658a68b365}
		if (w.internalId.toString() === internalId) {
			return w;
		}
	}
};

kwin.getWindowByInternalIdRequired = (internalId) => {
	const w = kwin.getWindowByInternalId(internalId);

	if (!w) {
		throw `No window found with internal id ${internalId}`;
	}

	return w;
};

kwin.getActiveWindow = () => {
	// KWin5
	if (typeof workspace.activeClient === "object") {
		log.info("Using KWin5 interface 'workspace.activeClient'");
		return workspace.activeClient;
	}

	// KWin6
	if (typeof workspace.activeWindow === "object") {
		log.info("Using KWin6 interface 'workspace.activeWindow'");
		return workspace.activeWindow;
	}

	throw "Could not find property for active client/window, unsupported version of KWin perhaps?";
};

kwin.setActiveWindow = (window) => {
	// KWin5
	if (typeof workspace.activeClient === "object") {
		log.info("Using KWin5 interface 'workspace.activeClient'");
		workspace.activeClient = window;
		return;
	}

	// KWin6
	if (typeof workspace.activeWindow === "object") {
		log.info("Using KWin6 interface 'workspace.activeWindow'");
		workspace.activeWindow = window;
		return;
	}

	throw "Could not find property for active client/window, unsupported version of KWin perhaps?";
};

////////////////////////////////////////////////////////////

// WTQ /////////////////////////////////////////////////////
wtq.DBUS_SERVICE	= "nl.flyingpie.wtq.svc";
wtq.DBUS_PATH		= "/wtq/kwin";
wtq.DBUS_INTERFACE	= "wtq.kwin";

// Send log to WTQ so we can see what's going on in the KWin script.
wtq.log = (level, msg) => {
	callDBus(
		wtq.DBUS_SERVICE,		// Service
		wtq.DBUS_PATH,			// Path
		wtq.DBUS_INTERFACE,		// Interface
		"Log",					// Method
		level,					// Argument 1
		msg,					// Argument 2
	);
};

// Ask WTQ for the next command to execute.
wtq.getNextCommand = () => {
	log.info(`GET_NEXT_COMMAND`);

	callDBus(
		wtq.DBUS_SERVICE,		// Service
		wtq.DBUS_PATH,			// Path
		wtq.DBUS_INTERFACE,		// Interface
		"GetNextCommand",		// Method
		wtq.onGotCommand		// Response callback
	);
};

// Called when a command has been received from WTQ.
wtq.onGotCommand = (cmdInfoStr) => {
	// Deserialize command info from JSON.
	const cmdInfo = JSON.parse(cmdInfoStr);

	try {
		log.info(`COMMAND TYPE: ${cmdInfo.type}`);

		// TODO: Check if session ends.
		const cmd = cmds[cmdInfo.type];

		// See if we can map the received command to a function.
		if (typeof cmd === "function") {
			const respParams = cmd(cmdInfo, cmdInfo.params) ?? {};
			wtq.sendResponse(cmdInfo, respParams);
		} else {
			throw `Unknown command '${cmdInfo.type}'`;
		}
	} catch (ex) {
		log.error(`OH NOES! Anyway ${ex}`);
		wtq.sendResponse(cmdInfo, {}, ex.message);
	}

	wtq.getNextCommand();
};

(() => { wtq.getNextCommand(); })();

wtq.sendResponse = (cmdInfo, params, exception_message) => {
	log.info(`SEND RESPONSE, a:${cmdInfo.type}`);

	callDBus(
		wtq.DBUS_SERVICE,		// Service
		wtq.DBUS_PATH,			// Path
		wtq.DBUS_INTERFACE,		// Interface
		"SendResponse",
		JSON.stringify({
			cmdType: cmdInfo.type,
			responderId: cmdInfo.responderId,
			params: params,
			exception_message: exception_message,
		}));
};
////////////////////////////////////////////////////////////

// Commands ////////////////////////////////////////////////
cmds["BRING_WINDOW_TO_FOREGROUND"] = (cmdInfo) => {
	const p = cmdInfo.params;
	const w = kwin.getWindowByInternalIdRequired(p.internalId);

	log.info(`Bringing to foreground window with internal id '${p.internalId}'`);

	kwin.setActiveWindow(w);
};

cmds["GET_CURSOR_POS"] = (cmdInfo) => {
	return workspace.cursorPos;
};

cmds["GET_FOREGROUND_WINDOW"] = (cmdInfo) => {
	const w = kwin.getActiveWindow();

	return utils.mapWindow(w);
};

cmds["GET_WINDOW"] = (cmdInfo) => {
	const p = cmdInfo.params;
	const w = kwin.getWindowByInternalIdRequired(p.internalId);

	return utils.mapWindow(w);
};

cmds["GET_WINDOW_LIST"] = (cmdInfo) => {
	return {
		windows: kwin
			.getWindows()
			.map(w => utils.mapWindow(w)),
	};
};

cmds["MOVE_WINDOW"] = (cmdInfo, p) => {
	// We used to both move- and resize the window in here, but this caused some issues with multi-monitor setups.
	// Not entirely sure why, apparently KWin doesn't like hammering so many widths and heights.
	// So we split it off, and now do a single RESIZE_WINDOW before a bunch of MOVE_WINDOWs.
	let w = kwin.getWindowByInternalIdRequired(p.internalId);

	log.info(`Moving window with id '${p.internalId}' to x:${p.x}, y:${p.y}`);

	// Note that it's important to set the entire "frameGeometry" object in one go, otherwise separate properties may become readonly,
	// allowing us to eg. only set the width, and not the height, or vice versa.
	// Not sure if this is a bug, but it took a bunch of time to figure out.
	let q = Object.assign({}, w.frameGeometry);

	q.x			= p.x;
	q.y			= p.y;

	w.frameGeometry = q;
};

cmds["RESIZE_WINDOW"] = (cmdInfo) => {
	const p = cmdInfo.params;
	let w = kwin.getWindowByInternalIdRequired(p.internalId);

	log.info(`Resizing window with id '${p.internalId}' to width: ${p.width}, height:${p.height}`);

	// Note that it's important to set the entire "frameGeometry" object in one go, otherwise separate properties may become readonly,
	// allowing us to e.g. only set the width, and not the height, or vice versa.
	// Not sure if this is a bug, but it took a bunch of time to figure out.

	let q = Object.assign({}, w.frameGeometry);

	q.width		= p.width;
	q.height	= p.height;

	w.frameGeometry = q;
};

cmds["NOOP"] = (cmdInfo) => { };

cmds["REGISTER_HOT_KEY"] = (cmdInfo, p) => {
	const descr = `sequence:'${p.sequence}', key char:'${p.keyChar}', key code:'${p.keyCode}', modifier:'${p.mod}'`;

	log.info(`Registering hotkey with name:'${p.name}', ${descr}`);

	registerShortcut(
		p.name,
		p.title,
		p.sequence,
		() => {
			log.info(`Firing hotkey with name:'${p.name}', ${descr}`);

			callDBus(
				wtq.DBUS_SERVICE,		// Service
				wtq.DBUS_PATH,			// Path
				wtq.DBUS_INTERFACE,		// Interface
				"OnPressShortcut",
				p.name,
				p.mod,
				p.keyChar,
				p.keyCode);
		});
}

cmds["SET_WINDOW_ALWAYS_ON_TOP"] = (cmdInfo) => {
	const p = cmdInfo.params;
	const w = kwin.getWindowByInternalIdRequired(p.internalId);

	log.info(`Setting 'always on top'-state for window with internal id '${p.internalId}' to '${p.isAlwaysOnTop}'`);

	// Booleans are currently coming in as strings, because by default booleans are serialized as _True_, which doesn't work in a KWin script.
	// But if we try to set a boolean to a string, it's going to convert to "true" for anything but "", "0", and such.
	// So explicitly convert the string to a boolean.
	// We should look into better JSON boolean conversion, so booleans are non-strings and lower case.
	w.keepAbove = p.isAlwaysOnTop == "true";
};

cmds["SET_WINDOW_OPACITY"] = (cmdInfo) => {
	const p = cmdInfo.params;
	const w = kwin.getWindowByInternalIdRequired(p.internalId);

	log.info(`Setting opacity for window with internal id '${p.internalId}' to '${p.opacity}'`);

	w.opacity = p.opacity;
};

cmds["SET_WINDOW_TASKBAR_ICON_VISIBLE"] = (cmdInfo) => {
	const p = cmdInfo.params;
	const w = kwin.getWindowByInternalIdRequired(p.internalId);

	const skip = !(p.isVisible == "true");

	log.info(`Setting taskbar icon visible for window with internal id '${p.internalId}' to '${p.isVisible}' (skip: ${skip})`);

	w.skipPager = skip;
	w.skipSwitcher = skip;
	w.skipTaskbar = skip;
};
////////////////////////////////////////////////////////////
