tru3pl.exe
	-dir [folder]
	{-[action_type]} {-[action_type]...n}	
	{-ftp {in|out}} {-csv}
	{-live}	{-w} {-del}

-[dir] output location

-[action_type] Create 3pl files. 
 File types are sku | po | so | wt or ALL for all.
 -wt #TXDOC creates a po file for the docment.

-ftp {in|out} Sends / Receives files. 
 Both if in/out not specfied.

-CSV For testing. Outputs data in CSV format. 
 Does not send or mark records as sent.

-live Send data to the LIVE system. 
 Defaults to TEST.
 This will also delete downloaded files from ftp.

 -del If not using the live environment
  this option deletes downloaded files from ftp.

-w Wait for keypress after completion.
