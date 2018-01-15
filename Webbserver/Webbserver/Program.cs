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

            Dictionary<string, int> sessionCounters = new Dictionary<string, int>();
            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request. 
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                Console.WriteLine("request to " + request.RawUrl);
                Cookie cookie = new Cookie
                {
                    Name = "counter",
                    Path = "",
                    Expires = DateTime.MinValue
                };

                //If cookie doesn't exist or can't be found, generate new sessionID.
                if (request.Cookies["counter"] == null || !sessionCounters.ContainsKey(request.Cookies["counter"].Value))
                {
                    Console.WriteLine("there was no cookie");
                    bool loopCondition = true;
                    while (loopCondition) {
                        cookie.Value = new Random().Next().ToString();
                        if (!sessionCounters.ContainsKey(cookie.Value))
                        {
                            sessionCounters.Add(cookie.Value, 1);
                            loopCondition = false;
                        }                       
                    }
                }
                else
                {
                    Console.WriteLine("the cookie was " + request.Cookies["counter"].Value);
                    cookie.Value = request.Cookies["counter"].Value;
                    sessionCounters[cookie.Value] += 1;
                }
                Console.WriteLine("The counter was: " + sessionCounters[cookie.Value]);
                response.SetCookie(cookie);

                // Construct a response.
                string requested = request.RawUrl;
                requested = FindPath(requested);                

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
                        if (!requested.Contains("/dynamic"))
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
        public static string FindPath(string requested) // Sets empty rawURL and Subfolder to correct path 
        {
            string path = "";
            if (requested == "/")
            {
                path = "/index.html";
            }
            else if (requested == "/Subfolder/")
            {
                //response.ContentType = "text/html";
                path = @"\Subfolder\index.html";
            }
            else
            {
                path = requested;
            }
            return path;
        }

        //Method that sets the expire date for the response to a date 1 year from the date that the
        //site was firstly accessed.
        public static void SetExpireDate(HttpListenerResponse response)
        {
            DateTime now = DateTime.Now;
            DateTime future = now.AddYears(1);
            string expires = future.ToString("o");
            response.AddHeader("Expires", expires);
        }

        //This method sets the correct value for the header property "Content-Type" depending on what type of
        //document we are requesting from the webserver.
        public static void SetResponseContentType(string requestedFile, HttpListenerResponse response, HttpListenerRequest request)
        {
            SetExpireDate(response);
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
                Console.WriteLine("Response was content type application/javascript");
                response.AddHeader("Content-Type", "application/javascript");
            }
            else if (requestedFile.EndsWith(".css"))
            {
                Console.WriteLine("Response was content type css");
                response.AddHeader("Content-Type", "text/css");
            }            
            else if (requestedFile.Contains("/dynamic?"))
            {                
                DynamicQuery(request, response);                
            }
            else if (requestedFile.EndsWith("/dynamic"))
            {
                string dynamicPage = "<html><body>No Parameters were specified<br> Nothing to Calculate</body></html>";
                byte[] buffer = Encoding.UTF8.GetBytes(dynamicPage);
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }           
        }
        public static void DynamicQuery(HttpListenerRequest request, HttpListenerResponse response)
        {
            if ((request.QueryString["input1"] != null) && (request.QueryString["input2"] != null))
            {
                response.AddHeader("Content-Type", "text/html");
                string[] splitQuery = new string[2];
                splitQuery[0] = request.QueryString["input1"];
                splitQuery[1] = request.QueryString["input2"];
                int input1 = int.Parse(splitQuery[0]);
                int input2 = int.Parse(splitQuery[1]);
                var accept = request.Headers["Accept"];
                int result = input1 + input2;
                if (accept == null || accept.StartsWith("text/html"))
                {

                    string dynamicPage = "<html><body>" + result.ToString() + "</body></html>";
                    byte[] buffer = Encoding.UTF8.GetBytes(dynamicPage);

                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);

                    output.Close();
                }
                else if (accept.StartsWith("application/xml"))
                {
                    response.AddHeader("Content-Type", "application/xml");
                    string dynamicPage = "<result><value>" + result.ToString() + "</value></result>";
                    byte[] buffer = Encoding.UTF8.GetBytes(dynamicPage);

                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);

                    output.Close();
                }

            }
            else if ((request.QueryString["input1"] != null) && (request.QueryString["input2"] == null))
            {
                string[] splitQuery = new string[2];
                splitQuery[0] = request.QueryString["input1"];               
                int input1 = int.Parse(splitQuery[0]);                
                int result = input1;
               // response.StatusCode = ;
                string dynamicPage = "<html><body>" + result.ToString() + "</body></html>";
                byte[] buffer = Encoding.UTF8.GetBytes(dynamicPage);
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
        }
    }    
}
