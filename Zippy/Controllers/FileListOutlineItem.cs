using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using SharpCompress.Archive;

namespace Zippy
{
	public class FileListOutlineItem : NSObject
	{
		DirectoryNode node;
		IArchiveEntry entry;
		string name;
		List<FileListOutlineItem> children;

		public FileListOutlineItem(IArchiveEntry entry)
		{
			this.name = entry.FilePath.Split('/').Last();
			this.entry = entry;
		}

		public FileListOutlineItem(string name, DirectoryNode node)
		{
			this.name = name;
			this.node = node;
		}

		public bool IsDirectory
		{
			get
			{
				return this.entry == null;
			}
		}

		private void BuildChildren()
		{
			if (children == null)
			{
				children = node.GetDirectories(new List<string>()).ConvertAll((i) => {
					return new FileListOutlineItem(i, node.GetDirectory(new List<string>(new String[]{i})));
				});
				children.AddRange(node.GetFiles(new List<string>()).ConvertAll((i) => new FileListOutlineItem(i)));
			}
		}

		public List<FileListOutlineItem> Children()
		{
			BuildChildren();
			return children;
		}

		public int ChildCount()
		{
			if (entry != null)
			{
				return 0;
			}
			return node.GetDirectories(new List<string>()).Count + node.GetFiles(new List<string>()).Count;
		}

		public NSString GetName()
		{
			return new NSString(name);
		}

		public FileListOutlineItem GetNode(List<string> path)
		{
			BuildChildren();

			if (path.Count > 0)
			{
				string next = path.First();
				path.RemoveAt(0);

				foreach (FileListOutlineItem child in children)
				{
					if (child.GetName() == next)
					{
						return child.GetNode(path);
					}
				}

				throw new FieldAccessException();
			}

			return this;
		}

		public IArchiveEntry GetEntry()
		{
			return entry;
		}
	}
}

