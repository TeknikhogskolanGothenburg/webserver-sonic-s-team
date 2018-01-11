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
        static string requested = "";
        static Dictionary<string, int> CookieCollection = new Dictionary<string, int>();

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
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                //Cookie handler
                var test = context.Request.Cookies["Counter"];
                int cookieValue = CookieCollection.Count + 1;
                if (request.Cookies["Counter"] == null)
                {
                    Cookie cookie = new Cookie { Name = "Counter", Value = cookieValue.ToString() };
                    CookieCollection.Add(cookie.Value, 1);
                    response.AppendCookie(cookie);
                }
                else if (CookieCollection.ContainsKey(request.Cookies["Counter"].Value.ToString()))
                {
                    int currentValue = 0;
                    CookieCollection.TryGetValue(request.Cookies.Count.ToString(), out currentValue);
                    CookieCollection[request.Cookies.Count.ToString()] = currentValue + 1;
                }

                // Construct a response.
                requested = request.RawUrl;
                if (requested == "/")
                {
                    requested = "/index.html";
                }

                ContentType(requested, response);

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
                    response.StatusCode = 404; // finns inte
                    byte[] buffer = Encoding.UTF8.GetBytes("File not found!");
                    // Get a response stream and write the response to it.
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
                //  listener.Stop();
            }
        }
        public static void ContentType(string requestedFile, HttpListenerResponse response)
        {
            DateTime now = DateTime.Now;
            DateTime future = now.AddYears(1);
            string expires = future.ToString();

            //response.AddHeader("Expires", expires);
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
        }
    }
}
