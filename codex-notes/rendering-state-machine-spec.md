# Rendering State Machine Spec

This note defines a concrete state-machine-oriented design for the `Rendering` namespace while preserving the current structure:

- `InfiniteCanvasControl`
- `ImageCache`
- `ImageEntry`
- `CanvasImage`
- `Viewport`

It also reflects important design decisions:

- during zoom transitions, tiles from multiple zoom levels may be visible at once
- this is intentional transitional behavior, not a bug
- `WorkingSet` owns `Epoch`
- `ImageEntry` uses `Unloaded` as its normal reloadable non-resident state
- cancellation is treated primarily as a transition cause, not a long-lived steady state
- cache and working-set mutation should occur on the UI thread, with background work limited to decoding/loading
- `ImageCache` supports multiple artifact modes, with zoom-transition logic active only for zoomable canvases
- each `WorkingSet` owns the full set of `ImageEntry` objects for its artifact slice
- working-set liveness is primarily determined by membership in `ImageCache.WorkingSets`

## Core idea

The renderer should be modeled around:

- tile metadata
- per-tile load state
- per-working-set entry ownership
- one viewport-driven target set
- one or more fallback sets that may continue contributing visible tiles temporarily

This is closer to map rendering than to traditional image viewing.

## Coordinate model

Authoritative coordinates should be integer-heavy:

- tile coordinates: integers
- zoom levels: integers
- world positions: large integers
- tile sizes: integers

At draw time, visible tiles are converted to local screen-space rectangles:

- small screen-space positions
- small screen-space sizes

Skia should be used as the raster renderer, not as the primary owner of world navigation math.

## `ImageCache` operating modes

`ImageCache` should have an explicit operating mode.

Suggested enum:

- `SingleImage`
- `MultiImageStatic`
- `ZoomableCanvas`

### `SingleImage`

- exactly one working set
- exactly one image entry in the simplest case
- no zoom-level transitions
- no fallback layers

### `MultiImageStatic`

- exactly one working set
- multiple fixed-position image entries
- no zoom-level transitions
- no fallback layers

Good fit for:

- striped output shown as multiple fixed-position images
- banded temporary image folders

### `ZoomableCanvas`

- one or more active working sets
- zoom-level transitions
- fallback presentation from older levels
- `Epoch`-based invalidation

This mode is the only one that needs the full transition model.

## Main objects

### `CanvasImage`

Immutable metadata for one drawable source.

Suggested responsibilities:

- identify the tile or image source
- know its zoom level
- know its world/tile location
- know its intrinsic pixel size
- provide a factory for loading / decoding into `SKImage`
- provide a size estimate

Suggested invariant:

- a `CanvasImage` never mutates in ways that affect identity

### `CanvasImage` identity

Do not use the loading factory delegate itself as identity.

Preferred identity shape:

- `ContentId`
- `ZoomLevel`
- `TileX`
- `TileY`

That can be represented as a structural tile key.

For a non-tiled single image, this can still work by treating the image as:

- one content id
- one tile at `(0, 0)`
- no special-case random identity required

For striped/static multi-image mode, immutable canvas coordinates are also valid structural identity, as long as they are represented by an explicit equality-bearing key type rather than relying on object hash codes alone.

## `ImageEntry`

Represents one tile/image in one working set.

Owns:

- current load state
- current `SKImage`, if loaded
- cancellation token source for the active load
- load generation id
- byte size for accounting

### Suggested `ImageEntryState`

- `Unloaded`
- `Loading`
- `Loaded`
- `Faulted`
- `Disposed`

### Meaning of each state

- `Unloaded`
  - no active load
  - no loaded `SKImage`
  - may be loaded again later
- `Loading`
  - one active load request exists
  - may be canceled
- `Loaded`
  - `SKImage` is installed and owned by the entry
  - eligible for drawing
- `Faulted`
  - last load attempt failed
  - no active request
  - may retry later
- `Disposed`
  - terminal
  - image disposed
  - no future transitions allowed

### `ImageEntry` fields needed for correctness

- `State`
- `LoadGeneration`
- `TileKey`
- `LastUsedTick`
- `ByteSize`

### `ImageEntry` transitions

#### `Unloaded -> Loading`

When:

- entry is wanted by an active working set
- memory policy allows a load attempt

Actions:

- increment `LoadGeneration`
- create CTS
- start async load

#### `Loading -> Loaded`

When:

- async load completes successfully
- generation matches current `LoadGeneration`
- entry is not disposed
- owning working set is still active

Actions:

- install `SKImage`
- compute `ByteSize`
- clear CTS
- mark state `Loaded`

Important rule:

- do not mark the entry `Loaded` before the `SKImage` is actually installed

#### `Loading -> Unloaded`

When:

- owning working set is canceled
- tile is no longer wanted
- global zoom transition invalidates this request

Actions:

- cancel CTS
- clear CTS
- leave no active load behind
- do not keep a long-lived `Canceled` state

#### `Loading -> Faulted`

When:

- async load fails
- generation still matches
- entry is still active

Actions:

- record fault state
- clear CTS
- no `SKImage` retained

#### `Faulted -> Loading`

When:

- retry policy decides to try again

#### `Loaded -> Disposed`

When:

- owning working set is disposed
- or entry is explicitly removed

Actions:

- dispose `SKImage`
- zero memory accounting
- clear flags

#### `Any non-terminal state -> Disposed`

When:

- cache clear
- working set disposal
- control disposal

Actions:

- cancel active load if any
- dispose loaded image if any
- mark terminal

### Generation rule

Every async completion must check:

- current `LoadGeneration`
- owning working set still active
- entry not disposed

If any check fails:

- dispose the newly decoded `SKImage`
- do not mutate visible state

This is the key anti-race invariant.

## Thread ownership rule

To keep the model tractable:

- background threads should only perform loading/decoding work
- cache, working-set, and entry state mutation should occur on the UI thread

That means:

- worker thread loads or decodes an `SKImage`
- completion is marshaled back to the UI thread
- the UI thread decides whether the result is still current
- only then is the result installed or discarded

This is the preferred way to keep thread-safety manageable.

## Working sets

A working set is a viewport-specific tile set for one zoom level and one epoch.

It represents:

- which entries exist for that artifact slice
- which entries are currently visible
- whether the set is still active
- whether the set contributes visible coverage

### Suggested `WorkingSetState`

- `Building`
- `Loading`
- `Active`
- `Superseded`
- `Canceled`

### Meaning of each state

- `Building`
  - tile keys are being computed
  - entries are being created
- `Loading`
  - entries are loading or waiting to load
- `Active`
  - may contribute tiles to the screen
  - may be target or fallback
- `Superseded`
  - newer target exists
  - may still contribute visible fallback tiles
- `Canceled`
  - no new loads should start
  - active loads should be canceled

### Working set properties

- `Epoch`
- `ZoomLevel`
- `ImageEntries`
- `VisibleSet`
- `IsTarget`
- `IsPrimaryPresented`
- `IsFallbackPresented`
- `LoadedTileCount`
- `VisibleLoadedTileCount`
- `ActiveLoadCount`

`DesiredTileKeys` is no longer the preferred central abstraction.

The clarified design is:

- `ImageEntries`: the full immutable membership of the set
- `VisibleSet`: the current viewport-derived drawable subset

## Working set transitions

### `Building -> Loading`

When:

- desired visible tile keys are computed
- entries are initialized

In the clarified design, initialization means creating the full `ImageEntries` collection for the set. Visibility remains a derived subset.

### `Loading -> Active`

When:

- at least one tile is loaded

This is important because zoom transitions may allow partial visibility from a set before it is "complete."

In `SingleImage` and `MultiImageStatic` modes, the same transition still makes sense, but there is only one working set and no older fallback sets.

### `Loading -> Canceled`

When:

- a newer zoom transition replaces this set before presentation completes

Actions:

- cancel all active loads

### `Canceled -> Removed`

When:

- no tiles were ever loaded

This is the immediate-drop rule for abandoned, never-visible working sets.

The preferred mechanism is removal from `ImageCache.WorkingSets`, rather than a long-lived explicit terminal state on the working set object itself.

### `Active -> Superseded`

When:

- a newer target working set is created

This set may remain visible as fallback.

### `Superseded -> Removed`

When:

- it contributes no visible tiles
- and it has no active loads worth keeping

This is how older levels get dropped quickly once the newer level covers the viewport.

Removal from the cache list replaces most uses of an explicit terminal working-set state.

## Multiple visible zoom levels

This is allowed and expected during transitions.

At a given moment, the screen may contain:

- current target-level tiles that have already loaded
- older fallback-level tiles covering areas the target has not filled yet

The rule is:

- newest available tile wins for any screen region it covers
- older levels are drawn only where newer levels have not yet supplied coverage

That means visibility is effectively layered by zoom-level recency.

## Composition rule

For drawing:

1. gather all active working sets that still contribute visible coverage
2. sort them from oldest fallback to newest target
3. draw oldest first
4. draw newer levels on top

This allows partial replacement naturally.

## Cache-level policy

`ImageCache` should manage:

- current viewport snapshot
- active working sets
- target zoom level
- epoch counter
- memory budget

### Cache-level responsibilities

- compute visible tile keys for the viewport
- detect whether a zoom-level change happened
- create new target working sets
- supersede older sets
- cancel irrelevant loads
- remove dead sets from the cache
- produce a stable draw list

Visibility and fallback contribution are best treated as cache-derived facts, not as persistent booleans on `ImageEntry`.

## Zoom-level change policy

### On zoom-level change

1. increment epoch
2. create new working set for the new zoom level
3. mark current target, if any, as canceled
4. if the canceled target never loaded a tile, remove it immediately
5. mark currently visible older sets as fallback-capable
6. start loads for the new target set

### Why this works

This prevents an explosion of long-lived sets:

- an abandoned target that never visibly contributed is removed immediately
- a partially visible or previously presented set may remain temporarily
- only sets that still help cover the viewport are retained

## Same-zoom-level pan policy

When the viewport moves without changing zoom level:

- update the visible subset of the current target/presented set
- start loads for newly visible tiles
- cancel loads for tiles no longer needed
- keep loaded tiles if they still contribute visible fallback or nearby reuse value
- mark invisible loaded tiles evictable

This should feel incremental, unlike zoom transitions.

## Disposal rules

### Immediate disposal

Remove a working set from `ImageCache.WorkingSets` immediately if:

- it was canceled
- it never loaded any tiles
- it is not currently visible

### Deferred disposal

Keep a set temporarily if:

- some of its loaded tiles are still visible on screen
- newer sets do not yet cover the same visible area

Remove it from the cache as soon as:

- it no longer contributes visible coverage
- and it has no useful active loads

## Memory accounting rules

The cache should track separately:

- bytes currently loaded
- bytes expected from currently active loads

Do not double-count already-loaded tiles when considering whether a new load may start.

## Budget model

It is reasonable to distinguish:

- `Soft budget`
- `Hard budget`

### Soft budget

- preferred operating range
- if exceeded, begin evicting invisible or non-contributing tiles aggressively
- avoid loading tiles that are not currently visible

### Hard budget

- emergency ceiling
- if exceeded, degrade more aggressively:
  - cancel lower-priority in-flight loads
  - evict invisible loaded tiles
  - evict less-useful fallback tiles if absolutely necessary

Visible tiles should not be the first thing sacrificed, but the hard budget exists to prevent uncontrolled growth.

## Recommended load-start order

When many tiles are needed, start with the tiles most useful to the current frame:

- tiles nearest viewport center first
- then tiles outward by distance

This is much better than arbitrary hash-set order and reduces visible holes.

## `InfiniteCanvasControl` interaction model

The control should:

1. update camera state
2. derive a viewport
3. notify `ImageCache` of viewport change
4. request a draw list
5. draw tiles using local screen-space rectangles

It should not directly own tile lifecycle decisions.

## Draw API direction

The intended Skia usage is:

- compute a destination rectangle in screen coordinates
- ask Skia to draw the `SKImage` into that rectangle

This allows:

- shrinking a `1024x1024` tile to `300x300`
- enlarging or reducing tiles according to zoom
- small screen-space coordinates even when world coordinates are enormous

## Summary invariants

The important invariants for implementation are:

- each `ImageEntry` has at most one active load
- each load completion must prove it is still current
- an entry is not `Loaded` until the `SKImage` is installed
- canceled never-visible working sets are removed from the cache immediately
- partially visible older working sets may remain as fallback
- older sets are removed as soon as they stop contributing visible coverage
- newest visible tiles draw on top of older fallback tiles
- the cache, not the control, is the authority on working sets

## Event flow

The clarified event flow is:

1. background work decodes or creates an `SKImage`
2. completion is marshaled to the UI thread
3. `ImageEntry.OnLoadCompleted(...)` validates:
   - current `LoadGeneration`
   - parent `WorkingSet.Epoch`
   - parent working set still active in the cache
4. if accepted, the entry installs the image and notifies its parent working set
5. the `WorkingSet` recomputes its `VisibleSet` and raises `VisibleSetChanged`
6. `ImageCache` recomposes the full visible set and raises `VisibleSetChanged`
7. `InfiniteCanvasControl` updates its cached draw list and invalidates

## Recommended first implementation order

1. define working-set and entry state enums
2. add structural equality-bearing entry keys
3. add generation ids and stale-completion checks to `ImageEntry`
4. make `WorkingSet` own the full `ImageEntries` membership for its artifact slice
5. make `ImageCache` maintain explicit active working sets with `Epoch`
6. make viewport changes always flow through cache update logic and then into working sets
7. ensure cache/entry state mutation is UI-thread-owned
8. switch drawing to layered draw lists from active working sets
9. remove canceled, never-visible working sets from the cache immediately
10. add fallback-removal logic based on visible contribution
