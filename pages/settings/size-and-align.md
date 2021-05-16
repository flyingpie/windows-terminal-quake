# Size & Align

## Horizontal Align

When "HorizontalScreenCoverage" is below 100, this setting determines where the terminal is place horizontally.

Possible values:

- Left
- Center (default)
- Right

```json
{
	"HorizontalAlign": "Center"
}
```

**Left**
![Left](https://user-images.githubusercontent.com/1295673/95656847-467ed980-0b11-11eb-87a4-2bff809c30d0.png)

**Center**
![Center](https://user-images.githubusercontent.com/1295673/95656889-8645c100-0b11-11eb-8310-c829f41e76bc.png)

**Right**
![Right](https://user-images.githubusercontent.com/1295673/95656866-6d3d1000-0b11-11eb-9680-cb67d1c5cc6c.png)

<span class="by">Suggested by [Rafael Pereira](https://github.com/bsides)</span>

## Horizontal Screen Coverage

How much horizontal space the terminal should use. When this is below 100, the terminal is centered (can be changed using the [Horizontal Align](/settings/horizontal-align)) setting.

This can also be above 100, which can be useful to compensate for the window not always taking a 100% of the screen width, due to the terminal being bound to column widths (eg. try 100.5 or 101).

```json
{
	"HorizontalScreenCoverage": 80
}
```

![Horizontal Coverage](https://files.flyingpie.nl/windows-terminal-quake/horizontal-coverage.png)

<span class="by">Suggested by [baslas](https://github.com/baslas)</span>

## Vertical Offset

How much room to leave between the top of the terminal and the top of the screen.

```json
{
  // How much room to leave between the top of the terminal and the top of the screen
  "VerticalOffset": 50
}
```

## Vertical Screen Coverage

The vertical space that is taken up by the terminal window can be configured through "VerticalScreenCoverage".

```json
{
  // How far the terminal should come down, in percentage (eg. 50 = half way, 100 = full screen)
  "VerticalScreenCoverage": 60
}
```

![Vertical Screen Coverage](https://files.flyingpie.nl/windows-terminal-quake/vertical-coverage.png)

<span class="by">Suggested by [ukWaqas](https://github.com/ukWaqas)</span>

![Vertical Offset](https://user-images.githubusercontent.com/1295673/95657244-b9894f80-0b13-11eb-97d5-3f984f4de912.png)

<span class="by">Suggested by [Neil Santos](https://github.com/nlsantos)</span>

## Maximize After Toggle

Whether to maximize the terminal after it has toggled into view.</para>
Note that this only applies when both ```HorizontalScreenCoverage``` and ```VerticalScreenCoverage``` are at least 100.

This is to fix the issue where the terminal columns don't line up with the width and height of the screen.

Since the terminal sizes in incremental values of 1 column, 100% horizontal coverage can actually mean slightly under the full width of the screen.

Defaults to ```true```.

```json
{
	"MaximizeAfterToggle": true
}
```
