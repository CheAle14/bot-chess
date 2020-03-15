namespace ChessInstaller
{
    partial class MainForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cbRegisterProtocol = new System.Windows.Forms.CheckBox();
            this.btnInstall = new System.Windows.Forms.Button();
            this.pbBar = new System.Windows.Forms.ProgressBar();
            this.lblUpdate = new System.Windows.Forms.Label();
            this.lblFolderFeedback = new System.Windows.Forms.Label();
            this.lblBytes = new System.Windows.Forms.Label();
            this.cbTerms = new System.Windows.Forms.CheckBox();
            this.cbUseDiscord = new System.Windows.Forms.CheckBox();
            this.cbUseAntiCheat = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(535, 38);
            this.label1.TabIndex = 0;
            this.label1.Text = "Chess Client Installation";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Folder:";
            // 
            // txtLocation
            // 
            this.txtLocation.Location = new System.Drawing.Point(70, 62);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.ReadOnly = true;
            this.txtLocation.Size = new System.Drawing.Size(363, 22);
            this.txtLocation.TabIndex = 2;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(439, 62);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(84, 23);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(511, 33);
            this.label3.TabIndex = 4;
            this.label3.Text = "Other Options";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbRegisterProtocol
            // 
            this.cbRegisterProtocol.AutoSize = true;
            this.cbRegisterProtocol.Checked = true;
            this.cbRegisterProtocol.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRegisterProtocol.Enabled = false;
            this.cbRegisterProtocol.Location = new System.Drawing.Point(15, 154);
            this.cbRegisterProtocol.Name = "cbRegisterProtocol";
            this.cbRegisterProtocol.Size = new System.Drawing.Size(454, 21);
            this.cbRegisterProtocol.TabIndex = 5;
            this.cbRegisterProtocol.Text = "Register protocol, allowing starting of program directly from the web";
            this.cbRegisterProtocol.UseVisualStyleBackColor = true;
            // 
            // btnInstall
            // 
            this.btnInstall.Enabled = false;
            this.btnInstall.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInstall.Location = new System.Drawing.Point(12, 269);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(511, 66);
            this.btnInstall.TabIndex = 6;
            this.btnInstall.Text = "Install";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // pbBar
            // 
            this.pbBar.Location = new System.Drawing.Point(12, 364);
            this.pbBar.Name = "pbBar";
            this.pbBar.Size = new System.Drawing.Size(511, 23);
            this.pbBar.TabIndex = 7;
            // 
            // lblUpdate
            // 
            this.lblUpdate.Location = new System.Drawing.Point(12, 338);
            this.lblUpdate.Name = "lblUpdate";
            this.lblUpdate.Size = new System.Drawing.Size(421, 23);
            this.lblUpdate.TabIndex = 8;
            // 
            // lblFolderFeedback
            // 
            this.lblFolderFeedback.AutoSize = true;
            this.lblFolderFeedback.Location = new System.Drawing.Point(70, 91);
            this.lblFolderFeedback.Name = "lblFolderFeedback";
            this.lblFolderFeedback.Size = new System.Drawing.Size(266, 17);
            this.lblFolderFeedback.TabIndex = 9;
            this.lblFolderFeedback.Text = "Please select an empty folder to install to";
            // 
            // lblBytes
            // 
            this.lblBytes.Location = new System.Drawing.Point(463, 338);
            this.lblBytes.Name = "lblBytes";
            this.lblBytes.Size = new System.Drawing.Size(60, 23);
            this.lblBytes.TabIndex = 10;
            // 
            // cbTerms
            // 
            this.cbTerms.AutoSize = true;
            this.cbTerms.Enabled = false;
            this.cbTerms.Location = new System.Drawing.Point(12, 242);
            this.cbTerms.Name = "cbTerms";
            this.cbTerms.Size = new System.Drawing.Size(491, 21);
            this.cbTerms.TabIndex = 11;
            this.cbTerms.Text = "Agree to Chess Terms and Conditions at https://ml-api.uk.ms/chess/terms\r\n";
            this.cbTerms.UseVisualStyleBackColor = true;
            this.cbTerms.CheckedChanged += new System.EventHandler(this.cbTerms_CheckedChanged);
            // 
            // cbUseDiscord
            // 
            this.cbUseDiscord.AutoSize = true;
            this.cbUseDiscord.Checked = true;
            this.cbUseDiscord.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUseDiscord.Location = new System.Drawing.Point(15, 181);
            this.cbUseDiscord.Name = "cbUseDiscord";
            this.cbUseDiscord.Size = new System.Drawing.Size(203, 21);
            this.cbUseDiscord.TabIndex = 12;
            this.cbUseDiscord.Text = "Use Discord Rich Presence";
            this.cbUseDiscord.UseVisualStyleBackColor = true;
            this.cbUseDiscord.CheckedChanged += new System.EventHandler(this.cbUseDiscord_CheckedChanged);
            // 
            // cbUseAntiCheat
            // 
            this.cbUseAntiCheat.AutoSize = true;
            this.cbUseAntiCheat.Checked = true;
            this.cbUseAntiCheat.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUseAntiCheat.Location = new System.Drawing.Point(15, 208);
            this.cbUseAntiCheat.Name = "cbUseAntiCheat";
            this.cbUseAntiCheat.Size = new System.Drawing.Size(242, 21);
            this.cbUseAntiCheat.TabIndex = 13;
            this.cbUseAntiCheat.Text = "Enable anti-cheating mechanisms";
            this.cbUseAntiCheat.UseVisualStyleBackColor = true;
            this.cbUseAntiCheat.CheckedChanged += new System.EventHandler(this.cbEnableAntiCheat_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 399);
            this.Controls.Add(this.cbUseAntiCheat);
            this.Controls.Add(this.cbUseDiscord);
            this.Controls.Add(this.cbTerms);
            this.Controls.Add(this.lblBytes);
            this.Controls.Add(this.lblFolderFeedback);
            this.Controls.Add(this.lblUpdate);
            this.Controls.Add(this.pbBar);
            this.Controls.Add(this.btnInstall);
            this.Controls.Add(this.cbRegisterProtocol);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtLocation);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "Installation";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbRegisterProtocol;
        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.ProgressBar pbBar;
        private System.Windows.Forms.Label lblUpdate;
        private System.Windows.Forms.Label lblFolderFeedback;
        private System.Windows.Forms.Label lblBytes;
        private System.Windows.Forms.CheckBox cbTerms;
        private System.Windows.Forms.CheckBox cbUseDiscord;
        private System.Windows.Forms.CheckBox cbUseAntiCheat;
    }
}

