# Prefer Monitor

What monitor to preferrably drop the terminal.

Can be "WithCursor" (default), "Primary" or "AtIndex".

If "PreferMonitor" is set to "AtIndex", the "MonitorIndex"-setting determines what monitor to choose.
Zero based, eg. 0, 1, etc.

Defaults to "WithCursor".

```json
{
  "PreferMonitor": "AtIndex",
  "MonitorIndex": 1
}
```

<span class="by">Suggested by [Christian Käser](https://github.com/dfyx)</span>
