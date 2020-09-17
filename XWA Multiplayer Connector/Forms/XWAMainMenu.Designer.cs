namespace XWA_Multiplayer_Connector.Forms
{
    partial class XWAMainMenu
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
            this.buttonHostServer = new System.Windows.Forms.Button();
            this.buttonJoinServer = new System.Windows.Forms.Button();
            this.buttonSetup = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // buttonHostServer
            // 
            this.buttonHostServer.Location = new System.Drawing.Point(12, 12);
            this.buttonHostServer.Name = "buttonHostServer";
            this.buttonHostServer.Size = new System.Drawing.Size(150, 50);
            this.buttonHostServer.TabIndex = 0;
            this.buttonHostServer.Text = "Host Server";
            this.buttonHostServer.UseVisualStyleBackColor = true;
            this.buttonHostServer.Click += new System.EventHandler(this.ButtonHostServer_Click);
            // 
            // buttonJoinServer
            // 
            this.buttonJoinServer.Location = new System.Drawing.Point(12, 68);
            this.buttonJoinServer.Name = "buttonJoinServer";
            this.buttonJoinServer.Size = new System.Drawing.Size(150, 50);
            this.buttonJoinServer.TabIndex = 1;
            this.buttonJoinServer.Text = "Join Server";
            this.buttonJoinServer.UseVisualStyleBackColor = true;
            this.buttonJoinServer.Click += new System.EventHandler(this.ButtonJoinServer_Click);
            // 
            // buttonSetup
            // 
            this.buttonSetup.Location = new System.Drawing.Point(12, 124);
            this.buttonSetup.Name = "buttonSetup";
            this.buttonSetup.Size = new System.Drawing.Size(150, 50);
            this.buttonSetup.TabIndex = 2;
            this.buttonSetup.Text = "Setup\r\n(Locate install directory)";
            this.buttonSetup.UseVisualStyleBackColor = true;
            this.buttonSetup.Click += new System.EventHandler(this.ButtonSetup_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "XWA exe|alliance.exe";
            this.openFileDialog1.InitialDirectory = "C:";
            // 
            // XWAMainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(177, 202);
            this.Controls.Add(this.buttonSetup);
            this.Controls.Add(this.buttonJoinServer);
            this.Controls.Add(this.buttonHostServer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "XWAMainMenu";
            this.Text = "XWA Connector - Menu";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.XWAMainMenu_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonHostServer;
        private System.Windows.Forms.Button buttonJoinServer;
        private System.Windows.Forms.Button buttonSetup;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}