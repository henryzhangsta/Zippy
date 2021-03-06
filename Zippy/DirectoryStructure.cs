﻿using System;
using System.Linq;
using System.Collections.Generic;
using SharpCompress.Archive;

namespace Zippy
{
	public class DirectoryNode
	{
		Dictionary<string, IArchiveEntry> files;
		Dictionary<string, DirectoryNode> directories;
		IArchiveEntry node;

		public DirectoryNode()
		{
			files = new Dictionary<string, IArchiveEntry>();
			directories = new Dictionary<string, DirectoryNode>();
		}

		public DirectoryNode(IArchiveEntry node) : this()
		{
			this.node = node;
		}

		public void AddEntry(List<string> path, IArchiveEntry entry)
		{
			if (path.Count > 1)
			{
				DirectoryNode next;
				if (directories.ContainsKey(path.First()))
				{
					next = directories[path.First()];
				}
				else
				{
					next = new DirectoryNode();
					directories.Add(path.First(), next);
				}

				path.RemoveAt(0);
				next.AddEntry(path, entry);
			}
			else
			{
				if (entry.IsDirectory)
				{
					directories.Add(path.First(), new DirectoryNode(entry));
				}
				else
				{
					files.Add(path.First(), entry);
				}
			}
		}

		public IArchiveEntry GetFile(List<string> path)
		{
			if (path.Count > 1)
			{
				string next = path.First();
				path.RemoveAt(0);
				return directories[next].GetFile(path);
			}

			return files[path.First()];
		}

		public List<IArchiveEntry> GetFiles(List<string> path)
		{
			if (path.Count > 0)
			{
				string next = path.First();
				path.RemoveAt(0);
				return directories[next].GetFiles(path);
			}

			return files.Values.ToList();
		}

		public DirectoryNode GetDirectory(List<string> path)
		{
			if (path.Count > 1)
			{
				string next = path.First();
				path.RemoveAt(0);
				return directories[next].GetDirectory(path);
			}

			return directories[path.First()];
		}

		public List<string> GetDirectories(List<string> path)
		{
			if (path.Count > 0)
			{
				string next = path.First();
				path.RemoveAt(0);
				return directories[next].GetDirectories(path);
			}

			return directories.Keys.ToList();
		}

		public List<string> GetLeafDirectories()
		{
			List<string> leaves = new List<string>();

			foreach (KeyValuePair<string, DirectoryNode> kv in directories)
			{
				if (kv.Value.directories.Count > 0)
				{
					leaves.AddRange(kv.Value.GetLeafDirectories().Select((name) => kv.Key + "/" + name));
				}
				else
				{
					leaves.Add(kv.Key);
				}
			}

			return leaves;
		}

		public List<string> GetLeafDirectories(List<string> path)
		{
			if (path.Count > 1)
			{
				string next = path.First();
				path.RemoveAt(0);
				return directories[next].GetLeafDirectories(path);
			}

			if (directories[path.First()].directories.Count > 0)
			{
				return directories[path.First()].GetLeafDirectories();
			}
			else
			{
				return new List<string>(new string[]{ path.First() });
			}
		}

		public List<KeyValuePair<string, IArchiveEntry>> GetLeafFiles()
		{
			List<KeyValuePair<string, IArchiveEntry>> leaves = new List<KeyValuePair<string, IArchiveEntry>>();
			foreach (KeyValuePair<string, IArchiveEntry> kv in files)
			{
				leaves.Add(new KeyValuePair<string, IArchiveEntry>(kv.Key, kv.Value));
			}

			foreach (KeyValuePair<string, DirectoryNode> kv in directories)
			{
				leaves.AddRange(kv.Value.GetLeafFiles().Select((item) => new KeyValuePair<string, IArchiveEntry>(kv.Key + "/" + item.Key, item.Value)));
			}

			return leaves;
		}

		public List<KeyValuePair<string, IArchiveEntry>> GetLeafFiles(List<string> path)
		{
			if (path.Count > 0)
			{
				string next = path.First();
				path.RemoveAt(0);
				return directories[next].GetLeafFiles(path);
			}

			return GetLeafFiles();
		}
	}
}

