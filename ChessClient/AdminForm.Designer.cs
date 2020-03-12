namespace ChessClient
{
    partial class AdminForm
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
            this.btnScreenshotW = new System.Windows.Forms.Button();
            this.lblWhite = new System.Windows.Forms.Label();
            this.lblBlack = new System.Windows.Forms.Label();
            this.resetTimer = new System.Windows.Forms.Timer(this.components);
            this.btnScreenB = new System.Windows.Forms.Button();
            this.btnWhiteWin = new System.Windows.Forms.Button();
            this.btnBlackWin = new System.Windows.Forms.Button();
            this.btnDraw = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnProcessB = new System.Windows.Forms.Button();
            this.btnProcessW = new System.Windows.Forms.Button();
            this.btnUndoMove = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnScreenshotW
            // 
            this.btnScreenshotW.Location = new System.Drawing.Point(15, 93);
            this.btnScreenshotW.Name = "btnScreenshotW";
            this.btnScreenshotW.Size = new System.Drawing.Size(219, 62);
            this.btnScreenshotW.TabIndex = 0;
            this.btnScreenshotW.Text = "Demand Screenshot";
            this.btnScreenshotW.UseVisualStyleBackColor = true;
            this.btnScreenshotW.Click += new System.EventHandler(this.btnScreenshot_Click);
            // 
            // lblWhite
            // 
            this.lblWhite.Location = new System.Drawing.Point(12, 9);
            this.lblWhite.Name = "lblWhite";
            this.lblWhite.Size = new System.Drawing.Size(222, 23);
            this.lblWhite.TabIndex = 1;
            this.lblWhite.Text = "label1";
            this.lblWhite.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBlack
            // 
            this.lblBlack.Location = new System.Drawing.Point(566, 9);
            this.lblBlack.Name = "lblBlack";
            this.lblBlack.Size = new System.Drawing.Size(222, 23);
            this.lblBlack.TabIndex = 2;
            this.lblBlack.Text = "label1";
            this.lblBlack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // resetTimer
            // 
            this.resetTimer.Interval = 10000;
            this.resetTimer.Tick += new System.EventHandler(this.resetTimer_Tick);
            // 
            // btnScreenB
            // 
            this.btnScreenB.Location = new System.Drawing.Point(569, 93);
            this.btnScreenB.Name = "btnScreenB";
            this.btnScreenB.Size = new System.Drawing.Size(219, 62);
            this.btnScreenB.TabIndex = 3;
            this.btnScreenB.Text = "Demand Screenshot";
            this.btnScreenB.UseVisualStyleBackColor = true;
            this.btnScreenB.Click += new System.EventHandler(this.btnScreenB_Click);
            // 
            // btnWhiteWin
            // 
            this.btnWhiteWin.BackColor = System.Drawing.Color.IndianRed;
            this.btnWhiteWin.Location = new System.Drawing.Point(6, 21);
            this.btnWhiteWin.Name = "btnWhiteWin";
            this.btnWhiteWin.Size = new System.Drawing.Size(219, 62);
            this.btnWhiteWin.TabIndex = 4;
            this.btnWhiteWin.Text = "Make White Win";
            this.btnWhiteWin.UseVisualStyleBackColor = false;
            this.btnWhiteWin.Click += new System.EventHandler(this.btnWhiteWin_Click);
            // 
            // btnBlackWin
            // 
            this.btnBlackWin.BackColor = System.Drawing.Color.IndianRed;
            this.btnBlackWin.Location = new System.Drawing.Point(548, 21);
            this.btnBlackWin.Name = "btnBlackWin";
            this.btnBlackWin.Size = new System.Drawing.Size(219, 62);
            this.btnBlackWin.TabIndex = 5;
            this.btnBlackWin.Text = "Make Black Win";
            this.btnBlackWin.UseVisualStyleBackColor = false;
            this.btnBlackWin.Click += new System.EventHandler(this.btnBlackWin_Click);
            // 
            // btnDraw
            // 
            this.btnDraw.BackColor = System.Drawing.Color.IndianRed;
            this.btnDraw.Location = new System.Drawing.Point(274, 21);
            this.btnDraw.Name = "btnDraw";
            this.btnDraw.Size = new System.Drawing.Size(219, 62);
            this.btnDraw.TabIndex = 6;
            this.btnDraw.Text = "Declare Draw";
            this.btnDraw.UseVisualStyleBackColor = false;
            this.btnDraw.Click += new System.EventHandler(this.btnDraw_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.btnWhiteWin);
            this.groupBox1.Controls.Add(this.btnBlackWin);
            this.groupBox1.Controls.Add(this.btnDraw);
            this.groupBox1.Location = new System.Drawing.Point(12, 294);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(773, 96);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "These buttons will end the game! Only do so when it should be! Do not abuse.";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.SpringGreen;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(240, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(323, 214);
            this.label1.TabIndex = 8;
            this.label1.Text = "Abuse of these anti-cheating systems will not be tolerated!\r\n\r\nReview the Terms a" +
    "nd Conditions\r\n";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnProcessB
            // 
            this.btnProcessB.Location = new System.Drawing.Point(569, 161);
            this.btnProcessB.Name = "btnProcessB";
            this.btnProcessB.Size = new System.Drawing.Size(219, 62);
            this.btnProcessB.TabIndex = 10;
            this.btnProcessB.Text = "Demand Processes";
            this.btnProcessB.UseVisualStyleBackColor = true;
            this.btnProcessB.Click += new System.EventHandler(this.btnProcessB_Click);
            // 
            // btnProcessW
            // 
            this.btnProcessW.Location = new System.Drawing.Point(15, 161);
            this.btnProcessW.Name = "btnProcessW";
            this.btnProcessW.Size = new System.Drawing.Size(219, 62);
            this.btnProcessW.TabIndex = 9;
            this.btnProcessW.Text = "Demand Processes";
            this.btnProcessW.UseVisualStyleBackColor = true;
            this.btnProcessW.Click += new System.EventHandler(this.btnProcessW_Click);
            // 
            // btnUndoMove
            // 
            this.btnUndoMove.Location = new System.Drawing.Point(239, 226);
            this.btnUndoMove.Name = "btnUndoMove";
            this.btnUndoMove.Size = new System.Drawing.Size(324, 62);
            this.btnUndoMove.TabIndex = 11;
            this.btnUndoMove.Text = "Undo Last (side) Move";
            this.btnUndoMove.UseVisualStyleBackColor = true;
            this.btnUndoMove.Click += new System.EventHandler(this.btnUndoMove_Click);
            // 
            // AdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 400);
            this.Controls.Add(this.btnUndoMove);
            this.Controls.Add(this.btnProcessB);
            this.Controls.Add(this.btnProcessW);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnScreenB);
            this.Controls.Add(this.lblBlack);
            this.Controls.Add(this.lblWhite);
            this.Controls.Add(this.btnScreenshotW);
            this.Name = "AdminForm";
            this.Text = "AdminForm";
            this.VisibleChanged += new System.EventHandler(this.AdminForm_VisibleChanged);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnScreenshotW;
        private System.Windows.Forms.Label lblWhite;
        private System.Windows.Forms.Label lblBlack;
        private System.Windows.Forms.Timer resetTimer;
        private System.Windows.Forms.Button btnScreenB;
        private System.Windows.Forms.Button btnWhiteWin;
        private System.Windows.Forms.Button btnBlackWin;
        private System.Windows.Forms.Button btnDraw;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnProcessB;
        private System.Windows.Forms.Button btnProcessW;
        private System.Windows.Forms.Button btnUndoMove;
    }
}