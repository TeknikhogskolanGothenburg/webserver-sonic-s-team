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
                
                byte[] buffer = new byte[50000];

                // Respond with correct Content-Type
                if (requested.EndsWith(".html"))
                {
                  //  response.ContentType = "text/html";
                    Console.WriteLine("Response was content type text/html");
                    string responseString = File.ReadAllText(@"..\..\..\..\Content" + requested);
                    buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    response.AddHeader("Content-Type", "text/html");

                }
                else if (requested.EndsWith(".htm"))
                {
                    //  response.ContentType = "text/html";
                    Console.WriteLine("Response was content type text/html");
                    string responseString = File.ReadAllText(@"..\..\..\..\Content" + requested);
                    buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    response.AddHeader("Content-Type", "text/html");
                }
                else if (requested.EndsWith(".jpg"))
                {
                  //  context.Response.ContentType = "image/jpeg";
                    Console.WriteLine("Response was content type image/jpeg");
                    buffer = File.ReadAllBytes(@"..\..\..\..\Content" + requested);
                    response.AddHeader("Content-Type", "image/jpeg");
                }
                else if (requested.EndsWith(".gif"))
                {
                  //  context.Response.ContentType = "image/gif";
                    Console.WriteLine("Response was content type image/gif");
                    buffer = File.ReadAllBytes(@"..\..\..\..\Content" + requested);
                    response.AddHeader("Content-Type", "image/gif");
                }
                else if (requested.EndsWith(".pdf"))
                {
                  //  context.Response.ContentType = "application/pdf";
                    Console.WriteLine("Response was content type application/pdf");
                    buffer = File.ReadAllBytes(@"..\..\..\..\Content" + requested);
                    response.AddHeader("Content-Type", "application/pdf");
                }
                else if (requested.EndsWith(".js"))
                {
                   // context.Response.ContentType = "application/x-javascript";
                    Console.WriteLine("Response was content type application/x-javascript");
                    buffer = File.ReadAllBytes(@"..\..\..\..\Content" + requested);
                    response.AddHeader("Content-Type", "application/x-javascript");
                }

                else if (requested.EndsWith(".css"))
                {
                   // context.Response.ContentType = "text/css";
                    Console.WriteLine("Response was content type css");
                    buffer = File.ReadAllBytes(@"..\..\..\..\Content" + requested);
                    response.AddHeader("Content-Type", "text/css");
                }



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
