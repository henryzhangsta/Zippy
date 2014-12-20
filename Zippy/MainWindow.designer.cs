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
		MonoMac.AppKit.NSTableCellView imageCell { get; set; }
	
		[Outlet]
		Zippy.CustomOutlineView outlineView { get; set; }

		[Outlet]
		Zippy.CustomTableView tableView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableCellView textCell { get; set; }

		[Action ("doubleClick:")]
		partial void doubleClick (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (imageCell != null) {
				imageCell.Dispose ();
				imageCell = null;
			}

			if (outlineView != null) {
				outlineView.Dispose ();
				outlineView = null;
			}

			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (textCell != null) {
				textCell.Dispose ();
				textCell = null;
			}

			if (outlineView != null) {
				outlineView.Dispose ();
				outlineView = null;
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
