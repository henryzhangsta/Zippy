
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using SharpCompress.Archive;

namespace Zippy
{
	public delegate string[] DecompressionHandler(NSUrl dropDestination);

	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		Archive archive;
		FileInfo file;
		List<string> path;

		TableListDataSource tds;
		FileListOutlineDataSource ods;

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			foreach (NSTableColumn tc in tableView.TableColumns())
			{
				tableView.RemoveColumn(tc);
			}
				
			tds = new TableListDataSource();
			ods = new FileListOutlineDataSource();

			tableView.DataSource = tds;
			tableView.Delegate = new TableListDelegate(tableImageCell, tableTextCell);

			tableView.MakeColumns(new List<string>(new string[] { "Name", "Size", "Packed Size", "Created Date", "Modified Date", "Accessed Date", "CRC" }));
			tableView.DoubleClick += table_doubleClick;
			tableView.decomp = DecompressTo;

			outlineView.DataSource = ods;
			outlineView.Delegate = new FileListOutlineDelegate(outlineImageCell);

			outlineView.Target = this;
			outlineView.DoubleAction = new MonoMac.ObjCRuntime.Selector("doubleClickedAction:");
		}

		#region Constructors

		// Called when created from unmanaged code
		public MainWindowController(IntPtr handle) : base(handle)
		{
			Initialize();
		}
		
		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MainWindowController(NSCoder coder) : base(coder)
		{
			Initialize();
		}
		
		// Call to load from the XIB/NIB file
		public MainWindowController() : base("MainWindow")
		{
			Initialize();
		}
		
		// Shared initialization code
		void Initialize()
		{

		}

		#endregion

		//strongly typed window accessor
		public new MainWindow Window
		{
			get
			{
				return (MainWindow)base.Window;
			}
		}

		[Action("outlineViewSelectionDidChange:")]
		void _selectionChange(NSObject sender)
		{
			if (outlineView.SelectedRow >= 0 && outlineView.SelectedRowCount == 1)
			{
				FileListOutlineItem item = (FileListOutlineItem)outlineView.ItemAtRow(outlineView.SelectedRow);
				if (item.IsDirectory)
				{
					List<string> selectedPath = new List<string>();
					while (item != null && outlineView.LevelForItem(item) > 0)
					{
						selectedPath.Insert(0, item.GetName());
						item = (FileListOutlineItem)outlineView.GetParent(item);
					}

					path = selectedPath;
					refreshView();
				}
			}
		}

		[Action("openDocument:")]
		void _openDocument(NSObject sender)
		{
			NSOpenPanel panel = new NSOpenPanel();
			panel.DirectoryUrl = new NSUrl(Globals.LAST_DIRECTORY);
			panel.Prompt = "Select Archive";
			panel.CanChooseDirectories = false;
			panel.AllowsMultipleSelection = false;
			panel.AllowsOtherFileTypes = true;
			panel.AllowedFileTypes = ArchiveInfo.ALLOWED_TYPES.ToArray();
			panel.ReleasedWhenClosed = true;

			int result = panel.RunModal();
			Globals.LAST_DIRECTORY = panel.DirectoryUrl.AbsoluteUrl.Path.ToString();
			if (result == 1)
			{
				FileInfo f = new FileInfo(panel.Url.AbsoluteUrl.Path.ToString());
				loadArchive(f);
			}
		}

		void table_doubleClick(Object sender, EventArgs e)
		{
			if (tableView.SelectedRow >= 0)
			{
				Dictionary<string, Object> row = tds.GetData()[tableView.SelectedRow];
				if ((bool)row["_directory"])
				{
					if (row.ContainsKey("_up"))
					{
						path.RemoveAt(path.Count - 1);
					}
					else
					{
						path.Add((string)row["Name"]);
					}
					refreshView();
				}
				else
				{
					IArchiveEntry fileEntry = archive.GetFile(String.Join("/", path) + "/" + (string)row["Name"]);
					string folderPath = Path.Combine(Path.GetTempPath(), new NSUuid().ToString());
					DirectoryInfo folder = Directory.CreateDirectory(folderPath);
					string filePath = Path.Combine(folderPath, fileEntry.FilePath.Split('/').Last());
					FileStream fs = new FileStream(filePath, FileMode.Create);
					fileEntry.WriteTo(fs);
					fs.Close();

					new NSWorkspace().OpenFile(filePath);
				}
			}
		}

		[Action("doubleClickedAction:")]
		void outline_doubleClick(NSObject sender)
		{
			if (outlineView.SelectedRow >= 0 && outlineView.SelectedRowCount == 1 && outlineView.LevelForRow(outlineView.SelectedRow) > 0)
			{
				FileListOutlineItem item = (FileListOutlineItem)outlineView.ItemAtRow(outlineView.SelectedRow);
				if (!item.IsDirectory)
				{
					IArchiveEntry fileEntry = item.GetEntry();
					string folderPath = Path.Combine(Path.GetTempPath(), new NSUuid().ToString());
					DirectoryInfo folder = Directory.CreateDirectory(folderPath);
					string filePath = Path.Combine(folderPath, fileEntry.FilePath.Split('/').Last());
					FileStream fs = new FileStream(filePath, FileMode.Create);
					fileEntry.WriteTo(fs);
					fs.Close();

					new NSWorkspace().OpenFile(filePath);
				}
			}
		}

		public void loadArchive(FileInfo file)
		{
			this.file = file;
			archive = ArchiveManager.LoadArchive(file);
			path = new List<string>();

			refreshView(true);
		}

		void refreshView()
		{
			refreshView(false);
		}

		void refreshView(bool initialLoad)
		{
			List<string> directories = archive.GetDirectories(String.Join("/", path));

			List<Dictionary<string, Object>> data = new List<Dictionary<string, Object>>();

			foreach (string s in directories)
			{
				Dictionary<string, Object> d = new Dictionary<string, Object>();
				d["Name"] = s;
				d["_virtual"] = false;
				d["_directory"] = true;
				data.Add(d);
			}

			List<IArchiveEntry> files = archive.GetFiles(String.Join("/", path));
			foreach (IArchiveEntry e in files)
			{
				Dictionary<string, Object> d = new Dictionary<string, Object>();
				d["Name"] = e.FilePath.Split('/').Last();
				d["Size"] = ByteSize.ByteSize.FromBytes(e.Size).ToString("#");
				d["Packed Size"] = (e.CompressedSize > 0) ? ByteSize.ByteSize.FromBytes(e.CompressedSize).ToString("#") : "0 B";
				d["Created Date"] = ArchiveUtils.GetCreatedTime(e);
				d["Modified Date"] = ArchiveUtils.GetModifiedTime(e);
				d["Accessed Date"] = ArchiveUtils.GetAccessedTime(e);
				d["CRC"] = e.Crc;
				d["_virtual"] = false;
				d["_directory"] = false;
				data.Add(d);
			}

			if (path.Count > 0)
			{
				Dictionary<string, object> up = new Dictionary<string, object>();
				up["Name"] = "..";
				up["_directory"] = true;
				up["_virtual"] = true;
				up["_up"] = true;
				data.Insert(0, up);
			}

			tds.UpdateView(data);
			tableView.ReloadData();

			if (initialLoad)
			{
				ods.SetData(file.Name, archive.root);
				outlineView.ReloadData();
			}

			outlineView.ExpandItem(ods.GetNode(path.ToArray()));
		}

		public string[] DecompressTo(NSUrl destination)
		{
			DirectoryInfo target = new DirectoryInfo(destination.FilePathUrl.Path.ToString());
			List<string> DirectoryPaths = new List<string>();
			List<KeyValuePair<string, IArchiveEntry>> FilePaths = new List<KeyValuePair<string, IArchiveEntry>>();

			foreach (int i in tableView.SelectedRows)
			{
				if (!(bool)tds.GetData()[i]["_virtual"])
				{
					string name = (string)tds.GetData()[i]["Name"];
					bool directory = (bool)tds.GetData()[i]["_directory"];

					if (directory)
					{
						DirectoryPaths.AddRange(archive.GetLeafDirectories(String.Join("/", path) + "/" + name).Select((item) => name + "/" + item));
						FilePaths.AddRange(archive.GetLeafFiles(String.Join("/", path) + "/" + name).Select((item) => new KeyValuePair<string, IArchiveEntry>(name + "/" + item.Key, item.Value)));
					}
					else
					{
						FilePaths.Add(new KeyValuePair<string, IArchiveEntry>(name, archive.GetFile(String.Join("/", path) + "/" + name)));
					}
				}
			}

			foreach (string dir in DirectoryPaths)
			{
				Console.WriteLine(dir);
				target.CreateSubdirectory(dir);
			}

			foreach (KeyValuePair<string, IArchiveEntry> f in FilePaths)
			{
				FileStream fs = new FileStream(Path.Combine(destination.FilePathUrl.Path.ToString(), f.Key), FileMode.Create);
				((IArchiveEntry)f.Value).WriteTo(fs);
				fs.Close();
			}

			return new string[]{ };
		}
	}
}

