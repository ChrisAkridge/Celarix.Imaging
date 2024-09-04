using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celarix.Imaging.ImageViewer.IO
{
	internal sealed class FileList
	{
		private string[] filePaths;
		private int position;

		public int Position
		{
			get => position;
			set
			{
				if (value < 0 || value >= Count)
				{
					throw new ArgumentOutOfRangeException(nameof(value), "The position is out of range.");
				}

				position = value;
			}
		}

		public int Count => filePaths.Length;
		public string Current => filePaths[Position];
		public IReadOnlyList<string> FilePaths => filePaths;
		public FileListType FileListType { get; }

		public FileList()
		{
			filePaths = Array.Empty<string>();
			FileListType = FileListType.Playlist;
		}
		
		public FileList(FileListType type, string folderPath)
		{
			var topLevelFiles = Directory.GetFiles(folderPath);

			if (type == FileListType.FolderStandardMode)
			{
				topLevelFiles = topLevelFiles.Where(IOHelpers.IsSupportedFile).ToArray();
			}

			filePaths = topLevelFiles;
			FileListType = type;
		}

		public FileList(FileListType type, string folderPath, string initialFilePath) : this(type, folderPath)
		{
			var initialFileIndex = Array.IndexOf(filePaths, initialFilePath);
			if (initialFileIndex == -1)
			{
				throw new ArgumentException("The initial file path is not in the list of files.");
			}
			Position = initialFileIndex;
		}

		public FileList(IEnumerable<string> filePaths)
		{
			this.filePaths = filePaths.ToArray();
			FileListType = FileListType.Playlist;
		}

		public void MoveNextOrWrap() => Position = (Position + 1) % Count;

		public void MoveLastOrWrap() => Position = Position == 0 ? Count - 1 : Position - 1;

		public void SortBy(SortMode sortMode, bool ascending)
		{
			var current = Current;

			switch (sortMode)
			{
				case SortMode.ByFileName:
				{
					filePaths = ascending
						? filePaths.OrderBy(Path.GetFileName).ToArray()
						: filePaths.OrderByDescending(Path.GetFileName).ToArray();

					break;
				}
				case SortMode.ByDateCreated:
				{
					filePaths = ascending
						? filePaths.OrderBy(File.GetCreationTime).ToArray()
						: filePaths.OrderByDescending(File.GetCreationTime).ToArray();

					break;
				}
				case SortMode.ByFileSize:
				{
					Func<string, long> selector = f => new FileInfo(f).Length;
					filePaths = ascending
						? filePaths.OrderBy(selector).ToArray()
						: filePaths.OrderByDescending(selector).ToArray();

					break;
				}
				default:
					throw new ArgumentException("Invalid sort mode.");
			}

			Position = Array.IndexOf(filePaths, current);
		}
		
		public void ReverseOrder()
		{
			filePaths = filePaths.Reverse().ToArray();
			Position = Count - 1 - Position;
		}
	}
}
