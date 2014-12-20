using System;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Zippy
{
	public class FileListOutlineDataSource : NSOutlineViewDataSource
	{
		FileListOutlineItem root;

		public FileListOutlineDataSource() : base()
		{
			this.root = new FileListOutlineItem("", new DirectoryNode());
		}

		public void SetData(DirectoryNode root)
		{
			this.root = new FileListOutlineItem("", root);
		}

		public override int GetChildrenCount(NSOutlineView outlineView, NSObject item)
		{
			if (item == null)
			{
				return GetChildrenCount(outlineView, root);
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
				return GetChild(outlineView, childIndex, root);
			}
			return ((FileListOutlineItem)item).Children()[childIndex];
		}

		public override NSObject GetObjectValue(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
		{
			NSTableCellView c = new NSTableCellView();
			c.TextField = new NSTextField();
			c.TextField.StringValue = ((FileListOutlineItem)item).GetName();
			return c;
		}
	}
}

