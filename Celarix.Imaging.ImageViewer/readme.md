# Celarix.Imaging.ImageViewer

Celarix.Imaging.ImageViewer is a Windows Forms image viewer written in C# for .NET 8. It contains a number of custom features, such as WebP and animated WebP support, a built-in VLC integration to view media files, support for browsing through folders of images or browsing through playlists, marking images by file path to be saved or copied as a list, integrated binary drawing courtesy of Celarix.Imaging.BinaryDrawing, a bitplane viewer, and more.

This is a custom-built application and is mostly tailored to things I need or want. I recommend you use [IrfanView](https://www.irfanview.com/) or [XnView](https://www.xnview.com/en/), which offer more traditional image viewing features.

## Architecture

ImageViewer supports the following formats:

- Images: JPEG, GIF (static and animated), PNG, Windows Bitmap, WebP (static and animated)
- Videos: Technically, any media format supported by VLC, but only MP4, AVI, MOV, FLV, M4V, and MKV files are permitted to be opened

We'll consider images and videos separately.

Images come in two varieties: *static* and *animated*. Static images are made of one image, whereas animated images are made of multiple images along with a duration that each frame should be shown for.

When loading an image, the application will load it into an ImageSharp image object, or one image object per frame for animated images. It will store that alongside metadata such as the image's format, bit depth, and, if animated, the frame duration.

EXIF metadata, if present, will be used to rotate the image before display. This is a common problem with photos taken by an iPhone, which contain metadata to properly rotate the image.

