// .Net Remote Spider Demo
// (c) - Copyright 2003 by Vasian CEPA
// http://madebits.com

using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization;

// <summary>
//	client callback when tasks ends
// </summary>
public delegate void OnTaskEnd(string name);

// <summary>
//	the server interface
// </summary>
public interface ISpider {

	//event OnTaskEnd onTaskEnd;
	long GetClientId();
	void AddTask(long id, TaskInfo t, OnTaskEnd onTaskEndSink);
	void RemoveTask(long id, string name);
	string[] ListTasks(long id);
	string[] GetTaskState(long id, string name);
	Stream GetTaskResult(long id, string name);
	void ResetTaskEnd(long id, string name, OnTaskEnd onTaskEndSink);

} // EOC

// <summary>
//	helper class to marshall the client callback
// </summary>
public class TaskEndSink : MarshalByRefObject  {

	// very important not to be sent at server
	[NonSerialized()] private OnTaskEnd pointer = null;
	
	public void SetOnTaskEnd(OnTaskEnd p){
		pointer = p;
	}
	
	public void OnTaskEndEvent(string name){
		if(pointer != null) pointer(name);
	}
} //EOC

// <summary>
//	task info
// </summary>
[Serializable()]
public class TaskInfo : IComparable {
	private string name;
	private string url;
	private int deepth;
	private bool thisSiteOnly = true; // follow links inside this site only

	public string Name {
		get { return name; }
		set {
			if(value == null) throw new ArgumentNullException();
			name = value.Trim();
		}
	}
	
	public string Url {
			get { return url; }
			set {
				if(value == null) throw new ArgumentNullException();
				url = value.Trim();
			}
	}
	
	public int Deepth {
			get { return deepth; }
			set {
				if(value < 0) deepth = 0;
				else deepth = value;
			}
	}
	
	public bool ThisSiteOnly {
				get { return thisSiteOnly; }
				set {
					thisSiteOnly = value;
				}
	}
	
	public int CompareTo(object o) {
		if(!(o is TaskInfo)) throw new InvalidCastException();
		return string.CompareOrdinal(name, ((TaskInfo)o).Name);
	}
			
	public override string ToString(){
		StringBuilder sb = new StringBuilder();
		sb.Append(name).Append(" ").Append(deepth).Append(" ").Append(url);
		return sb.ToString();
	}
		
} // EOC

// <summary>
//	a custom exception
// </summary>
[Serializable()]
public class SpiderException : ApplicationException {

	public SpiderException() : base() {}
	public SpiderException(string msg) : base(msg) {}
	//Deserialization constructor.
	public SpiderException (SerializationInfo info, StreamingContext context) : base(info, context) {}

}