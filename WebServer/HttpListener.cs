using System;
using System.Net;
using System.Threading;
using System.Text;

namespace WebServer
{
	public class Http
	{
		private readonly HttpListener Listener = new HttpListener();
		private readonly Func<HttpListenerRequest, String> Response;

		private static Http ws;
		internal static void startServer (String PortNumber) {
		
			if (PortNumber == null) {
				PortNumber = "8080";
			}

			String PORT = PortNumber;

			if (Http.res != null) {
				Http.ws = new Http (Http.res, "http://*:" + PORT + "/");
			} else {
				Http.ws = new Http (Http.defaultSettings, "http://*:" + PORT + "/");
			}

			Http.ws.Run();
			Console.WriteLine("Webserver Started. \n Press a key to quit.");
			Console.ReadKey();
			Http.ws.Stop();
		}

		private static String defaultSettings(HttpListenerRequest request) {
			Console.WriteLine ("Loading from Local? : " + request.IsLocal);
			return string.Format ("<HTML><BODY>My web page.<br>{0}</BODY></HTML>", DateTime.Now);    
		}

		public Http (string[] prefixes, Func<HttpListenerRequest, string> method)
		{
			if (!HttpListener.IsSupported)
				throw new NotSupportedException(
					"Needs Windows XP SP2, Server 2003 or later.");

			// URI prefixes are required, for example 
			// "http://localhost:/index/".
			if (prefixes == null || prefixes.Length == 0)
				throw new ArgumentException("prefixes");

			// A responder method is required
			if (method == null)
				throw new ArgumentException("method");

			foreach (string s in prefixes)
				Listener.Prefixes.Add(s);

			Response = method;
			Listener.Start();
		}

		public Http(Func<HttpListenerRequest, string> method, params string[] prefixes)
			: this(prefixes, method) { }

		public void Run()
		{
			ThreadPool.QueueUserWorkItem((o) =>
				{
					Console.WriteLine("Webserver running...");
					try
					{
						while (Listener.IsListening)
						{

							ThreadPool.QueueUserWorkItem((c) =>
								{
									var context = c as HttpListenerContext;
									try
									{
										string rstr = Response(context.Request);
										byte[] buf = Encoding.UTF8.GetBytes(rstr);
										context.Response.ContentLength64 = buf.Length;
										context.Response.OutputStream.Write(buf, 0, buf.Length);
									}
									catch { } // suppress any exceptions
									finally
									{
										Console.WriteLine(context.Response.OutputStream.ToString() );
										// always close the stream
										context.Response.OutputStream.Close();
									}
								}, Listener.GetContext());
						}
					}
					catch { } // suppress any exceptions
				});
		}

		public void Stop()
		{
			Listener.Stop();
			Listener.Close();
		}
	}
}

