using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace Zippy
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;

		public AppDelegate()
		{
		}

		public override void FinishedLaunching(NSObject notification)
		{
			if (mainWindowController == null)
			{
				mainWindowController = new MainWindowController();
				mainWindowController.Window.MakeKeyAndOrderFront(this);
			}
		}

		public override bool OpenFile(NSApplication sender, string filename)
		{
			MainWindowController w = new MainWindowController();

			if (mainWindowController == null)
			{
				mainWindowController = w;
			}

			w.Window.MakeKeyAndOrderFront(this);

			System.IO.FileInfo f = new System.IO.FileInfo(filename);
			if (f.Exists)
			{
				w.loadArchive(f);
				return true;
			}
			else
			{
				RunError("File Not Found", String.Format("File '{0}' could not be found.", filename));
				return false;
			}
		}

		public override void OpenFiles(NSApplication sender, string[] filenames)
		{
			foreach (string s in filenames)
			{
				OpenFile(sender, s);
			}
		}

		public void RunError(string message, string description)
		{
			NSAlert a = new NSAlert();
			a.Icon = new NSImage();
			a.AlertStyle = NSAlertStyle.Critical;
			a.MessageText = message;
			a.InformativeText = description;
			a.Window.StyleMask = NSWindowStyle.Borderless;
			a.RunModal();
		}
	}
}

