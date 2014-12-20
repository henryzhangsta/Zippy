using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using SharpCompress.Common;

namespace Zippy
{
	public class FileListOutlineItem : NSObject
	{
		DirectoryNode node;
		IEntry entry;
		string name;
		List<FileListOutlineItem> children;

		public FileListOutlineItem(IEntry entry)
		{
			this.name = entry.FilePath.Split('/').Last();
			this.entry = entry;
		}

		public FileListOutlineItem(string name, DirectoryNode node)
		{
			this.name = name;
			this.node = node;
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
	}
}

