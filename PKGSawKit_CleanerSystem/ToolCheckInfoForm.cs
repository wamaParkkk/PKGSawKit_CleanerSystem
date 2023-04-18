﻿using System;
using System.Windows.Forms;

namespace PKGSawKit_CleanerSystem
{
    public partial class ToolCheckInfoForm : Form
    {
        public ToolCheckInfoForm()
        {
            InitializeComponent();
        }

        private void ToolCheckInfoForm_Load(object sender, EventArgs e)
        {
            Top = 350;
            Left = 350;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }        
    }
}
