using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Webbserver
{
    class Program
    {
        
        static string requested = "";
        
        static void Main(string[] prefixes)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            // URI prefixes are required,
            // for example "http://contoso.com:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // Create a listener.
            HttpListener listener = new HttpListener();

            // Add the prefixes.
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
            listener.Start();
            Console.WriteLine("Listening...");
            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request. 
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                Console.WriteLine("request to " + request.RawUrl);
                Cookie cookie = new Cookie
                {
                    Name = "SessionID",
                    Path = "",
                    Expires = DateTime.MinValue
                };

                Dictionary<string, int> sessionCounters = new Dictionary<string, int>();

                //If cookie doesn't exist or can't be found, generate new sessionID.
                if (request.Cookies["SessionID"] == null || !sessionCounters.ContainsKey(request.Cookies["SessionID"].Value))
                {
                    Console.WriteLine("there was no cookie");
                    cookie.Value = new Random().Next().ToString();
                    sessionCounters.Add(cookie.Value, 1);
                }
                else
                {
                    Console.WriteLine("the cookie was " + request.Cookies["SessionID"].Value);
                    cookie.Value = request.Cookies["SessionID"].Value;
                    sessionCounters[cookie.Value] += 1;
                }
                Console.WriteLine("The counter was: " + sessionCounters[cookie.Value]);
                response.SetCookie(cookie);

                // Construct a response.
                requested = request.RawUrl;

                if (requested == "/")
                {
                    requested = "/index.html";
                }

                if (requested == "/counter")
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(sessionCounters[cookie.Value].ToString());
                    // Get a response stream and write the response to it.
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                }

                else
                {
                    SetResponseContentType(requested, response, request);
                    if (File.Exists(@"..\..\..\..\Content" + requested))
                    {
                        byte[] buffer = File.ReadAllBytes(@"..\..\..\..\Content" + requested);
                        // Get a response stream and write the response to it.
                        response.ContentLength64 = buffer.Length;
                        System.IO.Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);

                        output.Close();
                    }
                    else
                    {
                        if (!requested.Contains("/Dynamic"))
                        {
                            response.StatusCode = 404; // finns inte
                            byte[] buffer = Encoding.UTF8.GetBytes("File not found!");
                            // Get a response stream and write the response to it.
                            response.ContentLength64 = buffer.Length;
                            System.IO.Stream output = response.OutputStream;
                            output.Write(buffer, 0, buffer.Length);

                            output.Close();
                        }
                    }
                }               
                //  listener.Stop();
            }
        }
        public static void SetResponseContentType(string requestedFile, HttpListenerResponse response, HttpListenerRequest request)
        {
            DateTime now = DateTime.Now;
            DateTime future = now.AddYears(1);
            string expires = future.ToString();
            response.AddHeader("Expires", expires);

            if (requestedFile.EndsWith(".html"))
            {
                Console.WriteLine("Response was content type text/html");
                response.AddHeader("Content-Type", "text/html");
            }
            else if (requestedFile.EndsWith(".htm"))
            {
                Console.WriteLine("Response was content type text/html");
                response.AddHeader("Content-Type", "text/html");
            }
            else if (requestedFile.EndsWith(".jpg"))
            {
                Console.WriteLine("Response was content type image/jpeg");
                response.AddHeader("Content-Type", "image/jpeg");
            }
            else if (requestedFile.EndsWith(".gif"))
            {
                Console.WriteLine("Response was content type image/gif");
                response.AddHeader("Content-Type", "image/gif");
            }
            else if (requestedFile.EndsWith(".pdf"))
            {
                Console.WriteLine("Response was content type application/pdf");
                response.AddHeader("Content-Type", "application/pdf");
            }
            else if (requestedFile.EndsWith(".js"))
            {
                Console.WriteLine("Response was content type application/x-javascript");
                response.AddHeader("Content-Type", "application/x-javascript");
            }
            else if (requestedFile.EndsWith(".css"))
            {
                Console.WriteLine("Response was content type css");
                response.AddHeader("Content-Type", "text/css");
            }
            else if (requestedFile == "/Subfolder/")
            {
                response.ContentType = "text/html";
                requested = @"\Subfolder\index.html";
            }
            else if (requestedFile.Contains("/Dynamic"))
            {
                if ((request.QueryString["input1"] != null) && (request.QueryString["input1"] != null))
                {
                    string[] splitQuery = new string[2];
                    splitQuery[0] = request.QueryString["input1"];
                    splitQuery[1] = request.QueryString["input2"];
                    int input1 = int.Parse(splitQuery[0]);
                    int input2 = int.Parse(splitQuery[1]);

                    int result = input1 + input2;

                    string dynamicPage = "<HTML><BODY>" + result.ToString() + "</BODY></HTML>";

                    byte[] buffer = Encoding.UTF8.GetBytes(dynamicPage);

                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);

                    output.Close();
                }
            }
        }
    }
}
