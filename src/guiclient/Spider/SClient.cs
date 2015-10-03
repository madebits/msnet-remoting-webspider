using System;
using System.Collections;
using System.Runtime.Remoting;
using System.IO;
using System.Threading;
using System.Text;

namespace Spider
{
	/// <summary>
	/// Summary description for SCilent.
	/// </summary>
	public class SClient
	{
		private long id;
		ISpider server;
		private TaskEndSink end;
		private frmSpider uiHandle;
		private static string resultDir = Directory.GetCurrentDirectory() + "\\result";
		private static string idDir = Directory.GetCurrentDirectory() + "\\id";

		public static string IdDir 
		{
			get { return  idDir; }
			set { idDir = value; }
		}

		public static string ResultDir 
		{
			get { return  resultDir; }
			set { resultDir = value; }
		}

		public SClient(frmSpider uiHandle)
		{
			this.uiHandle = uiHandle;
		}

		// the async callback
		private void ConsumeTaskEnd(string name)
		{
			if(name.IndexOf("!Error:") > 0)
			{
				uiHandle.AppendStatusLine(String.Format("!!! Task {0} !!!", name));
			} 
			else 
			{
				uiHandle.AppendStatusLine(String.Format("*** Task {0} has ended. ***", name));
				Download(name, true);
			}
		}

		public void ChangeId(long id)
		{
			string[] s = server.ListTasks(id);
			this.id = id;
			uiHandle.AppendStatusLine(String.Format("New id set to: {0}", id));
			if(s == null)
			{
				uiHandle.AppendStatusLine(String.Format("{0} no tasks.", id));
			} 
			else 
			{
				uiHandle.AppendStatusLine("resetting tasks...");
				for(int i = 0; i < s.Length; i++)
				{
					int pos = s[i].IndexOf(' ');
					if(pos > 0)
					{
						server.ResetTaskEnd(id,
							s[i].Substring(0, pos),
							new OnTaskEnd(end.OnTaskEndEvent));
					}
				}
				uiHandle.AppendStatusLine("resetting done.");
			}

		}

		public string Connect()
		{
			RemoteConfig.Configure("Spider.exe.config");
			server = (ISpider)RemoteConfig.GetObject(typeof(ISpider));
			end = new TaskEndSink();
			end.SetOnTaskEnd(new OnTaskEnd(ConsumeTaskEnd));
			id = server.GetClientId();
			StreamWriter ido = null;
			try
			{
				Directory.CreateDirectory(IdDir);
				ido = new StreamWriter(IdDir + "\\id" + id + ".txt");
				ido.WriteLine(id);
			} 
			finally 
			{
				if(ido != null) ido.Close();
			}
			return id.ToString();
		}

		public void CreateTask(string name, string url, int deepth, bool siteOnly)
		{
			TaskInfo t = new TaskInfo();
			t.Name = name;
			t.Url = url;
			t.Deepth = deepth;
			t.ThisSiteOnly = siteOnly;
			server.AddTask(id, t, new OnTaskEnd(end.OnTaskEndEvent));
		}

		public void RemoveTask(string name)
		{
			server.RemoveTask(id, name);
		}

		public void GetStatus(string name)
		{
			string[] status = server.GetTaskState(id, name);
			StringBuilder sb = new StringBuilder(100);
			for(int i = 0; i < status.Length; i++)
			{
				sb.Append(status[i].Replace("\n", "\r\n")).Append("\r\n");
			}
			uiHandle.AppendStatus(sb.ToString());
		}

		public void Download(string name, bool automate)
		{
			FileStream stream = (FileStream)server.GetTaskResult(id, name);
			if(stream == null) 
				throw new Exception("!Error: null stream.");
			FileDownloader f = new FileDownloader(id, server, stream,
				name, new ThreadStart(ListTasks),
				automate, uiHandle);
			f.Start();
		}

		public void ListTasks(){
			string[] s = server.ListTasks(id);
			lock(uiHandle.lstTasks){
				uiHandle.lstTasks.BeginUpdate();
				uiHandle.lstTasks.Items.Clear();
				if(s != null) 
				{
					for(int i = 0; i < s.Length; i++)
						uiHandle.lstTasks.Items.Add(s[i].Replace('\n', ' '));
				} else uiHandle.AppendStatusLine("No tasks!");
				uiHandle.lstTasks.EndUpdate();
			}
		}

	} //EOC

	class RemoteConfig
	{
		private static bool notInited = true;
		private static IDictionary types = null;
		private static string cFile;
	
		public static void Configure(string file)
		{
			cFile = file;
		}
	
		public static object GetObject(Type t)
		{
			if(notInited)
			{
				Init();
				notInited = false;
			}
			return Activator.GetObject(t, (string)types[t.ToString()]);
		}
			
		private static void Init()
		{
			RemotingConfiguration.Configure(cFile);
			WellKnownClientTypeEntry[] te = RemotingConfiguration.GetRegisteredWellKnownClientTypes();
			types = new Hashtable();
			for(int i = 0; i < te.Length; i++)
				types.Add(te[i].TypeName, te[i].ObjectUrl);
		}

	} //EOC

	public class FileDownloader
	{
		FileStream stream = null;
		Thread downThread = null;
		string tname;
		ISpider server;
		long id;
		ThreadStart listTasks;
		bool automate;
		frmSpider uiHandle;

		public FileDownloader(long id, ISpider server, FileStream stream,
			string tname, ThreadStart listTasks,
			bool automate, frmSpider uiHandle)
		{
			this.stream = stream;
			this.tname = tname;
			this.server = server;
			this.id = id;
			this.listTasks = listTasks;
			this.automate = automate;
			this.uiHandle = uiHandle;
		}
	
		public void Start()
		{
			downThread = new Thread(new ThreadStart(Work));
			downThread.Start();
		}
	
		private void Work() 
		{
			if(stream == null) return;
			BinaryWriter result = null;
			BufferedStream bstream = null;
			string name = stream.Name;
			try
			{
				Directory.CreateDirectory(SClient.ResultDir);
				int pos = name.LastIndexOf('\\');
				if(pos > 0)
					name = name.Substring(pos + 1, name.Length - pos - 1);
				name = SClient.ResultDir + "\\" + tname + "_" + name;
				uiHandle.AppendStatusLine(String.Format("Downloading {0} result to:\r\n\t{1}", tname, name));
				result = new BinaryWriter(new FileStream(name,
					FileMode.Create, FileAccess.Write));
				int b = -1;
				bstream = new BufferedStream(stream);
				while((b = bstream.ReadByte()) != -1)
					result.Write((byte)b);
				stream.Close();
				stream = null;
				result.Close();
				result = null;
				uiHandle.AppendStatusLine(String.Format("Done:   {0}", name));
				if(automate)
				{
					uiHandle.AppendStatusLine(String.Format("Removing task: {0}", tname));
					server.RemoveTask(id, tname);
					listTasks();
				}
			} 
			catch(Exception e)
			{
				uiHandle.AppendStatusLine("Error: FileDownloader " + tname);
				frmSpider.ShowError("FileDownloader " + e.Message);
				uiHandle.AppendStatusLine(e.StackTrace);
			} 
			finally 
			{
				if(stream != null) stream.Close();
				if(bstream != null) bstream.Close();
				if(result != null) result.Close();
			}
		}
	} //EOC


}
