using System;
using SharpCompress.Archive;
using SharpCompress.Archive.GZip;
using SharpCompress.Archive.Rar;
using SharpCompress.Archive.SevenZip;
using SharpCompress.Archive.Tar;
using SharpCompress.Archive.Zip;
using SharpCompress.Common;
using SharpCompress.Reader;
using SharpCompress.Writer;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Zippy
{
	public class ArchiveManager
	{
		public static Archive LoadArchive(FileInfo file)
		{
			IArchive a = ArchiveFactory.Open(file, Options.None);
			return new Archive(a, a.Type);
		}

		public static void LoadArchive(FileInfo file, SharpCompress.Common.ArchiveType type)
		{
			
		}
	}

	public class Archive
	{
		IArchive archive;
		ArchiveType type;

		public DirectoryNode root;

		public Archive(IArchive archive, ArchiveType type)
		{
			this.archive = archive;
			this.type = type;
		}

		public bool CanWrite
		{
			get
			{
				switch (type)
				{
					case ArchiveType.GZip:
					case ArchiveType.Tar:
					case ArchiveType.Zip:
						return true;
					default:
						return false;
				}
			}
		}

		public void LoadStructure()
		{
			if (root == null)
			{
				root = new DirectoryNode();

				foreach (IArchiveEntry i in archive.Entries)
				{
					root.AddEntry(SplitPath(i.FilePath), i);
				}
			}
		}

		public List<string> GetDirectories(string path)
		{
			LoadStructure();
			return root.GetDirectories(SplitPath(path));
		}

		public IArchiveEntry GetFile(string path)
		{
			LoadStructure();
			return root.GetFile(SplitPath(path));
		}

		public List<IArchiveEntry> GetFiles(string path)
		{
			LoadStructure();
			return root.GetFiles(SplitPath(path));
		}

		public List<string> GetLeafDirectories(string path)
		{
			LoadStructure();
			return root.GetLeafDirectories(SplitPath(path));
		}

		public List<KeyValuePair<string, IArchiveEntry>> GetLeafFiles(string path)
		{
			LoadStructure();
			return root.GetLeafFiles(SplitPath(path));
		}

		private List<string> SplitPath(string path)
		{
			return path.Trim().Split('/').Where((s) => s.Length > 0).ToList();
		}
	}
}

