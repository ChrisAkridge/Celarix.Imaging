# Design Q&A

This file captures the design decisions and preferences established during the first Q&A pass about the future core API direction for `Celarix.Imaging`.

## Core API direction

- The library should move toward explicit workflow / service objects rather than staying mostly static and utility-shaped.
- This is preferred because workflows can have multiple legitimate result types and because that structure should make job saving and resuming easier where needed.

## Job persistence

- Job persistence should not be a universal requirement across the whole library.
- Resume support is mainly worth keeping for expensive workflows such as:
  - packing
  - large binary draws
- Most ordinary binary drawing operations are expected to complete quickly and do not need job infrastructure.

## Result shape

- A simple `Result<T>` is probably not sufficient for binary drawing because the operation can produce different artifact kinds.
- The preferred direction is a discriminated result model:
  - an enum indicating the result kind
  - nullable payload properties such as `Image<Rgba32>` and output path / folder path
- The result contract should make it impossible for callers to guess which payload to inspect.

## ImageSharp exposure

- The library should continue returning ImageSharp types directly for in-memory workflows.
- `Image<Rgba32>` is considered a deliberate part of the public surface, not an implementation detail to hide.
- Wrapping ImageSharp behind local passthrough abstractions is not desirable.

## Target selection

- Request-level configuration is preferred over global configuration.
- The old global `LibraryConfiguration` approach is considered hackish and should be deprecated over time.
- Tile size and similar choices should live on request objects.
- Existing zoomable canvas tile size can be inferred when needed by reading tile `0,0`.

## Binary drawing target semantics

- The current behavior where `TargetMode.SingleImage` can still return a zoomable canvas is acknowledged as wrong.
- `TargetMode.Automatic` is an acceptable and preferred addition.
- The likely target modes are:
  - `Automatic`
  - `SingleImage`
  - `ZoomableCanvas`
- Oversized single-image requests should be handled by a preflight warning flow in the UI, not by silently changing the output type.

## Preflight / size check

- Binary drawing should expose a size-check / preflight method, such as `CheckSize(DrawOptions)`, before actual execution.
- The UI can use this to inspect image size and warn the user before drawing a very large image.
- This preflight concept does not currently need to be generalized to other workflows.
- The preflight should return raw facts rather than opinionated warning categories.
- Preferred outputs include things like:
  - dimensions
  - pixel count
  - similar concrete measurements

## Progress and cancellation

- `CancellationToken` and progress reporting should move into the request object rather than remain separate method parameters.
- This keeps the execution API cleaner and allows the UI to assemble execution parameters incrementally.

## Request object shape

- Request objects should be immutable or mostly init-only rather than mutable DTOs.
- The UI can keep its own mutable state and map that into a validated request at execution time.
- Validation should still happen when the workflow is invoked.

## Validation and exceptions

- Invalid requests should throw exceptions rather than returning failure result objects.
- That is considered semantically correct because invalid requests are caller/programmer errors, while result objects should describe legitimate workflow outcomes.

## Input model

- The canonical binary drawing API should be file-path based.
- Stream-based input is not desirable as the main path because stream state is ambiguous and complicates concerns such as:
  - file titles
  - sizing
  - ownership
  - resumability

## Multi-file behavior

- Multi-file binary drawing is first-class and should remain so in the canonical API.
- Single-file drawing should conceptually be treated as the one-file case of the same multi-file model.
