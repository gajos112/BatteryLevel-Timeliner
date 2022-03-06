using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Isam.Esent.Interop;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Text.RegularExpressions;

namespace BatteryPower_Timeliner
{
    public partial class MainForm : Form
    {
        DateTime timeRange;

        FolderBrowserDialog openFolderBrowserDialog = new FolderBrowserDialog();
        OpenFileDialog openFileDialogPathToSrumdb = new OpenFileDialog();

        string pathToSrumdb;
        string pathToTimeline;

        List<Tuple<string, DateTime, int, int>> Processes = new List<Tuple<string, DateTime, int, int>>();

        JET_INSTANCE instance;
        JET_SESID sesid;
        JET_DBID dbid;
        JET_TABLEID tableid;

        JET_wrn wrn;

        JET_COLUMNDEF columndefEventTimestamp = new JET_COLUMNDEF();
        JET_COLUMNDEF columndefDesignedCapacity = new JET_COLUMNDEF();
        JET_COLUMNDEF columndefFullChargedCapacity = new JET_COLUMNDEF();
        JET_COLUMNDEF columndefChargeLevel = new JET_COLUMNDEF();

        JET_COLUMNID columnid;

        string nameTABLE = "{FEE4E14F-02A9-4550-B5CE-5FA2DA202E37}";
        string ColumnTimeStamp = "EventTimestamp";
        string ColumnDesignedCapacity = "DesignedCapacity";
        string ColumnFullChargedCapacity = "FullChargedCapacity";
        string ColumnChargeLevel = "ChargeLevel";

        List<Tuple<DateTime, float>> Data = new List<Tuple<DateTime, float>>();

        public MainForm()
        {
            InitializeComponent();
            chart1.MouseWheel += chart1_MouseWheel;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "It is the fully free software created by Krzysztof Gajewski.\r\n\r\nIcons made by Pixel perfect from www.flaticon.com";
            string title = "SRUM - Timeliner";
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Api.JetCloseTable(sesid, tableid);
                Api.JetEndSession(sesid, EndSessionGrbit.None);
                Api.JetTerm(instance);
            }
            catch { }

            Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialogPathToSrumdb.InitialDirectory = @"C:\";
            openFileDialogPathToSrumdb.Filter = "dat files (*.dat)|*.dat|All files (*.*)|*.*";

            if (openFileDialogPathToSrumdb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxPathSRUM.Text = openFileDialogPathToSrumdb.FileName;
                pathToSrumdb = textBoxPathSRUM.Text;
            }
        }

        private void saveToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFolderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxPathTIMELINE.Text = openFolderBrowserDialog.SelectedPath;
                pathToTimeline = textBoxPathTIMELINE.Text;
            }
        }

        public void PrintChart(List<Tuple<DateTime, float>> Data)
        {

            chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;

            LogBox.AppendText("\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Bulding the chart for battery history\r\n");
            chart1.Series["Power Level"].Points.Clear();
            Data = Data.OrderBy(t => t.Item1).ToList();
            int i = 0;
            foreach (var entry in Data)
            {
                try
                {
                    if (checkBox1.Checked == true)
                    {
                        timeRange = DateTime.UtcNow.AddDays(-6);
                        if (entry.Item1 > (DateTime)timeRange)
                        {
                            chart1.Series["Power Level"].Points.AddXY(entry.Item1.ToString("yyyy-MM-dd HH:mm:ss"), entry.Item2);
                            LogBox.AppendText(entry.Item1.ToString("yyyy-MM-dd HH:mm:ss") + ": " + entry.Item2 + "%\r\n");
                            i++;
                        }
                    }

                    if (checkBox2.Checked == true)
                    {
                        timeRange = DateTime.UtcNow.AddDays(-14);
                        if (entry.Item1 > (DateTime)timeRange)
                        {
                            chart1.Series["Power Level"].Points.AddXY(entry.Item1.ToString("yyyy-MM-dd HH:mm:ss"), entry.Item2);
                            LogBox.AppendText(entry.Item1.ToString("yyyy-MM-dd HH:mm:ss") + ": " + entry.Item2 + "%\r\n");
                            i++;
                        }
                    }

                    if (checkBox3.Checked == true)
                    {
                        timeRange = DateTime.UtcNow.AddDays(-29);
                        if (entry.Item1 > (DateTime)timeRange)
                        {
                            chart1.Series["Power Level"].Points.AddXY(entry.Item1.ToString("yyyy-MM-dd HH:mm:ss"), entry.Item2);
                            LogBox.AppendText(entry.Item1.ToString("yyyy-MM-dd HH:mm:ss") + ": " + entry.Item2 + "%\r\n");
                            i++;
                        }
                    }

                    if (checkBox4.Checked == true)
                    {
                        timeRange = DateTime.UtcNow.AddDays(-44);
                        if (entry.Item1 > (DateTime)timeRange)
                        {
                            chart1.Series["Power Level"].Points.AddXY(entry.Item1.ToString("yyyy-MM-dd HH:mm:ss"), entry.Item2);
                            LogBox.AppendText(entry.Item1.ToString("yyyy-MM-dd HH:mm:ss") + ": " + entry.Item2 + "%\r\n");
                            i++;
                        }
                    }

                    if (checkBox5.Checked == true)
                    {
                        timeRange = DateTime.UtcNow.AddDays(-59);
                        if (entry.Item1 > (DateTime)timeRange)
                        {
                            chart1.Series["Power Level"].Points.AddXY(entry.Item1.ToString("yyyy-MM-dd HH:mm:ss"), entry.Item2);
                            LogBox.AppendText(entry.Item1.ToString("yyyy-MM-dd HH:mm:ss") + ": " + entry.Item2 + "%\r\n");
                            i++;
                        }
                    }

                    if (checkBox6.Checked == true)
                    {
                        chart1.Series["Power Level"].Points.AddXY(entry.Item1.ToString("yyyy-MM-dd HH:mm:ss"), entry.Item2);
                        LogBox.AppendText(entry.Item1.ToString("yyyy-MM-dd HH:mm:ss") + ": " + entry.Item2 + "%\r\n");
                        i++;
                    }

                }
                catch { }
            }

            LogBox.AppendText("Found " + i + " entries.\r\n");
            chart1.ChartAreas["ChartArea1"].AxisX.Interval = 2;
        }


        private void buttonParseSRUM_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(pathToSrumdb) && !String.IsNullOrEmpty(pathToTimeline))
                {
                    string CSVPath = pathToTimeline + @"\Timeline_Battery_Level.csv";

                    if (File.Exists(CSVPath))
                    {
                        LogBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Removing the old TIMELINE...\r\n");
                        File.Delete(CSVPath);
                    }

                    string pathDB = pathToSrumdb;

                    if (instance.IsInvalid == true)
                    {
                        Api.JetCreateInstance(out instance, "instance");
                        Api.JetInit(ref instance);

                    }

                    Api.JetBeginSession(instance, out sesid, null, null);
                    try
                    {
                        wrn = Api.JetAttachDatabase(sesid, pathDB, AttachDatabaseGrbit.None);

                        LogBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Attaching the database " + pathDB + "\r\n");
                        LogBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Status: " + wrn + "\r\n");

                        wrn = Api.OpenDatabase(sesid, pathDB, out dbid, OpenDatabaseGrbit.None);
                        LogBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Opening the database: " + pathDB + "\r\n");
                        LogBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Status: " + wrn + "\r\n");

                        wrn = Api.OpenTable(sesid, dbid, nameTABLE, OpenTableGrbit.None, out tableid);
                        LogBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Opening table: " + nameTABLE + "\r\n");
                        LogBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Status: " + wrn + "\r\n");

                        Api.JetGetColumnInfo(sesid, dbid, nameTABLE, ColumnTimeStamp, out columndefEventTimestamp);
                        Api.JetGetColumnInfo(sesid, dbid, nameTABLE, ColumnFullChargedCapacity, out columndefFullChargedCapacity);
                        Api.JetGetColumnInfo(sesid, dbid, nameTABLE, ColumnChargeLevel, out columndefChargeLevel);

                        int i = 0;

                        StringBuilder stringbuilder = new StringBuilder();
                        string row = "";
                        LogBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Starting the eneumeration\r\n");
                        do
                        {
                            try
                            {
                                Int64 TimeInt = (Int64)Api.RetrieveColumnAsInt64(sesid, tableid, columndefEventTimestamp.columnid);
                                DateTime Time = DateTime.FromFileTimeUtc(TimeInt);
                                Int32 FullChargedCapacity = (Int32)Api.RetrieveColumnAsInt32(sesid, tableid, columndefFullChargedCapacity.columnid);
                                Int32 ChargeLevel = (Int32)Api.RetrieveColumnAsInt32(sesid, tableid, columndefChargeLevel.columnid);

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
                                i++;

                                if (i % 100 == 0)
                                {
                                    LogBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Analyzed " + i + " entries.\r\n");
                                }
                            }

                            catch(Exception ex)
                            {
                                MessageBox.Show("Something went wrong\r\n\r\nRrror: " + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                             }

                        } while (Api.TryMoveNext(sesid, tableid));

                        LogBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Found " + i + " entries.\r\n");
                        LogBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Saving the TIMELINE to " + CSVPath + "\r\n");

                        stringbuilder.Replace("\0", "");
                        File.AppendAllText(CSVPath, stringbuilder.ToString());

                        if (File.Exists(CSVPath))
                        {
                            LogBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Successfully created the TIMELINE\r\n");
                        }
                        else
                        {
                            LogBox.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " The TIMELINE was not created\r\n");
                        }

                        PrintChart(Data);

                        Api.JetCloseTable(sesid, tableid);
                        Api.JetEndSession(sesid, EndSessionGrbit.None);
                        Api.JetTerm(instance);

                    }
                    catch (Microsoft.Isam.Esent.Interop.EsentDatabaseDirtyShutdownException)
                    {
                        MessageBox.Show("Could not open the DB, it was not shutdown cleanly! \r\n\r\nRun these commands:\r\n\t'esentutl.exe /r sru /i'\r\n\t'esentutl.exe /p SRUDB.dat'", "DB was not shutdown cleanly", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please provide path to SRUM database and to the TIMELINE!");

                    Api.JetCloseTable(sesid, tableid);
                    Api.JetEndSession(sesid, EndSessionGrbit.None);
                    Api.JetTerm(instance);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong! Are you sure you are trying to open the SRUM database?\r\nFile you are trying to open: " + pathToSrumdb + "\r\n\r\nRrror: " + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        
    }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                checkBox6.Checked = false;

                PrintChart(Data);
            }
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                checkBox1.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                checkBox6.Checked = false;

                PrintChart(Data);
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                checkBox6.Checked = false;

                PrintChart(Data);
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox5.Checked = false;
                checkBox6.Checked = false;

                PrintChart(Data);
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox6.Checked = false;

                PrintChart(Data);
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked == true)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;

                PrintChart(Data);
            }
        }

        private void chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            var yAxis = chart.ChartAreas[0].AxisY;

            try
            {
                if (e.Delta < 0) // Scrolled down.
                {
                    xAxis.ScaleView.ZoomReset();
                    yAxis.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0) // Scrolled up.
                {
                    var xMin = xAxis.ScaleView.ViewMinimum;
                    var xMax = xAxis.ScaleView.ViewMaximum;
                    var yMin = yAxis.ScaleView.ViewMinimum;
                    var yMax = yAxis.ScaleView.ViewMaximum;

                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;
                    var posYStart = yAxis.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 4;
                    var posYFinish = yAxis.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 4;

                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    yAxis.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }
        }
    }
}
