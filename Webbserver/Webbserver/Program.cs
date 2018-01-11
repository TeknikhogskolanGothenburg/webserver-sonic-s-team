using System;
using System.Collections.Generic;
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
        public static void ContentType (string requestedFile, HttpListenerResponse response)
        {
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
            
            /*else if(requestedFile.EndsWith(@"/SubFolder/index.html"))
            {

            }*/
        }
        static void Main(string[] prefixes)
        {
            int counter = 1;
            
                // Create Cookie
                Cookie cookie = new Cookie
                {
                    Name = "Counter",
                    Value = counter.ToString(),
                };

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

                // Get current time
                DateTime time = DateTime.Now;                               
                
                response.SetCookie(cookie);                
                
                // Construct a response.
                string requested = request.RawUrl;
                
                if (requested == "/")
                {
                    requested = "/index.html";
                }
                

                byte[] buffer = File.ReadAllBytes(@"..\..\..\..\Content" + requested);
                // Respond with correct Content-Type


                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);

                // You must close the output stream.
                output.Close();
              //  listener.Stop();

                counter++;
            }
        }
    }
}
