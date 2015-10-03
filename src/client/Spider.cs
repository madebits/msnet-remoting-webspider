// .Net Remote Spider Demo
// (c) - Copyright 2003 by Vasian CEPA
// http://madebits.com

using System;
using System.Diagnostics;
using System.Collections;
using System.Runtime.Remoting;

// <summary>
//	a client deriver class
// </summary>
class SpiderClientDriver{

	static void Main(){
		try {
			SpiderClient client = new SpiderClient(Connect());
			client.Work();
		} catch(Exception e){
			Console.WriteLine("Spider client error: {0}", e.Message);
		}
	}

	public static ISpider Connect(){
		//ISpider s = (ISpider)Activator.GetObject(typeof(ISpider), "tcp://127.0.0.1:8123/Spider");
		RemoteConfig.Configure("Spider.exe.config");
		return (ISpider)RemoteConfig.GetObject(typeof(ISpider));
	}

} //EOC

// <summary>
//	I read this way somewhere in the internet
//	but I reimplemented it because I lost the link
// </summary>
class RemoteConfig{

	private static bool notInited = true;
	private static IDictionary types = null;
	private static string cFile;
	
	public static void Configure(string file){
		cFile = file;
	}
	
	public static object GetObject(Type t){
		if(notInited){
			Init();
			notInited = false;
		}
		return Activator.GetObject(t, (string)types[t.ToString()]);
	}
			
	private static void Init(){
		RemotingConfiguration.Configure(cFile);
		WellKnownClientTypeEntry[] te = RemotingConfiguration.GetRegisteredWellKnownClientTypes();
		types = new Hashtable();
		for(int i = 0; i < te.Length; i++)
			types.Add(te[i].TypeName, te[i].ObjectUrl);
	}
	
} //EOC