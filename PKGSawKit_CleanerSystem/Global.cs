﻿using Ajin_IO_driver;
using MsSqlManagerLibrary;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace PKGSawKit_CleanerSystem
{
    public struct TStep
    {
        public bool Flag;
        public byte Layer;
        public double Times;

        public void INC_TIMES()
        {
            Times++;
            Thread.Sleep(990);
        }

        public void INC_TIMES_10()
        {
            Times += 0.01;
        }

        public void INC_TIMES_100()
        {
            Times += 0.1;
        }
    }

    public class TBaseThread
    {
        public byte module;
        public string ModuleName;

        public TStep step;
    }

    class Global
    {
        public static string userdataPath = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\UserData.accdb"));
        public static string logfilePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\EventLog\"));
        public static string alarmHistoryPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\AlarmHistory\"));
        public static string RecipeFilePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\Recipes\"));
        public static string ConfigurePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\Configure\"));
        public static string serialPortInfoPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\"));
        public static string dailyCntfilePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\DailyCount\"));

        public static string hostEquipmentInfo = "K5EE_PKGsawCleaningSystem";
        public static string hostEquipmentInfo_Log = "K5EE_PKGsawCleaningSystemLog";

        private static Timer timer = new Timer();

        public static TDigSet digSet;
        public static TPrcsInfo prcsInfo;

        private static InterlockDisplayForm interlockDisplayForm;
        private static uint nSeqWaitCnt = 0;

        static string sendMsg_System = "Idle";

        #region 이벤트로그 파일 폴더 및 파일 생성       
        public static void EventLog(string Msg, string moduleName, string Mode)
        {
            string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
            string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
            string sDay = string.Format("{0:dd}", DateTime.Now).Trim();            
            string sDate = string.Format("{0}-{1}-{2}", sYear, sMonth, sDay);
            string sTime = DateTime.Now.ToString("HH:mm:ss");
            string sDateTime;            
            sDateTime = string.Format("[{0}, {1}] ", sDate, sTime);
            
            WriteFile(string.Format("{0}{1}", sDateTime, Msg), moduleName, Mode);

            if (Mode == "Event")
            {
                if (moduleName == "PM1")
                {
                    Define.bPM1Event = true;
                }

                if (moduleName == "PM2")
                {
                    Define.bPM2Event = true;
                }                
            }
            else if (Mode == "Alarm")
            {
                if (moduleName == "PM1")
                {
                    Define.bPM1OpAlmEvent = true;
                    Define.bPM1AlmEvent = true;
                }

                if (moduleName == "PM2")
                {
                    Define.bPM2OpAlmEvent = true;
                    Define.bPM2AlmEvent = true;
                }                
            }            
        }

        private static void WriteFile(string Msg, string moduleName, string Mode)
        {            
            string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
            string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
            string sDay = string.Format("{0:dd}", DateTime.Now).Trim();            
            string FileName = string.Format("{0}.txt", sDay);
            string sPath = string.Empty;
            if (Mode == "Event")
            {
                sPath = logfilePath;
            }                
            else if (Mode == "Alarm")
            {
                sPath = alarmHistoryPath;
            }

            try
            {                
                if (!Directory.Exists(string.Format("{0}{1}\\{2}", sPath, moduleName, sYear)))
                {
                    CreateYearFolder(string.Format("{0}{1}", sPath, moduleName));
                }
                
                if (!Directory.Exists(string.Format("{0}{1}\\{2}\\{3}", sPath, moduleName, sYear, sMonth)))
                {
                    CreateMonthFolder(string.Format("{0}{1}", sPath, moduleName));
                }
                
                if (File.Exists(string.Format("{0}{1}\\{2}\\{3}\\{4}", sPath, moduleName, sYear, sMonth, FileName)))
                {
                    StreamWriter writer;                    
                    writer = File.AppendText(string.Format("{0}{1}\\{2}\\{3}\\{4}", sPath, moduleName, sYear, sMonth, FileName));
                    writer.WriteLine(Msg);
                    writer.Close();
                }
                else
                {                    
                    CreateFile(string.Format("{0}{1}", sPath, moduleName), Msg);

                    StreamWriter writer;                    
                    writer = File.AppendText(string.Format("{0}{1}\\{2}\\{3}\\{4}", sPath, moduleName, sYear, sMonth, FileName));
                    writer.WriteLine(Msg);
                    writer.Close();
                }
            }
            catch (IOException)
            {
                
            }
        }

        private static void CreateYearFolder(string Path)
        {
            string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
            string FolderName = sYear;
            
            Directory.CreateDirectory(string.Format("{0}\\{1}", Path, FolderName));
        }

        private static void CreateMonthFolder(string Path)
        {
            string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
            string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
            string FolderName = sMonth;
            
            Directory.CreateDirectory(string.Format("{0}\\{1}\\{2}", Path, sYear, FolderName));
        }

        private static void CreateFile(string Path, string Msg)
        {
            StreamWriter writer;

            string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
            string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
            string sDay = string.Format("{0:dd}", DateTime.Now).Trim();            
            string FileName = string.Format("{0}.txt", sDay);
            
            if (!File.Exists(string.Format("{0}\\{1}\\{2}\\{3}", Path, sYear, sMonth, FileName)))
            {                
                using (File.Create(string.Format("{0}\\{1}\\{2}\\{3}", Path, sYear, sMonth, FileName))) ;
            }
        }
        #endregion

        #region Daily count 폴더 및 파일 생성
        public static void DailyLog(int iCnt, string moduleName)
        {
            string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
            string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
            string sDay = string.Format("{0:dd}", DateTime.Now).Trim();
            string FileName = string.Format("{0}.txt", sDay);
            string sPath = dailyCntfilePath;

            try
            {
                if (!Directory.Exists(string.Format("{0}{1}\\{2}", sPath, moduleName, sYear)))                
                {
                    CreateYearFolder(string.Format("{0}{1}", sPath, moduleName));                    
                }

                if (!Directory.Exists(string.Format("{0}{1}\\{2}\\{3}", sPath, moduleName, sYear, sMonth)))                
                {
                    CreateMonthFolder(string.Format("{0}{1}", sPath, moduleName));                    
                }

                if (File.Exists(string.Format("{0}{1}\\{2}\\{3}\\{4}", sPath, moduleName, sYear, sMonth, FileName)))                
                {
                    StreamWriter writer;
                    writer = File.CreateText(string.Format("{0}{1}\\{2}\\{3}\\{4}", sPath, moduleName, sYear, sMonth, FileName));
                    writer.Write(iCnt);
                    writer.Close();
                }
                else
                {
                    CreateFile(sPath + moduleName, "");

                    StreamWriter writer;                    
                    writer = File.CreateText(string.Format("{0}{1}\\{2}\\{3}\\{4}", sPath, moduleName, sYear, sMonth, FileName));
                    writer.Write(iCnt);
                    writer.Close();
                }
            }
            catch (IOException)
            {

            }
        }
        #endregion

        public static void Init()
        {
            digSet.curDigSet = new string[64];
            for (int i = 0; i < 64; i++)
            {
                digSet.curDigSet[i] = DIOClass.doVal.readDigOut[i];
            }

            prcsInfo.prcsRecipeName = new string[Define.MODULE_MAX];
            prcsInfo.prcsCurrentStep = new int[Define.MODULE_MAX];
            prcsInfo.prcsTotalStep = new int[Define.MODULE_MAX];
            prcsInfo.prcsStepName = new string[Define.MODULE_MAX];
            prcsInfo.prcsStepCurrentTime = new double[Define.MODULE_MAX];
            prcsInfo.prcsStepTotalTime = new double[Define.MODULE_MAX];
            prcsInfo.prcsEndTime = new string[Define.MODULE_MAX];

            for (int nModuleCnt = 0; nModuleCnt < Define.MODULE_MAX; nModuleCnt++)
            {
                prcsInfo.prcsRecipeName[nModuleCnt] = string.Empty;
                prcsInfo.prcsCurrentStep[nModuleCnt] = 0;
                prcsInfo.prcsTotalStep[nModuleCnt] = 0;
                prcsInfo.prcsStepName[nModuleCnt] = string.Empty;
                prcsInfo.prcsStepCurrentTime[nModuleCnt] = 1;
                prcsInfo.prcsStepTotalTime[nModuleCnt] = 0;
                prcsInfo.prcsEndTime[nModuleCnt] = string.Empty;
            }

            interlockDisplayForm = new InterlockDisplayForm();

            timer.Interval = 100;
            timer.Elapsed += new ElapsedEventHandler(VALUE_INTERLOCK_CHECK);
            timer.Start();

            /*
            string strRtn = HostConnection.Connect();
            if (strRtn == "OK")
            {
                HostConnection.Host_Set_SystemStatus(hostEquipmentInfo, "System", "Idle");                

                HostConnection.Host_Set_RunStatus(hostEquipmentInfo, "PM1", "Idle");
                HostConnection.Host_Set_RunStatus(hostEquipmentInfo, "PM2", "Idle");                

                HostConnection.Host_Set_RecipeName(hostEquipmentInfo, "PM1", "");
                HostConnection.Host_Set_RecipeName(hostEquipmentInfo, "PM2", "");                

                HostConnection.Host_Set_AlarmName(hostEquipmentInfo, "PM1", "");
                HostConnection.Host_Set_AlarmName(hostEquipmentInfo, "PM2", "");                

                HostConnection.Host_Set_ProgressTime(hostEquipmentInfo, "PM1", "0/0");
                HostConnection.Host_Set_ProgressTime(hostEquipmentInfo, "PM2", "0/0");                

                HostConnection.Host_Set_ProcessEndTime(hostEquipmentInfo, "PM1", "");
                HostConnection.Host_Set_ProcessEndTime(hostEquipmentInfo, "PM2", "");                                                

                GetDailyLogCount("PM1");
                GetDailyLogCount("PM2");
            }
            else
            {
                MessageBox.Show("EE 서버 접속에 실패했습니다", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } 
            */
        }

        public static void GetDailyLogCount(string moduleName)
        {
            string sTmpData;
            string sYear = string.Format("{0:yyyy}", DateTime.Now).Trim();
            string sMonth = string.Format("{0:MM}", DateTime.Now).Trim();
            string sDay = string.Format("{0:dd}", DateTime.Now).Trim();            
            string FileName = string.Format("{0}.txt", sDay);            
            string sPath = string.Format("{0}{1}\\{2}\\{3}\\{4}", dailyCntfilePath, moduleName, sYear, sMonth, FileName);

            if (File.Exists(sPath))
            {
                byte[] bytes;
                using (var fs = File.Open(sPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, (int)fs.Length);
                    sTmpData = Encoding.Default.GetString(bytes);

                    char sp = ',';
                    string[] spString = sTmpData.Split(sp);
                    for (int i = 0; i < spString.Length; i++)
                    {
                        if (moduleName == "PM1")
                        {
                            Define.iPM1DailyCnt = int.Parse(spString[0]);

                            //HostConnection.Host_Set_DailyCount(hostEquipmentInfo, "PM1", Define.iPM1DailyCnt.ToString("00"));
                        }
                        else if (moduleName == "PM2")
                        {
                            Define.iPM2DailyCnt = int.Parse(spString[0]);

                            //HostConnection.Host_Set_DailyCount(hostEquipmentInfo, "PM2", Define.iPM2DailyCnt.ToString("00"));
                        }                        
                    }
                }
            }
        }

        public static string GetDigValue(int ioName)
        {
            try
            {
                if ((0 <= ioName) && (15 >= ioName))
                {
                    if (DIOClass.diVal.checkHigh[ioName] != null)
                    {
                        return DIOClass.diVal.checkHigh[ioName];
                    }
                    else
                    {
                        return "Off";
                    }
                }
                else if ((16 <= ioName) && (32 >= ioName))
                {
                    if (DIOClass.diVal.checkLow[ioName - 16] != null)
                    {
                        return DIOClass.diVal.checkLow[ioName - 16];
                    }
                    else
                    {
                        return "Off";
                    }
                }
                else
                {
                    return "Off";
                }
            }
            catch (IOException)
            {
                return "Off";
            }
        }        

        public static void SetDigValue(int ioName, uint setValue, string ModuleName)
        {
            try
            {
                string retMsg = string.Empty;

                if (SETPOINT_INTERLOCK_CHECK(ioName, setValue, ModuleName, ref retMsg))
                {
                    if ((0 <= ioName) && (31 >= ioName))
                    {
                        DIOClass.SelectHighIndex(ioName, setValue);
                    }
                    else if ((32 <= ioName) && (63 >= ioName))
                    {
                        DIOClass.SelectHighIndex2(ioName, setValue);
                    }

                    Define.sAlarmName = "";
                    IO_StrToInt.io_code = ioName.ToString();
                    string IO_Name = IO_StrToInt.io_code;
                    if (setValue == 1)
                    {
                        digSet.curDigSet[ioName] = "On";

                        if ((IO_Name == "Tower_Lamp_Red_o") ||
                            (IO_Name == "Tower_Lamp_Yellow_o") ||
                            (IO_Name == "Tower_Lamp_Green_o"))
                        {
                            //
                        }
                        else
                        {                            
                            EventLog(string.Format("{0} : On", IO_Name), ModuleName, "Event");
                        }
                    }
                    else
                    {
                        digSet.curDigSet[ioName] = "Off";

                        if ((IO_Name == "Tower_Lamp_Red_o") ||
                            (IO_Name == "Tower_Lamp_Yellow_o") ||
                            (IO_Name == "Tower_Lamp_Green_o"))
                        {
                            //
                        }
                        else
                        {
                            EventLog(string.Format("{0} : Off", IO_Name), ModuleName, "Event");
                        }
                    }
                }
                else
                {
                    MessageBox.Show(retMsg, "Interlock");
                }
            }
            catch (IOException)
            {

            }
        }

        #region 항시 체크 인터락
        private static void VALUE_INTERLOCK_CHECK(object sender, ElapsedEventArgs e)
        {
            // Interlock이 해제 상태인지 체크
            if (!Define.bInterlockRelease)
            {
                if (GetDigValue((int)DigInputList.EMO_Switch_i) == "Off")
                {
                    if (Define.sInterlockMsg == string.Empty)
                    {
                        ALL_VALVE_CLOSE();
                        PROCESS_ABORT();

                        SetDigValue((int)DigOutputList.Buzzer_o, (uint)DigitalOffOn.On, "PM1");

                        Define.sInterlockMsg = "Emergency occurrence!";
                        Define.sInterlockChecklist = "Check the emergency switch";

                        DialogResult result = interlockDisplayForm.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            Define.sInterlockMsg = "";
                            Define.sInterlockChecklist = "";
                        }

                        if (sendMsg_System != "Alarm")
                        {
                            //HostConnection.Host_Set_SystemStatus(hostEquipmentInfo, "System", "Alarm");
                            sendMsg_System = "Alarm";
                        }
                    }
                    else
                    {
                        if (sendMsg_System != "Alarm")
                        {
                            //HostConnection.Host_Set_SystemStatus(hostEquipmentInfo, "System", "Alarm");
                            sendMsg_System = "Alarm";
                        }
                    }
                }
                else
                {
                    if (sendMsg_System != "Idle")
                    {
                        //HostConnection.Host_Set_SystemStatus(hostEquipmentInfo, "System", "Idle");
                        sendMsg_System = "Idle";
                    }
                }

                if (GetDigValue((int)DigInputList.Front_Door_Sensor_i) == "Off")
                {
                    if (Define.sInterlockMsg == string.Empty)
                    {
                        Define.sInterlockMsg = "Front door is open!";
                        Define.sInterlockChecklist = "Check the front door sensor";

                        DialogResult result = interlockDisplayForm.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            Define.sInterlockMsg = "";
                            Define.sInterlockChecklist = "";
                        }
                    }
                }

                if (GetDigValue((int)DigInputList.Left_Door_Sensor_i) == "Off")
                {
                    if (Define.sInterlockMsg == string.Empty)
                    {
                        Define.sInterlockMsg = "Left door is open!";
                        Define.sInterlockChecklist = "Check the left door sensor";

                        DialogResult result = interlockDisplayForm.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            Define.sInterlockMsg = "";
                            Define.sInterlockChecklist = "";
                        }
                    }
                }

                if (GetDigValue((int)DigInputList.Right_Door_Sensor_i) == "Off")
                {
                    if (Define.sInterlockMsg == string.Empty)
                    {
                        Define.sInterlockMsg = "Right door is open!";
                        Define.sInterlockChecklist = "Check the right door sensor";

                        DialogResult result = interlockDisplayForm.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            Define.sInterlockMsg = "";
                            Define.sInterlockChecklist = "";
                        }
                    }
                }

                if (GetDigValue((int)DigInputList.Back_Door_Sensor_i) == "Off")
                {
                    if (Define.sInterlockMsg == string.Empty)
                    {
                        Define.sInterlockMsg = "Back door is open!";
                        Define.sInterlockChecklist = "Check the back door sensor";

                        DialogResult result = interlockDisplayForm.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            Define.sInterlockMsg = "";
                            Define.sInterlockChecklist = "";
                        }
                    }
                }                
            }

            // CH1~2 Water sol valve open 체크
            if ((digSet.curDigSet[(int)DigOutputList.CH1_WaterValve_1_o] == "On") ||
                (digSet.curDigSet[(int)DigOutputList.CH1_WaterValve_2_o] == "On") ||
                (digSet.curDigSet[(int)DigOutputList.CH1_BrushClean_WaterValve_o] == "On") ||

                (digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_1_o] == "On") ||
                (digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_2_o] == "On") ||
                (digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_3_o] == "On") ||
                (digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_4_o] == "On") ||
                (digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_5_o] == "On"))
            {
                if (digSet.curDigSet[(int)DigOutputList.Water_Pump_o] != "On")
                {
                    SetDigValue((int)DigOutputList.Water_Pump_o, (uint)DigitalOffOn.On, "PM1");
                }
            }
            else
            {
                if (digSet.curDigSet[(int)DigOutputList.Water_Pump_o] != "Off")
                {
                    SetDigValue((int)DigOutputList.Water_Pump_o, (uint)DigitalOffOn.Off, "PM1");
                }
            }
        }
        #endregion

        #region 동작(IO) 명령 시 인터락
        private static bool SETPOINT_INTERLOCK_CHECK(int ioName, uint setValue, string ModuleName, ref string retMsg)
        {
            // Interlock이 해제 상태인지 체크
            if (Define.bInterlockRelease)
            {
                return true;
            }

            if (ModuleName == "PM1")
            {
                if ((ioName == (int)DigOutputList.CH1_AirValve_o) ||
                    
                    (ioName == (int)DigOutputList.CH1_WaterValve_1_o) ||
                    (ioName == (int)DigOutputList.CH1_WaterValve_2_o) ||

                    (ioName == (int)DigOutputList.CH1_BrushClean_WaterValve_o) ||
                    (ioName == (int)DigOutputList.CH1_BrushClean_AirValve_o))                    
                {
                    if (setValue == (uint)DigitalOffOn.On)
                    {
                        if ((GetDigValue((int)DigInputList.EMO_Switch_i) == "On") &&

                            (GetDigValue((int)DigInputList.CH1_Door_Op_i) == "Off") &&
                            (GetDigValue((int)DigInputList.CH1_Door_Cl_i) == "On"))
                        {
                            return true;
                        }                            
                        else
                        {
                            retMsg = "EMO switch is on or Door is open";
                            EventLog("[INTERLOCK#1] " + "EMO switch is on / Door is open", ModuleName, "Event");
                            return false;
                        }                            
                    }
                    else
                    {
                        return true;
                    }
                }

                if ((ioName == (int)DigOutputList.CH1_Brush_Fwd_o) ||
                    (ioName == (int)DigOutputList.CH1_Brush_Bwd_o))
                {
                    if (setValue == (uint)DigitalOffOn.On)
                    {
                        if ((GetDigValue((int)DigInputList.EMO_Switch_i) == "On") &&

                            (GetDigValue((int)DigInputList.CH1_Door_Op_i) == "Off") &&
                            (GetDigValue((int)DigInputList.CH1_Door_Cl_i) == "On"))
                        {
                            return true;
                        }                            
                        else
                        {
                            retMsg = "EMO switch is on or Door is open";
                            EventLog("[INTERLOCK#1] " + "EMO switch is on / Door is open", ModuleName, "Event");
                            return false;
                        }                            
                    }
                    else
                    {
                        if (GetDigValue((int)DigInputList.EMO_Switch_i) == "On")
                        {
                            return true;
                        }
                        else
                        {
                            retMsg = "EMO switch is on";
                            EventLog("[INTERLOCK#1] " + "EMO switch is on", ModuleName, "Event");
                            return false;
                        }
                    }
                }

                if (ioName == (int)DigOutputList.CH1_Brush_Fwd_o)                   
                {
                    if (setValue == (uint)DigitalOffOn.On)
                    {
                        if ((GetDigValue((int)DigInputList.CH1_Nozzle_Fwd_i) == "On") &&
                            (GetDigValue((int)DigInputList.CH1_Nozzle_Bwd_i) == "Off"))
                        {
                            return true;
                        }
                        else
                        {
                            retMsg = "Nozzle position is not forward";
                            EventLog("[INTERLOCK#1] " + "Nozzle position is not forward", ModuleName, "Event");
                            return false;
                        }
                    }                    
                }

                if ((ioName == (int)DigOutputList.CH1_Nozzle_Fwd_o) ||
                    (ioName == (int)DigOutputList.CH1_Nozzle_Bwd_o))
                {
                    if (setValue == (uint)DigitalOffOn.On)
                    {
                        if ((GetDigValue((int)DigInputList.EMO_Switch_i) == "On") &&

                            (GetDigValue((int)DigInputList.CH1_Door_Op_i) == "Off") &&
                            (GetDigValue((int)DigInputList.CH1_Door_Cl_i) == "On"))
                        {
                            return true;
                        }
                        else
                        {
                            retMsg = "EMO switch is on or Door is open";
                            EventLog("[INTERLOCK#1] " + "EMO switch is on / Door is open", ModuleName, "Event");
                            return false;
                        }
                    }
                    else
                    {
                        if (GetDigValue((int)DigInputList.EMO_Switch_i) == "On")
                        {
                            return true;
                        }
                        else
                        {
                            retMsg = "EMO switch is on";
                            EventLog("[INTERLOCK#1] " + "EMO switch is on", ModuleName, "Event");
                            return false;
                        }
                    }
                }

                if (ioName == (int)DigOutputList.CH1_Nozzle_Bwd_o)
                {
                    if (setValue == (uint)DigitalOffOn.On)
                    {
                        if ((GetDigValue((int)DigInputList.CH1_Brush_Fwd_i) == "On") &&
                            (GetDigValue((int)DigInputList.CH1_Brush_Bwd_i) == "On") &&
                            (GetDigValue((int)DigInputList.CH1_Brush_Home_i) == "Off"))
                        {
                            return true;
                        }
                        else
                        {
                            retMsg = "Brush block position is not home";
                            EventLog("[INTERLOCK#1] " + "Brush block position is not home", ModuleName, "Event");
                            return false;
                        }
                    }
                }

                if (ioName == (int)DigOutputList.CH1_Door_Open_o)
                {
                    if (setValue == (uint)DigitalOffOn.On)
                    {
                        if ((digSet.curDigSet[(int)DigOutputList.CH1_AirValve_o] == "Off") &&

                            (digSet.curDigSet[(int)DigOutputList.CH1_WaterValve_1_o] == "Off") &&
                            (digSet.curDigSet[(int)DigOutputList.CH1_WaterValve_2_o] == "Off") &&

                            (digSet.curDigSet[(int)DigOutputList.CH1_BrushClean_WaterValve_o] == "Off") &&
                            (digSet.curDigSet[(int)DigOutputList.CH1_BrushClean_AirValve_o] == "Off"))
                        {
                            return true;
                        }
                        else
                        {
                            retMsg = "Air or Water valve is open";
                            EventLog("[INTERLOCK#1] " + "Air or Water valve is open", ModuleName, "Event");
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }                
            }


            if (ModuleName == "PM2")
            {
                if ((ioName == (int)DigOutputList.CH2_AirValve_1_o) ||
                    (ioName == (int)DigOutputList.CH2_AirValve_2_o) ||

                    (ioName == (int)DigOutputList.CH2_WaterValve_1_o) ||
                    (ioName == (int)DigOutputList.CH2_WaterValve_2_o) ||
                    (ioName == (int)DigOutputList.CH2_WaterValve_3_o) ||
                    (ioName == (int)DigOutputList.CH2_WaterValve_4_o) ||
                    (ioName == (int)DigOutputList.CH2_WaterValve_5_o) ||
                    (ioName == (int)DigOutputList.CH2_WaterAirValve_o))
                {
                    if (setValue == (uint)DigitalOffOn.On)
                    {
                        if ((GetDigValue((int)DigInputList.EMO_Switch_i) == "On") &&

                            (GetDigValue((int)DigInputList.CH2_Door_Op_i) == "Off") &&
                            (GetDigValue((int)DigInputList.CH2_Door_Cl_i) == "On"))
                        {
                            return true;
                        }
                        else
                        {
                            retMsg = "EMO switch is on or Door is open";
                            EventLog("[INTERLOCK#2] " + "EMO switch is on / Door is open", ModuleName, "Event");
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                
                if (ioName == (int)DigOutputList.CH2_Door_Open_o)
                {
                    if (setValue == (uint)DigitalOffOn.On)
                    {
                        if ((digSet.curDigSet[(int)DigOutputList.CH2_AirValve_1_o] == "Off") &&
                            (digSet.curDigSet[(int)DigOutputList.CH2_AirValve_2_o] == "Off") &&

                            (digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_1_o] == "Off") &&
                            (digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_2_o] == "Off") &&
                            (digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_3_o] == "Off") &&
                            (digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_4_o] == "Off") &&
                            (digSet.curDigSet[(int)DigOutputList.CH2_WaterValve_5_o] == "Off") &&
                            (digSet.curDigSet[(int)DigOutputList.CH2_WaterAirValve_o] == "Off"))
                        {
                            return true;
                        }
                        else
                        {
                            retMsg = "Air or Water valve is open";
                            EventLog("[INTERLOCK#2] " + "Air or Water valve is open", ModuleName, "Event");
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return true;
        }
        #endregion

        public static bool MOTION_INTERLOCK_CHECK()
        {
            // Interlock이 해제 상태인지 체크
            if (Define.bInterlockRelease)
            {
                return true;
            }

            if ((GetDigValue((int)DigInputList.EMO_Switch_i) == "On") &&

                (GetDigValue((int)DigInputList.CH2_Door_Op_i) == "Off") &&
                (GetDigValue((int)DigInputList.CH2_Door_Cl_i) == "On"))
            {
                return true;
            }
            else
            {
                EventLog("[INTERLOCK#2] " + "EMO switch is on / Door is open", "PM2", "Event");
                return false;
            }
        }

        private static void ALL_VALVE_CLOSE()
        {
            SetDigValue((int)DigOutputList.Water_Pump_o, (uint)DigitalOffOn.Off, "PM1");


            SetDigValue((int)DigOutputList.CH1_AirValve_o, (uint)DigitalOffOn.Off, "PM1");
            SetDigValue((int)DigOutputList.CH1_WaterValve_1_o, (uint)DigitalOffOn.Off, "PM1");
            SetDigValue((int)DigOutputList.CH1_WaterValve_2_o, (uint)DigitalOffOn.Off, "PM1");            
            SetDigValue((int)DigOutputList.CH1_BrushClean_WaterValve_o, (uint)DigitalOffOn.Off, "PM1");
            SetDigValue((int)DigOutputList.CH1_BrushClean_AirValve_o, (uint)DigitalOffOn.Off, "PM1");
            

            SetDigValue((int)DigOutputList.CH2_AirValve_1_o, (uint)DigitalOffOn.Off, "PM2");
            SetDigValue((int)DigOutputList.CH2_AirValve_2_o, (uint)DigitalOffOn.Off, "PM2");
            SetDigValue((int)DigOutputList.CH2_WaterValve_1_o, (uint)DigitalOffOn.Off, "PM2");
            SetDigValue((int)DigOutputList.CH2_WaterValve_2_o, (uint)DigitalOffOn.Off, "PM2");
            SetDigValue((int)DigOutputList.CH2_WaterValve_3_o, (uint)DigitalOffOn.Off, "PM2");
            SetDigValue((int)DigOutputList.CH2_WaterValve_4_o, (uint)DigitalOffOn.Off, "PM2");
            SetDigValue((int)DigOutputList.CH2_WaterValve_5_o, (uint)DigitalOffOn.Off, "PM2");
            SetDigValue((int)DigOutputList.CH2_WaterAirValve_o, (uint)DigitalOffOn.Off, "PM2");
        }

        private static void PROCESS_ABORT()
        {
            if (Define.seqCtrl[(byte)MODULE._PM1] != Define.CTRL_IDLE)
            {
                Define.seqCtrl[(byte)MODULE._PM1] = Define.CTRL_ABORT;
            }

            if (Define.seqCtrl[(byte)MODULE._PM2] != Define.CTRL_IDLE)
            {
                Define.seqCtrl[(byte)MODULE._PM2] = Define.CTRL_ABORT;
            }            
        }

        public static bool Value_Check(string[] sValue)
        {
            bool bResult;
            int i;
            bool bRtn = true;
            double dVal = 0.0;

            for (i = 0; i < sValue.Length; i++)
            {
                bResult = double.TryParse(sValue[i], out dVal);
                if (!bResult)
                {
                    bRtn = false;
                    break;
                }
            }

            if (bRtn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
