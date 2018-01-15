# Webserver Architecture

* How are your webserver build?
* What resources can the user access?
* How does the server act in case of an error?

Our webserver is built as a simple http server using built in classes in C# like 'httpListener'. 
We have tried to divide our code into method that each has quite well-defined responsibilities,
for example the method 'SetResponseContentType' are only responsible for setting the correct
content type in the response header for each type of document that the user request to get from
the webserver. Future improvements of the webserver could be to divide different responsibilities
in the code into separate classes, for example it would be easier to maintain if one had separate
classes defining methods for response and request. 
The user can access html/text documents, a pdf-file, image, gif-file, a subfolder (not directly accessing
a file but accessing things through a subfolder instead), dynamic pages (where the content is generated
when the request is processed and made into a response). 
For example if a user tries to access a page that doesn't exist on the server we have constructed an if-else
statement to handle this, if the user for example types "localhost:8080/hej" the user will be re-directed to
a page with status code 404- page not found. 
