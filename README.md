mldeploy
========

This is a db deploy clone for the Marklogic xml database written written in C#.
It uses the XCC .NET connector provided by Marklogic to make a connection to the database.

Usage
=====
It runs as an exe. You can invoke it from powershell/nant or any build tool.
It takes 2 arguments 
1. The xcc connection string of the form xcc://username:password@host:port
2. The path where the deltas are stored 

The delta files should be xquery files of the format number.xqy (where number is any positive integer).
eg. 1.xqy, 100.xqy
	

Files named like foo.xqy in the deltas path will crash the exe (as of version 1)
