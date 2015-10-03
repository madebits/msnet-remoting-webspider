// .Net Remote Spider Demo
// (c) - Copyright 2003 by Vasian CEPA
// http://madebits.com

using System;
using System.Runtime.Remoting;

namespace vpcepa.spider {

// <summary>
//	server driver class
// </summary>
class SpiderServer {

	static void Main(){
		try{
			RemotingConfiguration.Configure("SpiderServer.exe.config");
			RemotingConfiguration.ApplicationName = "v-web.demo-net.spider";
			Console.WriteLine("PID:  {0}", RemotingConfiguration.ProcessId);
			Console.WriteLine("AID:  {0}", RemotingConfiguration.ApplicationId);
			Console.WriteLine("Name: {0}", RemotingConfiguration.ApplicationName);
			Console.WriteLine("Press Enter to terminate...");
			Console.ReadLine();
		} catch(Exception e){
			Console.WriteLine("!Error: Server cannot be started.");
			Console.WriteLine(e.Message);
		}
	}

} // EOC

} // END namespace vpcepa.spider