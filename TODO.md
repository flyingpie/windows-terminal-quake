# TODO

Findings from code review of `feat/remove-winforms-hotkeys` (2026-10-17).

## Behaviour / compatibility

- [ ] **No Windows hotkey fallback when SharpHook fails** — `src/30-Host/Wtq.Host.Windows/ServiceCollectionExtensions.cs:17`
  - With the WinForms `RegisterHotKey` path removed, nothing remains if SharpHook's low-level hook (`WH_KEYBOARD_LL`) doesn't work.
  - Elevated window focused (admin terminal, Task Manager, installer): UIPI blocks hook events to non-elevated WTQ, so the toggle hotkey does nothing. `RegisterHotKey` kept working here.
  - Windows silently removes a low-level hook whose callback exceeds `LowLevelHooksTimeout` (callback calls `GetKeySequence` and logs synchronously), so hotkeys stop until restart.
  - `FeatureFlags.SharpHook=false` used to be the workaround; it no longer exists. Decide: accept and document, or keep a fallback.
- [ ] **Old `"FeatureFlags": { "SharpHook": false }` silently ignored** — `src/10-Core/Wtq/Configuration/FeatureFlags.cs:10`
  - Users who opted out get SharpHook back with no log warning. Consider warning at startup when the old setting is present.
- [ ] **No CHANGELOG entry** — `CHANGELOG.md:3`
  - Add a vNext note that `FeatureFlags.SharpHook` and the WinForms hotkey backend were removed.

## Cleanup

- [ ] **Unused `WtqOptions opts` parameter** in `AddHotkeyService` — `src/30-Host/Wtq.Host.Windows/ServiceCollectionExtensions.cs:11`
  - Drop it, or call `services.AddSharpHookHotkeyService()` directly from `WtqWin32`.
- [ ] **Throwaway `BuildServiceProvider()`** in `WtqWin32.ConfigureServices` — `src/30-Host/Wtq.Host.Windows/WtqWin32.cs:20`
  - Only fed the removed feature-flag branch; builds a second DI container and re-binds settings on every startup (ASP0000). Remove together with the `opts` parameter.
- [ ] **Empty `FeatureFlags` class and `WtqOptions.FeatureFlags` property** — `src/10-Core/Wtq/Configuration/FeatureFlags.cs:10`
  - May show an empty "Feature flags" group in the settings GUI / JSON schema; `docs/pages/index.md` still documents it. Remove, or keep with a placeholder comment.
- [ ] **Stale NOTES.md** — `NOTES.md:20`
  - Still calls the WinForms message-loop method the default and links to the deleted `Native/HotkeyManager.cs`.
- [ ] **Restore a hotkey-backend log line** — `src/30-Host/Wtq.Host.Windows/ServiceCollectionExtensions.cs:17`
  - e.g. `LogInformation("Using SharpHook hotkey service")` so bug-report logs show the backend.
