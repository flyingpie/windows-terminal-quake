# Toggle Duration

How long it should take for the terminal to come down or go back up.
This is an estimate, since because of the way the toggling works, some slow downs can happen.

Setting this to ```0``` makes the toggle instant.

```json
{
	"ToggleDurationMs": 150
}
```

<span class="by">Suggested by [Mike F](https://github.com/mikef-nl)</span>

# Toggle Animation Frame Time

How long each frame in the toggle animation takes in milliseconds.
The lower this value, the smoother the animation, though values lower than 15 are not supported and will result in a toggle taking more than the configured.

Defaults to ```25```.

```json
{
	"ToggleAnimationFrameTimeMs": 25
}
```

<span class="by">Suggested by [Raphael Mobis Tacla](https://github.com/rmobis)</span>
