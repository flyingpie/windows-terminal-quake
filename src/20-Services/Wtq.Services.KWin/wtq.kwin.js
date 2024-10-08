"use strict";

let cmds = {};
let kwin = {};
let log = {};
let wtq = {};

// Logging /////////////////////////////////////////////////

log.log = (level, msg) => {
	console.log(`${new Date().toISOString()} [${level}] ${msg}`);
	wtq.log(level, msg);
};

log.error		= (msg) => log.log("ERR", msg);
log.info		= (msg) => log.log("INF", msg);
log.warning		= (msg)	=> log.log("WRN", msg);
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

// TODO: Use ByInternalId instead of ByResourceClass.
// TODO: Use shorter syntax.
kwin.getWindowByResourceClass = (resourceClass) => {
	for (const w of kwin.getWindows()) {
		if (w.resourceClass === resourceClass) {
			return w;
		}
	}
};

kwin.getWindowByResourceClassRequired = (resourceClass) => {
	const w = kwin.getWindowByResourceClass(resourceClass);

	if (!w) {
		throw `No window found with resource class ${resourceClass}`;
	}

	return w;
};

kwin.getActiveWindow = (window) => {
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
wtq.DBUS_SERVICE	= "wtq.svc";
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
		"arg 1",				// Argument 1
		"arg 2",				// Argument 2
		"arg 3",				// Argument 3
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
	const w = kwin.getWindowByResourceClassRequired(p.resourceClass);

	log.info(`Bringing to foreground window with resource class '${p.resourceClass}'`);

	kwin.setActiveWindow(w);
};

cmds["GET_CURSOR_POS"] = (cmdInfo) => {
	return workspace.cursorPos;
};

cmds["GET_FOREGROUND_WINDOW"] = (cmdInfo) => {
	const w = kwin.getActiveWindow();

	return {
		frameGeometry: w.frameGeometry,
		hidden: w.hidden,
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

cmds["GET_WINDOW"] = (cmdInfo) => {
	const p = cmdInfo.params;
	const w = kwin.getWindowByResourceClassRequired(p.resourceClass);

	return {
		frameGeometry: w.frameGeometry,
		hidden: w.hidden,
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

cmds["GET_WINDOW_LIST"] = (cmdInfo) => {
	return {
		windows: kwin
			.getWindows()
			.map(w => {
				return {
					internalId: w.internalId,
					resourceClass: w.resourceClass,
					resourceName: w.resourceName
				};
			}),
	};
};

cmds["MOVE_WINDOW"] = (cmdInfo, p) => {
	// We used to both move- and resize the window in here, but this caused some issues with multi-monitor setups.
	// Not entirely sure why, apparently KWin doesn't like hammering so many widths and heights.
	// So we split it off, and now do a single RESIZE_WINDOW before a bunch of MOVE_WINDOWs.
	let w = kwin.getWindowByResourceClassRequired(p.resourceClass);

	log.info(`Moving win ${p.resourceClass} to x:${p.x}, y:${p.y}, width: ${p.width}, height:${p.height}`);

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
	let w = kwin.getWindowByResourceClassRequired(p.resourceClass);

	log.info(`Moving win ${p.resourceClass} to x:${p.x}, y:${p.y}, width: ${p.width}, height:${p.height}`);

	// Note that it's important to set the entire "frameGeometry" object in one go, otherwise separate properties may become readonly,
	// allowing us to eg. only set the width, and not the height, or vice versa.
	// Not sure if this is a bug, but it took a bunch of time to figure out.

	let q = Object.assign({}, w.frameGeometry);

	q.width		= p.width;
	q.height	= p.height;

	w.frameGeometry = q;
};

cmds["NOOP"] = (cmdInfo) => { };

cmds["REGISTER_HOT_KEY"] = (cmdInfo, p) => {
	log.info(`Registering hotkey with name:'${p.name}', sequence:'${p.sequence}', key:'${p.key}' and mod:'${p.mod}'`);

	registerShortcut(
		p.name,
		p.title,
		p.sequence,
		() => {
			log.info(`Firing hotkey with name:'${p.name}', sequence:'${p.sequence}', key:'${p.key}' and mod:'${p.mod}'`);

			callDBus(
				"wtq.svc",
				"/wtq/kwin",
				"wtq.kwin",
				"OnPressShortcut",
				p.mod,
				p.key);
		});
}

cmds["SET_WINDOW_ALWAYS_ON_TOP"] = (cmdInfo) => {
	const p = cmdInfo.params;
	const w = kwin.getWindowByResourceClassRequired(p.resourceClass);

	log.info(`Setting 'always on top'-state for window with resource class '${p.resourceClass}' to '${p.isAlwaysOnTop}'`);

	w.keepAbove = p.isAlwaysOnTop;
};

cmds["SET_WINDOW_OPACITY"] = (cmdInfo) => {
	const p = cmdInfo.params;
	const w = kwin.getWindowByResourceClassRequired(p.resourceClass);

	log.info(`Setting opacity for window with resource class '${p.resourceClass}' to '${p.opacity}'`);

	w.opacity = p.opacity;
};

cmds["SET_WINDOW_TASKBAR_ICON_VISIBLE"] = (cmdInfo) => {
	const p = cmdInfo.params;
	const w = kwin.getWindowByResourceClassRequired(p.resourceClass);

	const skip = !(p.isVisible == "true");

	log.info(`Setting taskbar icon visible for window with resource class '${p.resourceClass}' to '${p.isVisible}' (skip: ${skip})`);

	w.skipPager = skip;
	w.skipSwitcher = skip;
	w.skipTaskbar = skip;
};

cmds["SET_WINDOW_VISIBLE"] = (cmdInfo) => {
	const p = cmdInfo.params;
	const w = kwin.getWindowByResourceClassRequired(p.resourceClass);

	log.info(`Setting visibility of window with resource class '${p.resourceClass}' to '${p.opacity}' to '${p.isVisible}'`);

	w.minimized = p.isVisible;
};
////////////////////////////////////////////////////////////
