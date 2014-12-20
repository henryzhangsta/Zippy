using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Zippy
{
	public class TableListDataSource : NSTableViewDataSource
	{
		List<Dictionary<string, Object>> data;

		public TableListDataSource() : base()
		{
			data = new List<Dictionary<string, Object>>();
		}

		public void UpdateView(List<Dictionary<string, Object>> data)
		{
			this.data = data;
		}

		public List<Dictionary<string, Object>> GetData()
		{
			return data;
		}

		public override int GetRowCount(NSTableView tableView)
		{
			return data.Count;
		}

		public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, int row)
		{
			Object d;
			data[row].TryGetValue(tableColumn.Identifier, out d);
			if (d == null)
			{
				d = "";
			}
			return new NSString(d.ToString());
		}

		public NSImage GetFileIcon(int row)
		{
			Dictionary<string, Object> r = data[row];

			Object name;
			Object directory;

			r.TryGetValue("Name", out name);
			r.TryGetValue("_directory", out directory);

			return DisplayUtils.GetFileIcon((string)name, (bool)directory);
		}

		public override NSDragOperation ValidateDrop(NSTableView tableView, NSDraggingInfo info, int row, NSTableViewDropOperation dropOperation)
		{
			return NSDragOperation.Copy;
		}

		public override bool AcceptDrop(NSTableView tableView, NSDraggingInfo info, int row, NSTableViewDropOperation dropOperation)
		{
			NSPasteboard pboard = info.DraggingPasteboard;
			NSArray files = (NSArray)pboard.GetPropertyListForType(NSPasteboard.NSFilenamesType);

			return true;
		}

		public override bool WriteRows(NSTableView tableView, NSIndexSet rowIndexes, NSPasteboard pboard)
		{
			pboard.DeclareTypes(new string[]{ NSPasteboard.NSFilesPromiseType }, this);
			NSArray a = NSArray.FromStrings(new string[]{ "pdf" });
			pboard.SetPropertyListForType(a, NSPasteboard.NSFilesPromiseType);

			return true;
		}

		public override string[] FilesDropped(NSTableView tableView, NSUrl dropDestination, NSIndexSet indexSet)
		{
			List<string> items = new List<string>();
			foreach (int i in indexSet)
			{
				items.Add((string)data[i]["Name"]);
			}

			return items.ToArray();
		}
	}
}

