using System;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Zippy
{
	public class FileListOutlineDelegate : NSOutlineViewDelegate
	{
		NSTableCellView imageCell;

		public FileListOutlineDelegate(NSTableCellView imageCell) : base()
		{
			this.imageCell = imageCell;
		}

		public override NSView GetView(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
		{
			string text = ((NSString)outlineView.DataSource.GetObjectValue(outlineView, tableColumn, item)).ToString();

			NSTableCellView result;

			result = (NSTableCellView)outlineView.MakeView(imageCell.Identifier, this);
			result.ImageView.Image = ((FileListOutlineDataSource)outlineView.DataSource).GetFileIcon(item);
			result.TextField.StringValue = text;
			return result;
		}


	}
}

