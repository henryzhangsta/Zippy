using System;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Zippy
{
	public class TableListDelegate : NSTableViewDelegate
	{
		NSTableCellView imageCell;
		NSTableCellView textCell;
		public TableListDelegate(NSTableCellView imageCell, NSTableCellView textCell) : base()
		{
			this.imageCell = imageCell;
			this.textCell = textCell;
		}

		public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row)
		{
			string text = ((NSString)tableView.DataSource.GetObjectValue(tableView, tableColumn, row)).ToString();

			NSTableCellView result;

			if (tableColumn.Identifier == "Name")
			{
				result = (NSTableCellView)tableView.MakeView(imageCell.Identifier, this);
				result.ImageView.Image = ((TableListDataSource)tableView.DataSource).GetFileIcon(row);
			}
			else
			{
				result = (NSTableCellView)tableView.MakeView(textCell.Identifier, this);

				if (tableColumn.Identifier == "Size" || tableColumn.Identifier == "Packed Size")
				{
					result.TextField.Alignment = NSTextAlignment.Right;
				}
			}

			result.TextField.StringValue = text;
			return result;
		}
	}
}

