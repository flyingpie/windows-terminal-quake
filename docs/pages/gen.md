### WtqOptions

#### 

##### Schema

<div>
            Path to wtq.schema.json.
            </div>

<remarks />

<div />

---

##### Animation target FPS

<div>
            How many frames per second the animation should be.<br />
            Note that this may not be hit if moving windows takes too long, hence "target" fps.<br />
            Must be between 5 and 120, to prevent issues that can arise with values that are too low or too high.<br />
            </div>

<remarks />

<div />

---

##### IsValid

<div>
            Convenience property.
            </div>

<remarks />

<div />

---

##### ValidationResults

<div>
            Convenience property to make binding from the GUI easier.
            </div>

<remarks />

<div />

---

##### Apps

<div>
            Applications to enable Quake-style dropdown for.
            </div>

<remarks />

<div />

---

##### Hotkeys

<div>
            Global hotkeys, that toggle either the first, or the most recently toggled app.
            </div>

<remarks />

<div />

---

##### Show UI on start

<div>
            Whether to show the GUI when WTQ is started.
            </div>

<remarks />

<div />

---

##### Attach mode

<div>
            How WTQ should get to an instance of a running app.<br />
            I.e. whether to start an app instance if one cannot be found.
            </div>

<remarks />

<div />

---

##### Always on top

<div>
            Whether the app should always be on top of other windows, regardless of whether it has focus.
            </div>

<remarks />

<div />

---

##### Hide on focus lost

<div>
            Whether the app should be toggled off when another app gets focus.
            </div>

<remarks />

<div />

---

##### Taskbar icon visibility

<div>
            When to show the app window icon on the taskbar.
            </div>

<remarks />

<div />

---

##### Opacity

<div>
            Make the window see-through (applies to the entire window, including the title bar).<br />
            0 (invisible) - 100 (opaque).
            </div>

<remarks />

<div />

---

##### Horizontal screen coverage

<div>
            Horizontal screen coverage, as a percentage.
            </div>

<remarks />

<div />

---

##### Horizontal align

<div>
            Where to position an app on the chosen monitor, horizontally.
            </div>

<remarks />

<div />

---

##### Vertical screen coverage

<div>
            Vertical screen coverage as a percentage (0-100).
            </div>

<remarks />

<div />

---

##### Vertical offset

<div>
            How much room to leave between the top of the app window and the top of the screen, in pixels.
            </div>

<remarks />

<div />

---

##### Off-screen locations

<div>
            When moving an app off the screen, WTQ looks for an empty space to move the window to.<br />
            Depending on your monitor setup, this may be above the screen, but switches to below if another monitor exists there.<br />
            By default, WTQ looks for empty space in this order: Above, Below, Left, Right.
            </div>

<remarks />

<div />

---

##### Prefer monitor

<div>
            Which monitor to preferably drop the app.
            </div>

<remarks />

<div />

---

##### Monitor index

<div>
            If <strong>PreferMonitor</strong> is set to <strong>AtIndex</strong>, this setting determines what monitor to choose.<br />
            Zero based, e.g. 0, 1, etc.
            </div>

<remarks />

<div />

---

##### Animation duration

<div>
            How long the animation should take, in milliseconds.
            </div>

<remarks />

<div />

---

##### Animation type (toggle ON)

<div>
            The animation type to use when toggling on an application.
            </div>

<remarks />

<div />

---

##### Animation type (toggle OFF)

<div>
            The animation type to use when toggling off an application.
            </div>

<remarks />

<div />

---



### WtqAppOptions

#### 

##### Global

<div>
            Used to refer from the app options object back to the global one, for cascading.
            </div>

<remarks />

<div />

---

##### IsValid

<div />

<remarks />

<div />

---

##### ValidationResults

<div />

<remarks />

<div />

---

#### App

##### Name

<div>
            A logical name for the app, used to identify it across config reloads.<br />
            Appears in logs.
            </div>

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

<div>
            One or more keyboard shortcuts that toggle in- and out this particular app.
            </div>

<remarks />

<div />

---

#### Process

##### Filename

<div>
            The <strong>filename</strong> to use when starting a new process for the app.<br />
            E.g. <strong>notepad</strong>, <strong>dolphin</strong>, etc.
            </div>

!!! note
	See the "Examples" page in the GUI for, well, examples.

<div />

---

##### Process name

<div>
            Apps sometimes have <Emph>process names</Emph> different from their <Emph>filenames</Emph>.
            This field can be used to look for the process name in such cases. Windows Terminal is an
            example, with filename <Emph>wt</Emph>, and process name <Emph>WindowsTerminal</Emph>.
            </div>

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

#### 

##### Arguments

<div>
            Command-line arguments that should be passed to the app when it's started.<br />
            Note that this only applies when using an <see cref="T:Wtq.Configuration.AttachMode" /> that starts the app.
            </div>

<remarks />

<div />

---

#### Process

##### Argument list

<div />

<remarks />

<div />

---

#### 

##### Attach mode

<div />

<remarks />

<div />

---

#### Process

##### Window title

<div />

<remarks />

<div />

---

#### 

##### Always on top

<div />

<remarks />

<div />

---

##### Hide on focus lost

<div />

<remarks />

<div />

---

##### Taskbar icon visibility

<div />

<remarks />

<div />

---

##### Opacity

<div />

<remarks />

<div />

---

#### Behavior

##### Window title override

<div>
            Attempt to set the window title to a specific value.
            </div>

<remarks />

<div />

---

#### 

##### Horizontal screen coverage

<div />

<remarks />

<div />

---

##### Horizontal align

<div />

<remarks />

<div />

---

##### Vertical screen coverage

<div />

<remarks />

<div />

---

##### Vertical offset

<div />

<remarks />

<div />

---

##### Off-screen locations

<div />

<remarks />

<div />

---

##### Prefer monitor

<div />

<remarks />

<div />

---

##### Monitor index

<div />

<remarks />

<div />

---

##### Animation duration

<div />

<remarks />

<div />

---

##### Animation type (toggle ON)

<div />

<remarks />

<div />

---

##### Animation type (toggle OFF)

<div />

<remarks />

<div />

---



