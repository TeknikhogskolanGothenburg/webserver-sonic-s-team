# Session hijacking


Session hijacking is the practice of copying or "stealing" a session id from
another user to try to impersonate them in order to steal their data.
This can be done using the session id contained in a cookie or by using the session id in an URL.

A cookie is used to authenticate a user to a server and if a malicious third party gets a hold of it
they can do anything the user would be able to do once authenticated.

There are many forms of session hijacking which differ in methods and difficulty.
The most frequent types of session hijacking are:

Session-sniffing, where the attacker uses a sniffer(a sort of packet analyzer) to get the session id
and then uses the id to access the server.

Cross-site script attack, where the attacker has malicious code running on client side.
The attacker could send a link that contains malicious code which sends the cookie to the attacker.

Man-in-the middle attacks makes the attacker act as a proxy to the server. 
The user thinks its communicating with the server but the attacker gets all the data.

man-in-the browser works by using a pre-installed trojan horse to intercept data between the browser
and the applications security mechanisms. 
The user sees all the transactions as normal and as such it's hard to realize the attack is happening.

Some solutions to prevent session hijacking include:

Encrypting the data traffic. This removes the possibility of sniff-style attacks.

Using a long random number or string for the session id. This makes it harder for attackers to aquire the session id by brute force.

Regenerating the session id after the user has logged in. The attacker can no longer find the same user after login.


References: 
https://en.wikipedia.org/wiki/Session_hijacking
https://www.owasp.org/index.php/Session_hijacking_attack


 


