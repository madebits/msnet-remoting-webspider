// .Net Remote Spider Demo
// (c) - Copyright 2003 by Vasian CEPA
// http://madebits.com

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Configuration;
using System.Diagnostics;
using System.Collections;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

// <summary>
//	service policy constants
// </summary>
public class ServicePolicy{

	private static int maxClients = 100;
	private static int maxTasks = 100;
	private static int threadsPerTask = 5;
	private static long taskStoreTime = 3600000 * 24; // one day in milisec
	private static string tempDir = @".\temp";
	private static int maxDeepth = 5;
	private static bool cleanClientDataOnExit = true;
	private static bool allowPartialResult = true;
	private static bool allowLocalUrls = true; // true during testing
	
	public static bool CleanClientDataOnExit {
		get { return cleanClientDataOnExit; }
		set {
			cleanClientDataOnExit = value;
		}
	}
	
	public static bool AllowLocalUrls {
			get { return allowLocalUrls; }
			set {
				allowLocalUrls = value;
			}
	}
	
	public static bool AllowPartialResult {
		get { return allowPartialResult; }
		set {
			allowPartialResult = value;
		}
	}
	
	public static int MaxClients {
		get { return maxClients; }
		set {
			if(value < 50) maxClients = 50;
			else maxClients = value;
		}
	}
	
	public static int MaxTasks {
		get { return maxTasks; }
		set {
			if(value < 50) maxTasks = 50;
			else maxTasks = value;
		}
	}
	
	public static int MaxDeepth {
		get { return maxDeepth; }
		set {
			// zero or negative means all
			maxDeepth = value;
		}
	}
	
	public static int ThreadsPerTask {
		get { return threadsPerTask; }
		set {
			if(value < 1) threadsPerTask = 1;
			else if(value > 15) threadsPerTask = 15;
			else threadsPerTask = value;
		}
	}
	
	public static long TaskStoreTime {
		get { return taskStoreTime; }
		set {
			if(value < 3600) taskStoreTime = 3600000;
			else taskStoreTime = value;
		}
	}
	
	public static string TempDir {
		get { return tempDir; }
		set {
			if(value != null && !value.Equals(string.Empty)) tempDir = value;
		}
	}
	
	public static void Init(){
		try {
			Console.Write("Reading ServicePolicy settings from config file ... ");
			System.Collections.Specialized.NameValueCollection sr = ConfigurationSettings.AppSettings;
			foreach(string keyName in sr.AllKeys){
				switch(keyName){
				case "MaxClients":
					ServicePolicy.MaxClients = Convert.ToInt32(sr[keyName]);
					break;
				case "MaxTasks":
					ServicePolicy.MaxTasks = Convert.ToInt32(sr[keyName]);
					break;
				case "MaxDeepth":
					ServicePolicy.MaxDeepth = Convert.ToInt32(sr[keyName]);
					break;
				case "ThreadsPerTask":
					ServicePolicy.ThreadsPerTask = Convert.ToInt32(sr[keyName]);
					break;
				case "TaskStoreTime":
					// in minutes in config file -> to miliseconds
					ServicePolicy.TaskStoreTime = Convert.ToInt64(sr[keyName]) * 60000;
					break;
				case "TempDir":
					ServicePolicy.TempDir = sr[keyName].ToString();
					break;
				case "CleanClientDataOnExit":
					ServicePolicy.CleanClientDataOnExit = Convert.ToBoolean(sr[keyName]);
					break;
				case "AllowPartialResult":
					ServicePolicy.AllowPartialResult = Convert.ToBoolean(sr[keyName]);
					break;
					
				case "AllowLocalUrls":
					ServicePolicy.AllowLocalUrls = Convert.ToBoolean(sr[keyName]);
					break;
				}
			}
			Console.WriteLine("Done.");
		} catch(Exception e){
			Console.WriteLine("!Error: ServicePolicy" + e.Message);
		}
	}
	
} //EOC

// <summary>
//	the server object, delegates to TaskManager
// </summary>
public class Spider : MarshalByRefObject, ISpider {

	TaskManager tm;
	
	public Spider(){
		Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
		Debug.AutoFlush = true;
		ServicePolicy.Init();
		tm = new TaskManager();
		SpiderTask.tm = tm;
	}

	public long GetClientId(){
		return DateTime.Now.Ticks;
	}
	
	public void AddTask(long id, TaskInfo t, OnTaskEnd onTaskEndSink){
		InputValidator.Validate(id);
		InputValidator.Validate(t);
		Console.WriteLine("AddTask       {0}/{1}", id, t.Name);
		SpiderTask task = new SpiderTask(id, t, onTaskEndSink);
		tm.Add(id, task);
		task.Start();
	}
	
	public void RemoveTask(long id, string name){
		InputValidator.Validate(id);
		InputValidator.ValidateName(name);
		Console.WriteLine("RemoveTask    {0}/{1}", id, name);
		tm.Remove(id, name);
	}
	
	public string[] ListTasks(long id){
		InputValidator.Validate(id);
		Console.WriteLine("ListTasks     {0}", id);
		return tm.List(id);
	}
	
	public string[] GetTaskState(long id, string name){
		InputValidator.Validate(id);
		InputValidator.ValidateName(name);
		Console.WriteLine("GetTaskState  {0}/{1}", id, name);
		return tm.State(id, name);
	}
	
	public Stream GetTaskResult(long id, string name){
		InputValidator.Validate(id);
		InputValidator.ValidateName(name);
		Console.WriteLine("GetTaskResult {0}/{1}", id, name);
		return tm.Result(id, name);
	}
	
	public void ResetTaskEnd(long id, string name, OnTaskEnd onTaskEndSink){
		InputValidator.Validate(id);
		InputValidator.ValidateName(name);
		Console.WriteLine("ResetTaskEnd  {0}/{1}", id, name);
		if(onTaskEndSink != null)
			tm.Reset(id, name, onTaskEndSink);
	}
	
	public override object InitializeLifetimeService(){
		return null;
	}
   
} // EOC

// <summary>
// if we use strong naming, the validation logic
// can be placed in TaskInfo class and run in client
// </summary>
class InputValidator{

	public static void Validate(TaskInfo t){
		ValidateName(t.Name);
		ValidateUrl(t.Url);
		ValidateDeepth(t.Deepth);
	}
	
	public static void Validate(long id){
		if(
			(id < 631784301163902640) ||
			(id > DateTime.MaxValue.Ticks)
		)  throw new SpiderException("Client id out of range.");
	}
	
	public static void ValidateName(string name){
		if(
			(name == null) ||
			(name.Equals(string.Empty)) ||
			(name.Length > 256)
		) throw new SpiderException("Task name null or longer than 256 chars.");
	}
	
	public static void ValidateUrl(string url){
		if(
			(url == null) ||
			(!url.ToLower().StartsWith("http://"))
		) throw new SpiderException("Only http urls are supported");
		if(url.Length > 2096) throw new SpiderException("Url is too long!");
		if(url.Length < 8)  throw new SpiderException("Url is too short!");
		// try also this
		try {
			(new Uri(url)).GetLeftPart(UriPartial.Authority);
		} catch (Exception ex) {
			throw new SpiderException(ex.Message);
		}
		if(!ServicePolicy.AllowLocalUrls){
			if(
				url.ToLower().StartsWith("http://localhost") ||
				url.ToLower().StartsWith("http://127.0.0.1") 
			) throw new SpiderException("Local urls have not meaning!");
		}
	}
	
	public static void ValidateDeepth(int deepth){
		if(
			((deepth < 0) && (ServicePolicy.MaxDeepth > 0)) ||
			((deepth > 0) && (deepth > ServicePolicy.MaxDeepth))
		) throw new SpiderException("Service restricts max deepth to be: " + ServicePolicy.MaxDeepth);
	}
	
} //EOC

// <summary>
//	Manages the clients and spider tasks
// </summary>
public class TaskManager{

	private Hashtable clients;
			
	public TaskManager(){
		clients = Hashtable.Synchronized(new Hashtable(100));
	}
		
	public void Add(long id, SpiderTask task){
		if(!clients.Contains(id)){
			if(clients.Count > ServicePolicy.MaxClients)
				throw new SpiderException("Clients number equals max allowed.");
		}
		SortedList tasks = (SortedList)clients[id];
		if(tasks == null) tasks = SortedList.Synchronized(new SortedList(30));
		// each client has its own copy of tasks, so they obtain
		// different locks and this method does really block
		// sync is needed however if two or more copies
		// of the same client (id) run at the same time
		// the same is valid for other methods also
		lock(tasks.SyncRoot){
			if(tasks[task.TaskInfo.Name] != null) throw new SpiderException("Task exists.");
			if(tasks.Count > ServicePolicy.MaxTasks) throw new SpiderException("Tasks number equals max allowed.");
			tasks.Add(task.TaskInfo.Name, task);
			clients[id] = tasks;
		}
	}
	
	public void Remove(long id, string name){
		SortedList tasks = GetTasks(id);
		lock(tasks.SyncRoot){
			int pos = tasks.IndexOfKey(name);
			if(pos < 0)
				throw new SpiderException("No such task (" + name + ") for client: " + id);
			SpiderTask task = (SpiderTask)tasks.GetByIndex(pos);
			tasks.RemoveAt(pos);
			if(tasks.Count == 0){
				// remove this client from list
				tasks = null;
				clients.Remove(id);
			}
			task.FreeResources();
			task = null;
		}
	}
	
	public string[] List(long id){
		SortedList tasks = (SortedList)clients[id];
		if((tasks == null) || (tasks.Count == 0)) return null;
		string[] result = null;
		lock(tasks.SyncRoot){
			result = new string[tasks.Count];
			for(int i = 0; i < tasks.Count; i++){
				result[i] = ((SpiderTask)tasks.GetByIndex(i)).ToString();
			}
		}
		return result;
	}
	
	public Stream Result(long id, string name){
		SortedList tasks = GetTasks(id);
		Stream result = null;
		lock(tasks.SyncRoot){
			SpiderTask task = (SpiderTask)tasks[name];
			if(task == null)
				throw new SpiderException("No such task (" + name + ") for client: " + id);
			result = task.GetResultStream();
		}
		return result;
	}
	
	public string[] State(long id, string name){
		Debug.WriteLine(ToString());
		SortedList tasks = GetTasks(id);
		string[] result;
		lock(tasks.SyncRoot){
			SpiderTask task = (SpiderTask)tasks[name];
			if(task == null)
				throw new SpiderException("No such task (" + name + ") for client: " + id);			
			result = task.GetState();
		}
		return result;
	}
	
	public void Reset(long id, string name, OnTaskEnd onTaskEndSink){
		SortedList tasks = GetTasks(id);
		lock(tasks.SyncRoot){
			SpiderTask task = (SpiderTask)tasks[name];
			if(task == null)
				throw new SpiderException("No such task (" + name + ") for client: " + id);
			task.ResetEndListener(onTaskEndSink);
		}
	}
	
	private SortedList GetTasks(long id){
		SortedList tasks = null;
		lock(clients){
			if(!clients.Contains(id))
				throw new SpiderException("Client : " + id + " has no tasks!");
			tasks = (SortedList)clients[id];
		}
		if(tasks == null) throw new SpiderException("No tasks for client: " + id);
		return tasks;
	}
	
	public override string ToString(){
		// this method is really a bottleneck
		// to be used during debuging olny
		StringBuilder sb = new StringBuilder();
		sb.Append("- service state -\n");
		lock(clients){
			IDictionaryEnumerator en = clients.GetEnumerator();
			while(en.MoveNext()){
				SortedList tasks = (SortedList)en.Value;
				sb.Append(en.Key.ToString());
				if(tasks == null) sb.Append(0).Append(" (");
				else sb.Append(tasks.Count);
				sb.Append(")\n");
				if(tasks == null) continue;
				for(int i = 0; i < tasks.Count; i++){
					SpiderTask task = (SpiderTask)tasks.GetByIndex(i);
					sb.Append('\t').Append(task.ToString()).Append('\n');
				}
			}
			sb.Append(clients.Count).Append(" client(s).\n");
		}
		return sb.ToString();
	}
	
} //EOC

// <summary>
//	manages a spider task
// </summary>
public class SpiderTask : IComparable {
	
	public static TaskManager tm = null; // only one
	private bool clean = false;
	private string destroyTime = null;
	private string resultFileName = null;
	private Thread workThread = null;
	private long id;
	private TaskInfo t;
	private OnTaskEnd onTaskEndSink;
	private Timer timeToLive = null; // time to live after completing work
	private StreamWriter resultFile = null;
	private HttpSpider spider = null;
		
	public enum SpiderTaskStatus { Running, Done };
	SpiderTaskStatus status;

	public SpiderTask(long id, TaskInfo t, OnTaskEnd onTaskEndSink){
		this.id = id;
		this.t = t;
		this.onTaskEndSink = onTaskEndSink;
		resultFileName = SUtils.CreateDirs(id, t.Name)
				+ '\\' + SUtils.Url2FileName(t.Url);
	}

	public TaskInfo TaskInfo {
		get { return t; }
	}

	public int CompareTo(object o) {
		if(!(o is SpiderTask)) throw new InvalidCastException();
		return t.CompareTo(((SpiderTask)o).TaskInfo);
	}

	public void ResetEndListener(OnTaskEnd onTaskEndSink){
		if(status == SpiderTaskStatus.Running)
			this.onTaskEndSink = onTaskEndSink;
		else onTaskEndSink(t.Name);
	}

	public void Start(){
		status = SpiderTaskStatus.Running;
		try {
			workThread = new Thread(new ThreadStart(DoWork));
			workThread.Start();
		} catch(Exception e1){
			status = SpiderTaskStatus.Done;
			try {
				tm.Remove(id, t.Name);
			} catch(Exception){}
			throw new SpiderException(e1.Message);
		}
	}
	
	private void DoWork(){
		try {
			//Debug.WriteLine("SpiderTask-DoWork() " + resultFileName);
			resultFile = new StreamWriter(resultFileName);
			spider = new HttpSpider(t, resultFile, new ThreadStart(WorkEnd));
			spider.Start();
		} catch(ThreadAbortException){
			if(spider != null){
				try {
					spider.Stop();
				} catch(Exception){}
				spider = null;
			}
			//Thread.ResetAbort();
		} catch(Exception e1){
			status = SpiderTaskStatus.Done;
			try {
				onTaskEndSink(t.Name + " !Error: " + e1.Message);
			} catch(Exception){}
			tm.Remove(id, t.Name);
		}
	}
	
	private void WorkEnd(){
		spider = null;
		workThread = null;
		if(resultFile != null){
			resultFile.Close();
			resultFile = null;
		}
		status = SpiderTaskStatus.Done;
		TimerCallback timerDelegate = new TimerCallback(OnTaskStoreTimeEnd);
		timeToLive = new Timer(timerDelegate, this, ServicePolicy.TaskStoreTime, Timeout.Infinite);
		SetDestroyTime();
		try {
			onTaskEndSink(t.Name);
		} catch(Exception){
			// if client is not alive then timeToLive will free
			// the resources after ServicePolicy.TaskStoreTime time 
		}
	}
	
	private void SetDestroyTime(){
		DateTime now = DateTime.Now;
		StringBuilder sb = new StringBuilder();
		sb.Append("Finished at ");
		sb.Append(now.ToString("yyyy-MM-dd hh:mm:ss"));
		sb.Append(". Live up to ");
		now = now.Add(new TimeSpan(ServicePolicy.TaskStoreTime * TimeSpan.TicksPerMillisecond));
		sb.Append(now.ToString("yyyy-MM-dd hh:mm:ss"));
		destroyTime = sb.ToString();
	}
	
	public SpiderTaskStatus State(){
		return SpiderTaskStatus.Running;
	}
	
	public void OnTaskStoreTimeEnd(Object state){
		Console.WriteLine("Self-destroying timeout task {0}/{1}", id, t.Name);
		tm.Remove(id, t.Name);
	}
	
	// to be called when task is deleted
	public void FreeResources(){
		Thread end = new Thread(new ThreadStart(ThreadDispose));
		end.Start();
	}
	
	private void ThreadDispose(){
		Dispose(false);
	}
	
	private void Dispose(bool infinalize){
		if(clean) return;
		clean = true;
		try{
			if(timeToLive != null) timeToLive.Dispose();
			if(workThread != null){
				if(spider != null){
					spider.Stop();
					spider = null;
				}
				workThread.Abort();
				workThread.Join(180000);
				workThread = null;
			}
			if(resultFile != null){
				resultFile.Close();
				resultFile = null;
			}
			if(infinalize && ServicePolicy.CleanClientDataOnExit)
				SUtils.CleanFiles(id, t.Name);
			else {
				try{
					SUtils.CleanFiles(id, t.Name);
				} catch(Exception){};
			}
		} catch(Exception e){
			Console.WriteLine("!Error: SpiderTask-Dispose " + e.Message);
		}
	}
	
	public string[] GetState(){
		string[] state = new string[7];
		state[0] = "Name          : " + t.Name;
		state[1] = "Deepth        : " + t.Deepth.ToString();
		state[2] = "Url           : " + t.Url;
		state[3] = "Status        : " + status.ToString();
		state[4] = "ResultFile    : " + resultFileName;
		state[5] = "DestroyTime   : " + ((destroyTime == null) ? "empty" : destroyTime);
		state[6] = "Spider        : " + ((spider == null) ? "empty" : spider.GetState());
		return state;
	}
	
	public Stream GetResultStream(){
		if(!File.Exists(resultFileName)) throw new SpiderException("Result file is empty.");
		string file = resultFileName;
		if(status == SpiderTaskStatus.Running){
			if(!ServicePolicy.AllowPartialResult) throw new SpiderException("Task is not finished yet.");
			file = (SUtils.GetDir(id, t.Name)[1]);
			file = file + '\\' + SUtils.Date2String() + ".tmp";
			File.Copy(resultFileName, file);
		}
		return new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read); 
	}
	
	public override string ToString(){
		StringBuilder sb = new StringBuilder();
		sb.Append(t.ToString()).Append(" ").Append(status.ToString());
		if(destroyTime != null)
			sb.Append("\n").Append(destroyTime);
		return sb.ToString();
	}
		
	~SpiderTask(){
		// if we call Dispose(false) here all client data
		// will disappear when we turn the server down
		Dispose(true);
	}
	
} //EOC

public delegate void OnEndSpiderWorkThread(SpiderWorkThread thread);

// <summary>
//	the spider task, multithread
// </summary>
class HttpSpider {

	private Hashtable foundUrls = null;
	private Queue links = null;
	private int currentDeepth = 0;
	private int deepth;
	private StreamWriter result;
	private ThreadStart onWorkEnd = null;
	private ManualResetEvent moreThreads = null, moreWork = null;
	private bool stop = false;
	private ArrayList threads;
	private string lastUrl = null;
	private string urlPrefix;
	private TaskInfo t;
		
	public HttpSpider(TaskInfo t, StreamWriter result, ThreadStart onWorkEnd){
		this.t = t;
		this.deepth = t.Deepth;
		this.result = result;
		this.onWorkEnd = onWorkEnd;
		threads = new ArrayList(ServicePolicy.ThreadsPerTask);
		foundUrls = new Hashtable(2048);
		links = new Queue(1024);
		foundUrls.Add(t.Url, string.Empty);
		links.Enqueue(currentDeepth);
		links.Enqueue(t.Url);
		urlPrefix = (new Uri(t.Url)).GetLeftPart(UriPartial.Authority);
		moreWork = new ManualResetEvent(true);
		moreThreads = new ManualResetEvent(true);
	}
		
	public string GetState(){
		StringBuilder sb = new StringBuilder(100);
		sb.Append("CurrentDeepth ");
		sb.Append(currentDeepth);
		sb.Append(" Threads ");
		lock(threads){
			sb.Append(threads.Count);
		}
		if(lastUrl != null){
			sb.Append("\n\tLastUrl\t");
			sb.Append(lastUrl);
		}
		return sb.ToString(); 
	}
	
	public void Start(){
		try {
			int QCount = 0;
			object obj = null;
			while(true){
				//Debug.WriteLine("HttpSpider-Start " + currentDeepth);
				if(GetStop()) break;
				lock(links){
					QCount = links.Count;
				}
				if(QCount > 0){
					obj = links.Dequeue();
					if(obj is System.Int32){
						currentDeepth = (int)obj;
						lock(links){
							obj = links.Dequeue();
						}
					}
					if((deepth > 0) && (currentDeepth >= deepth)) continue;
					else {
						if(CanFollow((string)obj))
							StartSpider(currentDeepth + 1, (string)obj);
					}
				} else {
					if(ThereAreMoreThreads()){
						WaitForMoreWork();
					} else break;
				}
			}
		}finally {
			Dispose();
			if(!GetStop()) onWorkEnd();		
		}
	}
	
	private bool CanFollow(string url){
		if(t.ThisSiteOnly){
			if(!url.StartsWith(urlPrefix)) return false;
		}
		return true;
	}
		
	private void WaitForMoreWork(){
		//Debug.WriteLine("WaitForMoreWork");
		moreWork.Reset();
		moreWork.WaitOne();
	}
	
	private void WaitForFreeThread(){
		//Debug.WriteLine("WaitForFreeThread");
		moreThreads.Reset();
		moreThreads.WaitOne();
	}
		
	private void StartSpider(int d, string u){
		//Debug.WriteLine("StartSpider " + u);
		if(!CanAddThread()){
			WaitForFreeThread();
		}
		lastUrl = u;
		SpiderWorkThread thread = new SpiderWorkThread(links, foundUrls,
			d, u, result, new OnEndSpiderWorkThread(EndSpider));
		if(GetStop()) return;
		threads.Add(thread);
		thread.Start();
	}
			
	[MethodImpl (MethodImplOptions.Synchronized)]
	private void EndSpider(SpiderWorkThread thread){
		if(GetStop()) return;
		moreWork.Set();
		lock(threads){
			threads.Remove(thread);
			thread = null;
			if(CanAddThread()) moreThreads.Set();
		}
	}

	private bool ThereAreMoreThreads(){
		bool b = false;
		lock(threads.SyncRoot){
			if(threads.Count > 0) b = true;
		}
		return b;
	}
	
	private bool CanAddThread() {
		bool b = false;
		lock(threads.SyncRoot){
			if(threads.Count < ServicePolicy.ThreadsPerTask) b = true;
		}
		return b;
	}

	public void Stop(){
		SetStop(true);
		lock(threads){
			for(int i = 0; i < threads.Count; i++){
				((SpiderWorkThread)threads[i]).Stop();
				threads[i] = null;
			}
		}
		Dispose();
	}
	
	[MethodImpl (MethodImplOptions.Synchronized)]
	private void SetStop(bool on){
		stop = on;
	}
	
	[MethodImpl (MethodImplOptions.Synchronized)]
	private bool GetStop(){
		return stop;
	}
	
	private void Dispose(){
		if(moreThreads != null)	moreThreads.Close();
		if(moreWork != null)	moreWork.Close();
		moreThreads = null;
		moreWork = null;
	}

} //EOC

// <summary>
//	a spider working thread, scans a page olny
//	rudimentary spider, but focus is on remoting not is spiders
// </summary>
public class SpiderWorkThread {

	private Queue links;
	private Hashtable foundUrls;
	private string url;
	private int deepth;
	private StreamWriter result;
	private Thread spiderThread = null;
	private OnEndSpiderWorkThread onSpiderEnd;
	private Uri baseUrl;
	
	public SpiderWorkThread(Queue links, Hashtable foundUrls, int deepth,
			string url, StreamWriter result,
			OnEndSpiderWorkThread onSpiderEnd){
		this.links = links;
		this.deepth = deepth;
		this.url = url;
		baseUrl = new Uri(url);
		this.foundUrls = foundUrls;
		this.result = result;
		this.onSpiderEnd = onSpiderEnd;
	}

	public void Start(){
		spiderThread = new Thread(new ThreadStart(ProcessUrl));
		spiderThread.Start();
	}

	public void Stop(){
		if(spiderThread != null){
			spiderThread.Abort();
			spiderThread = null;
		}
	}

	private void ProcessUrl(){
		//Debug.WriteLine("ProcessUrl " + url);
		StreamReader reader = null;
		try{
			string nocase = url.ToLower();
			if(!nocase.StartsWith("http://")){
				throw new Exception("Non http url.");
			}
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.UserAgent = "vweb-demo-net-spider";
			request.AllowAutoRedirect = true;
			request.MaximumAutomaticRedirections = 4;
			request.Timeout = 13000; // 13 sec.
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			string cType = response.ContentType.Trim();
			if(!cType.StartsWith("text"))
				throw new Exception("Cannot follow not text/html links.");
			reader = new StreamReader(response.GetResponseStream());
			string content = reader.ReadToEnd();
			string newUrl = null;
			ArrayList newLinks = new ArrayList(100);
			Regex hrefRegex = new Regex("href\\s*=\\s*\"([^\"]*)\"", RegexOptions.IgnoreCase);
			MatchCollection matches = hrefRegex.Matches(content);
			foreach(Match match in matches){
				newUrl = match.Groups[1].ToString().Trim();
				newUrl = new Uri(baseUrl, newUrl).ToString(); // normalize
				lock(foundUrls.SyncRoot){
					if(!foundUrls.ContainsKey(newUrl)){
						nocase = newUrl.ToLower();
						if(nocase.StartsWith("http://"))
							foundUrls.Add(newUrl, string.Empty);
					}
					newLinks.Add(newUrl);
				}
			}
			if(newLinks.Count > 0){
				bool levelUnset = true;
				lock(links){
					for(int i = 0; i < newLinks.Count; i++){
						newUrl = (String)newLinks[i];
						nocase = newUrl.ToLower();
						if(nocase.StartsWith("http://")){
							if(levelUnset){
								levelUnset = false;
								links.Enqueue(deepth);
							}
							links.Enqueue(newLinks[i]);
						}
					}
				}
				LogLinks(newLinks);
			}
		} catch(ThreadAbortException){
			if(reader != null) reader.Close();
			return;
		} catch(Exception e){
			if(result == null) return;
			lock(result){
				try{
					result.WriteLine("Failed: (D" + deepth + ") " + url + " [" + e.Message + "]");
					result.Flush();
				} catch(Exception){
					return;
				}
			}
		} finally {
			if(reader != null) reader.Close();
			onSpiderEnd(this);
		}
	}

	private void LogLinks(ArrayList newLinks){
		StringBuilder sb = new StringBuilder(newLinks.Count * 50);
		for(int i = 0; i < newLinks.Count; i++){
			sb.Append((string)newLinks[i]).Append("\r\n");
		}
		string res = sb.ToString();
		if(result == null) return;
		lock(result){
			try {
				result.WriteLine("<!-- level: " + deepth + " page: " + url + " -->");
				result.Write(res);
				result.Flush();
			} catch(Exception) {};
		}
	}
	
} //EOC

// <summary>
//	static utility methods for files
// </summary>
public class SUtils {

	public static string Date2String(){
		return DateTime.Now.ToString("yyyyMMddhhmmss");
	}
	
	public static string Url2FileName(string url){
		string prefix = Date2String();
		string s = url;
		if(url.Length > 100) s = url.Substring(0, 100);
		StringBuilder sb = new StringBuilder(s.Length + prefix.Length + 5);
		sb.Append('s').Append(prefix).Append('_').Append(s.Replace("://", "_")).Append(".txt");
		sb.Replace('\\', '_');
		sb.Replace('/', '_');
		sb.Replace('*', '_');
		sb.Replace('?', '_');
		sb.Replace('\"', '_');
		sb.Replace('<', '_');
		sb.Replace('>', '_');
		sb.Replace('|', '_');
		sb.Replace(':', '_');
		sb.Replace(' ', '_');
		return sb.ToString();
	}
	
	public static string[] GetDir(long id, string name){
			string[] dirs = new string[2];
			StringBuilder sb = new StringBuilder();
			sb.Append(ServicePolicy.TempDir).Append('\\').Append(id);
			dirs[0] = sb.ToString(); // client
			sb.Append('\\').Append(name);
			dirs[1] = sb.ToString(); // task
			return dirs;
	}
	
	public static string CreateDirs(long id, string name){
		string[] dirs = GetDir(id, name);
		Directory.CreateDirectory(dirs[1]);
		return dirs[1];
	}
		
	public static void CleanFiles(long id, string name){
		//Console.WriteLine("CleanFiles {0} {1}", id, name);
		string[] dirs = GetDir(id, name);
		string[] files = Directory.GetFiles(dirs[1]);
		if((files != null) || (files.Length > 0)){
			for(int i = 0; i < files.Length; i++)
				File.Delete(files[i]);
		}
		Directory.Delete(dirs[1]);
		string[] subdirs = Directory.GetDirectories(dirs[0]);
		if((subdirs == null) || (subdirs.Length == 0))
			Directory.Delete(dirs[0]);
	}

} //EOC