using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//Usings for common part of the API
using Hbm.Api.Common;
using Hbm.Api.Common.Exceptions;
using Hbm.Api.Common.Entities;
using Hbm.Api.Common.Entities.Problems;
using Hbm.Api.Common.Entities.Connectors;
using Hbm.Api.Common.Entities.Channels;
using Hbm.Api.Common.Entities.Signals;
using Hbm.Api.Common.Entities.Filters;
using Hbm.Api.Common.Entities.ConnectionInfos;
using Hbm.Api.Common.Enums;

//Usings for logging
using Hbm.Api.Logging;
using Hbm.Api.Logging.Enums;
using Hbm.Api.Logging.Logger;

using System.Reflection;


//Usings for sensor database access
using Hbm.Api.SensorDB.Entities.Sensors;
using Hbm.Api.SensorDB.Entities.Scalings;

//Usings for common API events 
using Hbm.Api.Common.Messaging;

//Usings for PMX (only necessary, if you want to use special features of PMX devices)
using Hbm.Api.Pmx;
using Hbm.Api.Pmx.Connectors;
using Hbm.Api.Pmx.Channels;
using Hbm.Api.Pmx.Signals;

//Usings for MGC (only necessary, if you want to use special features of MGC devices)
using Hbm.Api.Mgc;
using Hbm.Api.Mgc.Connectors;
using Hbm.Api.Mgc.Channels;
using Hbm.Api.Mgc.Signals;

//Usings for QuantumX (only necessary, if you want to use special features of QuantumX devices)
using Hbm.Api.QuantumX;
using Hbm.Api.QuantumX.Connectors;
using Hbm.Api.QuantumX.Channels;
using Hbm.Api.QuantumX.Signals;

//Usings for GenericStreaming (only necessary, if you want to use generic streaming devices)
//using Hbm.Api.GenericStreaming;


using Hbm.Api.SensorDB;
using Hbm.Api.SensorDB.Entities;

using System.Windows.Forms.VisualStyles;

using Hbm.Api.Common.Entities.SpectrumInfos;
using Hbm.Api.Common.Entities.Values;
using System.Xml;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using SharpGL.SceneGraph.Lighting;
using SharpGL.SceneGraph;
using Baumer.OXApi;
using Baumer.OXApi.Types;
using Timer = System.Windows.Forms.Timer;
using System.Globalization;
using System.Diagnostics;
using Baumer.OXApi.UdpStreaming;

using System.Threading.Tasks;


namespace Laser_Wenglor_3D
{
    public partial class Home : Form
    {
        #region wenglor
        [DllImport("EthernetScanner.dll", EntryPoint = "EthernetScanner_Connect", CallingConvention = CallingConvention.StdCall)]
        //private unsafe static extern IntPtr EthernetScanner_Connect(StringBuilder chIP, StringBuilder chPort, int uiNonBlockingTimeOut);
        public static extern IntPtr EthernetScanner_Connect(string ipAddress, string port, int nonBlockingTimeOut);

        [DllImport("EthernetScanner.dll", EntryPoint = "EthernetScanner_Disconnect", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr EthernetScanner_Disconnect(IntPtr hScanner);

        [DllImport("EthernetScanner.dll", EntryPoint = "EthernetScanner_GetConnectStatus", CallingConvention = CallingConvention.StdCall)]
        public static extern void EthernetScanner_GetConnectStatus(IntPtr hScanner, int[] uiConnectStatus);

        [DllImport("EthernetScanner.dll", EntryPoint = "EthernetScanner_WriteData", CallingConvention = CallingConvention.StdCall)]
        public static extern int EthernetScanner_WriteData(IntPtr hScanner, byte[] chWriteBuffer, int uiWriteBufferLength);

        [DllImport("EthernetScanner.dll", EntryPoint = "EthernetScanner_GetVersion", CallingConvention = CallingConvention.StdCall)]
        public static extern int EthernetScanner_GetVersion(StringBuilder ucBuffer, int uiBuffer);

        [DllImport("EthernetScanner.dll", EntryPoint = "EthernetScanner_GetXZIExtended", CallingConvention = CallingConvention.StdCall)]
        public static extern int EthernetScanner_GetXZIExtended(IntPtr sensorHandle,
                                                                double[] x,
                                                                double[] z,
                                                                int[] intensity,
                                                                int[] peakWidth,
                                                                int buffer,
                                                                int[] encoder,
                                                                byte[] pucUSRIO,
                                                                int timeOut,
                                                                byte[] ucBufferRaw,
                                                                int iBufferRaw,
                                                                int[] picCount);

        [DllImport("EthernetScanner.dll", EntryPoint = "EthernetScanner_ReadData", CallingConvention = CallingConvention.StdCall)]
        public static extern int EthernetScanner_ReadData(IntPtr sensorHandle, string strPropertyName, StringBuilder RetBuf, int iRetBuf, int iCashTime);

        [DllImport("EthernetScanner.dll", EntryPoint = "EthernetScanner_GetInfo", CallingConvention = CallingConvention.StdCall)]
        public static extern int EthernetScanner_GetInfo(IntPtr sensorHandle, StringBuilder info, int buffer, string mode);

        [DllImport("EthernetScanner.dll", EntryPoint = "EthernetScanner_ResetDllFiFo", CallingConvention = CallingConvention.StdCall)]
        public static extern int EthernetScanner_ResetDllFiFo(IntPtr sensorHandle);

        [DllImport("EthernetScanner.dll", EntryPoint = "EthernetScanner_GetDllFiFoState", CallingConvention = CallingConvention.StdCall)]
        public static extern int EthernetScanner_GetDllFiFoState(IntPtr sensorHandle);

        internal int iETHERNETSCANNER_TCPSCANNERDISCONNECTED = 0;

        internal int iETHERNETSCANNER_TCPSCANNERCONNECTED = 3;

        internal int iETHERNETSCANNER_PEAKSPERCMOSSCANLINEMAX = 2;

        internal int iETHERNETSCANNER_SCANXMAX = 2048;

        public int iETHERNETSCANNER_GETINFOSIZEMAX = 128 * 1024;

        public int iETHERNETSCANNER_GETINFONOVALIDINFO = -3;

        public int iETHERNETSCANNER_GETINFOSMALLBUFFER = -2;

        public int iETHERNETSCANNER_ERROR = -1;



        private double yhbm;
        private double xhbm;
        private double zhbm;
        private double tmphbm;
        private double xhbmtmp;

        internal int m_iScannerDataLen = 0;


        public float m_CScanView1_Z_Start = 0;

        public float m_CScanView1_Z_Range = 0;

        public float m_CScanView1_X_Range_At_Start = 0;

        public float m_CScanView1_X_Range_At_End = 0;

        public float m_CScanView1_WidthX = 0;

        public float m_CScanView1_WidthZ = 0;

        public int[] iConnectionStatus = null;

        public IntPtr ScannerHandle;

        public String strApplicationPath = "";

        public String strModelNumber = "";

        public String strProductVersion = "";

        public String strVendorName = "";

        public String strDescription = "";

        public String strHardwareVersion = "";

        public String strFirwareVersion = "";

        public String strScannerMAC = "";

        public String strSerialNummber = "";

        public String strIPAddress = "";
        private List<(double, double, double, double)> hbmData = new List<(double, double, double, double)>();

        private Thread globalth;
        private bool runglobalth = false;

        public StringBuilder m_strScannerInfoXML = null;
        private Thread dataThread;


        public double[] m_doX = null;

        public double[] m_doZ = null;

        public int[] m_iIntensity = null;

        internal int[] m_iPeakWidth = null;

        internal int[] m_iEncoder = null;

        internal byte[] m_bUSRIO = null;

        internal int[] m_iPicCnt = null;

        internal int[] m_iPicCntTemp = null;


        private bool isRunning = false;
        private bool startscan = false;
        private bool stopenquedata = false;

        private Thread ydataThread;
        private bool runingData = false;
        double offsetTmpHbm;



        public float frequslaserweng;

        #endregion

        #region Global variables
        private bool isConnectedhbm = false;

        // Variables used to show the general workflow 
        private DaqEnvironment _daqEnvironment;   // main object to scan, connect and parameterize devices
        private DaqMeasurement _daqMeasurement;   // main object to execute measurements
        private Device _myDevice;         // device to connect and to use within this demo
        private List<Signal> _signalsToMeasure; // list of signals to use for a continuous measurement
        private bool _runMeasurement;   // true, while data acquisition is running...
        private List<Device> _deviceList;       // list of devices found by the scan

        private delegate void VisualizeDeviceEventHandler(DeviceEventArgs e); // delegate for our event handling

        // Sensor data base access
        private ISensorDB _sensorDBManagerForHbmSensorDatabase;  // sensor manager, used to access the HBM sensor database 
        private ISensorDB _sensorDBManagerForUserSensorDatabase; // sensor manager, used to access a user sensor database

        // Logging
        private ILogger _logger;                     // a logger object that can be used to log messages
        private LogContext _logContextApiDemoMeasuring; // context to log messages in a hierarchical way here: Messages related to measurement issues

        private LogContext
            _logContextApiDemoProblems; // context to log messages in a hierarchical way here: Messages related to problems that occurred during the execution of the demo

        private int _logNumberDummy = 0; // just a counter used to generate different log entries...

        #endregion

        public List<DataPoint> Data = new List<DataPoint>();
        public bool WenglorConnected = false;

        public double minY = 0.0;
        public double minX = 0.0;

        public Ox ox;
        public OxStream stream;

        public bool oxisconnected = false;
        private System.Windows.Forms.Timer GUI_Timer;

        private double x;
        private double z;
        private double Intensity;
        private ulong tmpscan;
        float valeurini = 1000;
        double offsetTmpScan;
        public float frequencyHzscan1;
        internal int iNumberProfiles = 0;
        private int Varconstante;
        public bool xflag = false;
        public Home()
        {

            InitializeComponent();
            m_strScannerInfoXML = new StringBuilder(new String(' ', iETHERNETSCANNER_GETINFOSIZEMAX));
            ScannerHandle = (IntPtr)null;
            //EthernetScanner connection status
            iConnectionStatus = new int[1];
            //Current Path of the exe-File
            strApplicationPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

            m_doX = new double[iETHERNETSCANNER_SCANXMAX * iETHERNETSCANNER_PEAKSPERCMOSSCANLINEMAX + 1];
            m_doZ = new double[iETHERNETSCANNER_SCANXMAX * iETHERNETSCANNER_PEAKSPERCMOSSCANLINEMAX + 1];
            m_iIntensity = new int[iETHERNETSCANNER_SCANXMAX * iETHERNETSCANNER_PEAKSPERCMOSSCANLINEMAX + 1];
            m_iPeakWidth = new int[iETHERNETSCANNER_SCANXMAX * iETHERNETSCANNER_PEAKSPERCMOSSCANLINEMAX + 1];
            m_iEncoder = new int[1];
            m_bUSRIO = new byte[1];
            m_iPicCnt = new int[1];
            m_iPicCntTemp = new int[1];




            GUI_Timer = new System.Windows.Forms.Timer();
            GUI_Timer.Interval = 1000;
            GUI_Timer.Tick += new EventHandler(Timer_function);

            if (int.TryParse(valconstante.Texts, out int inputValue))
            {
                Varconstante = inputValue;
            }
            else
            {
                MessageBox.Show("Entrée invalide. S'il vous plait, entrez un nombre valide.");
            }


        }
        private void Home_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            button3D_Click(sender, e);
            try
            {
                _daqEnvironment = DaqEnvironment.GetInstance();
                _daqMeasurement = new DaqMeasurement();

                MessageBroker.DeviceConnected += MessageBroker_DeviceConnected;
                MessageBroker.DeviceDisconnected += MessageBroker_DeviceDisconnected;

                AddToProtocol("DaqEnvironment and DaqMeasurement objects initialized");
                AddToProtocol("Device event handlers registered");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.ToString());
            }

        }
        private void Timer_function(object sender, EventArgs e)
        {
            textBoxYhbm.Texts = yhbm.ToString("F2");

            textBoxxhbm.Texts = xhbm.ToString("F2");

            textboxSpeedx.Texts = speedx.ToString("F2");

            textBoxzhbm.Texts = zhbm.ToString("F2");

            textboxSpeedy.Texts = speedy.ToString("F2");

            textboxfreqhbm.Texts = hzHbmtimer.ToString("F2");
 


            if (WenglorConnected == true)
            {

                getfreqscan1();

            }

            if (oxisconnected == true)
            {


                Trigger trigger = ox.GetTrigger();
                double timeInSeconds = trigger.Time / 1000000.0;
                int frequency = (int)(1.0 / timeInSeconds);

                textboxfreq.Texts = frequency.ToString();
            }



        }
        public int frequency;
            
        private void btnToggleConnection_Click(object sender, EventArgs e)
        {
            if (checkBox1Baumer.Checked)
            {
                if (!oxisconnected)
                {
                    string ipAddress = txtIPAddress.Texts;
                    ox = Ox.Create(ipAddress);

                    try
                    {
                        ox.Connect();
                        ox.Login("admin", "");
                        Trigger trigger = ox.GetTrigger();
                        double timeInSeconds = trigger.Time / 1000000.0;
                        frequency = (int)(1.0 / timeInSeconds);

                        textboxfreq.Texts = frequency.ToString();
                        textBoxTime.Texts = frequency.ToString();


                        stream = ox.CreateStream();
                        stream.Start();


                        dataThread = new Thread(GetDataThread);
                        dataThread.Start();
                        isRunning = true;

                        /*AlldataThread = new Thread(towdscanner);
                        AlldataThread.Start();
                        alldata = true;
                        */

                        GUI_Timer.Start();

                        oxisconnected = true;

                        AppendTextToCommandTextBox("Laser Baumer est Connectée");
                        btnToggleConnection.Text = "Déconnecter";
                        btnToggleConnection.BackColor = Color.Green;




                    }
                    catch (Exception ex)
                    {
                        AppendTextToCommandTextBox("Erreur de connexion à l'appareil: " + ex.Message);
                    }
                }
                else
                {
                    if (dataThread != null && dataThread.IsAlive)
                    {
                        isRunning = false;
                        if (!dataThread.Join(TimeSpan.FromSeconds(2)))
                        {
                            dataThread.Abort();
                        }
                        dataThread = null;
                    }



                    if (globalth != null && globalth.IsAlive)
                    {
                        runglobalth = false;
                        if (!globalth.Join(TimeSpan.FromSeconds(2)))
                        {
                            globalth.Abort();
                        }
                        globalth = null;
                    }

                    DisconnectDevice();
                    btnToggleConnection.Text = "Connecter";
                    btnToggleConnection.BackColor = Color.SaddleBrown;
                }
            }
            if (checkBox1Wenglor.Checked)
            {

                if (!WenglorConnected)
                {
                    if (ScannerHandle != (IntPtr)null)
                    {
                        return;
                    }

                    /*textBoxLinearizationTableSerialNumber1.Text = "";
                    textBoxLinearizationTableSerialNumber2.Text = "";
                    textBoxLinearizationTableSerialNumber3.Text = "";*/

                    strIPAddress = txtIPAddress.Texts;
                    string strPort = "32001";
                    //int iTimeOut = Int32.Parse(textBoxTimeOut.Text);
                    int iTimeOut = 0;

                    //start the connection to the Scanner
                    ScannerHandle = EthernetScanner_Connect(strIPAddress, strPort, iTimeOut);

                    //check the connection state with timeout 3000 ms
                    DateTime startConnectTime = DateTime.Now;
                    TimeSpan connectTime = new TimeSpan();
                    do
                    {
                        if (connectTime.TotalMilliseconds > 1500)
                        {
                            ScannerHandle = EthernetScanner_Disconnect(ScannerHandle);
                            MessageBox.Show("Erreur : aucune connexion !!!", "ERREUR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        Thread.Sleep(10);
                        EthernetScanner_GetConnectStatus(ScannerHandle, iConnectionStatus);
                        connectTime = DateTime.Now - startConnectTime;
                    } while (iConnectionStatus[0] != iETHERNETSCANNER_TCPSCANNERCONNECTED);


                    int iGetInfoRes = EthernetScanner_GetInfo(ScannerHandle, m_strScannerInfoXML, iETHERNETSCANNER_GETINFOSIZEMAX, "xml");
                    if (iGetInfoRes > 0)
                    {
                        GetXMLParser(m_strScannerInfoXML);
                        dataThread = new Thread(GetDataThread);
                        dataThread.Priority = ThreadPriority.Highest;
                        dataThread.Start();
                        isRunning = true;

                        
                        AppendTextToCommandTextBox("Laser Wenglor est Connectée");

                        GUI_Timer.Start();
                        btnToggleConnection.BackColor = Color.Green;
                        btnToggleConnection.Text = "Déconnecter";
                        getfreqscan1();
                        WenglorConnected = true;
                    }
                    else
                    {

                        MessageBox.Show("Erreur : aucun paquet d'informations valide !!!", "ERREUR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }


                }
                else
                {
                    if (dataThread != null && dataThread.IsAlive)
                    {
                        isRunning = false;
                        if (!dataThread.Join(TimeSpan.FromSeconds(2)))
                        {
                            dataThread.Abort();
                        }
                        dataThread = null;
                    }



                    if (globalth != null && globalth.IsAlive)
                    {
                        runglobalth = false;
                        if (!globalth.Join(TimeSpan.FromSeconds(2)))
                        {
                            globalth.Abort();
                        }
                        globalth = null;
                    }


                    if (ScannerHandle != IntPtr.Zero)
                    {
                        ScannerHandle = EthernetScanner_Disconnect(ScannerHandle);
                        AppendTextToCommandTextBox("Laser Wenglor est déconnecté");

                    }


                    btnToggleConnection.Text = "Connecter";
                    btnToggleConnection.BackColor = Color.SaddleBrown;


                    WenglorConnected = false;
                }










            }
        }

        private void sendfreq_Click(object sender, EventArgs e)
        {
            if (checkBox1Wenglor.Checked)
            {
                double frequency;
                if (!double.TryParse(freqchnage.Texts, out frequency))
                {
                    return;
                }
                double microseconds = 1000000 / frequency;
                int microsecondsRounded = (int)Math.Round(microseconds);
                string command = "SetAcquisitionLineTime=" + microsecondsRounded.ToString();
                byte[] buffer = Encoding.ASCII.GetBytes(command);
                int iRes = Home.EthernetScanner_WriteData(ScannerHandle, buffer, buffer.Length);


            }

            if (checkBox1Baumer.Checked)
            {
                if (float.TryParse(freqchnage.Texts, out float freqValue))
                {
                    float timePeriod = 1.0f / freqValue * 1000000.0f;
                    uint timePeriodUint = (uint)timePeriod;

                    if (timePeriodUint >= 500 && timePeriodUint <= 4000000)
                    {
                        ox.ConfigureTrigger(2, 0, timePeriodUint, 0);

                    }
                    else
                    {
                        MessageBox.Show("La valeur de fréquence doit être comprise entre 0,3 et 800.", "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Valeur de fréquence invalide. Veuillez saisir un nombre réel valide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }

        }
        private void getfreqscan1()
        {

            StringBuilder retBuf = new StringBuilder(256);

            int iRes = Home.EthernetScanner_ReadData(ScannerHandle, "GetAcquisitionLineTime", retBuf, retBuf.Capacity, 0);
            if (iRes == 0)
            {
                frequslaserweng = float.Parse(retBuf.ToString());

                float acquisitionLineTimeUs = float.Parse(retBuf.ToString());
                frequencyHzscan1 = (float)(1.0 / (acquisitionLineTimeUs * 0.000001));
                frequencyHzscan1 = (float)Math.Round(frequencyHzscan1);
                textboxfreq.Texts = frequencyHzscan1.ToString();
                textBoxTime.Texts = frequencyHzscan1.ToString();

            }
        }
        private void DisconnectDevice()
        {
            if (ox != null)
            {
                try
                {
                    ox.Disconnect();
                    oxisconnected = false;
                    stream.Close();
                    stream.Stop();
                    AppendTextToCommandTextBox("Laser Baumer est déconnecté");
                }
                catch (Exception ex)
                {
                    AppendTextToCommandTextBox("Erreur lors de la déconnexion de l'appareil : " + ex.Message);
                }
            }
        }
        private void GetXMLParser(StringBuilder strXML)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(strXML.ToString());

            XmlNodeList nodes1 = doc.DocumentElement.SelectNodes("/device/general");
            foreach (XmlNode node in nodes1)
            {
                string strTemp = node.SelectSingleNode("workingrange_z_start").InnerText;

                m_CScanView1_Z_Start = float.Parse(node.SelectSingleNode("workingrange_z_start").InnerText.Replace('.', ','));
                m_CScanView1_Z_Range = float.Parse(node.SelectSingleNode("measuringrange_z").InnerText.Replace('.', ','));
                m_CScanView1_X_Range_At_Start = float.Parse(node.SelectSingleNode("fieldwidth_x_start").InnerText.Replace('.', ','));
                m_CScanView1_X_Range_At_End = float.Parse(node.SelectSingleNode("fieldwidth_x_end").InnerText.Replace('.', ','));
                m_CScanView1_WidthX = float.Parse(node.SelectSingleNode("pixel_x_max").InnerText.Replace('.', ','));
                m_CScanView1_WidthZ = float.Parse(node.SelectSingleNode("pixel_z_max").InnerText.Replace('.', ','));
                strModelNumber = node.SelectSingleNode("ordernumber").InnerText;
                strProductVersion = node.SelectSingleNode("productversion").InnerText;
                strVendorName = node.SelectSingleNode("producer").InnerText;
                strDescription = node.SelectSingleNode("description").InnerText;
                strHardwareVersion = node.SelectSingleNode("hardwareversion").InnerText;
                strFirwareVersion = node.SelectSingleNode("firmwareversion/general").InnerText;
                strScannerMAC = node.SelectSingleNode("mac").InnerText;
                strSerialNummber = node.SelectSingleNode("serialnumber").InnerText;


            }
        }


        private bool stop3D = true;






        #region HBM
        private void AddToProtocol(string message)
        {
            ProtocolTb.AppendText(message + Environment.NewLine);
        }




        void MessageBroker_DeviceConnected(object sender, DeviceEventArgs e)
        {
            UpdateConsoleDeviceConnected(e);
        }

        void MessageBroker_DeviceDisconnected(object sender, DeviceEventArgs e)
        {
            // Show info in our console
            UpdateConsoleDeviceDisConnected(e);
        }

        private void UpdateConsoleDeviceConnected(DeviceEventArgs e)
        {
            if (ProtocolTb.InvokeRequired)
            {
                // Call this method again but on the GUI thread
                ProtocolTb.Invoke(new VisualizeDeviceEventHandler(UpdateConsoleDeviceConnected), new object[] { e });
            }
            else
            {
                AddToProtocol("Connected to device: " + e.UniqueDeviceID);
            }
        }
        private void UpdateConsoleDeviceDisConnected(DeviceEventArgs e)
        {
            if (ProtocolTb.InvokeRequired)
            {
                // Call this method again but on the GUI thread
                ProtocolTb.Invoke(new VisualizeDeviceEventHandler(UpdateConsoleDeviceDisConnected), new object[] { e });
            }
            else
            {
                AddToProtocol("DisConnected from device: " + e.UniqueDeviceID);
            }
        }


        private void ScanForDevicesBt_Click_1(object sender, EventArgs e)
        {
            try
            {
                // check, which device families are supported...
                List<string> supportedDeviceFamilies = _daqEnvironment.GetAvailableDeviceFamilyNames();

                foreach (string family in supportedDeviceFamilies)
                {
                    AddToProtocol("Supported device family:" + family);
                }

                // scan for all supported device families
                _deviceList = _daqEnvironment.Scan(supportedDeviceFamilies);

                // notice that the list of devices already has some information about the devices - 
                // although they are NOT yet connected. The information is delivered by the scan!

                //sort the list by device name
                _deviceList = _deviceList.OrderBy(n => n.Name).ToList();

                AddToProtocol(string.Format("Found devices:{0}", _deviceList.Count));

                foreach (Device dev in _deviceList)
                {
                    AddToProtocol(string.Format("Devicename: {0} Serialnumber: {1}  FirmwareVersion: {2}", dev.Name.PadRight(22), dev.SerialNo.PadRight(16), dev.FirmwareVersion));
                }

                //update comboBox with found devices and their IP addresses:
                _deviceListComboBox.Items.Clear();

                foreach (Device device in _deviceList)
                {
                    _deviceListComboBox.Items.Add(device.Name.PadRight(14) + " (" + (device.ConnectionInfo as EthernetConnectionInfo).IpAddress + ")");
                }

                //select first device within the 
                if (_deviceListComboBox.Items.Count > 0)
                {
                    _deviceListComboBox.SelectedIndex = 0;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.ToString());
            }
        }

        private void ConnectDisconnectButton_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!isConnectedhbm)
                {
                    if (_deviceList.Count > 0)
                    {
                        List<Problem> problemList = new List<Problem>();

                        _myDevice = _deviceList[_deviceListComboBox.SelectedIndex];
                        _daqEnvironment.Connect(_myDevice, out problemList);

                        ydataThread = new Thread(Getydatathread);
                        ydataThread.Start();
                        runingData = true;
                        isConnectedhbm = true;
                        ConnectDisconnectButton.BackColor = Color.Green;
                        ConnectDisconnectButton.Text = "Disconnect";

                        AddToProtocol(string.Format("Device {0} is connected; It has {1} connectors", _myDevice.Name, _myDevice.Connectors.Count));
                    }
                    else
                    {
                        MessageBox.Show("No devices scanned.", "Error");
                    }
                }
                else
                {
                    _daqEnvironment.Disconnect(_myDevice);

                    isConnectedhbm = false;

                    ConnectDisconnectButton.BackColor = Color.SaddleBrown;
                    ConnectDisconnectButton.Text = "Connect";

                    AddToProtocol(string.Format("Device {0} has been disconnected.", _myDevice.Name));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }

        #endregion


        private void GetDataThread()
        {
            while (isRunning)
            {
                if (checkBox1Baumer.Checked)
                {
                    try
                    {
                       
                        
                        if (stream.ProfileAvailable)
                        {
                            var profile = stream.ReadProfile();
                            float pre = 100;
                            bool isflag2 = true;
                            if (startscan == true)
                            {

                                for (int i = 0; i < profile.Length; i++)
                                {
                                    x = profile.X[i] / pre;
                                    z = profile.Z[i] / pre;
                                    tmpscan = profile.Timestamp;

                                    double newlaserTime = 0;

                                    if (isflag2)
                                    {
                                        newlaserTime = 0;
                                        isflag2 = false;
                                    }
                                    else
                                    {
                                        newlaserTime = tmpscan - offsetTmpScan;
                                    }



                                    DataPoint dataPoint = new DataPoint
                                    {
                                        X = x,
                                        Z = z,
                                        Timestamp = tmpscan,
                                        Ilaser = Intensity,
                                        NewTime = newlaserTime

                                    };
                                    Data.Add(dataPoint);

                                    if (Data.Count > 0)
                                    {
                                        offsetTmpScan = Data[0].Timestamp;
                                    }
                                }
                            }



                            }
                        






                        


                    }
                    catch (Exception ex)
                    {
                        AppendTextToCommandTextBox("Erreur lors de la récupération des données de l'appareil : " + ex.Message);
                    }

                }
                if (checkBox1Wenglor.Checked)
                {

                    WenglorData();
                    if (m_iScannerDataLen <= 0)
                    {
                        continue;
                    }
                }

            }


        }



        int nbprofile = 0;
        private bool isflag3 = true;


        private void WenglorData()
        {
            try
            {

                EthernetScanner_GetConnectStatus(ScannerHandle, iConnectionStatus);
                if (iConnectionStatus[0] == iETHERNETSCANNER_TCPSCANNERCONNECTED)
                {
                    m_iScannerDataLen = EthernetScanner_GetXZIExtended(
                                                                    ScannerHandle,
                                                                    m_doX,
                                                                    m_doZ,
                                                                    m_iIntensity,
                                                                    m_iPeakWidth,
                                                                    iETHERNETSCANNER_PEAKSPERCMOSSCANLINEMAX * iETHERNETSCANNER_SCANXMAX,
                                                                    m_iEncoder,
                                                                    m_bUSRIO,
                                                                    100,
                                                                    null,
                                                                    0,
                                                                    m_iPicCnt);



                    if (startscan == true)
                    {
                        nbprofile++;
                        for (int k = 0; k <= m_iScannerDataLen - 1; k++)
                        {
                            
                            double newlaserTime = 0;

                            if (isflag3 == true)
                            {
                                newlaserTime = 0;
                                isflag3 = false;
                            }
                            else
                            {
                                newlaserTime = frequslaserweng * nbprofile - offsetTmpScan;
                            }


                            if (stopenquedata == false)
                            {

                                DataPoint dataPoint = new DataPoint
                                {
                                    X = m_doX[k],
                                    Z = m_doZ[k] - zhbm - Varconstante,
                                    Ilaser = m_iIntensity[k],
                                    Timestamp = frequslaserweng,
                                    NewTime = newlaserTime,
                                };
                                if (m_iIntensity[k] == 0)
                                {



                                }
                                else
                                {

                                    Data.Add(dataPoint);

                                }

                            }



                            if (Data.Count == 1)
                            {
                                offsetTmpScan = Data[0].Timestamp;
                            }

                        }

                    }

                }



            }
            catch (Exception ex)
            {
                AppendTextToCommandTextBox("Error getting data from the device: " + ex.Message);
            }



        }
        bool isflag2 = true;







        private List<(double, double)> xhbmList = new List<(double, double)>();
        private List<(double, double)> yhbmList = new List<(double, double)>();

        private double speedx = 0.0;
        private double speedx1 = 0.0;
        private double speedx2 = 0.0;
        private double speedx3 = 0.0;
        private double speedx4 = 0.0;

        private double speedy = 0.0;
        private double speedy1 = 0.0;
        private double speedy2 = 0.0;
        private double speedy3 = 0.0;
        private double speedy4 = 0.0;

        private double oldFirstPoint;
        private double oldLastPoint;
        private decimal hzHbm = 10;
        private decimal hzHbmtimer;

        private void Getydatathread()
        {
            bool isflag1 = true;
            bool waitFlag = false;
            double waitStartTime = 0;
            double elapsedMilliseconds = 0;
            Stopwatch stopwatch = new Stopwatch();

            while (runingData)
            {

                if (startscan)
                {

                    double intervalMilliseconds = 1000.0 / (double)hzHbm;

                    if (!waitFlag)
                    {
                        stopwatch.Start();

                        Signal tempSignal = _myDevice.Connectors[5].Channels[0].Signals[0];

                        _myDevice.ReadSingleMeasurementValue(new List<Signal>() { tempSignal });
                        yhbm = tempSignal.GetSingleMeasurementValue().Value;
                        tmphbm = tempSignal.GetSingleMeasurementValue().Timestamp;
                        hzHbmtimer = tempSignal.SampleRate;


                        Signal signalx = _myDevice.Connectors[4].Channels[0].Signals[0];
                        _myDevice.ReadSingleMeasurementValue(new List<Signal>() { signalx });
                        xhbm = signalx.GetSingleMeasurementValue().Value;
                        xhbmtmp = signalx.GetSingleMeasurementValue().Timestamp ;

                        yhbmList.Add((yhbm, tmphbm));
                        xhbmList.Add((xhbm, xhbmtmp));

                        if (xhbmList.Count >= 10)
                        {
                            double firstpoint = xhbmList[0].Item1;
                            double lastpoint = xhbmList[2].Item1;

                            double firsttime = xhbmList[0].Item2;
                            double lasttime = xhbmList[2].Item2 ;


                            speedx1 = (firstpoint - lastpoint) / (firsttime - lasttime);


                            double firstpoint2 = xhbmList[1].Item1;
                            double lastpoint2 = xhbmList[3].Item1;

                            double firsttime2 = xhbmList[1].Item2;
                            double lasttime2 = xhbmList[3].Item2 ;

                            speedx2 = (firstpoint2 - lastpoint2) / (firsttime2 - lasttime2);


                            double firstpoint3 = xhbmList[4].Item1;
                            double lastpoint3 = xhbmList[6].Item1;

                            double firsttime3 = xhbmList[4].Item2 ;
                            double lasttime3 = xhbmList[6].Item2 ;

                            speedx3 = (firstpoint3 - lastpoint3) / (firsttime3 - lasttime3);


                            double firstpoint4 = xhbmList[7].Item1;
                            double lastpoint4 = xhbmList[9].Item1;

                            double firsttime4 = xhbmList[7].Item2 ;
                            double lasttime4 = xhbmList[9].Item2 ;

                            speedx4 = (firstpoint4 - lastpoint4) / (firsttime4 - lasttime4);




                            speedx = (speedx1 + speedx2 + speedx3 + speedx4) / 4;
                        }
                        if (xhbmList.Count > 10)
                        {
                            xhbmList.RemoveAt(0);

                        }




                        if (yhbmList.Count >= 10)
                        {
                            double firstpoint = yhbmList[0].Item1;
                            double lastpoint = yhbmList[5].Item1;

                            double firsttime = yhbmList[0].Item2 ;
                            double lasttime = yhbmList[5].Item2 ;
                            speedy1 = (firstpoint - lastpoint) / (firsttime - lasttime);




                            double firstpoint2 = yhbmList[1].Item1;
                            double lastpoint2 = yhbmList[6].Item1;

                            double firsttime2 = yhbmList[1].Item2 ;
                            double lasttime2 = yhbmList[6].Item2 ;

                            speedy2 = (firstpoint2 - lastpoint2) / (firsttime2 - lasttime2);

                            double firstpoint3 = yhbmList[3].Item1;
                            double lastpoint3 = yhbmList[8].Item1;

                            double firsttime3 = yhbmList[3].Item2 ;
                            double lasttime3 = yhbmList[8].Item2 ;
                            speedy3 = (firstpoint3 - lastpoint3) / (firsttime3 - lasttime3);


                            double firstpoint4 = yhbmList[4].Item1;
                            double lastpoint4 = yhbmList[9].Item1;

                            double firsttime4 = yhbmList[4].Item2 ;
                            double lasttime4 = yhbmList[9].Item2 ;
                            speedy4 = (firstpoint4 - lastpoint4) / (firsttime4 - lasttime4);


                            speedy = (speedy1 + speedy2 + speedy3 + speedy4) / 4;

                        }
                        if (yhbmList.Count > 10)
                        {
                            yhbmList.RemoveAt(0);

                        }






                        if (stopenquedata == false)
                        {
                            double newHbmTime;
                            double calctime = 0;

                            if (isflag1)
                            {
                                newHbmTime = 0;
                                isflag1 = false;
                            }
                            else
                            {
                                newHbmTime = tmphbm - offsetTmpHbm;
                                calctime = newHbmTime * 1000000;
                            }


                            hbmData.Add((yhbm, tmphbm, xhbm, calctime));
                           // Console.WriteLine(calctime);

                            if (hbmData.Count == 1)
                            {
                                offsetTmpHbm = hbmData[0].Item2;
                            }

                            minY = hbmData.Select(item => item.Item1).Min();
                            minX = hbmData.Select(item => item.Item3).Min();
                        }
                        stopwatch.Stop();

                        elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;

                        if (elapsedMilliseconds < intervalMilliseconds)
                        {
                            waitFlag = true;
                            waitStartTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
                        }

                        stopwatch.Reset();
                    }
                    else
                    {
                        double currentTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
                        if (currentTime - waitStartTime >= intervalMilliseconds - elapsedMilliseconds)
                        {
                            waitFlag = false;
                        }
                    }
                }



             

            }
        }

        private object hbmDataLock = new object();
        private int indexlaser = 0;
        private int jHbm = 0;
        private double DataTimeprofile = 0;


        private void globalthread()
        {
            while (runglobalth == true)
            {
                Parallel.ForEach(Data, (dataPoint, state) =>
                {
                    ProcessData();
                });


            }
        }
        private void ProcessData()
        {
            if (startscan)
            {



                lock (hbmDataLock)
                {
                    while (indexlaser < Data.Count)
                    {
                        if (indexlaser >= 0 && indexlaser < Data.Count)
                        {
                            DataPoint dataPoint = Data[indexlaser];

                            if (dataPoint != null)
                            {
                                double dataTimeLaser = dataPoint.NewTime;

                                if (DataTimeprofile != dataTimeLaser)
                                {
                                    DataTimeprofile = dataTimeLaser;
                                    // Console.WriteLine(jHbm + " " + dataCount + " " + hbmData.Count);
                                }

                                bool pointfind = false;
                                do
                                {
                                    if (jHbm < hbmData.Count)
                                    {
                                        var hbmPoint = hbmData[jHbm];

                                        if (dataTimeLaser == hbmPoint.Item4)
                                        {
                                            dataPoint.Yhbm = hbmPoint.Item1;
                                            dataPoint.Xhbm = hbmPoint.Item3;

                                            pointfind = true;
                                            if (DataTimeprofile != dataTimeLaser)
                                            {
                                                jHbm++;
                                            }
                                        }
                                        else if (jHbm > 0)
                                        {
                                            var hbmPoint1 = hbmData[jHbm - 1];
                                            var hbmPoint2 = hbmData[jHbm];

                                            if (hbmPoint2.Item4 > hbmPoint1.Item4)
                                            {
                                                if (dataTimeLaser < hbmPoint2.Item4)
                                                {
                                                    double v = (hbmPoint2.Item1 - hbmPoint1.Item1) / (hbmPoint2.Item4 - hbmPoint1.Item4);
                                                    double timeDiff = dataTimeLaser - hbmPoint1.Item4;
                                                    dataPoint.Yhbm = hbmPoint1.Item1 + v * timeDiff;

                                                    double vs = (hbmPoint2.Item3 - hbmPoint1.Item3) / (hbmPoint2.Item4 - hbmPoint1.Item4);
                                                    dataPoint.Xhbm = hbmPoint1.Item3 + vs * timeDiff;


                                                    pointfind = true;
                                                }
                                                else
                                                {
                                                    jHbm++;
                                                }
                                            }
                                        }
                                        else if (jHbm == 0)
                                        {
                                            jHbm++;
                                        }
                                    }
                                } while (jHbm <= hbmData.Count && !pointfind);

                                if (pointfind)
                                {
                                    indexlaser++;
                                }
                            }
                            else
                            {
                                indexlaser++;
                            }
                        }
                        else
                        {
                            indexlaser++;
                        }
                    }
                }
            }

        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            /*runglobalth = false;
            isflag3 = true; 
            Data.Clear();
            hbmData.Clear();
            minY = 0.0;
            indexlaser = 0;
            nbprofile = 0;
            jHbm = 0;
            DataTimeprofile = 0;
            offsetTmpScan = 0;
            offsetTmpHbm = 0;*/
            

            isRunning = false;
            runingData = false; 
            runglobalth = false; 

            // Clear data structures and reset variables
            Data.Clear();
            xhbmList.Clear();
            yhbmList.Clear();
            hbmData.Clear();
            // Reset other variables as needed

            // Reset flags
            isflag2 = true;
            isflag3 = true;
            // Reset other flags as needed

            // Reset counters and indices
            indexlaser = 0;
            jHbm = 0;


            isRunning = true;
            runingData = true;
            runglobalth = true;
        }

        /*private void ProcessData()
        {
            if (startscan && indexlaser < Data.Count)
            {

                // Parcourir la liste de données Data
                if (indexlaser >= 0 && indexlaser <= Data.Count)
                {
                    DataPoint dataPoint = Data[indexlaser];




                    if (dataPoint != null)
                    {

                        double dataTimeLaser = dataPoint.NewTime;




                        if (DataTimeprofile != dataTimeLaser)
                        {
                            DataTimeprofile = dataTimeLaser;
                            //Console.WriteLine(jHbm + " "+ Data.Count + " " +hbmData.Count);



                        }



                        bool pointfind = false;
                        do
                        {
                            lock (hbmDataLock)
                            {
                                if (jHbm < hbmData.Count)
                                {
                                    var hbmPoint = hbmData[jHbm];


                                    if (dataTimeLaser == hbmPoint.Item4)
                                    {

                                        dataPoint.Yhbm = hbmPoint.Item1;
                                        pointfind = true;
                                        if (DataTimeprofile != dataTimeLaser)
                                        {

                                            jHbm++;

                                        }


                                    }
                                    else if (jHbm > 0)
                                    {

                                        var hbmPoint1 = hbmData[jHbm - 1];
                                        var hbmPoint2 = hbmData[jHbm];

                                        if (hbmPoint2.Item4 > hbmPoint1.Item4)
                                        {
                                            if (dataTimeLaser < hbmPoint2.Item4)
                                            {

                                                double v = (hbmPoint2.Item1 - hbmPoint1.Item1) / (hbmPoint2.Item4 - hbmPoint1.Item4);
                                                double timeDiff = dataTimeLaser - hbmPoint1.Item4;
                                                dataPoint.Yhbm = hbmPoint1.Item1 + v * timeDiff;
                                                pointfind = true;

                                            }
                                            else
                                            {

                                                jHbm++;


                                            }




                                        }


                                    }
                                    else if (jHbm == 0)
                                    {
                                        jHbm++;


                                    }
                                }
                            }
                        } while (jHbm <= hbmData.Count && pointfind == false);
                    }
                }
               indexlaser++;
                
            }
        }*/





        private void AppendTextToCommandTextBox(string text)
        {
            if (txtCommand.InvokeRequired)
            {
                txtCommand.Invoke((MethodInvoker)delegate
                {
                    txtCommand.AppendText(text + Environment.NewLine);
                });
            }
            else
            {
                txtCommand.AppendText(text + Environment.NewLine);
            }
        }



        private void buttonstart_Click_1(object sender, EventArgs e)
        {
          

            globalth = new Thread(globalthread);
            globalth.Start();
            runglobalth = true;

            startscan = true;

            if(oxisconnected == true)
            {
                stream.ClearMeasurementQueue();
                stream.ClearProfileQueue();
            }
            if (isConnectedhbm == true)
            {

            Signal signalx = _myDevice.Connectors[6].Channels[0].Signals[0];
            _myDevice.ReadSingleMeasurementValue(new List<Signal>() { signalx });
            zhbm = signalx.GetSingleMeasurementValue().Value;

            }

            buttonstart.BackColor = Color.Green;
            stopbutton.BackColor = Color.DimGray;

        }

        private void stopbutton_Click_1(object sender, EventArgs e)
        {
            startscan = false;
            runglobalth = false;
            globalth = null;
            buttonstart.BackColor = Color.DimGray;
            stopbutton.BackColor = Color.Red;


        }
        private BackgroundWorker saveWorker;

        private void savebtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
            saveFileDialog.Title = "Save Data";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                StringBuilder dataBuilder = new StringBuilder();
                for (int i = 0; i < Data.Count; i++)
                {

                    if (checkboxposition.Checked)
                    {

                        string formattedLine = $"{-Data[i].X + Data[i].Yhbm}\t{Data[i].Xhbm}\t{Data[i].Z}\t{Data[i].Ilaser}"
                                    .Replace(',', '.');

                        dataBuilder.AppendLine(formattedLine);

                    }
                    else
                    {

                        string formattedLine = $"{-Data[i].X + Data[i].Xhbm}\t{Data[i].Yhbm}\t{Data[i].Z}\t{Data[i].Ilaser}"
                                                            .Replace(',', '.');

                        dataBuilder.AppendLine(formattedLine);

                    }
                }
                string dataFilePath = Path.ChangeExtension(filePath, "_Data.txt");
                File.WriteAllText(dataFilePath, dataBuilder.ToString());

                MessageBox.Show("Données enregistrées avec succès !");
            }
        }
    





        private void btncalcvitesse_Click(object sender, EventArgs e)
        {
            string distanceText = textBoxDistance.Texts;

            distanceText = distanceText.Replace(',', '.');

            CultureInfo culture = CultureInfo.InvariantCulture;

            if (float.TryParse(distanceText, NumberStyles.Float, culture, out float distance))
            {
                float frequency = Convert.ToSingle(textBoxTime.Texts);
                float time = 1f / frequency;
                float res = distance / time;
                //modernTextBox1.Text = "Vitesse du robot : " + res.ToString("0.00") + " mm/s";
            }
            else
            {
                MessageBox.Show("Valeur de distance invalide. Veuillez saisir un nombre valide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btncalcvitesse_Click_1(object sender, EventArgs e)
        {
            string distanceText = textBoxDistance.Texts;

            distanceText = distanceText.Replace(',', '.');

            CultureInfo culture = CultureInfo.InvariantCulture;

            if (float.TryParse(distanceText, NumberStyles.Float, culture, out float distance))
            {
                float frequency = Convert.ToSingle(textBoxTime.Texts);
                float time = 1f / frequency;
                float res = distance / time;
                textboxResVitesse.Texts = "Vitesse du robot : " + res.ToString("0.00") + " mm/s";
            }
            else
            {
                MessageBox.Show("Valeur de distance invalide. Veuillez saisir un nombre valide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        private _2Dview current2DView = null;
        private _3Dview current3DView = null;


        private void button2D_Click(object sender, EventArgs e)
        {
            if ((WenglorConnected || oxisconnected) && current2DView == null)
            {
                if (current3DView != null)
                {
                    current3DView.Close();
                    current3DView = null;
                }

                current2DView = new _2Dview(this);
                current2DView.TopLevel = false;
                current2DView.Dock = DockStyle.Fill;
                panel3d2d.Controls.Clear();
                panel3d2d.Controls.Add(current2DView);

                current2DView.Show();

                button2D.BackColor = Color.Green;
                button3D.BackColor = Color.MediumSlateBlue;

            }
            else if (!WenglorConnected && !oxisconnected)
            {
                MessageBox.Show("Le laser n'est pas connecté. Veuillez connecter le laser pour ouvrir les paramètres.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button3D_Click(object sender, EventArgs e)
        {
            if (current2DView != null)
            {
                current2DView.Close();
                current2DView = null;
            }

            if (current3DView == null)
            {
                current3DView = new _3Dview(this);
                current3DView.TopLevel = false;
                current3DView.Dock = DockStyle.Fill;

                panel3d2d.Controls.Clear();
                panel3d2d.Controls.Add(current3DView);

                current3DView.Show();

            }
            button2D.BackColor = Color.MediumSlateBlue;
            button3D.BackColor = Color.Green;
        }

        private void envoieconstnate_Click(object sender, EventArgs e)
        {
            if (int.TryParse(valconstante.Texts, out int inputValue))
            {
                Varconstante = inputValue;
            }
            else
            {
                MessageBox.Show("Entrée invalide. S'il vous plait, entrez un nombre valide.");
            }
        }
        public int valueX = 0;
        public int valueY = 0;
        public int valueZ = 0;

        private void textBoxx_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBoxx.Text, out valueX))
                {
                valueX = valueX;


            }
        }

        private void textBoxy_TextChanged(object sender, EventArgs e)
        {

            if (int.TryParse(textBoxy.Text, out valueY))
            {
                valueX = valueY;


            }




        }

        private void textBoxz_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBoxz.Text, out valueZ))
            {
                valueZ = valueZ;


            }
        }

        private void checkboxposition_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkboxposition_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void checkboxposition_CheckedChanged_2(object sender, EventArgs e)
        {
            xflag = true;
        }
    }
}
