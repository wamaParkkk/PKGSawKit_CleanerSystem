using Ajin_motion_driver;
using MsSqlManagerLibrary;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace PKGSawKit_CleanerSystem
{
    public partial class PM2Form : UserControl
    {
        private MaintnanceForm m_Parent;
        int module;
        string ModuleName;

        RecipeSelectForm recipeSelectForm;
        DigitalDlg digitalDlg;

        private Timer logdisplayTimer = new Timer();

        public PM2Form(MaintnanceForm parent)
        {
            m_Parent = parent;

            module = (int)MODULE._PM2;
            ModuleName = "PM2";

            InitializeComponent();            
        }

        private void PM2Form_Load(object sender, EventArgs e)
        {
            Width = 1172;
            Height = 824;
            Top = 0;
            Left = 0;

            logdisplayTimer.Interval = 500;
            logdisplayTimer.Elapsed += new ElapsedEventHandler(Eventlog_Display);
            logdisplayTimer.Start();
        }

        private void SetDoubleBuffered(Control control, bool doubleBuffered = true)
        {
            PropertyInfo propertyInfo = typeof(Control).GetProperty
            (
                "DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            propertyInfo.SetValue(control, doubleBuffered, null);
        }

        public void Display()
        {
            SetDoubleBuffered(Door_Close);
            SetDoubleBuffered(Door_Open);

            // Process seq status
            if (Define.seqMode[module] == Define.MODE_PROCESS)
            {
                if (Define.seqCtrl[module] != Define.CTRL_IDLE)
                {
                    if (btnProcess.Enabled != false)
                        btnProcess.Enabled = false;

                    if (Define.seqCtrl[module] == Define.CTRL_ALARM)
                    {
                        if (btnProcess.BackColor != Color.Red)
                            btnProcess.BackColor = Color.Red;
                        else
                            btnProcess.BackColor = Color.Transparent;

                        if (!btnRetry.Enabled)
                            btnRetry.Enabled = true;
                    }
                    else
                    {
                        if (btnProcess.BackColor != Color.YellowGreen)
                            btnProcess.BackColor = Color.YellowGreen;
                        else
                            btnProcess.BackColor = Color.Transparent;

                        if (label_Alarm.Text != "--")
                            label_Alarm.Text = "--";

                        if (btnRetry.Enabled != false)
                            btnRetry.Enabled = false;
                    }

                    if (!btnAbort.Enabled)
                        btnAbort.Enabled = true;


                    if (btnInit.Enabled != false)
                        btnInit.Enabled = false;

                    if (btnInitStop.Enabled != false)
                        btnInitStop.Enabled = false;

                    if (btnInit.BackColor != Color.Transparent)
                        btnInit.BackColor = Color.Transparent;
                }
            }
            else if (Define.seqMode[module] == Define.MODE_INIT)
            {
                if (Define.seqCtrl[module] != Define.CTRL_IDLE)
                {
                    if (btnInit.Enabled != false)
                        btnInit.Enabled = false;

                    if (Define.seqCtrl[(byte)MODULE._PM2] == Define.CTRL_ALARM)
                    {
                        if (btnInit.BackColor != Color.Red)
                            btnInit.BackColor = Color.Red;
                        else
                            btnInit.BackColor = Color.Transparent;
                    }
                    else
                    {
                        if (btnInit.BackColor != Color.YellowGreen)
                            btnInit.BackColor = Color.YellowGreen;
                        else
                            btnInit.BackColor = Color.Transparent;

                        if (label_Alarm.Text != "--")
                            label_Alarm.Text = "--";
                    }

                    if (!btnInitStop.Enabled)
                        btnInitStop.Enabled = true;


                    if (btnProcess.Enabled != false)
                        btnProcess.Enabled = false;

                    if (btnRetry.Enabled != false)
                        btnRetry.Enabled = false;

                    if (btnAbort.Enabled != false)
                        btnAbort.Enabled = false;

                    if (btnProcess.BackColor != Color.Transparent)
                        btnProcess.BackColor = Color.Transparent;
                }
            }
            else if (Define.seqMode[module] == Define.MODE_IDLE)
            {
                if (!btnProcess.Enabled)
                {
                    btnProcess.Enabled = true;

                    //HostConnection.Host_Set_RunStatus(Global.hostEquipmentInfo, ModuleName, "Idle");
                }

                if (btnProcess.BackColor != Color.Transparent)
                    btnProcess.BackColor = Color.Transparent;

                if (btnRetry.Enabled != false)
                    btnRetry.Enabled = false;

                if (btnAbort.Enabled != false)
                    btnAbort.Enabled = false;

                if (label_Alarm.Text != "--")
                    label_Alarm.Text = "--";

                if (!btnInit.Enabled)
                    btnInit.Enabled = true;

                if (btnInitStop.Enabled != false)
                    btnInitStop.Enabled = false;

                if (btnInit.BackColor != Color.Transparent)
                    btnInit.BackColor = Color.Transparent;
            }

            // Process recipe 정보
            if (Global.prcsInfo.prcsRecipeName[module] != null)
                textBoxRecipeName.Text = Global.prcsInfo.prcsRecipeName[module];

            textBoxStepNum.Text = Global.prcsInfo.prcsCurrentStep[module].ToString() + " / " + Global.prcsInfo.prcsTotalStep[module];

            if (Global.prcsInfo.prcsStepName[module] != null)
                textBoxStepName.Text = Global.prcsInfo.prcsStepName[module];

            textBoxProcessTime.Text = Global.prcsInfo.prcsStepCurrentTime[module].ToString() + " / " + Global.prcsInfo.prcsStepTotalTime[module].ToString();
            textBoxProcessEndTime.Text = Global.prcsInfo.prcsEndTime[module];

            // Input display
            if ((Global.GetDigValue((int)DigInputList.CH2_Door_Op_i) == "Off") &&
                (Global.GetDigValue((int)DigInputList.CH2_Door_Cl_i) == "On"))
            {
                //textBoxDoor.Text = "Close";
                textBoxDoor.BackColor = Color.LightSkyBlue;
                if (Door_Open.Visible != false)
                    Door_Open.Visible = false;

                if (!Door_Close.Visible)
                    Door_Close.Visible = true;
            }
            else if ((Global.GetDigValue((int)DigInputList.CH2_Door_Op_i) == "On") &&
                     (Global.GetDigValue((int)DigInputList.CH2_Door_Cl_i) == "Off"))
            {
                //textBoxDoor.Text = "Open";
                textBoxDoor.BackColor = Color.OrangeRed;
                if (!Door_Open.Visible)
                    Door_Open.Visible = true;

                if (Door_Close.Visible != false)
                    Door_Close.Visible = false;
            }
            else
            {
                //textBoxDoor.Text = "None";
                textBoxDoor.BackColor = Color.WhiteSmoke;
                if (!Door_Open.Visible)
                    Door_Open.Visible = true;

                if (Door_Close.Visible != false)
                    Door_Close.Visible = false;
            }
            
            if (MotionClass.motor[Define.axis_y].sR_HomeStatus == "Home")
            {
                if (PM2NozzleFwdSns.BackColor != Color.Silver)
                    PM2NozzleFwdSns.BackColor = Color.Silver;                

                if (PM2NozzleHomeSns.BackColor != Color.Lime)
                    PM2NozzleHomeSns.BackColor = Color.Lime;
            }
            else if (MotionClass.motor[Define.axis_y].sR_HomeStatus == "+Limit")
            {               
                if (PM2NozzleFwdSns.BackColor != Color.Lime)
                    PM2NozzleFwdSns.BackColor = Color.Lime;

                if (PM2NozzleHomeSns.BackColor != Color.Silver)
                    PM2NozzleHomeSns.BackColor = Color.Silver;
            }            
            else
            {
                if (PM2NozzleFwdSns.BackColor != Color.Silver)
                    PM2NozzleFwdSns.BackColor = Color.Silver;

                if (PM2NozzleHomeSns.BackColor != Color.Silver)
                    PM2NozzleHomeSns.BackColor = Color.Silver;                
            }
            

            // Output display
            if (Global.digSet.curDigSet[(int)DigOutputList.CH2_Door_Close_o] == "On")
                textBoxDoor.Text = "Close";
            else if (Global.digSet.curDigSet[(int)DigOutputList.CH2_Door_Open_o] == "On")
                textBoxDoor.Text = "Open";
            else
                textBoxDoor.Text = "None";            

            if (Global.digSet.curDigSet[(int)DigOutputList.CH2_AirValve_1_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH2_AirValve_1_o] == "On")
                {
                    textBoxAir1.Text = "Open";
                    textBoxAir1.BackColor = Color.LightSkyBlue;

                    if (!PM2Air1.Visible)
                        PM2Air1.Visible = true;
                    else
                        PM2Air1.Visible = false;                    
                }
                else
                {
                    textBoxAir1.Text = "Close";
                    textBoxAir1.BackColor = Color.WhiteSmoke;

                    if (PM2Air1.Visible != false)
                        PM2Air1.Visible = false;                    
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH2_AirValve_2_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH2_AirValve_2_o] == "On")
                {
                    textBoxAir2.Text = "Open";
                    textBoxAir2.BackColor = Color.LightSkyBlue;

                    if (!PM2Air2.Visible)
                        PM2Air2.Visible = true;
                    else
                        PM2Air2.Visible = false;
                }
                else
                {
                    textBoxAir2.Text = "Close";
                    textBoxAir2.BackColor = Color.WhiteSmoke;

                    if (PM2Air2.Visible != false)
                        PM2Air2.Visible = false;
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH2_WaterAirValve_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH2_WaterAirValve_o] == "On")
                {
                    textBoxWaterAir.Text = "Open";
                    textBoxWaterAir.BackColor = Color.LightSkyBlue;

                    if (!PM2WaterAir1.Visible)
                        PM2WaterAir1.Visible = true;
                    else
                        PM2WaterAir1.Visible = false;

                    if (!PM2WaterAir2.Visible)
                        PM2WaterAir2.Visible = true;
                    else
                        PM2WaterAir2.Visible = false;

                    if (!PM2WaterAir3.Visible)
                        PM2WaterAir3.Visible = true;
                    else
                        PM2WaterAir3.Visible = false;

                    if (!PM2WaterAir4.Visible)
                        PM2WaterAir4.Visible = true;
                    else
                        PM2WaterAir4.Visible = false;

                    if (!PM2WaterAir5.Visible)
                        PM2WaterAir5.Visible = true;
                    else
                        PM2WaterAir5.Visible = false;
                }
                else
                {
                    textBoxWaterAir.Text = "Close";
                    textBoxWaterAir.BackColor = Color.WhiteSmoke;

                    if (PM2WaterAir1.Visible != false)
                        PM2WaterAir1.Visible = false;

                    if (PM2WaterAir2.Visible != false)
                        PM2WaterAir2.Visible = false;

                    if (PM2WaterAir3.Visible != false)
                        PM2WaterAir3.Visible = false;

                    if (PM2WaterAir4.Visible != false)
                        PM2WaterAir4.Visible = false;

                    if (PM2WaterAir5.Visible != false)
                        PM2WaterAir5.Visible = false;
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_1_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_1_o] == "On")
                {
                    textBoxWater1.Text = "Open";
                    textBoxWater1.BackColor = Color.LightSkyBlue;

                    if (!PM2Water1.Visible)
                        PM2Water1.Visible = true;
                    else
                        PM2Water1.Visible = false;
                }
                else
                {
                    textBoxWater1.Text = "Close";
                    textBoxWater1.BackColor = Color.WhiteSmoke;

                    if (PM2Water1.Visible != false)
                        PM2Water1.Visible = false;
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_2_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_2_o] == "On")
                {
                    textBoxWater2.Text = "Open";
                    textBoxWater2.BackColor = Color.LightSkyBlue;

                    if (!PM2Water2.Visible)
                        PM2Water2.Visible = true;
                    else
                        PM2Water2.Visible = false;
                }
                else
                {
                    textBoxWater2.Text = "Close";
                    textBoxWater2.BackColor = Color.WhiteSmoke;

                    if (PM2Water2.Visible != false)
                        PM2Water2.Visible = false;
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_3_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_3_o] == "On")
                {
                    textBoxWater3.Text = "Open";
                    textBoxWater3.BackColor = Color.LightSkyBlue;

                    if (!PM2Water3.Visible)
                        PM2Water3.Visible = true;
                    else
                        PM2Water3.Visible = false;
                }
                else
                {
                    textBoxWater3.Text = "Close";
                    textBoxWater3.BackColor = Color.WhiteSmoke;

                    if (PM2Water3.Visible != false)
                        PM2Water3.Visible = false;
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_4_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_4_o] == "On")
                {
                    textBoxWater4.Text = "Open";
                    textBoxWater4.BackColor = Color.LightSkyBlue;

                    if (!PM2Water4.Visible)
                        PM2Water4.Visible = true;
                    else
                        PM2Water4.Visible = false;
                }
                else
                {
                    textBoxWater4.Text = "Close";
                    textBoxWater4.BackColor = Color.WhiteSmoke;

                    if (PM2Water4.Visible != false)
                        PM2Water4.Visible = false;
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_5_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_5_o] == "On")
                {
                    textBoxWater5.Text = "Open";
                    textBoxWater5.BackColor = Color.LightSkyBlue;

                    if (!PM2Water5.Visible)
                        PM2Water5.Visible = true;
                    else
                        PM2Water5.Visible = false;
                }
                else
                {
                    textBoxWater5.Text = "Close";
                    textBoxWater5.BackColor = Color.WhiteSmoke;

                    if (PM2Water5.Visible != false)
                        PM2Water5.Visible = false;
                }
            }
            

            textBoxAxis2Runsts.Text = MotionClass.motor[Define.axis_y].sR_BusyStatus;
            textBoxAxis2Alarm.Text = MotionClass.motor[Define.axis_y].sR_AlarmStatus;
            textBoxAxis2SpeedCur.Text = string.Format("{0:0.0}", MotionClass.motor[Define.axis_y].dR_CmdVelocity);
            textBoxAxis2PositionCur.Text = string.Format("{0:0.000}", MotionClass.motor[Define.axis_y].dR_ActPosition_step);
            
            // Daily count
            textBoxDailyCnt.Text = Define.iPM2DailyCnt.ToString("00");

        }

        private void Eventlog_Display(object sender, ElapsedEventArgs e)
        {
            if (Define.bPM2Event)
            {
                Eventlog_File_Read();
            }

            if (Define.bPM2AlmEvent)
            {
                Alarmlog_File_Read();
            }
        }

        private void Eventlog_File_Read()
        {
            Define.bPM2Event = false;
            
            try
            {
                string sTmpData;

                string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
                string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
                string sDay = string.Format("{0:dd}", DateTime.Now).Trim();
                string FileName = sDay + ".txt";

                if (File.Exists(string.Format("{0}{1}\\{2}\\{3}\\{4}", Global.logfilePath, ModuleName, sYear, sMonth, FileName)))
                {
                    byte[] bytes;
                    using (var fs = File.Open(string.Format("{0}{1}\\{2}\\{3}\\{4}", Global.logfilePath, ModuleName, sYear, sMonth, FileName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        try
                        {
                            bytes = new byte[fs.Length];
                            fs.Read(bytes, 0, (int)fs.Length);
                            sTmpData = Encoding.Default.GetString(bytes);

                            string[] data = sTmpData.Split('\n');
                            int iLength = data.Length;
                            if (iLength >= 2)
                            {
                                string sVal = data[iLength - 2].ToString();

                                Invoke((Action)(() =>
                                {
                                    listBoxEventLog.Update();

                                    if (listBoxEventLog.Items.Count >= 10)
                                        listBoxEventLog.Items.Clear();

                                    listBoxEventLog.Items.Add(sVal);
                                    listBoxEventLog.SelectedIndex = listBoxEventLog.Items.Count - 1;
                                }));
                            }
                        }
                        catch (ArgumentException)
                        {

                        }                        
                    }
                }
            }
            catch (IOException)
            {

            }
        }

        private void Alarmlog_File_Read()
        {
            Define.bPM2AlmEvent = false;

            try
            {
                string sTmpData;

                string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
                string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
                string sDay = string.Format("{0:dd}", DateTime.Now).Trim();
                string FileName = sDay + ".txt";

                if (File.Exists(string.Format("{0}{1}\\{2}\\{3}\\{4}", Global.alarmHistoryPath, ModuleName, sYear, sMonth, FileName)))
                {
                    byte[] bytes;
                    using (var fs = File.Open(string.Format("{0}{1}\\{2}\\{3}\\{4}", Global.alarmHistoryPath, ModuleName, sYear, sMonth, FileName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        try
                        {
                            bytes = new byte[fs.Length];
                            fs.Read(bytes, 0, (int)fs.Length);
                            sTmpData = Encoding.Default.GetString(bytes);

                            string[] data = sTmpData.Split('\n');
                            int iLength = data.Length;
                            if (iLength >= 2)
                            {
                                string sVal = data[iLength - 2].ToString();

                                Invoke((Action)(() =>
                                {
                                    label_Alarm.Text = sVal;
                                }));
                            }
                        }
                        catch (ArgumentException)
                        {

                        }                        
                    }
                }
            }
            catch (IOException)
            {

            }
        }

        private void Digital_Click(object sender, EventArgs e)
        {
            if (!Define.bInterlockRelease)
            {
                if ((Define.seqCtrl[module] != Define.CTRL_IDLE) ||
                    (Define.seqCylinderCtrl[module] != Define.CTRL_IDLE))
                {
                    MessageBox.Show("공정 진행 중입니다", "알림");
                    return;
                }
            }

            TextBox btn = (TextBox)sender;
            digitalDlg = new DigitalDlg();

            string strTmp = btn.Tag.ToString();
            switch (strTmp)
            {
                case "16":
                    {
                        digitalDlg.Init2("None", "Close", "Open", "CH2 Cover Door");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "None")
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_Door_Open_o, (uint)DigitalOffOn.Off, ModuleName);
                                Global.SetDigValue((int)DigOutputList.CH2_Door_Close_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_Door_Open_o, (uint)DigitalOffOn.Off, ModuleName);
                                Global.SetDigValue((int)DigOutputList.CH2_Door_Close_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                            else if (digitalDlg.m_strResult == "Open")
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_Door_Open_o, (uint)DigitalOffOn.On, ModuleName);
                                Global.SetDigValue((int)DigOutputList.CH2_Door_Close_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                        }
                    }
                    break;

                case "18":
                    {
                        digitalDlg.Init("Close", "Open", "CH2 Air Valve#1");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_AirValve_1_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_AirValve_1_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                        }
                    }
                    break;

                case "19":
                    {
                        digitalDlg.Init("Close", "Open", "CH2 Air Valve#2");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_AirValve_2_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_AirValve_2_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                        }
                    }
                    break;

                case "20":
                    {
                        digitalDlg.Init("Close", "Open", "CH2 Water Valve#1");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_WaterValve_1_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_WaterValve_1_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                        }
                    }
                    break;

                case "21":
                    {
                        digitalDlg.Init("Close", "Open", "CH2 Water Valve#2");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_WaterValve_2_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_WaterValve_2_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                        }
                    }
                    break;

                case "22":
                    {
                        digitalDlg.Init("Close", "Open", "CH2 Water Valve#3");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_WaterValve_3_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_WaterValve_3_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                        }
                    }
                    break;

                case "23":
                    {
                        digitalDlg.Init("Close", "Open", "CH2 Water Valve#4");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_WaterValve_4_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_WaterValve_4_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                        }
                    }
                    break;

                case "24":
                    {
                        digitalDlg.Init("Close", "Open", "CH2 Water Valve#5");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_WaterValve_5_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_WaterValve_5_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                        }
                    }
                    break;

                case "25":
                    {
                        digitalDlg.Init("Close", "Open", "CH2 Water Air Valve");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_WaterAirValve_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else
                            {
                                Global.SetDigValue((int)DigOutputList.CH2_WaterAirValve_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                        }
                    }
                    break;
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            string strTmp = btn.Text.ToString();
            switch (strTmp)
            {
                case "Start":
                    {
                        if (!Define.bInterlockRelease)
                        {
                            if (Global.GetDigValue((int)DigInputList.Front_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Front door가 열려 있습니다", "알림");
                                return;
                            }

                            if (Global.GetDigValue((int)DigInputList.Left_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Left door가 열려 있습니다", "알림");
                                return;
                            }

                            if (Global.GetDigValue((int)DigInputList.Right_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Right door가 열려 있습니다", "알림");
                                return;
                            }

                            if (Global.GetDigValue((int)DigInputList.Back_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Back door가 열려 있습니다", "알림");
                                return;
                            }
                        }

                        if (MessageBox.Show("공정을 진행 하겠습니까?", "알림", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            Define.iSelectRecipeModule = module;

                            recipeSelectForm = new RecipeSelectForm();

                            if (recipeSelectForm.ShowDialog() == DialogResult.OK)
                            {
                                Define.seqMode[module] = Define.MODE_PROCESS;
                                Define.seqCtrl[module] = Define.CTRL_RUN;
                                Define.seqSts[module] = Define.STS_IDLE;
                            }
                        }
                    }
                    break;

                case "Retry":
                    {
                        if (!Define.bInterlockRelease)
                        {
                            if (Global.GetDigValue((int)DigInputList.Front_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Front door가 열려 있습니다", "알림");
                                return;
                            }

                            if (Global.GetDigValue((int)DigInputList.Left_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Left door가 열려 있습니다", "알림");
                                return;
                            }

                            if (Global.GetDigValue((int)DigInputList.Right_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Right door가 열려 있습니다", "알림");
                                return;
                            }

                            if (Global.GetDigValue((int)DigInputList.Back_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Back door가 열려 있습니다", "알림");
                                return;
                            }
                        }

                        Define.seqCtrl[module] = Define.CTRL_RETRY;
                    }
                    break;

                case "Stop":
                    {
                        if (MessageBox.Show("공정을 중지하겠습니까?", "알림", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            Define.seqCtrl[module] = Define.CTRL_ABORT;
                        }
                    }
                    break;
            }
        }
        
        private void btnInit_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            string strTmp = btn.Text.ToString();
            switch (strTmp)
            {
                case "Init":
                    {
                        if (!Define.bInterlockRelease)
                        {
                            if (Global.GetDigValue((int)DigInputList.Front_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Front door가 열려 있습니다", "알림");
                                return;
                            }

                            if (Global.GetDigValue((int)DigInputList.Left_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Left door가 열려 있습니다", "알림");
                                return;
                            }

                            if (Global.GetDigValue((int)DigInputList.Right_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Right door가 열려 있습니다", "알림");
                                return;
                            }

                            if (Global.GetDigValue((int)DigInputList.Back_Door_Sensor_i) == "Off")
                            {
                                MessageBox.Show("Back door가 열려 있습니다", "알림");
                                return;
                            }
                        }

                        Define.seqMode[module] = Define.MODE_INIT;
                        Define.seqCtrl[module] = Define.CTRL_RUN;
                        Define.seqSts[module] = Define.STS_IDLE;
                    }
                    break;

                case "Stop":
                    {
                        Define.seqCtrl[module] = Define.CTRL_ABORT;
                    }
                    break;
            }
        }
    }
}
