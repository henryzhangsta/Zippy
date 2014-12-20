
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Zippy
{
	public delegate string[] DecompressionHandler(NSUrl dropDestination);

	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		Archive archive;
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
			tableView.Delegate = new TableListDelegate(imageCell, textCell);

			tableView.MakeColumns(new List<string>(new string[] {"Name", "Size", "Packed Size", "Created Date", "Modified Date", "Accessed Date"}));
			tableView.DoubleClick += table_doubleClick;
			tableView.decomp = DecompressTo;

			outlineView.DataSource = ods;
		}

		#region Constructors

		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base (handle)
		{
			Initialize();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
			Initialize();
		}
		
		// Call to load from the XIB/NIB file
		public MainWindowController () : base ("MainWindow")
		{
			Initialize();
		}
		
		// Shared initialization code
		void Initialize ()
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

		[Action("openDocument:")]
		void _openDocument(NSObject sender)
		{
			NSOpenPanel panel = new NSOpenPanel();
			panel.DirectoryUrl = new NSUrl(Globals.LAST_DIRECTORY);
			panel.Prompt = "Select Archive";
			panel.CanChooseDirectories = false;
			panel.AllowsMultipleSelection = false;
			panel.AllowsOtherFileTypes = true;
			panel.AllowedFileTypes = (new string[]{ "zip", "7z" });
			panel.ReleasedWhenClosed = true;

			int result = panel.RunModal();
			Globals.LAST_DIRECTORY = panel.DirectoryUrl.AbsoluteUrl.Path.ToString();
			if (result == 1)
			{
				Console.WriteLine(panel.Url.AbsoluteUrl.Path.ToString());
				System.IO.FileInfo f = new System.IO.FileInfo(panel.Url.AbsoluteUrl.Path.ToString());
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
			}
		}

		void loadArchive(System.IO.FileInfo file)
		{
			archive = ArchiveManager.LoadArchive(file);
			path = new List<string>();

			refreshView();
		}

		void refreshView()
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

			List<SharpCompress.Common.IEntry> files = archive.GetFiles(String.Join("/", path));
			foreach (SharpCompress.Common.IEntry e in files)
			{
				Dictionary<string, Object> d = new Dictionary<string, Object>();
				d["Name"] = e.FilePath.Split('/').Last();
				d["Size"] = ByteSize.ByteSize.FromBytes(e.Size).ToString("#");
				d["Packed Size"] = (e.CompressedSize > 0) ? ByteSize.ByteSize.FromBytes(e.CompressedSize).ToString("#") : "0 B";
				try { d["Created Date"] = e.CreatedTime; }
				catch (NotImplementedException ex) {}
				try { d["Modified Date"] = e.LastModifiedTime; }
				catch (NotImplementedException ex) {}
				try { d["Accessed Time"] = e.LastAccessedTime; }
				catch (NotImplementedException ex) {}
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
			ods.SetData(archive.root);
			outlineView.ReloadData();
		}

		public string[] DecompressTo(NSUrl destination)
		{
			System.IO.DirectoryInfo target = new System.IO.DirectoryInfo(destination.FilePathUrl.Path.ToString());
			List<string> DirectoryPaths = new List<string>();
			List<KeyValuePair<string, SharpCompress.Common.IEntry>> FilePaths = new List<KeyValuePair<string, SharpCompress.Common.IEntry>>();

			foreach (int i in tableView.SelectedRows)
			{
				if (!(bool)tds.GetData()[i]["_virtual"])
				{
					string name = (string)tds.GetData()[i]["Name"];
					bool directory = (bool)tds.GetData()[i]["_directory"];

					if (directory)
					{
						DirectoryPaths.AddRange(archive.GetLeafDirectories(String.Join("/", path) + "/" + name).Select((item) => name + "/" + item));
						FilePaths.AddRange(archive.GetLeafFiles(String.Join("/", path) + "/" + name).Select((item) => new KeyValuePair<string, SharpCompress.Common.IEntry>(name + "/" + item.Key, item.Value)));
					}
					else
					{
						FilePaths.Add(new KeyValuePair<string, SharpCompress.Common.IEntry>(name, archive.GetFile(String.Join("/", path) + "/" + name)));
					}
				}
			}

			foreach (string dir in DirectoryPaths)
			{
				Console.WriteLine(dir);
				target.CreateSubdirectory(dir);
			}

			foreach (KeyValuePair<string, SharpCompress.Common.IEntry> f in FilePaths)
			{
				System.IO.FileStream fs = new System.IO.FileStream(System.IO.Path.Combine(destination.FilePathUrl.Path.ToString(), f.Key), System.IO.FileMode.Create);
				((SharpCompress.Archive.IArchiveEntry)f.Value).WriteTo(fs);
				fs.Close();
			}

			return new string[]{ };
		}
	}
}

