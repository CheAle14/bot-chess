namespace ChessClient
{
    partial class GameForm
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
            this.lblWhite = new System.Windows.Forms.Label();
            this.lblBlack = new System.Windows.Forms.Label();
            this.btnSurrender = new System.Windows.Forms.Button();
            this.panelDs = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.lblDsUser = new System.Windows.Forms.Label();
            this.btnDsYes = new System.Windows.Forms.Button();
            this.panelDs.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblWhite
            // 
            this.lblWhite.AutoSize = true;
            this.lblWhite.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWhite.Location = new System.Drawing.Point(12, 714);
            this.lblWhite.Name = "lblWhite";
            this.lblWhite.Size = new System.Drawing.Size(53, 20);
            this.lblWhite.TabIndex = 0;
            this.lblWhite.Text = "label1";
            // 
            // lblBlack
            // 
            this.lblBlack.AutoSize = true;
            this.lblBlack.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBlack.Location = new System.Drawing.Point(12, 9);
            this.lblBlack.Name = "lblBlack";
            this.lblBlack.Size = new System.Drawing.Size(53, 20);
            this.lblBlack.TabIndex = 1;
            this.lblBlack.Text = "label1";
            // 
            // btnSurrender
            // 
            this.btnSurrender.Location = new System.Drawing.Point(826, 705);
            this.btnSurrender.Name = "btnSurrender";
            this.btnSurrender.Size = new System.Drawing.Size(100, 40);
            this.btnSurrender.TabIndex = 2;
            this.btnSurrender.Text = "Resign";
            this.btnSurrender.UseVisualStyleBackColor = true;
            this.btnSurrender.Click += new System.EventHandler(this.btnSurrender_Click);
            // 
            // panelDs
            // 
            this.panelDs.Controls.Add(this.btnDsYes);
            this.panelDs.Controls.Add(this.label2);
            this.panelDs.Controls.Add(this.lblDsUser);
            this.panelDs.Location = new System.Drawing.Point(743, 12);
            this.panelDs.Name = "panelDs";
            this.panelDs.Size = new System.Drawing.Size(283, 119);
            this.panelDs.TabIndex = 3;
            this.panelDs.Visible = false;
            this.panelDs.Paint += new System.Windows.Forms.PaintEventHandler(this.panelDs_Paint);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(277, 26);
            this.label2.TabIndex = 1;
            this.label2.Text = "wishes to join your game";
            // 
            // lblDsUser
            // 
            this.lblDsUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDsUser.Location = new System.Drawing.Point(3, 1);
            this.lblDsUser.Name = "lblDsUser";
            this.lblDsUser.Size = new System.Drawing.Size(277, 35);
            this.lblDsUser.TabIndex = 0;
            this.lblDsUser.Text = "Name#Discrim";
            this.lblDsUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnDsYes
            // 
            this.btnDsYes.Location = new System.Drawing.Point(6, 73);
            this.btnDsYes.Name = "btnDsYes";
            this.btnDsYes.Size = new System.Drawing.Size(274, 37);
            this.btnDsYes.TabIndex = 2;
            this.btnDsYes.Text = "Accept";
            this.btnDsYes.UseVisualStyleBackColor = true;
            this.btnDsYes.Click += new System.EventHandler(this.btnDsYes_Click);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1038, 958);
            this.Controls.Add(this.panelDs);
            this.Controls.Add(this.btnSurrender);
            this.Controls.Add(this.lblBlack);
            this.Controls.Add(this.lblWhite);
            this.Name = "GameForm";
            this.Text = "GameForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GameForm_FormClosed);
            this.Load += new System.EventHandler(this.GameForm_Load);
            this.VisibleChanged += new System.EventHandler(this.GameForm_VisibleChanged);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.GameForm_MouseDoubleClick);
            this.panelDs.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWhite;
        private System.Windows.Forms.Label lblBlack;
        public System.Windows.Forms.Button btnSurrender;
        private System.Windows.Forms.Panel panelDs;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblDsUser;
        private System.Windows.Forms.Button btnDsYes;
    }
}