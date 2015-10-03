// .Net Remote Spider Demo
// (c) - Copyright 2003 by Vasian CEPA
// http://madebits.com

using System;
using System.IO;
using System.Threading;
using System.Runtime.Remoting;

// <summary>
//	the console spider client
// </summary>
public class SpiderClient {

	private ISpider server;
	private TaskEndSink end;
	private long id;
	
	public SpiderClient(ISpider s){
		if(s == null) throw new ArgumentNullException();
		end = new TaskEndSink();
		end.SetOnTaskEnd(new OnTaskEnd(ConsumeTaskEnd));
		server = s;
		id = server.GetClientId();
		WriteInfo();
		
	}

	private void WriteInfo(){
		RemotingConfiguration.ApplicationName = "v-web.demo-net.spider";
		Console.WriteLine("ProgName:   {0}", RemotingConfiguration.ApplicationName);
		Console.WriteLine("ClientId:   {0}", id);
		StreamWriter ido = null;
		try{
			Directory.CreateDirectory(@".\id");
			ido = new StreamWriter(@".\id\id" + id + ".txt");
			ido.WriteLine(id);
		} finally {
			if(ido != null) ido.Close();
		}
	}
	
	// the async callback
	private void ConsumeTaskEnd(string name){
			if(name.IndexOf("!Error:") > 0){
				Console.WriteLine("\n\n!!! Task {0} !!!", name);
			} else {
				Console.WriteLine("\n\n*** Task {0} has ended. ***", name);
				Download(name, true);
			}
	}

	public void Work(){
		string choice = null;
		WriteMenu();
		while(true){
			Console.Write(": ");
			choice = Console.ReadLine();
			if(choice == null || choice == "") break;
			parseChoice(choice);
		}
	}

	private void WriteMenu(){
		Console.WriteLine("");
		Console.WriteLine("Press Enter to quit or use one of these commands:");
		Console.WriteLine("");
		Console.WriteLine("$id                   :\tchange client id");		// change id
		Console.WriteLine("+name,deepth,url,true :\tadd a task");	// add
		Console.WriteLine("?name                 :\tget task status");		// status
		Console.WriteLine("-name                 :\tremove a task");		// remove, get result
		Console.WriteLine("<name                 :\tget task result");		// get result
		Console.WriteLine("*                     :\tlist tasks");			// list
		Console.WriteLine("m                     :\tshow this menu");			// menu
		Console.WriteLine("");
	}

	private void parseChoice(string choice){
		switch(choice[0]){
			case '$':	try{	
						id = Convert.ToInt64(choice.Substring(1));
						Console.WriteLine("New client id: {0}", id);
						// automatic task callback reset
						string[] s = server.ListTasks(id);
						if(s == null){
							Console.WriteLine("${0} no tasks.", id);
						} else {
							Console.WriteLine("resetting tasks...");
							for(int i = 0; i < s.Length; i++){
								int pos = s[i].IndexOf(' ');
								if(pos > 0){
									server.ResetTaskEnd(id,
										s[i].Substring(0, pos),
										new OnTaskEnd(end.OnTaskEndEvent));
								}
							}
							Console.WriteLine("resetting done.");
							ListTasks();
						}
					} catch(FormatException e){
						Console.WriteLine("!Error: {0}", e.Message);
					}
					break;
			case '+':	string[] parts = choice.Split(new Char[] {','});
					if((parts.Length < 3) || (parts.Length > 4)) {
						Console.WriteLine("!Error: Input string was not in a correct format.");
						break;
					}
					try{
						TaskInfo t = new TaskInfo();
						t.Name = parts[0].Substring(1);
						t.Url = parts[2];
						t.Deepth = Convert.ToInt32(parts[1]);
						if(parts.Length > 3){
							t.ThisSiteOnly = Convert.ToBoolean(parts[3]);
						} else t.ThisSiteOnly = true;
						server.AddTask(id, t, new OnTaskEnd(end.OnTaskEndEvent));
					} catch(Exception e){
						Console.WriteLine("!Error: {0}", e.Message);
					}
					break;
			
			case '-':	try{	
						server.RemoveTask(id, choice.Substring(1));
					} catch(Exception e){
						Console.WriteLine("!Error: {0}", e.Message);
					}
					break;
			case '*':	try{	
						ListTasks();
					} catch(Exception e){
						Console.WriteLine("!Error: {0}", e.Message);
					}	
					break;
			case '<':	try{
						Download(choice.Substring(1), false);
					} catch(Exception e){
						Console.WriteLine("!Error: {0}", e.Message);
					}
					break;
			case '?':	try{
						string[] status = server.GetTaskState(id, choice.Substring(1));
						Console.WriteLine("- status -");
						if(status == null) break;
						for(int i = 0; i < status.Length; i++){
							Console.WriteLine("| {0}", status[i]);
						}
						
					} catch(Exception e){
						Console.WriteLine("!Error: {0}", e.Message);
					}			
					break;
			case 'm':	WriteMenu();
					break;
			default:	Console.WriteLine("!Error: Unknown command.");
					break;
		}
		
		
	}
	
	private void ListTasks(){
		string[] s = server.ListTasks(id);
		if(s == null){
			Console.WriteLine("${0} no tasks.", id);
		} else {
			Console.WriteLine("- tasks -");
			for(int i = 0; i < s.Length; i++)
			Console.WriteLine(s[i]);
		}
	}
	
	private void Download(string name, bool automate){
		FileStream stream = (FileStream)server.GetTaskResult(id, name);
		if(stream == null) {
			Console.WriteLine("!Error: null stream.");
			return;
		}
		FileDownloader f = new FileDownloader(id, server, stream,
			name, new ThreadStart(ListTasks), automate);
		f.Start();
	}
		
} //EOC

// <summary>
//	downloads a task result file and saves it locally
// </summary>
public class FileDownloader{
	FileStream stream = null;
	Thread downThread = null;
	string tname;
	ISpider server;
	long id;
	ThreadStart listTasks;
	bool automate;
	
	public FileDownloader(long id, ISpider server, FileStream stream,
			string tname, ThreadStart listTasks, bool automate){
		this.stream = stream;
		this.tname = tname;
		this.server = server;
		this.id = id;
		this.listTasks = listTasks;
		this.automate = automate;
	}
	
	public void Start(){
		downThread = new Thread(new ThreadStart(Work));
		downThread.Start();
	}
	
	private void Work() {
		if(stream == null) return;
		BinaryWriter result = null;
		BufferedStream bstream = null;
		string name = stream.Name;
		try{
			Directory.CreateDirectory(@".\result");
			int pos = name.LastIndexOf('\\');
			if(pos > 0)
				name = name.Substring(pos + 1, name.Length - pos - 1);
			name = ".\\result\\" + tname + "_" + name;
			Console.WriteLine("Downloading {0} result to:\n\t{1}", tname, name);
			result = new BinaryWriter(new FileStream(name,
				FileMode.Create, FileAccess.Write));
			int b = -1;
			bstream = new BufferedStream(stream);
			while((b = bstream.ReadByte()) != -1)
				result.Write((byte)b);
			Console.WriteLine("Done:   {0}", name);
			if(automate){
				Console.WriteLine("Removing task: {0}", tname);
				server.RemoveTask(id, tname);
				Console.WriteLine("Checking other tasks ...");
				listTasks();
			}
		} catch(Exception e){
			Console.WriteLine("!Error: FileDownloader {0}", e.Message);
			Console.WriteLine(e.StackTrace);
		} finally {
			if(stream != null) stream.Close();
			if(bstream != null) bstream.Close();
			if(result != null) result.Close();
			// reshow prompt
			Console.Write("\n: ");
		}
	}
	
} //EOC