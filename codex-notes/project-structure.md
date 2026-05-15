# Project Structure

## High-level read

This repository is a multi-project .NET solution centered on `Celarix.Imaging`, a `net8.0` class library built on `SixLabors.ImageSharp`, `ImageSharp.Drawing`, and `SixLabors.Fonts`.

At the top level, the shape is:

- `Celarix.Imaging`: the core imaging library
- `Celarix.Imaging.ByteView`: a UI project for binary drawing / byte visualization
- `Celarix.Imaging.ByteViewCLI`: a command-line entry point for similar functionality
- `Celarix.Imaging.PictureTiler`: a UI or app surface around tiling / packing workflows
- `Celarix.Imaging.ImageViewer`: an image viewing application
- `Celarix.Imaging.ImagingPlayground`: a broader experimentation app for imaging operations
- `Celarix.Imaging.Formats`: a separate solution area for format-related work
- `ImagingTest`: a test or scratch project
- `Celarix.Imaging New UI`: an apparent placeholder or work area for the new UI direction
- `Test Images`: sample assets

The main solution file is `Celarix.Imaging.sln`. It currently includes:

- `Celarix.Imaging`
- `Celarix.Imaging.ByteView`
- `Celarix.Imaging.PictureTiler`
- `Celarix.Imaging.ImageViewer`
- `Celarix.Imaging.ByteViewCLI`
- `Celarix.Imaging.ImagingPlayground`

## Structural impression

The repository looks like a library-first codebase that grew a family of tools around a shared imaging core.

That core appears responsible for:

- turning bytes and streams into images
- composing many images into single packed outputs
- producing zoomable tiled canvases
- carrying basic IO, recovery, and utility infrastructure used by the app layers

The surrounding projects appear to be different front ends over overlapping capabilities rather than fully separate domains. That is useful for a future unifying UI because there is already a natural center of gravity in the shared library.

## Notes for a future unified UI

The current layout suggests a clean separation point:

- keep `Celarix.Imaging` as the execution and domain layer
- treat the existing desktop and CLI projects as clients of that layer
- identify which workflows are truly canonical: binary drawing, zoomable canvas generation, image tiling / packing, and image viewing

From a UI architecture standpoint, the main question is not where the logic lives. The main question is whether the library APIs are already shaped around reusable workflow models, or whether they still reflect individual app histories. My first impression is that both are present: the library is shared, but some API surfaces still look task-specific and older-generation.
