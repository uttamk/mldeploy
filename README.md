mldeploy
========

This is a db deploy clone for the Marklogic xml database written written in C#.
It uses the XCC .NET connector provided by Marklogic to make a connection to the database.

Usage
==============================================================================
It runs as an exe. You can invoke it from powershell/nant or any build tool.

To build it, just compile the compile the solution in visual studio 2010.

Running Console.mldeploy.exe gives you the following

 deploy      - deploys delta xquery scripts from the specified deltas directo

 script      - Generates xquery scripts from the specified deltas directory
 
 ===============================================================================
 Running Console.mldeploy.exe deploy gives you
 
 Expected usage: Console.MLDeploy.exe deploy <options>
 
 <options> available:
 
      --connstring, -c[=VALUE]
                             xcc connection string to the Marklogic XDBC Server : For example xcc://username:password@127.0.0.1:8080
							 
      --deltaspath, -d[=VALUE]
                             directory path where the deltas are stored

 

The delta files should be xquery files of the format number[description].xqy (where number is any positive integer).
eg. 1 inserting foo.xqy, 100.xqy. Files named like foo.xqy (without a leading number) will throw an exception.
=====================================================================================================
Running Console.mldeploy.exe script gives you

 
Expected usage: Console.MLDeploy.exe script <options>

<options> available:

      --deltaspath, -d[=VALUE]
                             The path where the deltas reside
							 
      --outputpath, -o[=VALUE]
                             Output path for the script
							 
      --startdelta, -s[=VALUE]
                             The delta to start the script
							 				 
      --enddelta, -e[=VALUE] The last delta in the script
	  
	  
      --rollback, -r         If the script type is rollback
	  
	  
The rollback scripts should be embedded as xquery comments in the delta files themselves like the example below
	 
(:Xquery comment followed by delta:)
xdmp:document-insert("/foo.xml", <foo>bar</foo>);

(:Rollback
	xdmp:document-delete("/foo.xml");
:)	 


The semi colons at the end are very important as they denote a transaction in Marklogic.

==========================================================================================================