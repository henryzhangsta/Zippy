using System;
using System.Collections.Generic;

namespace Zippy
{
	public class ArchiveInfo
	{
		public static readonly IReadOnlyCollection<string> ALLOWED_TYPES = new string[] {
			"gz", "bz2", "X", "xz", "jar", "zip", "rar", "7z"
		};
	}
}

