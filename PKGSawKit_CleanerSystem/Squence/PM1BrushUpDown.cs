using Ajin_motion_driver;
using MsSqlManagerLibrary;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PKGSawKit_CleanerSystem.Squence
{
    class PM1BrushUpDown : TBaseThread
    {
        Thread thread;
        private new TStep step;
        Alarm_List alarm_List;  // Alarm list
        
        int nHomeCnt;

        public PM1BrushUpDown()
        {
            ModuleName = "PM1";
            module = (byte)MODULE._PM1;
            
            thread = new Thread(new ThreadStart(Execute));
            
            alarm_List = new Alarm_List();

            Define.bBrushHomeFlag = false;            

            thread.Start();
        }

        public void Dispose()
        {
            thread.Abort();
        }

        private void Execute()
        {            
            try
            {
                while (true)
                {                    
                    if (Define.seqBrushUpDnCtrl == Define.CTRL_ABORT)
                    {
                        AlarmAction("Abort");
                    }
                    else if (Define.seqBrushUpDnCtrl == Define.CTRL_RETRY)
                    {
                        AlarmAction("Retry");
                    }
                    
                    Home_Progress();
                    Up_Progress();
                    Down_Progress();

                    Thread.Sleep(10);
                }
            }
            catch (Exception)
            {
                
            }
        }

        private void AlarmAction(string sAction)
        {
            if (sAction == "Retry")
            {
                step.Flag = true;
                step.Times = 1;

                Define.seqBrushUpDnCtrl = Define.CTRL_RUNNING;
                
                if (Define.seqCtrl[module] == Define.CTRL_ALARM)
                {
                    Define.seqCtrl[module] = Define.CTRL_RUNNING;
                }
            }
            else if (sAction == "Abort")
            {
                ActionList();
                
                Define.seqBrushUpDnMode = Define.MODE_BRUSH_UPDN_IDLE;
                Define.seqBrushUpDnCtrl = Define.CTRL_IDLE;
                Define.seqBrushUpDnSts = Define.STS_BRUSH_UPDN_ABORTOK;

                step.Times = 1;                

                Global.EventLog("Brush up/down movement stopped : " + sAction, ModuleName, "Event");
            }
        }

        private void ActionList()
        {
            F_PROCESS_ALL_VALVE_CLOSE();

            MotionClass.SetMotorSStop(Define.axis_z);
        }

        private void ShowAlarm(string almId)
        {
            ActionList();

            Define.seqBrushUpDnCtrl = Define.CTRL_ALARM;

            // 프로세스 시퀀스 알람으로 멈춤
            Define.seqCtrl[module] = Define.CTRL_ALARM;

            // Buzzer IO On.
            Global.SetDigValue((int)DigOutputList.Buzzer_o, (uint)DigitalOffOn.On, ModuleName);

            // Alarm history.
            Define.sAlarmName = "";
            alarm_List.alarm_code = almId;
            Define.sAlarmName = alarm_List.alarm_code;

            Global.EventLog(almId + ":" + Define.sAlarmName, ModuleName, "Alarm");

            HostConnection.Host_Set_RunStatus(Global.hostEquipmentInfo, ModuleName, "Alarm");
            HostConnection.Host_Set_AlarmName(Global.hostEquipmentInfo, ModuleName, Define.sAlarmName);
        }

        public void F_INC_STEP()
        {
            step.Flag = true;
            step.Layer++;
            step.Times = 1;
        }

        // BRUSH UPDN HOME PROGRESS /////////////////////////////////////////////////////////
        #region BRUSH_UPDN_HOME_PROGRESS
        private void Home_Progress()
        {
            if ((Define.seqBrushUpDnMode == Define.MODE_BRUSH_UPDN_HOME) && (Define.seqBrushUpDnCtrl == Define.CTRL_RUN))
            {
                Thread.Sleep(500);
                step.Layer = 1;
                step.Times = 1;
                step.Flag = true;

                Define.bBrushHomeFlag = false;

                Define.seqBrushUpDnCtrl = Define.CTRL_RUNNING;
                Define.seqBrushUpDnSts = Define.STS_BRUSH_UPDN_HOMEING;                

                Global.EventLog("START THE BRUSH UP/DOWN HOME.", ModuleName, "Event");
            }
            else if ((Define.seqBrushUpDnMode == Define.MODE_BRUSH_UPDN_HOME) && (Define.seqBrushUpDnCtrl == Define.CTRL_RUNNING))
            {
                switch (step.Layer)
                {
                    case 1:
                        {
                            P_BRUSH_UpDn_Home();
                        }
                        break;

                    case 2:
                        {
                            P_BRUSH_UpDn_HomeEnd();
                        }
                        break;
                }
            }
        }

        private void Up_Progress()
        {
            if ((Define.seqBrushUpDnMode == Define.MODE_BRUSH_UPDN_UP) && (Define.seqBrushUpDnCtrl == Define.CTRL_RUN))
            {
                Thread.Sleep(500);
                step.Layer = 1;
                step.Times = 1;
                step.Flag = true;

                Define.seqBrushUpDnCtrl = Define.CTRL_RUNNING;
                Define.seqBrushUpDnSts = Define.STS_BRUSH_UPDN_UPING;                

                Global.EventLog("START THE BRUSH UP.", ModuleName, "Event");
            }
            else if ((Define.seqBrushUpDnMode == Define.MODE_BRUSH_UPDN_UP) && (Define.seqBrushUpDnCtrl == Define.CTRL_RUNNING))
            {
                switch (step.Layer)
                {
                    case 1:
                        {
                            P_BRUSH_UpDn_Home();
                        }
                        break;

                    case 2:
                        {
                            P_BRUSH_UpDn("Up");
                        }
                        break;

                    case 3:
                        {
                            P_BRUSH_UpDn_UpEnd();
                        }
                        break;
                }
            }
        }

        private void Down_Progress()
        {
            if ((Define.seqBrushUpDnMode == Define.MODE_BRUSH_UPDN_DOWN) && (Define.seqBrushUpDnCtrl == Define.CTRL_RUN))
            {
                Thread.Sleep(500);
                step.Layer = 1;
                step.Times = 1;
                step.Flag = true;

                Define.seqBrushUpDnCtrl = Define.CTRL_RUNNING;
                Define.seqBrushUpDnSts = Define.STS_BRUSH_UPDN_DOWNING;                

                Global.EventLog("START THE BRUSH DOWN.", ModuleName, "Event");
            }
            else if ((Define.seqBrushUpDnMode == Define.MODE_BRUSH_UPDN_DOWN) && (Define.seqBrushUpDnCtrl == Define.CTRL_RUNNING))
            {
                switch (step.Layer)
                {
                    case 1:
                        {
                            P_BRUSH_UpDn_Home();
                        }
                        break;

                    case 2:
                        {
                            P_BRUSH_UpDn("Down");
                        }
                        break;

                    case 3:
                        {
                            P_BRUSH_UpDn_DownEnd();
                        }
                        break;
                }
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////
        ///
        // FUNCTION /////////////////////////////////////////////////////////////////////////
        #region FUNCTION

        private void P_BRUSH_UpDn_Home()
        {
            if (step.Flag)
            {
                //if (Define.bBrushHomeFlag)
                if ((MotionClass.motor[Define.axis_z].sR_HomeStatus == "Home") &&
                    (MotionClass.motor[Define.axis_z].sR_BusyStatus == "Ready"))
                {
                    F_INC_STEP();
                }
                else
                {
                    Global.EventLog("Brush : Home", ModuleName, "Event");

                    MotionClass.SetMotorHome(Define.axis_z);

                    step.Flag = false;
                    step.Times = 1;

                    nHomeCnt = 0;
                }                
            }
            else
            {
                if (step.Times > 15)
                {
                    if ((MotionClass.motor[Define.axis_z].sR_HomeStatus == "Home") && 
                        (MotionClass.motor[Define.axis_z].sR_BusyStatus == "Ready"))                        
                    {
                        Define.bBrushHomeFlag = true;

                        if (nHomeCnt > 5)
                        {
                            F_INC_STEP();
                        }
                        else
                        {
                            nHomeCnt++;
                        }                        
                    }
                    else
                    {
                        if (step.Times >= Configure_List.Brush_UpDn_Timeout)
                        {
                            ShowAlarm("1042");
                        }
                        else
                        {
                            step.INC_TIMES();
                        }

                        nHomeCnt = 0;
                    }
                }
                else
                {
                    step.INC_TIMES();
                }                
            }
        }        

        private void P_BRUSH_UpDn(string UpDn)
        {
            if (step.Flag)
            {                
                Global.EventLog("Brush : " + UpDn, ModuleName, "Event");

                MotionClass.SetMotorVelocity(Define.axis_z, Configure_List.Brush_UpDown_Speed);
                MotionClass.SetMotorAccel(Define.axis_z, Configure_List.Brush_UpDown_Speed * 2);
                MotionClass.SetMotorDecel(Define.axis_z, Configure_List.Brush_UpDown_Speed * 2);
                MotionClass.SetMotorGearing(Define.axis_z, 1);

                if (UpDn == "Up")
                {
                    if ((MotionClass.motor[Define.axis_z].sR_HomeStatus == "Home") &&
                        (MotionClass.motor[Define.axis_z].sR_BusyStatus == "Ready"))
                    {
                        F_INC_STEP();
                    }
                    else
                    {
                        MotionClass.MotorMove(Define.axis_z, Configure_List.Brush_Up_Position);
                        Thread.Sleep(1000);
                        MotionClass.MotorMove(Define.axis_z, Configure_List.Brush_Up_Position);

                        step.Flag = false;
                        step.Times = 1;
                    }                    
                }
                else if (UpDn == "Down")
                {
                    /*
                    if ((MotionClass.motor[Define.axis_z].sR_HomeStatus == "+Limit") &&
                        (MotionClass.motor[Define.axis_z].sR_BusyStatus == "Ready"))
                    {
                        F_INC_STEP();
                    }
                    else
                    {
                        MotionClass.MotorMove(Define.axis_z, Configure_List.Brush_Down_Position);
                        Thread.Sleep(1000);
                        MotionClass.MotorMove(Define.axis_z, Configure_List.Brush_Down_Position);

                        step.Flag = false;
                        step.Times = 1;
                    } 
                    */
                    if (MotionClass.motor[Define.axis_z].sR_BusyStatus == "Ready")
                    {
                        MotionClass.MotorMove(Define.axis_z, Configure_List.Brush_Down_Position);
                        Thread.Sleep(1000);
                        MotionClass.MotorMove(Define.axis_z, Configure_List.Brush_Down_Position);
                        Thread.Sleep(500);

                        step.Flag = false;
                        step.Times = 1;
                    }                    
                }                
            }
            else
            {                
                if (UpDn == "Up")
                {
                    if ((MotionClass.motor[Define.axis_z].sR_HomeStatus == "Home") &&
                        (MotionClass.motor[Define.axis_z].sR_BusyStatus == "Ready"))
                    {
                        //Thread.Sleep(500);
                        Task.Delay(500);

                        F_INC_STEP();
                    }
                    else
                    {
                        if (step.Times >= Configure_List.Brush_UpDn_Timeout)
                        {
                            ShowAlarm("1040");
                        }
                        else
                        {
                            step.INC_TIMES_10();
                        }
                    }
                }
                else
                {
                    if (//(MotionClass.motor[Define.axis_z].sR_HomeStatus == "+Limit") &&
                        (MotionClass.motor[Define.axis_z].sR_BusyStatus == "Ready"))
                    {
                        //Thread.Sleep(500);
                        Task.Delay(500);

                        F_INC_STEP();
                    }
                    else
                    {
                        if (step.Times >= Configure_List.Brush_UpDn_Timeout)
                        {
                            ShowAlarm("1041");
                        }
                        else
                        {
                            step.INC_TIMES_10();
                        }
                    }
                }
            }
        }

        private void P_BRUSH_UpDn_HomeEnd()
        {
            Define.seqBrushUpDnMode = Define.MODE_BRUSH_UPDN_IDLE;
            Define.seqBrushUpDnCtrl = Define.CTRL_IDLE;
            Define.seqBrushUpDnSts = Define.STS_BRUSH_UPDN_HOMEEND;

            Global.EventLog("COMPLETE THE BRUSH UP/DOWN HOME.", ModuleName, "Event");
        }

        private void P_BRUSH_UpDn_UpEnd()
        {
            Define.seqBrushUpDnMode = Define.MODE_BRUSH_UPDN_IDLE;
            Define.seqBrushUpDnCtrl = Define.CTRL_IDLE;
            Define.seqBrushUpDnSts = Define.STS_BRUSH_UPDN_UPEND;            

            Global.EventLog("COMPLETE THE BRUSH UP.", ModuleName, "Event");            
        }

        private void P_BRUSH_UpDn_DownEnd()
        {
            Define.seqBrushUpDnMode = Define.MODE_BRUSH_UPDN_IDLE;
            Define.seqBrushUpDnCtrl = Define.CTRL_IDLE;
            Define.seqBrushUpDnSts = Define.STS_BRUSH_UPDN_DOWNEND;

            Global.EventLog("COMPLETE THE BRUSH DOWN.", ModuleName, "Event");
        }

        private void F_PROCESS_ALL_VALVE_CLOSE()
        {
            // Air
            Global.SetDigValue((int)DigOutputList.CH1_AirValve_o, (uint)DigitalOffOn.Off, ModuleName);

            // Water
            Global.SetDigValue((int)DigOutputList.CH1_WaterValve_1_o, (uint)DigitalOffOn.Off, ModuleName);
            Global.SetDigValue((int)DigOutputList.CH1_WaterValve_2_o, (uint)DigitalOffOn.Off, ModuleName);
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////
    }
}
