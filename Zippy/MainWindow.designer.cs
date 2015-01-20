// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace Zippy
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSTableCellView outlineImageCell { get; set; }

		[Outlet]
		Zippy.CustomOutlineView outlineView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableCellView tableImageCell { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableCellView tableTextCell { get; set; }

		[Outlet]
		Zippy.CustomTableView tableView { get; set; }

		[Action ("doubleAction:")]
		partial void doubleAction (MonoMac.Foundation.NSObject sender);

		[Action ("doubleClick:")]
		partial void doubleClick (MonoMac.Foundation.NSObject sender);

		[Action ("doubleClieck:")]
		partial void doubleClieck (MonoMac.Foundation.NSObject sender);

		[Action ("outlineViewSelectionDidChange:")]
		partial void outlineViewSelectionDidChange (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (outlineImageCell != null) {
				outlineImageCell.Dispose ();
				outlineImageCell = null;
			}

			if (outlineView != null) {
				outlineView.Dispose ();
				outlineView = null;
			}

			if (tableImageCell != null) {
				tableImageCell.Dispose ();
				tableImageCell = null;
			}

			if (tableTextCell != null) {
				tableTextCell.Dispose ();
				tableTextCell = null;
			}

			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}
		}
	}

	[Register ("MainWindow")]
	partial class MainWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
