# Toggle Animation

## Toggle Animation Type

Which animation type is used during toggle up/down.

Supported values:

- Linear
- EaseInBack
- EaseInCubic
- EaseInOutSine
- EaseInQuart
- EaseOutBack
- EaseOutCubic
- EaseOutQuart

See [easings.net](https://easings.net/) for examples of the easings.

Defaults to ```EaseOutQuart```.

```json
{
	"ToggleAnimationType": "EaseOutQuart"
}
```

<span class="by">Suggested and implemented by [Tim Molderez](https://github.com/timmolderez)</span>

## Toggle Duration

How long it should take for the terminal to come down or go back up.
This is an estimate, since because of the way the toggling works, some slow downs can happen.

Setting this to ```0``` makes the toggle instant.

```json
{
	"ToggleDurationMs": 150
}
```

<span class="by">Suggested by [Mike F](https://github.com/mikef-nl)</span>

## Toggle Mode

How the terminal actually gets toggled on- and off the screen.

Supported values:

**Resize**

Resizes the terminal window vertically until it is either at full size, or shrunk out of the way.
This should work on any setup, but can cause some jumping of the terminal characters, such as the cursor.

**Move**

Moves the entire window off-screen to the top, which prevents jumping of the terminal characters, since the size of the terminal is kept constant.

This doesn't work too great with vertically-stacked monitor setups, since the terminal might be pushed off to the monitor north of the active one.

Defaults to ```Resize```.

```json
{
	"ToggleMode": "Resize"
}
```

<span class="by">Suggested by [Mark Johnson](https://github.com/marxjohnson) and [Scott Hanselman](https://github.com/shanselman)</span>

## Toggle Animation Frame Time

How long each frame in the toggle animation takes in milliseconds.
The lower this value, the smoother the animation, though values lower than 15 are not supported and will result in a toggle taking more than the configured.

Defaults to ```25```.

```json
{
	"ToggleAnimationFrameTimeMs": 25
}
```

<span class="by">Suggested by [Raphael Mobis Tacla](https://github.com/rmobis)</span>
