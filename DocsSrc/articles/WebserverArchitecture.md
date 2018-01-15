# Webserver Architecture

* How are your webserver build?
* What resources can the user access?
* How does the server act in case of an error?

Our webserver is built as a simple http server using built in classes in C# like 'httpListener'. 
We have tried to divide our code into method that each has quite well-defined responsibilities,
for example the method 'SetResponseContentType' has the responsibility to set the correct
content type in the response header for each type of document that the user request to get from
the webserver. One improvement of this method would be to extract the last part of it where we 
actually do not handle the content type but instead the case when the user types just 
"/dynamics" without query parameters. To follow the principle of as far as possible constructing
methods with well-defined responsibilities, this part should be moved to a separate method.
Other future improvements of the webserver could be to divide different responsibilities
in the code into separate classes, for example it would be easier to maintain if one had separate
classes defining methods for response and request. On the other hand one should not make the programs
structure more complex than necessary.
One important part of the webserver is the construction of header data that we send with the response.
We have constructed methods like the one described above where Content-Type for the resource
is set depending on the resource that is requested, also we have constructed a method that sets the expire
date parameter for the response header. 
Another thing worth mentioning when describing how the webserver is built is the use of a cookie called counter.
To keep track of how many resource requests each session (client) makes to the webserver, a cookie object is created. 
A C# Dictionary is then used to map individual sessionIDs (cookies) to the amount of requests, the value for the counter 
goes up with one for each request made with the same sessionID. For security reasons the only thing sent to the client
is the sessionID itself and a representation of the data for counter: a text representation of how many requests that
has been made.

The resources that the user can access from the web server is html/text documents, a pdf-file, image, gif-file, 
a subfolder (not directly accessing a file but accessing things through a subfolder instead), dynamic pages (where the content is generated when the request is processed and made into a response) and representation of cookie data through the page /counter.

To handle errors we have coded the following:
For example if a user tries to access a page that doesn't exist on the server we have constructed an if-else
statement to handle this, if the user for example types "localhost:8080/hej" the user will be re-directed to
a page with status code 404- page not found. In the method "DynamicQuery" we also handle the case if a user only types in
one input, i.e. "input1" and doesn't give any value for "input2", this is also generates an error: "Missing input value" and
sets status code 500 in the response.
Before declaring a HTTPListener object in the code we also check whether the parameter "prefixes" contains an URI or not, if it doesn't an ArgumentException is thrown.

