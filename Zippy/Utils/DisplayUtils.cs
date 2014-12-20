using System;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Zippy
{
	public class DisplayUtils
	{
		private static NSWorkspace _ws;

		public static NSWorkspace GetWorkspace()
		{
			if (_ws == null)
			{
				_ws = new NSWorkspace();
			}

			return _ws;
		}

		public static NSImage GetFileIcon(string name)
		{
			return GetFileIcon(name, false);
		}

		public static NSImage GetFileIcon(string name, bool directory)
		{
			if (directory)
			{
				return NSImage.ImageNamed(NSImageName.Folder);
			}

			if (name.Contains('.'))
			{
				name = name.Split('.').Last();
			}
			else
			{
				name = "";
			}

			return GetWorkspace().IconForFileType(name);
		}
	}
}

