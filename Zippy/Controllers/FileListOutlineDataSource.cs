using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Zippy
{
	public class FileListOutlineDataSource : NSOutlineViewDataSource
	{
		FileListOutlineItem root;
		FileListOutlineItem displayRoot;

		public FileListOutlineDataSource() : base()
		{
			this.root = null;
		}

		public void SetData(string name, DirectoryNode root)
		{
			this.root = new FileListOutlineItem(name, root);
		}

		public override int GetChildrenCount(NSOutlineView outlineView, NSObject item)
		{
			if (item == null)
			{
				return (this.root == null) ? 0 : 1;
			}
			return ((FileListOutlineItem)item).ChildCount();
		}

		public override bool ItemExpandable(NSOutlineView outlineView, NSObject item)
		{
			return GetChildrenCount(outlineView, item) > 0;
		}

		public override NSObject GetChild(NSOutlineView outlineView, int childIndex, NSObject item)
		{
			if (item == null)
			{
				return this.root;
			}
			return ((FileListOutlineItem)item).Children()[childIndex];
		}

		public override NSObject GetObjectValue(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
		{
			NSString c = ((FileListOutlineItem)item).GetName();
			return c;
		}

		public NSImage GetFileIcon(NSObject item)
		{
			return DisplayUtils.GetFileIcon(((FileListOutlineItem)item).GetName(), ((FileListOutlineItem)item).IsDirectory && item != root);
		}

		public NSObject GetNode(string[] path)
		{
			return root.GetNode(path.ToList());
		}

		public override bool OutlineViewwriteItemstoPasteboard(NSOutlineView outlineView, NSArray items, NSPasteboard pboard)
		{
			if (outlineView.SelectedRow < 0 || outlineView.SelectedRowCount <= 0)
			{
				return false;
			}

			pboard.DeclareTypes(new string[]{ NSPasteboard.NSFilesPromiseType }, this);
			pboard.SetPropertyListForType(NSArray.FromStrings(new string[] {""}), NSPasteboard.NSFilesPromiseType);

			return true;
		}
	}
}

