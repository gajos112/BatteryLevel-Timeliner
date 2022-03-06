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

{DD6636C4-8929-4683-974E-22C046A43763} - Network Connectivity data
{D10CA2FE-6FCF-4F6D-848E-B2E99266FA89} - Application Resource usage data
{973F5D5C-1D90-4944-BE8E-24B94231A174} - Network usage data
{D10CA2FE-6FCF-4F6D-848E-B2E99266FA86} - Windows Push Notification data
{FEE4E14F-02A9-4550-B5CE-5FA2DA202E37} - Energy usage data

# Timeline
The timeline contains all entires extracted from the database. You can easily review them using BASH and simply GREP what you want to analyze.

![alt text](https://github.com/gajos112/BatteryLevel-Timeliner/blob/main/images/0.png?raw=true)
