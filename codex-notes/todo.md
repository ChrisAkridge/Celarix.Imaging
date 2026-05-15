# TODO

## Core library follow-up

### Global configuration

- Add a safe default for `LibraryConfiguration.Instance` inside the core library so library consumers do not need app-specific bootstrap code just to avoid `NullReferenceException`.
- Decide whether configuration should remain a mutable global singleton, or become:
  - a lazily initialized singleton with defaults
  - an immutable options object passed into workflows
  - a hybrid where library defaults exist but callers can override them explicitly

### `NamedMultiStream.Position`

- Fix the `Position` setter in `Celarix.Imaging/IO/NamedMultiStream.cs` so seeking is relative to the selected inner stream's file offset, not the current underlying stream position.
- Review whether the type should keep full seeking support or narrow its contract if random access is not a real requirement.

### Binary drawing target semantics

- Fix the API contract where a caller can request `TargetMode.SingleImage` but still receive a zoomable canvas because the library decides the image is too large.
- Decide on the intended behavior for oversized single-image requests:
  - warn before execution and let the caller choose
  - return a typed "too large for requested target" result
  - require an explicit auto-target policy instead of overloading `SingleImage`
- Keep the request intent and result kind consistent so the library does not silently substitute a different artifact type than the caller asked for.

### `ImagingPlayground` result handling

- Widen the playground host contract so operations can surface more than a single in-memory image.
- Replace the current image-only operation callback model with one that can accept multiple artifact kinds, especially:
  - in-memory `Image<Rgba32>`
  - zoomable canvas folder outputs
  - future temp-folder / banded-image outputs
- Update `BinaryDrawFilesOperation` to handle all relevant `DrawResultKind` values instead of only `SingleImageInMemory`.

### `ImagingPlayground` canvas loading

- Fix `InfiniteCanvasControl.LoadZoomableCanvas` so tile positions are determined from parsed numeric folder/file coordinates, not loop indices derived from lexically sorted paths.
- Decide whether zoomable canvas filenames/folders should be normalized with zero-padding or whether numeric parsing alone is sufficient and preferred.
- Review zoom-level folder ordering explicitly as numeric ordering rather than string ordering.

### `ImagingPlayground` image fallback behavior

- Fix `CanvasImage.FromFile` so failed metadata probing does not overwrite the fallback placeholder size with a zero/default size.
- Implement the intended error-image behavior when image loading fails, instead of throwing and leaving the canvas with no visual explanation.
- Decide what diagnostics the viewer should surface for bad tiles or unreadable images:
  - on-canvas error tile
  - log entry
  - both

### Remove remaining global config dependencies

- Remove the remaining `LibraryConfiguration.Instance` dependency from `ImagingPlayground`.
- In particular, stop using global tile size in:
  - `Program.cs`
  - `Viewport.cs`
  - zoomable canvas loading logic
- Ensure zoomable canvas viewers infer tile size from the canvas itself where appropriate, rather than relying on process-global configuration.
