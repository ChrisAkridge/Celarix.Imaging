_Compiled by Claude from a design conversation, March 2026._

---

## What ByteView Actually Is

ByteView is a **playground for curiosity**, not a professional analysis tool. The emotional core is "hex editor at 30,000 feet" — the hex editor shows you bytes through a mail slot; ByteView pulls the camera back until you can see the shape of the whole file at once. It's not trying to tell you what a file _means_. It's letting you see what a file _feels like_.

This is why the Chroma Playground isn't a separate app bolted on the side — it's the same question asked of image data. ByteView and the Chroma Playground are the same idea.

---

## The New UI Vision

A **single three-pane window** replacing the current multi-app setup (ByteView, Chroma Playground, PictureTiler):

- **Left pane** — scrollable list of action buttons with icons. Options appear, disappear, or gray out depending on what's loaded.
- **Center pane** — the image viewer. Shared across all tools. Supports pan and zoom.
- **Right pane** — a Properties-window-style panel. All settings live here as a single POCO options object that each operation reads from as needed.

The POCO options object is the lingua franca between the UI, the CLI, and any future JSON pipeline format. Each component only reads the properties it cares about.

---

## Left Pane Actions

### Immediate operations (buttons)

- **Load Bytes from File(s)...** — file dialog, uses Properties options
- **Load Bytes from Folder** — recursive; warns if sum of files > 50 MB
- **Load Picture...**
- **Save As...**
- **Redraw Bytes** — only enabled if loaded from files; re-renders with current Properties
- **Cancel** — invokes CancellationToken on long-running operations
- **Sort** — sorts pixels by sorting mode in Properties
- **Unique Colors** — reduces image to distinct colors per color space in Properties
- **Color Space Coverage** — for ≤24bpp images, produces a grid image showing all possible colors in that bit depth with only the colors present in the image lit up (sizes: 2×1 through 4096×4096 depending on bit depth)
- **View Color Channel** — shows selected channels only; multiple color spaces available but no mixing across spaces (e.g. can show R+B but not R+Cr). Configuration and combination shown in Properties.
- **View Bit Plane** — pick channel(s) and bit index 0..7; shows set bits at full intensity
- **Chroma Subsample** — pick subsampling level or specify custom X:1:1
- **Reduce Bit Depth** — pick target bit depth and palette mode

### Wizard launchers (separately grouped)

- **Analog Signal** — listen to or save mock analog signals for the displayed image (inline playback preferred over save-only)
- **Large File Processor** — wizard for Fixed-Size-To-Folder byte rendering
- **Tiler Wizard** — input folder or file list, specified W×H, crops/resizes and lays out raster on a big image or zoomable canvas
- **Packer Wizard** — packing algorithm for icons/images of any dimension onto one big sheet; outputs single image or zoomable canvas
- **Canvas from Existing** — breaks a large image into a zoomable canvas
- **FFmpeg Tools** — join images into video or extract video into images (requires FFmpeg path)
- **Image Combination**: Combines two images using specified operations. Combines them at 0, 0 and shows the result in the main window.

---

## Byte Rendering Changes

- **Stripe mode** dropped as a separate concept. Replaced by:
    - Fixed-width mode with UI hints showing nearest byte-aligned multiples
    - A **vertical raster** scan order (top-to-bottom, then left-to-right) as an option alongside the standard horizontal raster
- Floating point pixel formats (single and double precision) are new in V2, treated as curiosity formats rather than first-class citizens
- **Per-stream title bars** with progress fill for Fixed-Size-To-Folder (earned feature from V1 video watching)
- **Click-to-hex** on the zoomable canvas: click any pixel, get an overlay showing byte offset, raw hex value, and pixel format interpretation — closing the loop back to the hex editor roots

---

## Zoomable Canvas

- Tiles are 1024×1024
- **Tile layout**: a snaking concentric spiral from the top-left (not left-to-right rows), so the file reads top-to-bottom without horizontal sprawl. The layout is mathematically predictable — any tile index maps to (x, y) and back via closed-form integer arithmetic (shell-based).
- **Semantic zoom**: zoom level changes the _pixel format density_, not just the scale. Zoom level 0 = 1bpp, each level out increases bit depth (2bpp, 4bpp, 8bpp, 16bpp, 32bpp). This means tiles at every zoom level are rendered fresh at the requested bit depth rather than downsampled from a finished image — same rendering pipeline, just parameterized differently.
- Tiles generated on demand; no progress-dialog pyramid generation needed
- OpenSeadragon used as the viewer, via Electron or a lightweight local host

---

## Architecture Notes

- All actual logic stays in `Celarix.Imaging` as library functions — this is already true and working
- The CLI already exists and should stay tidy; it becomes more powerful once options are a clean POCO
- JSON pipeline format is a quiet long-term goal: a pipeline is just a list of named operations with attached options objects
- UI framework: WinForms is fine for now; Avalonia worth revisiting if the form-building tedium gets too painful. The UserControl-per-tool approach (swapped into a main panel) avoids needing a custom framework while keeping things organized.

---

## Future Ideas (Someday Shelf)

- Zoned image transformations: break image into grid, process chunks on separate threads, show chunks as they complete
- Centered image view with scroll-to-zoom (more image-viewer-like)
- Palette export: extract all colors from an image, save to a loadable palette format
- More color spaces with a more extensible registration system
- Entropy heatmap overlay on zoomable canvas (high entropy = compressed/encrypted regions visible at a glance)

---

## Design Philosophy Reminder

The expert move isn't more tools. It's a sharper answer to one question: _does this help the user see the file's character more clearly?_

Iterative is fine. Curiosity-led is fine. The playground teaches you what it wants to be next.