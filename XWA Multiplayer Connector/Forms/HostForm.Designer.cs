namespace XWA_Multiplayer_Connector.Forms
{
    partial class HostForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainerTop = new System.Windows.Forms.SplitContainer();
            this.listBoxMissionSelection = new System.Windows.Forms.ListBox();
            this.splitContainerTopRight = new System.Windows.Forms.SplitContainer();
            this.buttonSendTempTie = new System.Windows.Forms.Button();
            this.textBoxMissionInfo = new System.Windows.Forms.TextBox();
            this.colourableListBoxClients = new XWA_Multiplayer_Connector.Controls.ColourableListBox();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.timerBeat = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTop)).BeginInit();
            this.splitContainerTop.Panel1.SuspendLayout();
            this.splitContainerTop.Panel2.SuspendLayout();
            this.splitContainerTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTopRight)).BeginInit();
            this.splitContainerTopRight.Panel1.SuspendLayout();
            this.splitContainerTopRight.Panel2.SuspendLayout();
            this.splitContainerTopRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerTop);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.textBoxLog);
            this.splitContainerMain.Size = new System.Drawing.Size(800, 450);
            this.splitContainerMain.SplitterDistance = 266;
            this.splitContainerMain.SplitterWidth = 10;
            this.splitContainerMain.TabIndex = 0;
            // 
            // splitContainerTop
            // 
            this.splitContainerTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerTop.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerTop.Location = new System.Drawing.Point(0, 0);
            this.splitContainerTop.Name = "splitContainerTop";
            // 
            // splitContainerTop.Panel1
            // 
            this.splitContainerTop.Panel1.Controls.Add(this.listBoxMissionSelection);
            // 
            // splitContainerTop.Panel2
            // 
            this.splitContainerTop.Panel2.Controls.Add(this.splitContainerTopRight);
            this.splitContainerTop.Size = new System.Drawing.Size(800, 266);
            this.splitContainerTop.SplitterDistance = 357;
            this.splitContainerTop.SplitterWidth = 10;
            this.splitContainerTop.TabIndex = 0;
            // 
            // listBoxMissionSelection
            // 
            this.listBoxMissionSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxMissionSelection.FormattingEnabled = true;
            this.listBoxMissionSelection.Location = new System.Drawing.Point(0, 0);
            this.listBoxMissionSelection.Name = "listBoxMissionSelection";
            this.listBoxMissionSelection.Size = new System.Drawing.Size(357, 266);
            this.listBoxMissionSelection.TabIndex = 0;
            this.listBoxMissionSelection.SelectedIndexChanged += new System.EventHandler(this.ListBoxMissionSelection_SelectedIndexChanged);
            // 
            // splitContainerTopRight
            // 
            this.splitContainerTopRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerTopRight.Location = new System.Drawing.Point(0, 0);
            this.splitContainerTopRight.Name = "splitContainerTopRight";
            // 
            // splitContainerTopRight.Panel1
            // 
            this.splitContainerTopRight.Panel1.Controls.Add(this.buttonSendTempTie);
            this.splitContainerTopRight.Panel1.Controls.Add(this.textBoxMissionInfo);
            // 
            // splitContainerTopRight.Panel2
            // 
            this.splitContainerTopRight.Panel2.Controls.Add(this.colourableListBoxClients);
            this.splitContainerTopRight.Size = new System.Drawing.Size(433, 266);
            this.splitContainerTopRight.SplitterDistance = 204;
            this.splitContainerTopRight.SplitterWidth = 10;
            this.splitContainerTopRight.TabIndex = 0;
            // 
            // buttonSendTempTie
            // 
            this.buttonSendTempTie.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSendTempTie.Location = new System.Drawing.Point(3, 231);
            this.buttonSendTempTie.Name = "buttonSendTempTie";
            this.buttonSendTempTie.Size = new System.Drawing.Size(198, 32);
            this.buttonSendTempTie.TabIndex = 3;
            this.buttonSendTempTie.Text = "Send out \"temp.tie\"";
            this.buttonSendTempTie.UseVisualStyleBackColor = true;
            this.buttonSendTempTie.Click += new System.EventHandler(this.ButtonSendTempTie_Click);
            // 
            // textBoxMissionInfo
            // 
            this.textBoxMissionInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMissionInfo.Location = new System.Drawing.Point(3, 0);
            this.textBoxMissionInfo.Multiline = true;
            this.textBoxMissionInfo.Name = "textBoxMissionInfo";
            this.textBoxMissionInfo.ReadOnly = true;
            this.textBoxMissionInfo.Size = new System.Drawing.Size(198, 225);
            this.textBoxMissionInfo.TabIndex = 1;
            // 
            // colourableListBoxClients
            // 
            this.colourableListBoxClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.colourableListBoxClients.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.colourableListBoxClients.Enabled = false;
            this.colourableListBoxClients.FormattingEnabled = true;
            this.colourableListBoxClients.Location = new System.Drawing.Point(0, 0);
            this.colourableListBoxClients.Name = "colourableListBoxClients";
            this.colourableListBoxClients.Size = new System.Drawing.Size(219, 266);
            this.colourableListBoxClients.TabIndex = 0;
            // 
            // textBoxLog
            // 
            this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLog.Location = new System.Drawing.Point(0, 0);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(800, 174);
            this.textBoxLog.TabIndex = 0;
            // 
            // timerBeat
            // 
            this.timerBeat.Enabled = true;
            this.timerBeat.Interval = 1000;
            this.timerBeat.Tick += new System.EventHandler(this.TimerBeat_Tick);
            // 
            // HostForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainerMain);
            this.Name = "HostForm";
            this.Text = "XWA Connector - Host Server";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HostForm_FormClosed);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerTop.Panel1.ResumeLayout(false);
            this.splitContainerTop.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTop)).EndInit();
            this.splitContainerTop.ResumeLayout(false);
            this.splitContainerTopRight.Panel1.ResumeLayout(false);
            this.splitContainerTopRight.Panel1.PerformLayout();
            this.splitContainerTopRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTopRight)).EndInit();
            this.splitContainerTopRight.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.SplitContainer splitContainerTop;
        private System.Windows.Forms.SplitContainer splitContainerTopRight;
        private System.Windows.Forms.TextBox textBoxMissionInfo;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.Button buttonSendTempTie;
        private System.Windows.Forms.ListBox listBoxMissionSelection;
        private Controls.ColourableListBox colourableListBoxClients;
        private System.Windows.Forms.Timer timerBeat;
    }
}