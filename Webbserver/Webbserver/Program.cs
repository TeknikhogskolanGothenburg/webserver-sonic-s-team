using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Webbserver
{
    class Program
    {
        static void Main(string[] prefixes)
        {
           while (true)
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
                // Note: The GetContext method blocks while waiting for a request. 
               
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                context.Response.ContentType = "text/html";

                response.AddHeader("Content-Type" , "text/html");
                
                // Construct a response.
                string reqed = request.RawUrl;
                byte[] buffer = File.ReadAllBytes(@"..\..\..\..\Content" + reqed);
                
               // string responseString = test;                
               // byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);



               


                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
               // response.ContentLength64 = image.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                
                // You must close the output stream.

               
                output.Close();
                listener.Stop();
            }
        }
    }
}
