# Logging Plan

This note proposes a practical logging design for `Celarix.Imaging.ImagingPlayground`.

Goals:

- reliable file logging for full-session diagnostics
- a lightweight in-app live log view
- thread-safe UI updates
- bounded memory usage for long-running sessions
- a simple way to open the full log externally

## Recommended overall pattern

Use two distinct logging experiences:

1. a full Serilog file sink as the authoritative log
2. a bounded in-app live log viewer for recent activity only

The in-app viewer should not try to become the long-term archive.

## Why not a multiline `TextBox`

A plain `TextBox` is a poor fit for high-volume live logging because:

- appending gets slower as text grows
- trimming old text is expensive
- memory usage grows with one giant string
- frequent UI-thread updates can cause stutter

For debug-heavy rendering work, this becomes especially risky.

## Recommended in-app control

Prefer one of these:

- `ListBox`
- `ListView`

For now, `ListBox` is the simplest strong option.

### Why `ListBox`

- append-only item model
- easy to cap to the last `N` messages
- much less fragile than one huge text buffer
- easy to auto-scroll
- easy to replace later if needed

### Future option

If needed later, replace the `ListBox` with a custom virtualized log viewer backed by a ring buffer.

That is likely overkill for the first pass.

## File logging

Serilog should be the main logging pipeline.

Suggested file logging characteristics:

- one log file per run or per session
- predictable log folder
- timestamped file names
- plain text output for easy opening in Notepad / editors

### Suggested UI actions

Add one or both:

- `Open Current Log`
- `Open Log Folder`

This keeps the in-app log lightweight while still providing full history externally.

## File locking / external viewing

The intended operational model is:

- Serilog writes the log file continuously
- the user may open the log file in an editor while the app is running

Practical expectation:

- opening the file in Notepad for reading is usually fine
- the app should continue writing
- the editor may or may not refresh live very well, depending on the editor

So the logging architecture should assume:

- in-app log = recent live activity
- external log file = full searchable record

## UI log sink design

Do not scatter `InvokeRequired` logging logic across the app.

Instead, implement one central UI sink.

### Suggested pipeline

1. Serilog emits a log event
2. the UI sink formats it into a display string
3. the sink enqueues the string into a thread-safe queue
4. the sink schedules a UI-thread flush if one is not already pending
5. the UI thread drains a batch and appends items to the UI control

This keeps all cross-thread marshaling in one place.

## Marshaling strategy

Prefer:

- `BeginInvoke`

over:

- `Invoke`

Reason:

- background threads should not block on the UI thread just to write a log line

## Batching strategy

Do not append one UI item per log event synchronously.

Use batching:

- queue log lines from any thread
- flush periodically or on scheduled UI work
- append multiple items per UI pass

Suggested flush cadence:

- every `50ms` to `100ms`
- or one scheduled `BeginInvoke` that drains the queue

This dramatically reduces UI churn.

## Retention strategy

The in-app viewer must be bounded.

Suggested first cap:

- last `2,000` lines

Possible larger cap:

- `5,000` or `10,000`

Only the file log should be effectively unbounded for a session.

### Retention rule

When the cap is exceeded:

- remove oldest entries first

This is a classic ring-buffer style behavior, even if implemented initially with a UI-item cap rather than a fully separate ring-buffer model.

## Suggested UI log content

Keep UI log lines compact and useful.

Suggested display format:

- time
- level
- source or subsystem
- message

Example shape:

```text
[21:14:02.153 INF Rendering.ImageCache] Started load for z=3 tile (12, 8)
```

The full file log can be richer if desired.

## Logging levels

Recommended split:

- file log:
  - `Debug` and above, possibly `Verbose` during deep investigations
- in-app log:
  - `Information` and above by default
  - optionally allow `Debug` temporarily during rendering work

For the rendering rewrite/debug phase, it is reasonable to expose debug logs in the UI as long as retention remains bounded.

## Namespace / subsystem filtering

It may be useful to filter the UI log to important areas, especially:

- `Rendering`
- operations
- major workflow lifecycle events

That prevents the UI from drowning in unrelated noise while the file keeps the full record.

## Auto-scroll behavior

The UI log should auto-scroll only when the user is already at or near the bottom.

That way:

- normal use follows the live stream automatically
- a user inspecting older lines is not constantly yanked back to the bottom

This behavior matters more if a `ListBox` or `ListView` is used.

## Failure safety

The UI sink must stop attempting to update the control if:

- the form is closing
- the control is disposed
- the app is shutting down

The file sink should remain authoritative even if the UI viewer becomes unavailable.

## Suggested control migration

Replace `MainForm.TextLog` with:

- a `ListBox` for the first pass

Possible future upgrade:

- a `ListView` with columns:
  - time
  - level
  - source
  - message

## Suggested first implementation

### Phase 1

- configure Serilog file sink
- choose a log path for the current run
- replace `TextLog` with a bounded `ListBox`
- add a UI sink that batches updates onto the UI thread
- add `Open Current Log`
- optionally add `Open Log Folder`

### Phase 2

- add UI filtering by level and/or subsystem
- add colors or owner-draw if useful
- add session metadata to startup log lines

### Phase 3

- if needed, replace the `ListBox` with a custom virtualized viewer

## Practical conclusion

The right shape here is:

- Serilog for the real log
- bounded recent log in-app
- one central thread-safe UI sink
- external file opening for full detail

That gives you fast feedback during rendering work without making the UI log itself a performance liability.
