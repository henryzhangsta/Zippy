using System;
using SharpCompress.Archive;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Zippy
{
	public class ArchiveUtils
	{
		public static DateTime? GetCreatedTime(IArchiveEntry entry)
		{
			try
			{
				return entry.CreatedTime;
			}
			catch (NotImplementedException ex)
			{
				Logging.Trace(ex);
				return null;
			}
		}

		public static DateTime? GetModifiedTime(IArchiveEntry entry)
		{
			try
			{
				return entry.LastModifiedTime;
			}
			catch (NotImplementedException ex)
			{
				Logging.Trace(ex);
				return null;
			}
		}

		public static DateTime? GetAccessedTime(IArchiveEntry entry)
		{
			try
			{
				return entry.LastAccessedTime;
			}
			catch (NotImplementedException ex)
			{
				Logging.Trace(ex);
				return null;
			}
		}

		public static DateTime? GetArchivedTime(IArchiveEntry entry)
		{
			try
			{
				return entry.ArchivedTime;
			}
			catch (NotImplementedException ex)
			{
				Logging.Trace(ex);
				return null;
			}
		}
	}
}

