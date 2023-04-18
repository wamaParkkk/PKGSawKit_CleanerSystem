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
    public partial class PM1Form : UserControl
    {
        private MaintnanceForm m_Parent;
        int module;
        string ModuleName;

        RecipeSelectForm recipeSelectForm;
        DigitalDlg digitalDlg;

        private Timer logdisplayTimer = new Timer();        

        public PM1Form(MaintnanceForm parent)
        {
            m_Parent = parent;

            module = (int)MODULE._PM1;
            ModuleName = "PM1";

            InitializeComponent();         
        }

        private void PM1Form_Load(object sender, EventArgs e)
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

                    if (Define.seqCtrl[(byte)MODULE._PM1] == Define.CTRL_ALARM)
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

            if (Define.seqBrushFwBwMode == Define.MODE_BRUSH_FWBW_CLEAN)
            {
                if (Define.seqBrushFwBwCtrl != Define.CTRL_IDLE)
                {
                    if (!labelBrushCleaning.Visible)
                        labelBrushCleaning.Visible = true;
                    else
                        labelBrushCleaning.Visible = false;
                }
            }
            else
            {
                if (labelBrushCleaning.Visible != false)
                    labelBrushCleaning.Visible = false;
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
            if ((Global.GetDigValue((int)DigInputList.CH1_Door_Op_i) == "Off") &&
                (Global.GetDigValue((int)DigInputList.CH1_Door_Cl_i) == "On"))
            {
                //textBoxDoor.Text = "Close";
                textBoxDoor.BackColor = Color.LightSkyBlue;
                if (Door_Open.Visible != false)
                    Door_Open.Visible = false;

                if (!Door_Close.Visible)
                    Door_Close.Visible = true;
            }
            else if ((Global.GetDigValue((int)DigInputList.CH1_Door_Op_i) == "On") &&
                     (Global.GetDigValue((int)DigInputList.CH1_Door_Cl_i) == "Off"))
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

            if ((Global.GetDigValue((int)DigInputList.CH1_Brush_Fwd_i) == "Off") &&
                (Global.GetDigValue((int)DigInputList.CH1_Brush_Bwd_i) == "On") &&
                (Global.GetDigValue((int)DigInputList.CH1_Brush_Home_i) == "On"))
            {
                if (PM1BrushFwdSns.BackColor != Color.Lime)
                    PM1BrushFwdSns.BackColor = Color.Lime;

                if (PM1BrushBwdSns.BackColor != Color.Silver)
                    PM1BrushBwdSns.BackColor = Color.Silver;

                if (PM1BrushHomeSns.BackColor != Color.Silver)
                    PM1BrushHomeSns.BackColor = Color.Silver;
            }
            else if ((Global.GetDigValue((int)DigInputList.CH1_Brush_Fwd_i) == "On") &&
                     (Global.GetDigValue((int)DigInputList.CH1_Brush_Bwd_i) == "Off") &&
                     (Global.GetDigValue((int)DigInputList.CH1_Brush_Home_i) == "On"))
            {
                if (PM1BrushFwdSns.BackColor != Color.Silver)
                    PM1BrushFwdSns.BackColor = Color.Silver;

                if (PM1BrushBwdSns.BackColor != Color.Lime)
                    PM1BrushBwdSns.BackColor = Color.Lime;

                if (PM1BrushHomeSns.BackColor != Color.Silver)
                    PM1BrushHomeSns.BackColor = Color.Silver;
            }
            else if ((Global.GetDigValue((int)DigInputList.CH1_Brush_Fwd_i) == "On") &&
                     (Global.GetDigValue((int)DigInputList.CH1_Brush_Bwd_i) == "On") &&
                     (Global.GetDigValue((int)DigInputList.CH1_Brush_Home_i) == "Off"))
            {
                if (PM1BrushFwdSns.BackColor != Color.Silver)
                    PM1BrushFwdSns.BackColor = Color.Silver;

                if (PM1BrushBwdSns.BackColor != Color.Silver)
                    PM1BrushBwdSns.BackColor = Color.Silver;

                if (PM1BrushHomeSns.BackColor != Color.Lime)
                    PM1BrushHomeSns.BackColor = Color.Lime;
            }
            else
            {
                if (PM1BrushFwdSns.BackColor != Color.Silver)
                    PM1BrushFwdSns.BackColor = Color.Silver;

                if (PM1BrushBwdSns.BackColor != Color.Silver)
                    PM1BrushBwdSns.BackColor = Color.Silver;

                if (PM1BrushHomeSns.BackColor != Color.Silver)
                    PM1BrushHomeSns.BackColor = Color.Silver;
            }

            if ((Global.GetDigValue((int)DigInputList.CH1_Nozzle_Fwd_i) == "Off") &&
                (Global.GetDigValue((int)DigInputList.CH1_Nozzle_Bwd_i) == "On"))                
            {
                if (PM1NozzleFwdSns.BackColor != Color.Lime)
                    PM1NozzleFwdSns.BackColor = Color.Lime;

                if (PM1NozzleBwdSns.BackColor != Color.Silver)
                    PM1NozzleBwdSns.BackColor = Color.Silver;                
            }
            else if ((Global.GetDigValue((int)DigInputList.CH1_Nozzle_Fwd_i) == "On") &&
                     (Global.GetDigValue((int)DigInputList.CH1_Nozzle_Bwd_i) == "Off"))
            {
                if (PM1NozzleFwdSns.BackColor != Color.Silver)
                    PM1NozzleFwdSns.BackColor = Color.Silver;

                if (PM1NozzleBwdSns.BackColor != Color.Lime)
                    PM1NozzleBwdSns.BackColor = Color.Lime;                
            }            
            else
            {
                if (PM1NozzleFwdSns.BackColor != Color.Silver)
                    PM1NozzleFwdSns.BackColor = Color.Silver;

                if (PM1NozzleBwdSns.BackColor != Color.Silver)
                    PM1NozzleBwdSns.BackColor = Color.Silver;                
            }

            if (MotionClass.motor[Define.axis_z].sR_HomeStatus == "Home")
            {
                if (PM1BrushUpDnDownSns.BackColor != Color.Silver)
                    PM1BrushUpDnDownSns.BackColor = Color.Silver;

                if (PM1BrushUpDnHomeSns.BackColor != Color.Lime)
                    PM1BrushUpDnHomeSns.BackColor = Color.Lime;
            }
            else if (MotionClass.motor[Define.axis_z].sR_HomeStatus == "+Limit")
            {
                if (PM1BrushUpDnDownSns.BackColor != Color.Lime)
                    PM1BrushUpDnDownSns.BackColor = Color.Lime;

                if (PM1BrushUpDnHomeSns.BackColor != Color.Silver)
                    PM1BrushUpDnHomeSns.BackColor = Color.Silver;
            }
            else
            {
                if (PM1BrushUpDnDownSns.BackColor != Color.Silver)
                    PM1BrushUpDnDownSns.BackColor = Color.Silver;

                if (PM1BrushUpDnHomeSns.BackColor != Color.Silver)
                    PM1BrushUpDnHomeSns.BackColor = Color.Silver;
            }

            if ((MotionClass.motor[Define.axis_r].sR_BusyStatus) == "Moving")
            {
                if (!PM1BrushRotate1.Visible)
                    PM1BrushRotate1.Visible = true;
                else
                    PM1BrushRotate1.Visible = false;

                if (!PM1BrushRotate2.Visible)
                    PM1BrushRotate2.Visible = true;
                else
                    PM1BrushRotate2.Visible = false;
            }
            else
            {
                if (PM1BrushRotate1.Visible != false)
                    PM1BrushRotate1.Visible = false;

                if (PM1BrushRotate2.Visible != false)
                    PM1BrushRotate2.Visible = false;
            }

            // Output display
            if (Global.digSet.curDigSet[(int)DigOutputList.CH1_Door_Close_o] == "On")
                textBoxDoor.Text = "Close";
            else if (Global.digSet.curDigSet[(int)DigOutputList.CH1_Door_Open_o] == "On")
                textBoxDoor.Text = "Open";
            else
                textBoxDoor.Text = "None";

            if (Global.digSet.curDigSet[(int)DigOutputList.CH1_Brush_Fwd_o] == "On")
                textBoxBrushFwdBwd.Text = "Forward";
            else if (Global.digSet.curDigSet[(int)DigOutputList.CH1_Brush_Bwd_o] == "On")
                textBoxBrushFwdBwd.Text = "Backward";
            else
                textBoxBrushFwdBwd.Text = "None";

            if (Global.digSet.curDigSet[(int)DigOutputList.CH1_Nozzle_Fwd_o] == "On")
                textBoxNozzleFwdBwd.Text = "Forward";
            else if (Global.digSet.curDigSet[(int)DigOutputList.CH1_Nozzle_Bwd_o] == "On")
                textBoxNozzleFwdBwd.Text = "Backward";
            else
                textBoxNozzleFwdBwd.Text = "None";            

            if (Global.digSet.curDigSet[(int)DigOutputList.CH1_AirValve_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH1_AirValve_o] == "On")
                {
                    textBoxAir1.Text = "Open";
                    textBoxAir1.BackColor = Color.LightSkyBlue;

                    if (!PM1Air1.Visible)
                        PM1Air1.Visible = true;
                    else
                        PM1Air1.Visible = false;

                    if (!PM1Air2.Visible)
                        PM1Air2.Visible = true;
                    else
                        PM1Air2.Visible = false;
                }
                else
                {
                    textBoxAir1.Text = "Close";
                    textBoxAir1.BackColor = Color.WhiteSmoke;

                    if (PM1Air1.Visible != false)
                        PM1Air1.Visible = false;

                    if (PM1Air2.Visible != false)
                        PM1Air2.Visible = false;
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH1_BrushClean_AirValve_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH1_BrushClean_AirValve_o] == "On")
                {
                    textBoxBrushCleanAir.Text = "Open";
                    textBoxBrushCleanAir.BackColor = Color.LightSkyBlue;                    
                }
                else
                {
                    textBoxBrushCleanAir.Text = "Close";
                    textBoxBrushCleanAir.BackColor = Color.WhiteSmoke;                    
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH1_WaterValve_1_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH1_WaterValve_1_o] == "On")
                {
                    textBoxWater1.Text = "Open";
                    textBoxWater1.BackColor = Color.LightSkyBlue;

                    if (!PM1Water1_1.Visible)
                        PM1Water1_1.Visible = true;
                    else
                        PM1Water1_1.Visible = false;

                    if (!PM1Water1_2.Visible)
                        PM1Water1_2.Visible = true;
                    else
                        PM1Water1_2.Visible = false;
                }
                else
                {
                    textBoxWater1.Text = "Close";
                    textBoxWater1.BackColor = Color.WhiteSmoke;

                    if (PM1Water1_1.Visible != false)
                        PM1Water1_1.Visible = false;

                    if (PM1Water1_2.Visible != false)
                        PM1Water1_2.Visible = false;
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH1_WaterValve_2_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH1_WaterValve_2_o] == "On")
                {
                    textBoxWater2.Text = "Open";
                    textBoxWater2.BackColor = Color.LightSkyBlue;

                    if (!PM1Water2_1.Visible)
                        PM1Water2_1.Visible = true;
                    else
                        PM1Water2_1.Visible = false;

                    if (!PM1Water2_2.Visible)
                        PM1Water2_2.Visible = true;
                    else
                        PM1Water2_2.Visible = false;
                }
                else
                {
                    textBoxWater2.Text = "Close";
                    textBoxWater2.BackColor = Color.WhiteSmoke;

                    if (PM1Water2_1.Visible != false)
                        PM1Water2_1.Visible = false;

                    if (PM1Water2_2.Visible != false)
                        PM1Water2_2.Visible = false;
                }
            }

            if (Global.digSet.curDigSet[(int)DigOutputList.CH1_BrushClean_WaterValve_o] != null)
            {
                if (Global.digSet.curDigSet[(int)DigOutputList.CH1_BrushClean_WaterValve_o] == "On")
                {
                    textBoxBrushCleanWater.Text = "Open";
                    textBoxBrushCleanWater.BackColor = Color.LightSkyBlue;                   
                }
                else
                {
                    textBoxBrushCleanWater.Text = "Close";
                    textBoxBrushCleanWater.BackColor = Color.WhiteSmoke;                    
                }
            }
           

            textBoxAxis0Runsts.Text = MotionClass.motor[Define.axis_z].sR_BusyStatus;            
            textBoxAxis0SpeedCur.Text = string.Format("{0:0.0}", MotionClass.motor[Define.axis_z].dR_CmdVelocity);            

            textBoxAxis1Runsts.Text = MotionClass.motor[Define.axis_r].sR_BusyStatus;            
            textBoxAxis1SpeedCur.Text = string.Format("{0:0.0}", MotionClass.motor[Define.axis_r].dR_CmdVelocity);
            

            // Daily count
            textBoxDailyCnt.Text = Define.iPM1DailyCnt.ToString("00");            
        }

        private void Eventlog_Display(object sender, ElapsedEventArgs e)
        {
            if (Define.bPM1Event)
            {                
                Eventlog_File_Read();                
            }

            if (Define.bPM1AlmEvent)
            {
                Alarmlog_File_Read();
            }
        }

        private void Eventlog_File_Read()
        {
            Define.bPM1Event = false;
            
            try
            {
                string sTmpData;

                string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
                string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
                string sDay = string.Format("{0:dd}", DateTime.Now).Trim();
                string FileName = string.Format("{0}.txt", sDay);                

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
            Define.bPM1AlmEvent = false;

            try
            {
                string sTmpData;

                string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
                string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
                string sDay = string.Format("{0:dd}", DateTime.Now).Trim();
                string FileName = string.Format("{0}.txt", sDay);                

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
                case "0":
                    {
                        digitalDlg.Init2("None", "Close", "Open", "CH1 Cover Door");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "None")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_Door_Open_o, (uint)DigitalOffOn.Off, ModuleName);
                                Global.SetDigValue((int)DigOutputList.CH1_Door_Close_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_Door_Open_o, (uint)DigitalOffOn.Off, ModuleName);
                                Global.SetDigValue((int)DigOutputList.CH1_Door_Close_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                            else if (digitalDlg.m_strResult == "Open")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_Door_Open_o, (uint)DigitalOffOn.On, ModuleName);
                                Global.SetDigValue((int)DigOutputList.CH1_Door_Close_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                        }
                    }
                    break;

                case "2":
                    {
                        digitalDlg.Init("Close", "Open", "CH1 Air Valve");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_AirValve_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_AirValve_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                        }
                    }
                    break;

                case "3":
                    {
                        digitalDlg.Init("Close", "Open", "CH1 Water Valve#1");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_WaterValve_1_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_WaterValve_1_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                        }
                    }
                    break;

                case "4":
                    {
                        digitalDlg.Init("Close", "Open", "CH1 Water Valve#2");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_WaterValve_2_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_WaterValve_2_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                        }
                    }
                    break;

                case "5":
                    {
                        digitalDlg.Init("Close", "Open", "CH1 Brush Clean Water Valve");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_BrushClean_WaterValve_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_BrushClean_WaterValve_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                        }
                    }
                    break;

                case "6":
                    {
                        digitalDlg.Init("Close", "Open", "CH1 Brush Clean Air Valve");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "Close")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_BrushClean_AirValve_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_BrushClean_AirValve_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                        }
                    }
                    break;

                case "8":
                    {
                        digitalDlg.Init2("None", "Backward", "Forward", "Nozzle Fwd/Bwd");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "None")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_Nozzle_Fwd_o, (uint)DigitalOffOn.Off, ModuleName);
                                Global.SetDigValue((int)DigOutputList.CH1_Nozzle_Bwd_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else if (digitalDlg.m_strResult == "Backward")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_Nozzle_Pwr_o, (uint)DigitalOffOn.On, ModuleName);

                                Global.SetDigValue((int)DigOutputList.CH1_Nozzle_Fwd_o, (uint)DigitalOffOn.Off, ModuleName);
                                Global.SetDigValue((int)DigOutputList.CH1_Nozzle_Bwd_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                            else if (digitalDlg.m_strResult == "Forward")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_Nozzle_Pwr_o, (uint)DigitalOffOn.On, ModuleName);

                                Global.SetDigValue((int)DigOutputList.CH1_Nozzle_Fwd_o, (uint)DigitalOffOn.On, ModuleName);
                                Global.SetDigValue((int)DigOutputList.CH1_Nozzle_Bwd_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                        }
                    }
                    break;

                case "10":
                    {
                        digitalDlg.Init2("None", "Backward", "Forward", "Brush Fwd/Bwd");
                        if (digitalDlg.ShowDialog() == DialogResult.OK)
                        {
                            if (digitalDlg.m_strResult == "None")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_Brush_Fwd_o, (uint)DigitalOffOn.Off, ModuleName);
                                Global.SetDigValue((int)DigOutputList.CH1_Brush_Bwd_o, (uint)DigitalOffOn.Off, ModuleName);
                            }
                            else if (digitalDlg.m_strResult == "Backward")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_Brush_Pwr_o, (uint)DigitalOffOn.On, ModuleName);

                                Global.SetDigValue((int)DigOutputList.CH1_Brush_Fwd_o, (uint)DigitalOffOn.Off, ModuleName);
                                Global.SetDigValue((int)DigOutputList.CH1_Brush_Bwd_o, (uint)DigitalOffOn.On, ModuleName);
                            }
                            else if (digitalDlg.m_strResult == "Forward")
                            {
                                Global.SetDigValue((int)DigOutputList.CH1_Brush_Pwr_o, (uint)DigitalOffOn.On, ModuleName);

                                Global.SetDigValue((int)DigOutputList.CH1_Brush_Fwd_o, (uint)DigitalOffOn.On, ModuleName);
                                Global.SetDigValue((int)DigOutputList.CH1_Brush_Bwd_o, (uint)DigitalOffOn.Off, ModuleName);
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
