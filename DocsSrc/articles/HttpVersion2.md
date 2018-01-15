# HTTP version 2

The main purpose of HTTP/2 is to optimize the communication between client and server.

Unlike HTTP/1’s  plaintext protocol, HTTP/2 uses a binary protocol. Binary protocols are compact, more efficiently parsed and reduces errors.

HTTP/2 uses Multiplexing. Multiplexing enables for multiple requests and responses to be sent in one single TCP connection. Previously this has been done by opening several TCP connections between the client and server. Multiplexing increases performance.

A new key feature in HTTP/2 is header compression. In HTTP/1.X headers are sent without any form of compression. HTTP/2 uses a new compressor called HPACK to reduce transfer size and only send header fields that differentiates from previously sent requests and responses.

With HTTP/2 comes server push. Server push enables the server to push unrequested resources to the client. By escaping the request and response pattern the server can send resources that it knows the client will need for the page it is requesting.
