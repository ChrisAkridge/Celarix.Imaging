# Celarix.Imaging Library Structure

## Core identity

`Celarix.Imaging` is the central engine of the repository. Its package description is accurate: it handles several imaging tasks, especially:

- binary drawing: interpreting bytes as pixels
- image packing / collage generation
- zoomable canvas generation
- related stream, sizing, and utility support

The library targets `net8.0` and uses ImageSharp as its primary image-processing dependency.

## Main namespaces and responsibilities

### `BinaryDrawing`

This is one of the defining parts of the library.

Key responsibilities:

- convert streams into pixels at different bit depths
- compute image sizes from byte counts
- optionally draw source text labels onto images
- generate either a single in-memory image or a tiled zoomable canvas folder
- support pixel sorting modes such as `RGB`, `HSV`, and `YCbCr`

Representative types:

- `Drawer.cs`: main binary drawing entry point in the original API surface
- `DefaultPalettes.cs`: built-in palettes
- `PartiallyKnownSize.cs`: fixed-width / fixed-size style input
- `PixelBufferer.cs`, `UniqueColorsImage.cs`: supporting drawing logic

The original `BinaryDrawing.Drawer` is currently a strong example of the library's style: static entry points, explicit stream handling, direct ImageSharp image creation, and progress/cancellation hooks threaded through the workflow.

### `BinaryDrawing/v2`

This is important because it looks like the beginning of a more unified second-generation drawing model.

Representative types:

- `DrawOptions`
- `DrawResult`
- `DrawProgress`
- `Drawer`
- `SingleImageSizer`
- `PixelDrawer`

What stands out in `v2`:

- drawing options are consolidated into a single options object
- concepts are more explicit: `PixelFormat`, `ColorMode`, `PixelLayout`, `SizeMode`, `TitleMode`, `TargetMode`, `BandDirection`, `TextMapEncoding`
- the result model is formalized with `DrawResultKind`

Architecturally, this is a stronger shape for a future unified UI than the older static overload set. It reads more like a workflow contract. It is not complete yet, though: some target modes still throw `NotImplementedException`.

### `Packing`

This area handles laying out many images into one larger result.

Representative types:

- `ImagePacker`
- `Packer`
- `Block`
- `Node`
- `PackingOptions`
- `PackingJob`
- `ImageSizeDictionary`
- `ImageSizeLoader`

Behaviorally, this subsystem:

- gathers source image sizes
- arranges rectangles with a packing algorithm
- either renders one composite image or emits positioned images for a multipicture zoomable canvas flow
- persists job state for resumability

This looks like a mature feature area with a clear pipeline from file discovery to packed output.

### `ZoomableCanvas`

This area turns large images or positioned image sets into tiled output folders and derived zoom levels.

Representative types:

- `CanvasGenerator`
- `ZoomLevelGenerator`
- `PositionedImage`

Behaviorally, it:

- splits the output into level-0 tiles
- stores those tiles in a folder hierarchy
- combines tiles into higher zoom levels
- reuses an `ImageCache` to limit repeated image loads

This namespace looks like shared infrastructure that both binary drawing and packing can target.

### `Tiling`

This area is separate from rectangle packing. It is more about regular grid composition.

Representative types:

- `Tiler`
- `TileOptions`

Behaviorally, it:

- crops each image to the requested aspect ratio
- resizes to a common tile size
- places results on a computed grid canvas

This is a simpler, more deterministic layout flow than `Packing`.

### `IO`

This area contains file and stream wrappers used across workflows.

Representative types:

- `NamedStream`
- `NamedMultiStream`
- `LazyNamedStream`
- `LazyNamedMultiStream`
- `FileSource`
- `FilePathWithSize`
- `ImageCache`

This code suggests the library often works with sequences of files while preserving source names and stream identity, especially for binary drawing and canvas generation.

### `IO/v2`

This namespace mirrors the `BinaryDrawing/v2` direction.

Representative types:

- `NamedStream<T>`
- `NamedByteStream`

The notable improvement here is that byte-stream traversal across multiple files is being expressed as a formal stream abstraction with file boundaries and total-length tracking. That feels like a good foundation for a new UI because progress, per-file titles, and multi-file workflows all depend on this information.

### `JobRecovery`

Representative types:

- `IBinaryJob`
- `JobManager`
- `JobSources`

This area persists job state to an app-data folder so longer workflows can resume. That is a meaningful feature if the eventual UI wants to support large binary drawings or large pack operations without being fragile.

### `Utilities`, `Collections`, `Progress`, `Pipeline`, `Formats`, `Misc`

These namespaces are support layers.

Highlights:

- `Utilities.Helpers` contains important geometry and sizing helpers used widely across the library
- `Utilities.ImageLoader` wraps robust image loading and result reporting
- `Progress` defines workflow progress models
- `Formats/CCIF.cs` indicates a custom or project-specific image format effort
- `Pipeline` looks like the start of image-operation metadata and channel selection infrastructure
- `Misc` holds experimental or specialized imaging code

## Cross-cutting design patterns

A few recurring patterns show up across the library:

- static service-style classes instead of heavy object graphs
- `CancellationToken` and `IProgress<T>` included directly in long-running operations
- ImageSharp `Image<TPixel>` as the main image representation
- folder-based outputs for large or multi-resolution artifacts
- resumable jobs for longer operations
- helper-heavy geometry and layout logic

That makes the code practical and direct. It also means a future UI can map fairly naturally onto explicit workflows.

## Architectural impression

The core library already contains the right domain pieces for a unifying UI. The important split is not between "library" and "apps"; that split already exists. The more relevant split is between:

- older workflow entry points that are useful but task-specific
- newer `v2` workflow models that look closer to a consolidated product surface

If the new UI is intended to unify the project, `BinaryDrawing/v2` and `IO/v2` are the most interesting areas to build around because they are already moving toward explicit options, result objects, and reusable workflow concepts.

## My first practical takeaway

If I were designing the new UI on top of this codebase, I would treat these as the primary top-level feature pillars:

- Binary Drawing
- Zoomable Canvas
- Tiling
- Packing
- Image Viewing / Inspection

And I would treat these as the first library questions to answer before building much UI:

- Which APIs are canonical today: original `BinaryDrawing` or `BinaryDrawing/v2`?
- Which output modes are complete enough to expose confidently?
- Where should job persistence live in the user experience?
- Which option models need consolidation so different front ends stop inventing their own workflow parameters?
