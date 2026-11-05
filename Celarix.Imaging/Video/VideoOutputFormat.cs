using System.Collections.Generic;

namespace Celarix.Imaging.Video
{
	public enum VideoOutputCategory
	{
		Compressed,
		Lossless,
		Uncompressed
	}

	/// <summary>
	/// An output video format: a container, a video encoder and its settings, and what to do with the audio.
	/// </summary>
	/// <param name="DisplayName">What the format is called in the UI.</param>
	/// <param name="Category">Whether the format is lossy, lossless, or uncompressed.</param>
	/// <param name="FileExtension">The container's file extension, with the dot.</param>
	/// <param name="VideoArguments">ffmpeg arguments that choose and configure the video encoder.</param>
	/// <param name="AudioArguments">ffmpeg arguments for the audio. Copying is preferred; where the container cannot
	/// hold the original audio codec, the audio is re-encoded to one it can.</param>
	/// <param name="Notes">A short explanation shown beside the format list.</param>
	public sealed record VideoOutputFormat(string DisplayName,
		VideoOutputCategory Category,
		string FileExtension,
		string VideoArguments,
		string AudioArguments,
		string Notes)
	{
		public override string ToString() => $"{Category} - {DisplayName}";

		/// <summary>
		/// Formats ffmpeg can write. Whether a particular ffmpeg build includes each encoder varies; ffmpeg's own
		/// error is reported if one is missing.
		/// </summary>
		public static IReadOnlyList<VideoOutputFormat> All { get; } = new VideoOutputFormat[]
		{
			// The compressed formats use 4:2:0 chroma, and their notes say so: effects like bit planes and dithering
			// put detail into color, which 4:2:0 throws away. The lossless and uncompressed formats keep it.
			new("H.264 (MP4)", VideoOutputCategory.Compressed, ".mp4",
				"-c:v libx264 -crf 18 -preset medium -pix_fmt yuv420p -movflags +faststart", "-c:a copy",
				"The most compatible choice. Uses 4:2:0 chroma, which blurs fine color detail."),
			new("H.265 (MP4)", VideoOutputCategory.Compressed, ".mp4",
				"-c:v libx265 -crf 20 -preset medium -pix_fmt yuv420p -tag:v hvc1 -movflags +faststart", "-c:a copy",
				"Smaller than H.264 at similar quality, but slower to encode. Uses 4:2:0 chroma."),
			new("VP9 (WebM)", VideoOutputCategory.Compressed, ".webm",
				"-c:v libvpx-vp9 -crf 28 -b:v 0 -pix_fmt yuv420p", "-c:a libopus",
				"WebM cannot hold most original audio codecs, so audio is re-encoded to Opus."),
			new("AV1 (MKV)", VideoOutputCategory.Compressed, ".mkv",
				"-c:v libsvtav1 -crf 30 -preset 8 -pix_fmt yuv420p", "-c:a copy",
				"The most efficient compressed option. Uses 4:2:0 chroma."),

			new("H.264 lossless RGB (MKV)", VideoOutputCategory.Lossless, ".mkv",
				"-c:v libx264rgb -crf 0 -preset medium -pix_fmt rgb24", "-c:a copy",
				"Exact pixels, with no chroma subsampling. Many players cannot decode it."),
			new("FFV1 (MKV)", VideoOutputCategory.Lossless, ".mkv",
				"-c:v ffv1 -level 3 -coder 1 -context 1 -slicecrc 1 -pix_fmt bgr0", "-c:a copy",
				"Exact pixels, an archival standard. Most players need codec support, but VLC and mpv play it."),

			new("RGB24 (AVI)", VideoOutputCategory.Uncompressed, ".avi",
				"-c:v rawvideo -pix_fmt bgr24", "-c:a pcm_s16le",
				"Exact pixels, three bytes each. Very large. AVI cannot hold most original audio, so audio becomes PCM."),
			new("YUV 4:4:4 planar (NUT)", VideoOutputCategory.Uncompressed, ".nut",
				"-c:v rawvideo -pix_fmt yuv444p", "-c:a copy",
				"Full-resolution luma and chroma, three bytes per pixel. Only the NUT container can hold this; ffplay, mpv and VLC play it."),
			new("YUV 4:2:0 planar (MKV)", VideoOutputCategory.Uncompressed, ".mkv",
				"-c:v rawvideo -pix_fmt yuv420p", "-c:a copy",
				"Half-resolution chroma, one and a half bytes per pixel."),
			new("UYVY 4:2:2 packed (MOV)", VideoOutputCategory.Uncompressed, ".mov",
				"-c:v rawvideo -pix_fmt uyvy422 -tag:v 2vuy", "-c:a copy",
				"Half-width chroma, two bytes per pixel."),
			new("v210 10-bit 4:2:2 (MOV)", VideoOutputCategory.Uncompressed, ".mov",
				"-c:v v210 -pix_fmt yuv422p10le", "-c:a copy",
				"Professional 10-bit 4:2:2, about 2.67 bytes per pixel.")
		};
	}
}
