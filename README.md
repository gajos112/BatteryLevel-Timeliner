# BatteryPower-Timeliner

BatteryPower Timeliner was designed and created for all DFIR analysts who want to use SRUM database to find out what was the battery level at the time of infection. There are few tools that can extract that information from SRUDB.DAT, but in my opinion analysts do not understand what information are avaialbe in other tables than {973F5D5C-1D90-4944-BE8E-24B94231A174} (a very popular table, that stores information about network connections) and how that information can help during the investigation. 

BatteryPower - Timeliner does two things:

- parses srudb.dat and builds a TLN timeline (where each entry shows the current level of the battery),
- generates a graph showing the use of the battery over a specified period of time.

It is a GUI tool written in C# .Net Framework 4.7.2. In order to access ESE database (the format used by SRUM) I used ManagedEsent version 2.0.3 (older versions do not work properly). Link to it can be found here: https://github.com/microsoft/ManagedEsent. In order to make that executable portable I used Costura.Fody which merges assemblies as embedded resources, therefore you do not have to care about other dependencies.

It was tested on:

- Windows 10.0.16299,
- Windows 10.0.19042.


The tool accesses and parses data from the table called {FEE4E14F-02A9-4550-B5CE-5FA2DA202E37}. More information about the SRUM structure can be found under these links:

- https://deepsec.net/docs/Slides/2019/Beyond_Windows_Forensics_with_Built-in_Microsoft_Tooling_Thomas_Fischer.pdf,
- https://velociraptor.velocidex.com/digging-into-the-system-resource-usage-monitor-srum-afbadb1a375,
- https://github.com/libyal/esedb--kb/blob/main/documentation/System%20Resource%20Usage%20Monitor%20(SRUM).asciidoc.

To give you a quick overview of the database, I listed few useful (for DFIR analysts) tables below.
- {DD6636C4-8929-4683-974E-22C046A43763} - Network Connectivity data
- {D10CA2FE-6FCF-4F6D-848E-B2E99266FA89} - Application Resource usage data
- {973F5D5C-1D90-4944-BE8E-24B94231A174} - Network usage data 
- {D10CA2FE-6FCF-4F6D-848E-B2E99266FA86} - Windows Push Notification data
- {FEE4E14F-02A9-4550-B5CE-5FA2DA202E37} - Energy usage data

# How does it work?
First you have to provide a path to a SRUM db that you want to parse. Then you also have to provide the path where you want to save the output (timeline in TLN format) and click "PARSE". 

If the file you provided is not a valid SRUM db, the tool will throw an error. If everything is okay it will try to:

1. Attach the database:

        wrn = Api.JetAttachDatabase(sesid, pathDB, AttachDatabaseGrbit.None);
        LogTextBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Attaching the database " + pathDB + "\r\n");
        LogTextBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Status: " + wrn + "\r\n");

2. Open the database:

        wrn = Api.OpenDatabase(sesid, pathDB, out dbid, OpenDatabaseGrbit.None);
        LogTextBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Opening the database: " + pathDB + "\r\n");
        LogTextBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Status: " + wrn + "\r\n");

3. Open the table:

        wrn = Api.OpenTable(sesid, dbid, nameTABLE, OpenTableGrbit.None, out tableid);
        LogTextBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Opening table: " + nameTABLE + "\r\n");
        LogTextBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Status: " + wrn + "\r\n");

4. Get information about the columns:

        Api.JetGetColumnInfo(sesid, dbid, nameTABLE, ColumnTimeStamp, out columndefEventTimestamp);
        Api.JetGetColumnInfo(sesid, dbid, nameTABLE, ColumnFullChargedCapacity, out columndefFullChargedCapacity);
        Api.JetGetColumnInfo(sesid, dbid, nameTABLE, ColumnChargeLevel, out columndefChargeLevel);

5. Going further the tool loops through all rows in the table and gets the value from each column:

         Int64 TimeInt = (Int64)Api.RetrieveColumnAsInt64(sesid, tableid, columndefEventTimestamp.columnid);
         DateTime Time = DateTime.FromFileTimeUtc(TimeInt);
         Int32 FullChargedCapacity = (Int32)Api.RetrieveColumnAsInt32(sesid, tableid, columndefFullChargedCapacity.columnid);
         Int32 ChargeLevel = (Int32)Api.RetrieveColumnAsInt32(sesid, tableid, columndefChargeLevel.columnid);

As you could observe above, each value is stored using a different data type. It's quite important as you have to know which method you will choose to extract that data. I found one article that shows data types for all SRUM's tables: 
- http://dfir.pro/index.php?link_id=92259,

6. Calculates a percentage value based on the ChargeLevel and FullChargedCapacity. If both values equal zero, it means that batter was not connected.

        if (ChargeLevel == 0 && FullChargedCapacity == 0)
        {
            float procent = 0;
            stringbuilder.Append(Time.ToString("yyyy-MM-dd HH:mm:ss") + ",SRUM,,,[Battery Level] SRUM - Battery not detected%\r\n");
            Data.Add(Tuple.Create(Time, procent));
        }
        
        else if (ChargeLevel == 0 && FullChargedCapacity != 0)
        {
            float procent = 0;
            stringbuilder.Append(Time.ToString("yyyy-MM-dd HH:mm:ss") + ",SRUM,,,[Battery Level] SRUM - Power level: " + procent + "%\r\n");
            Data.Add(Tuple.Create(Time, procent));
        }

        else
        {
            float procent = (ChargeLevel * 100 / FullChargedCapacity);
            stringbuilder.Append(Time.ToString("yyyy-MM-dd HH:mm:ss") + ",SRUM,,,[Battery Level] SRUM - Power level: " + procent + "%\r\n");
            Data.Add(Tuple.Create(Time, procent));
        }
        
7. Finally, it creates a CSV file and generates a chart.
![alt text](https://github.com/gajos112/BatteryLevel-Timeliner/blob/main/images/1.png?raw=true)

# Timeline
The timeline contains all entires extracted from the database. You can easily review them using BASH and simply GREP what you want to analyze.

![alt text](https://github.com/gajos112/BatteryLevel-Timeliner/blob/main/images/0.png?raw=true)
