mldeploy
========

This is a db deploy clone for the Marklogic xml database written written in C#. It also lets you run arbitrary xquery files from the command line like sqlcmd.
It uses the XCC .NET connector provided by Marklogic to make a connection to the database.
An XDBC server on https will not work because XCC for .net doesn't support https yet. 
Other types of servers like vanilla HTTP and webdav are not supported.

Usage
==============================================================================
It runs as an exe. You can invoke it from the command line or powershell/nant or any build tool.

To build it, just compile the solution in visual studio 2010.


Running bin/console.mldeploy.exe gives you the following

    runscript   - runs an xquery script from a file
   
    deploy      - deploys delta xquery scripts from the specified deltas directory
	
    script      - Generates xquery scripts from the specified deltas directory
	
====================================================================================	
 Running bin/Console.MLDeploy.exe runscript gives you
 
 Expected usage: Console.MLDeploy.exe runscript <options>
 
<options> available:

      --connstring, -c[=VALUE]
                             xcc connection string to the Marklogic XDBC server : For example xcc://username:password@127.0.0.1:8080
							 
      --scriptpath, -s[=VALUE]
                             path to the xquery script
 
 ===============================================================================
 Running bin/console.mldeploy.exe deploy gives you
 
 Expected usage: Console.MLDeploy.exe deploy <options>
 
 <options> available:
 
      --connstring, -c[=VALUE]
                             xcc connection string to the Marklogic XDBC Server : For example xcc://username:password@127.0.0.1:8080
							 
      --deltaspath, -d[=VALUE]
                             directory path where the deltas are stored

 

The delta files should be xquery files of the format number[description].xqy (where number is any positive integer).
eg. 1 inserting foo.xqy, 100.xqy. Files named like foo.xqy (without a leading number) will throw an exception.
=====================================================================================================
Running bin/console.mldeploy.exe script gives you

 
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