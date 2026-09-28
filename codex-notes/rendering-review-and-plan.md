# Rendering Review And Plan

This note captures two things:

1. the main correctness risks in the current `Rendering` namespace
2. a preservation-oriented plan for stabilizing it without throwing away `InfiniteCanvasControl`, `ImageCache`, or `ImageEntry`

## Current correctness risks

### Visibility is not tied tightly enough to camera motion

Right now, image visibility is updated when content is loaded into the control, but not consistently when the viewport changes because of:

- panning
- drag inertia
- mouse-wheel zoom

That means the viewer can keep drawing only the originally loaded set even after the user has moved somewhere else.

### In-flight loads outlive their usefulness

`ImageEntry` can be:

- loading
- canceled
- evicted
- removed from the cache

but an old background load can still complete later and try to install its image anyway.

This creates several kinds of risk:

- stale tiles becoming visible after they are no longer wanted
- resurrected images after `Clear`
- wasted decoding and allocation work
- leaked or orphaned `SKImage` ownership

### Cancellation is not generation-safe

The current model can cancel a `CancellationTokenSource`, but it does not associate a load request with a unique generation / epoch.

That means:

- old requests can complete after newer ones start
- the code has no hard rule for ignoring stale completions
- state transitions depend too much on timing

This is the classic source of intermittent viewer bugs.

### Load state is ahead of resource state

An entry can become logically `Loaded` before the `SKImage` is actually installed on the UI thread.

That makes the state machine inconsistent:

- the cache thinks the tile is loaded
- the actual image may still be null
- memory accounting may be wrong during the gap

### Memory budgeting is optimistic in the wrong places

The current cache accounting appears to over-count some already-loaded images during visibility refresh and can therefore stop loading visible tiles earlier than intended.

Also, in-memory ImageSharp images used to create Skia images need careful disposal, or large artifacts will briefly or permanently exist in both forms.

### Cache keys and mutable metadata

`CanvasImage` currently behaves like a metadata container and also acts as a dictionary key.

That is risky when fields that affect equality or hash identity can change later, especially size.

Even if this has not blown up yet, it is the kind of hidden hazard that tends to surface later.

### Zoom-level behavior is under-specified

The current code has the beginnings of zoom-aware rendering via `CanvasImage.OnlyAtZoomLevel`, but zoom changes are not yet treated as major state transitions.

Without that, fast zooming can cause:

- load storms
- many requests for the wrong level
- flicker
- incomplete level swaps
- poor memory behavior

## Preservation-oriented direction

The right move is not a rewrite. The right move is to preserve the current structure and tighten responsibilities.

Keep:

- `InfiniteCanvasControl`
- `ImageCache`
- `ImageEntry`
- `CanvasImage`
- `Viewport`

But make the state transitions, ownership boundaries, and zoom-level behavior explicit.

## Proposed responsibilities

## Cache operating modes

`ImageCache` should be general-purpose enough to support the main artifact shapes the app can display, not only zoomable canvases.

Prefer a mode enum over a loose boolean such as `ZoomLevelsEnabled`.

Suggested shape:

- `SingleImage`
- `MultiImageStatic`
- `ZoomableCanvas`

### `SingleImage`

- one image
- one working set
- no zoom-level transitions
- no fallback layers

### `MultiImageStatic`

- multiple images in fixed world positions
- one working set
- no zoom-level transitions
- no fallback layers

This mode is a good fit for:

- striped temp-image output
- banded intermediate images

### `ZoomableCanvas`

- multiple zoom levels
- multiple working sets may coexist temporarily
- fallback layers are allowed during transitions
- zoom-level transitions are first-class events

This keeps the architecture unified without forcing map-style complexity onto every artifact type.

### `InfiniteCanvasControl`

Should be responsible for:

- input handling
- camera state
- converting camera state into a viewport description
- asking the cache what should be shown
- drawing already-available images

It should not be the place where load-state decisions become ad hoc.

### `ImageCache`

Should be responsible for:

- deciding which working sets are active
- deciding which entries are needed now
- starting loads
- canceling old loads
- eviction policy
- zoom-level presentation transitions

It should be the authority on visibility and desired state.

In non-zoom modes, `ImageCache` still owns the single working set and entry lifecycle, but zoom-level transition logic is inactive.

### `ImageEntry`

Should be responsible for exactly one tile or image artifact:

- unloaded
- queued or wanted
- loading
- loaded
- faulted

It should own:

- its current `SKImage`, if any
- its current load request generation
- its cancellation token source for the active request

It should reject stale completions.

### `CanvasImage`

Should become immutable metadata describing a drawable asset:

- source identity
- world/tile coordinates
- pixel size
- zoom level association

It should not mutate in ways that invalidate cache identity.

### `WorkingSet`

Should own the full set of `ImageEntry` objects for one artifact slice:

- the single image
- the full striped/static image list
- or one zoom level of a zoomable canvas

The intended split is:

- `WorkingSet.ImageEntries`: all entries owned by the set
- `WorkingSet.VisibleSet`: the currently visible and drawable subset

That keeps membership stable while allowing `SKImage` residency to change freely.

## Zoom levels as first-class events

This is the major new idea needed for correctness.

Zoom changes should not be treated as just another visibility refresh. They should be treated as presentation-set transitions.

That means the cache should track concepts like:

- presented zoom level
- target zoom level
- current load epoch
- visible working set for that epoch

This logic is only active in `ZoomableCanvas` mode.

## Working-set model

At any moment, the cache should think in terms of a working set:

- which entries belong to the current artifact or zoom level
- which of those entries are currently visible
- which tiles are needed for the target level in zoomable-canvas mode
- which already-loaded older tiles can serve as fallback while the target level is loading

### Same-level viewport motion

When the user pans within the same presentation zoom level:

- recompute the visible tiles for that level
- start newly needed loads
- cancel no-longer-needed pending loads
- mark invisible loaded tiles as evictable

This is incremental behavior.

In `SingleImage` and `MultiImageStatic` modes, this is the only kind of working-set update that exists.

The intended call flow is:

1. `InfiniteCanvasControl` detects a viewport change
2. `ImageCache.ViewportChanged(...)` updates cache-level state
3. `ImageCache` calls `ViewportChanged(...)` on relevant working sets
4. each `WorkingSet` recomputes its `VisibleSet` and starts/cancels loads on its own entries
5. working sets raise `VisibleSetChanged`
6. `ImageCache` recomposes the overall visible set and raises its own `VisibleSetChanged`
7. the control updates its cached draw list and invalidates

### Zoom-level change

When the user crosses into a different presentation zoom level:

- create a new load epoch
- record a new target zoom level
- cancel pending loads from the old epoch
- keep already-loaded tiles from the old presented level visible as fallback
- start loading the needed tiles for the new target level
- switch presentation to the new level only when the chosen completion rule is met

This makes zoom changes explicit and prevents chaotic thrashing.

## Suggested promotion rule

The simplest initial promotion rule is:

- keep presenting the old level
- promote to the new level once all tiles required to cover the current viewport at the new level are loaded

That is conservative but conceptually clean.

Later, this could be relaxed to:

- full viewport coverage
- partial threshold
- timeout-based fallback

The important nuance from later discussion is that a newer working set may become partially visible before it is complete, while older fallback sets remain visible only where they still contribute coverage.

## Integer world math and local screen math

This is the preferred direction.

## Recommended coordinate model

Do not rely on one giant floating-point transform matrix as the authoritative model for the world.

Instead:

- store tile/world positions as integers
- store zoom level as an integer
- store tile coordinates as integers
- compute screen-space draw positions relative to the viewport origin at draw time

Then drawing becomes conceptually:

- determine which images are visible
- for each visible image, compute a local screen coordinate
- issue `DrawImage` using that small screen coordinate

This has several advantages:

- avoids depending heavily on large float-space transforms
- reduces precision fear for enormous canvases
- keeps the main geometry model discrete and exact
- makes tile addressing easier to reason about

Skia can still draw the images. The important change is that the authoritative camera/world model is integer-heavy and local-coordinate-based, not "trust one giant matrix forever."

## Practical interpretation

Yes: this likely means moving away from relying on Skia's global matrix transform as the primary navigation model.

Instead, a better model is:

- `CanvasImage` keeps very large integer world coordinates
- viewport origin is tracked in integer-like world units as much as practical
- draw calls receive small local coordinates near the screen origin

So the runtime operation becomes much more like:

- "please draw this `SKImage` at this screen coordinate"

rather than:

- "apply a huge transform and hope the floating-point math remains nice at extreme scales"

## Proposed `ImageEntry` state model

The exact enum names can vary, but the shape should be something like:

- `Unloaded`
- `Wanted`
- `Loading`
- `Loaded`
- `Faulted`

And each entry should also carry:

- active epoch / generation id
- whether it belongs to the target zoom level
- whether it is currently presented
- whether it is only fallback-visible

## Required invariants

These are the important rules:

- only one active load request per `ImageEntry`
- every async completion must check whether its generation is still current
- an entry is not `Loaded` until the `SKImage` is actually installed
- `Clear` and removal operations must cancel active loads before disposing state
- cache accounting must distinguish:
  - already loaded bytes
  - bytes expected if queued loads complete

## Implementation direction

The next implementation pass should aim for:

1. make viewport changes call into cache visibility updates
2. add generation ids to `ImageEntry`
3. make stale completions no-op
4. unify cancel/remove/clear semantics
5. make zoom-level transitions explicit in `ImageCache`
6. reduce reliance on giant world-space float transforms
7. keep old level tiles as fallback during zoom-level promotion

## Bottom line

This system does not need to be discarded.

It needs:

- a stricter `ImageEntry` lifecycle
- a stronger `ImageCache` authority over working sets
- zoom-level changes treated as real events
- more integer-based world/tile math

That should preserve the current approach while making it much less likely to suffer from timing bugs, stale loads, and large-space precision surprises.
