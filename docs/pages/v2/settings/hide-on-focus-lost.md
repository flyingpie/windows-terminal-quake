# Hide On Focus Lost

When clicking or alt-tabbing away to another app, the terminal will automatically (and instantly) hide.

Defaults to ```true```.

## Global
```json
{
	"HideOnFocusLost": false
}
```

## Per App
```json
{
	"Apps": [
		{
			"Name": "Terminal",
			"HideOnFocusLost": false
			...
		}
	]
}
```

<span class="by">Suggested by [Douglas Lara](https://github.com/douglara)</span>
