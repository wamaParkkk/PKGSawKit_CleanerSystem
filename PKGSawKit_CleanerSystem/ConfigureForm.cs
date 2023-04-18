using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PKGSawKit_CleanerSystem
{
    public partial class ConfigureForm : Form
    {
        AnalogDlg AnaDlg;

        public ConfigureForm()
        {            
            InitializeComponent();
        }

        private void ConfigureForm_Load(object sender, EventArgs e)
        {
            Width = 1172;
            Height = 824;
            Top = 0;
            Left = 0;

            PARAMETER_LOAD();
            MOTION_PARAMETER_LOAD();
        }

        private void ConfigureForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose();
        }

        private void PARAMETER_LOAD()
        {
            string sTmpData;
            string FileName = "Configure.txt";

            if (File.Exists(Global.ConfigurePath + FileName))
            {
                byte[] bytes;
                using (var fs = File.Open(Global.ConfigurePath + FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, (int)fs.Length);
                    sTmpData = Encoding.Default.GetString(bytes);

                    char sp = ',';
                    string[] spString = sTmpData.Split(sp);
                    for (int i = 0; i < spString.Length; i++)
                    {
                        Configure_List.Door_OpCl_Timeout = int.Parse(spString[0]);
                        Configure_List.Brush_UpDn_Timeout = int.Parse(spString[1]);
                        Configure_List.Brush_Rotation_Timeout = int.Parse(spString[2]);
                        Configure_List.Brush_FwdBwd_Timeout = int.Parse(spString[3]);
                        Configure_List.Nozzle_FwdBwd_Timeout = int.Parse(spString[4]);
                        Configure_List.Brush_Clean_Time = int.Parse(spString[5]);


                        txtBoxDoorOpenCloseTimeout.Text = (Configure_List.Door_OpCl_Timeout).ToString();
                        txtBoxBrushUpDnTimeout.Text = (Configure_List.Brush_UpDn_Timeout).ToString();
                        txtBoxBrushRotateTimeout.Text = (Configure_List.Brush_Rotation_Timeout).ToString();
                        txtBoxBrushFwdBwdTimeout.Text = (Configure_List.Brush_FwdBwd_Timeout).ToString();
                        txtBoxNozzleFwdBwdTimeout.Text = (Configure_List.Nozzle_FwdBwd_Timeout).ToString();
                        txtBoxBrushCleanTime.Text = (Configure_List.Brush_Clean_Time).ToString();
                    }
                }
            }
        }

        private void MOTION_PARAMETER_LOAD()
        {
            string sTmpData;
            string FileName = "MotionConfigure.txt";

            if (File.Exists(Global.ConfigurePath + FileName))
            {
                byte[] bytes;
                using (var fs = File.Open(Global.ConfigurePath + FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, (int)fs.Length);
                    sTmpData = Encoding.Default.GetString(bytes);

                    char sp = ',';
                    string[] spString = sTmpData.Split(sp);
                    for (int i = 0; i < spString.Length; i++)
                    {
                        Configure_List.Brush_UpDown_Speed = int.Parse(spString[0]);
                        Configure_List.Brush_Rotation_Speed = int.Parse(spString[1]);                        
                        Configure_List.Brush_Up_Position = int.Parse(spString[2]);
                        Configure_List.Brush_Down_Position = int.Parse(spString[3]);
                        Configure_List.WaterBlock_Move_Speed = int.Parse(spString[4]);
                        Configure_List.WaterBlock_Fwd_Position = int.Parse(spString[5]);
                        Configure_List.WaterBlock_Bwd_Position = int.Parse(spString[6]);
                        Configure_List.Cmd_Move_Timeout = int.Parse(spString[7]);

                        txtBoxBrushUpDnSpeed.Text = (Configure_List.Brush_UpDown_Speed).ToString();
                        txtBoxBrushRotationSpeed.Text = (Configure_List.Brush_Rotation_Speed).ToString();                        
                        txtBoxBrushUpPosition.Text = (Configure_List.Brush_Up_Position).ToString();
                        txtBoxBrushDownPosition.Text = (Configure_List.Brush_Down_Position).ToString();
                        txtBoxWaterblockMoveSpeed.Text = (Configure_List.WaterBlock_Move_Speed).ToString();
                        txtBoxWaterblockFwdPosition.Text = (Configure_List.WaterBlock_Fwd_Position).ToString();
                        txtBoxWaterblockBwdPosition.Text = (Configure_List.WaterBlock_Bwd_Position).ToString();
                        txtBoxMoveTimeout.Text = (Configure_List.Cmd_Move_Timeout).ToString();
                    }
                }
            }
        }

        private void txtBoxDoorOpenCloseTimeout_Click(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            AnaDlg = new AnalogDlg();
            AnaDlg.Init();
            if (AnaDlg.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = AnaDlg.m_strResult;

                string[] sVal = new string[1];
                string sTemp = textBox.Text.ToString().Trim();
                sVal[0] = sTemp;
                if (!Global.Value_Check(sVal))
                {
                    MessageBox.Show("잘못 된 값이 입력되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox.Text = "0";
                }
            }
        }

        private void btnParameterSave_Click(object sender, EventArgs e)
        {
            string sDoorOpClTimeout = txtBoxDoorOpenCloseTimeout.Text.ToString().Trim();
            string sBrushUpDnTimeout = txtBoxBrushUpDnTimeout.Text.ToString().Trim();
            string sBrushRotateTimeout = txtBoxBrushRotateTimeout.Text.ToString().Trim();
            string sBrushFwdBwdTimeout = txtBoxBrushFwdBwdTimeout.Text.ToString().Trim();
            string sNozzleFwdBwdTimeout = txtBoxNozzleFwdBwdTimeout.Text.ToString().Trim();
            string sBrushCleanTime = txtBoxBrushCleanTime.Text.ToString().Trim();

            if (Parameter_WriteFile(sDoorOpClTimeout, sBrushUpDnTimeout, sBrushRotateTimeout, sBrushFwdBwdTimeout, sNozzleFwdBwdTimeout, sBrushCleanTime))
            {
                Configure_List.Door_OpCl_Timeout = int.Parse(sDoorOpClTimeout);
                Configure_List.Brush_UpDn_Timeout = int.Parse(sBrushUpDnTimeout);
                Configure_List.Brush_Rotation_Timeout = int.Parse(sBrushRotateTimeout);
                Configure_List.Brush_FwdBwd_Timeout = int.Parse(sBrushFwdBwdTimeout);
                Configure_List.Nozzle_FwdBwd_Timeout = int.Parse(sNozzleFwdBwdTimeout);
                Configure_List.Brush_Clean_Time = int.Parse(sBrushCleanTime);

                MessageBox.Show("Configure 값이 저장 되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Configure 값이 저장 되지 않았습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool Parameter_WriteFile(string param1, string param2, string param3, string param4, string param5, string param6)
        {
            string FileName = "Configure.txt";

            try
            {                
                File.WriteAllText(Global.ConfigurePath + FileName, param1 + "," + param2 + "," + param3 + "," + param4 + "," + 
                    param5 + "," + param6, Encoding.Default);                

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "알림");
                return false;
            }
        }

        private void btnMotionParameterSave_Click(object sender, EventArgs e)
        {
            string sBrushUpDnSpeed = txtBoxBrushUpDnSpeed.Text.ToString().Trim();
            string sBrushRotationSpeed = txtBoxBrushRotationSpeed.Text.ToString().Trim();            
            string sBrushUpPosition = txtBoxBrushUpPosition.Text.ToString().Trim();
            string sBrushDownPosition = txtBoxBrushDownPosition.Text.ToString().Trim();
            string sWaterBlockSpeed = txtBoxWaterblockMoveSpeed.Text.ToString().Trim();
            string sWaterBlockFwdPosi = txtBoxWaterblockFwdPosition.Text.ToString().Trim();
            string sWaterBlockBwdPosi = txtBoxWaterblockBwdPosition.Text.ToString().Trim();
            string sMotorMoveTimeout = txtBoxMoveTimeout.Text.ToString().Trim();

            if (Motion_Parameter_WriteFile(sBrushUpDnSpeed, sBrushRotationSpeed, sBrushUpPosition, sBrushDownPosition, 
                sWaterBlockSpeed, sWaterBlockFwdPosi, sWaterBlockBwdPosi, sMotorMoveTimeout))
            {
                Configure_List.Brush_UpDown_Speed = int.Parse(sBrushUpDnSpeed);
                Configure_List.Brush_Rotation_Speed = int.Parse(sBrushRotationSpeed);                
                Configure_List.Brush_Up_Position = int.Parse(sBrushUpPosition);
                Configure_List.Brush_Down_Position = int.Parse(sBrushDownPosition);
                Configure_List.WaterBlock_Move_Speed = int.Parse(sWaterBlockSpeed);
                Configure_List.WaterBlock_Fwd_Position = int.Parse(sWaterBlockFwdPosi);
                Configure_List.WaterBlock_Bwd_Position = int.Parse(sWaterBlockBwdPosi);
                Configure_List.Cmd_Move_Timeout = int.Parse(sMotorMoveTimeout);

                MessageBox.Show("Motion parameter 값이 저장 되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Motion parameter 값이 저장 되지 않았습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool Motion_Parameter_WriteFile(string param1, string param2, string param3, string param4, 
            string param5, string param6, string param7, string param8)
        {
            string FileName = "MotionConfigure.txt";

            try
            {
                File.WriteAllText(Global.ConfigurePath + FileName, param1 + "," + param2 + "," + param3 + "," + param4 + "," + 
                    param5 + "," + param6 + "," + param7 + "," + param8, Encoding.Default);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "알림");
                return false;
            }
        }
    }
}
