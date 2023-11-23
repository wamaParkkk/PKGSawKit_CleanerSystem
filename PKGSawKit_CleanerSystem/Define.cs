namespace PKGSawKit_CleanerSystem
{
    public enum MODULE
    {
        _PM1 = 0,
        _PM2 = 1,
        _MOTOR = 2
    }

    public enum Page
    {
        LogInPage = 0,
        OperationPage = 1,
        MaintnancePage = 2,
        RecipePage = 3,
        ConfigurePage = 4,
        IOPage = 5,
        AlarmPage = 6,
        EventLogPage = 7,
        UserRegist = 8
    }

    public enum DigitalOffOn
    {
        Off = 0,
        On = 1
    }

    public enum DigitalOnOff
    {
        On = 0,
        Off = 1
    }

    public enum RecipeEditMode : byte
    {
        NORMAL_MODE = 0,
        VIEW_MODE = 1,
        EDIT_MODE = 2
    }

    public struct TDigSet
    {
        public string[] curDigSet;
    }

    // 공정 진행 시 화면에 표시해줄 내용
    public struct TPrcsInfo
    {
        public string[] prcsRecipeName;
        public int[] prcsCurrentStep;
        public int[] prcsTotalStep;
        public string[] prcsStepName;
        public double[] prcsStepCurrentTime;
        public double[] prcsStepTotalTime;
        public string[] prcsEndTime;
    }

    // IO LIST /////////////////////////////////////////////
    public enum DigInputList
    {
        CH1_Door_Op_i = 0,
        CH1_Door_Cl_i = 1,
        CH1_Brush_Fwd_i = 2,
        CH1_Brush_Bwd_i = 3,
        CH1_Brush_Home_i = 4,
        CH1_Nozzle_Fwd_i = 5,
        CH1_Nozzle_Bwd_i = 6,        

        CH2_Door_Op_i = 16,
        CH2_Door_Cl_i = 17,

        EMO_Switch_i = 22,
        Front_Door_Sensor_i = 23,
        Left_Door_Sensor_i = 24,
        Right_Door_Sensor_i = 25,
        Back_Door_Sensor_i = 26
    }

    public enum DigOutputList
    {
        CH1_Door_Open_o = 0,
        CH1_Door_Close_o = 1,
        CH1_AirValve_o = 2,
        CH1_WaterValve_1_o = 3,
        CH1_WaterValve_2_o = 4,
        CH1_BrushClean_WaterValve_o = 5,
        CH1_BrushClean_AirValve_o = 6,
        FluorescentLamp_o = 7,
        CH1_Nozzle_Bwd_o = 8,
        CH1_Nozzle_Fwd_o = 9,
        CH1_Brush_Bwd_o = 10,
        CH1_Brush_Fwd_o = 11,        
        CH1_Nozzle_Pwr_o = 12,
        CH1_Brush_Pwr_o = 13,

        Water_Pump_o = 15,

        CH2_Door_Open_o = 16,
        CH2_Door_Close_o = 17,
        CH2_AirValve_1_o = 18,
        CH2_AirValve_2_o = 19,
        CH2_WaterValve_1_o = 20,
        CH2_WaterValve_2_o = 21,
        CH2_WaterValve_3_o = 22,
        CH2_WaterValve_4_o = 23,
        CH2_WaterValve_5_o = 24,
        CH2_WaterAirValve_o = 25,

        Tower_Lamp_Green_o = 28,
        Tower_Lamp_Yellow_o = 29,        
        Tower_Lamp_Red_o = 30,
        Buzzer_o = 31
    }
    ////////////////////////////////////////////////////////

    // IO (String to int)///////////////////////////////////
    public static class IO_StrToInt
    {
        private static string _io_Name = "";

        public static string io_code
        {
            get
            {
                if (string.IsNullOrEmpty(_io_Name))
                {
                    _io_Name = "IO Name is null";
                }

                return _io_Name;
            }
            set
            {
                if (value == "0")           _io_Name = "CH1_Door_Open_o";              
                else if (value == "1")      _io_Name = "CH1_Door_Close_o";
                else if (value == "2")      _io_Name = "CH1_AirValve_o";
                else if (value == "3")      _io_Name = "CH1_WaterValve_1_o";
                else if (value == "4")      _io_Name = "CH1_WaterValve_2_o";
                else if (value == "5")      _io_Name = "CH1_BrushClean_WaterValve_o";
                else if (value == "6")      _io_Name = "CH1_BrushClean_AirValve_o";
                else if (value == "7")      _io_Name = "FluorescentLamp_o";
                else if (value == "8")      _io_Name = "CH1_Nozzle_Bwd_o";
                else if (value == "9")      _io_Name = "CH1_Nozzle_Fwd_o";
                else if (value == "10")     _io_Name = "CH1_Brush_Bwd_o";
                else if (value == "11")     _io_Name = "CH1_Brush_Fwd_o";
                else if (value == "12")     _io_Name = "CH1_Nozzle_Pwr_o";
                else if (value == "13")     _io_Name = "CH1_Brush_Pwr_o";

                else if (value == "15")     _io_Name = "Water_Pump_o";

                else if (value == "16")     _io_Name = "CH2_Door_Open_o";
                else if (value == "17")     _io_Name = "CH2_Door_Close_o";
                else if (value == "18")     _io_Name = "CH2_AirValve_1_o";
                else if (value == "19")     _io_Name = "CH2_AirValve_2_o";
                else if (value == "20")     _io_Name = "CH2_WaterValve_1_o";
                else if (value == "21")     _io_Name = "CH2_WaterValve_2_o";
                else if (value == "22")     _io_Name = "CH2_WaterValve_3_o";
                else if (value == "23")     _io_Name = "CH2_WaterValve_4_o";
                else if (value == "24")     _io_Name = "CH2_WaterValve_5_o";
                else if (value == "25")     _io_Name = "CH2_WaterAirValve_o";

                else if (value == "28")     _io_Name = "Tower_Lamp_Green_o";
                else if (value == "29")     _io_Name = "Tower_Lamp_Yellow_o";
                else if (value == "30")     _io_Name = "Tower_Lamp_Red_o";
                else if (value == "31")     _io_Name = "Buzzer_o";
            }
        }
    }
    ////////////////////////////////////////////////////////

    // ALARM LIST //////////////////////////////////////////
    public class Alarm_List
    {
        private string _alarm_Name = "";

        public string alarm_code
        {
            get
            {
                if (string.IsNullOrEmpty(_alarm_Name))
                {
                    _alarm_Name = "Alarm name is missing";
                }

                return _alarm_Name;
            }
            set
            {
                if (value == "900")         _alarm_Name = "Tool does not exist";
                
                else if (value == "1000")   _alarm_Name = "Door open time out";
                else if (value == "1001")   _alarm_Name = "Door close time out";
                
                else if (value == "1010")   _alarm_Name = "Failed to read recipe file";

                else if (value == "1020")   _alarm_Name = "Nozzle cylinder forward time out";
                else if (value == "1021")   _alarm_Name = "Nozzle cylinder backward time out";
                else if (value == "1022")   _alarm_Name = "Nozzle cylinder home time out";

                else if (value == "1030")   _alarm_Name = "Brush cylinder forward time out";
                else if (value == "1031")   _alarm_Name = "Brush cylinder backward time out";
                else if (value == "1032")   _alarm_Name = "Brush cylinder home time out";

                else if (value == "1040")   _alarm_Name = "Brush up time out (Motor)";
                else if (value == "1041")   _alarm_Name = "Brush down time out (Motor)";
                else if (value == "1042")   _alarm_Name = "Brush home time out (Motor)";

                else if (value == "1045")   _alarm_Name = "Brush rotation run time out (Motor)";
                else if (value == "1046")   _alarm_Name = "Brush rotation stop time out (Motor)";
            }
        }
    }
    ////////////////////////////////////////////////////////

    // CONFIGURE LIST //////////////////////////////////////
    public class Configure_List
    {
        // System
        public static int Door_OpCl_Timeout = 0;
        public static int Brush_UpDn_Timeout = 0;
        public static int Brush_Rotation_Timeout = 0;
        public static int Brush_FwdBwd_Timeout = 0;
        public static int Nozzle_FwdBwd_Timeout = 0;
        public static int Brush_Clean_Time = 0;

        // Motion parameter (공정 진행 시)
        public static double Brush_Rotation_Speed = 0;
        public static double Brush_UpDown_Speed = 0;
        public static double Brush_Up_Position = 0;
        public static double Brush_Down_Position = 0;
        public static double WaterBlock_Move_Speed = 0;
        public static double WaterBlock_Fwd_Position = 0;
        public static double WaterBlock_Bwd_Position = 0;
        public static double Cmd_Move_Timeout = 0;        
    }
    ////////////////////////////////////////////////////////
    
    class Define
    {
        public const int BUFSIZ = 512;
        public const int MODULE_MAX = 2;
        public const int CH_MAX = 32;
        public const int RECIPE_MAX_STEP = 50;

        // Login 여부
        public static bool bLogin = false;

        // User info
        public static string UserId = "";
        public static string UserName = "";
        public static string UserLevel = "";

        // Eventlog 발생 여부
        public static bool bPM1Event;
        public static bool bPM2Event;        

        public static bool bPM1AlmEvent;
        public static bool bPM2AlmEvent;        
        public static bool bPM1OpAlmEvent;
        public static bool bPM2OpAlmEvent;               


        public static bool bOpActivate = false;
        public static byte currentPage = 0;
        public static byte MaintCurrentPage = 0;
        public static byte RecipeCurrentPage = 0;

        public static bool bInterlockRelease = false;
        public static string sInterlockMsg = "";        
        public static string sInterlockChecklist = "";
        public static bool bDoorAutoRelease = false;
        public static bool bSimulation = false;
        public static bool bManualLamp = false;
        

        // Sequence에서 사용 할 변수
        // PM1, PM2 Process seq//////////////////////////
        public static byte[] seqMode = { 0, 0 };
        public static byte[] seqCtrl = { 0, 0 };
        public static byte[] seqSts = { 0, 0 };

        public const byte MODE_IDLE = 0;
        public const byte MODE_PROCESS = 1;
        public const byte MODE_INIT = 2;

        public const byte CTRL_IDLE = 0;
        public const byte CTRL_RUN = 1;
        public const byte CTRL_RUNNING = 2;
        public const byte CTRL_ALARM = 3;
        public const byte CTRL_RETRY = 4;
        public const byte CTRL_HOLD = 5;
        public const byte CTRL_ABORT = 6;

        public const byte STS_IDLE = 0;
        public const byte STS_PROCESS_ING = 1;
        public const byte STS_PROCESS_END = 2;
        public const byte STS_INIT_ING = 3;
        public const byte STS_INIT_END = 4;
        public const byte STS_ABORTOK = 5;

        public static byte iStepNum1;
        public static byte iStepNum2;
        /////////////////////////////////////////////////

        // PM1, PM2 Nozzle fwd/bwd seq //////////////////
        public static byte[] seqCylinderMode = { 0, 0 };
        public static byte[] seqCylinderCtrl = { 0, 0 };
        public static byte[] seqCylinderSts = { 0, 0 };

        public const byte MODE_CYLINDER_IDLE = 0;
        public const byte MODE_CYLINDER_RUN = 1;
        public const byte MODE_CYLINDER_HOME = 2;

        public const byte STS_CYLINDER_IDLE = 0;
        public const byte STS_CYLINDER_RUNING = 1;
        public const byte STS_CYLINDER_RUNEND = 2;
        public const byte STS_CYLINDER_HOMEING = 3;
        public const byte STS_CYLINDER_HOMEEND = 4;
        public const byte STS_CYLINDER_ABORTOK = 5;
        /////////////////////////////////////////////////

        // PM1 Brush up/down seq ////////////////////////
        public static byte seqBrushUpDnMode;
        public static byte seqBrushUpDnCtrl;
        public static byte seqBrushUpDnSts;

        public const byte MODE_BRUSH_UPDN_IDLE = 0;
        public const byte MODE_BRUSH_UPDN_HOME = 1;
        public const byte MODE_BRUSH_UPDN_UP = 2;
        public const byte MODE_BRUSH_UPDN_DOWN = 3;

        public const byte STS_BRUSH_UPDN_IDLE = 0;
        public const byte STS_BRUSH_UPDN_HOMEING = 1;
        public const byte STS_BRUSH_UPDN_HOMEEND = 2;
        public const byte STS_BRUSH_UPDN_UPING = 3;
        public const byte STS_BRUSH_UPDN_UPEND = 4;
        public const byte STS_BRUSH_UPDN_DOWNING = 5;
        public const byte STS_BRUSH_UPDN_DOWNEND = 6;
        public const byte STS_BRUSH_UPDN_ABORTOK = 7;
        /////////////////////////////////////////////////

        // PM1 Brush fwd/bwd seq ////////////////////////
        public static byte seqBrushFwBwMode;
        public static byte seqBrushFwBwCtrl;
        public static byte seqBrushFwBwSts;

        public const byte MODE_BRUSH_FWBW_IDLE = 0;
        public const byte MODE_BRUSH_FWBW_RUN = 1;
        public const byte MODE_BRUSH_FWBW_HOME = 2;
        public const byte MODE_BRUSH_FWBW_CLEAN = 3;

        public const byte STS_BRUSH_FWBW_IDLE = 0;
        public const byte STS_BRUSH_FWBW_RUNING = 1;
        public const byte STS_BRUSH_FWBW_RUNEND = 2;
        public const byte STS_BRUSH_FWBW_HOMEING = 3;
        public const byte STS_BRUSH_FWBW_HOMEEND = 4;
        public const byte STS_BRUSH_FWBW_CLEANING = 5;
        public const byte STS_BRUSH_FWBW_CLEANEND = 6;
        public const byte STS_BRUSH_FWBW_ABORTOK = 7;
        /////////////////////////////////////////////////


        // Recipe 선택 관련 변수
        public static int iSelectRecipeModule;                        // 선택 된 Module
        public static string[] sSelectRecipeName = { null, null };    // 선택 된 Recipe name

        // 알람 name
        public static string sAlarmName;

        // Daily count
        public static int iPM1DailyCnt;
        public static int iPM2DailyCnt;

        // SPH : 2대 * 3(1대 기준 시간 당 최대 공정 tool 갯수)
        public const int iCapa = 108;   // SPH * 24hour * 0.75(가동률 75%)

        // 가동 시간
        public static double dTodayRunTime;
        public const int iSemiAutoTime = 86400; // 24h * 60m * 60s

        // Chamber Enable/Disable
        public static bool[] bChamberDisable = { false, false };

        // Tool check
        public static bool[] bDontCheckTool = { false, false };

        // 모터 Home 여부
        public static bool bBrushHomeFlag;
        public static bool bHomeFlag;

        // Motor axis
        public const int axis_z = 0;   // Brush up/down axis
        public const int axis_r = 1;   // Brush rotation axis
        public const int axis_y = 2;   // Water block axis

        public static double dTolerance = 0.2;        
    }    
}
