using System;
using WebServer;
using System.Net;

namespace Zulu
{
	class MainClass
	{
		static void Main(string[] args)
		{
			WebServer.Http.startServer ("8080");
		}

		public static string SendResponse(HttpListenerRequest request)
		{
			Console.WriteLine ("Loading from Local? : " + request.IsLocal);
			return string.Format("<HTML><BODY>My web page.<br>{0}</BODY></HTML>", DateTime.Now);    
		}
	}
}
