namespace ChessClient
{
    partial class PromoteForm
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
            this.btnQueen = new System.Windows.Forms.Button();
            this.btnBishop = new System.Windows.Forms.Button();
            this.btnRook = new System.Windows.Forms.Button();
            this.btnKnight = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnQueen
            // 
            this.btnQueen.Location = new System.Drawing.Point(12, 62);
            this.btnQueen.Name = "btnQueen";
            this.btnQueen.Size = new System.Drawing.Size(128, 128);
            this.btnQueen.TabIndex = 0;
            this.btnQueen.UseVisualStyleBackColor = true;
            // 
            // btnBishop
            // 
            this.btnBishop.Location = new System.Drawing.Point(12, 224);
            this.btnBishop.Name = "btnBishop";
            this.btnBishop.Size = new System.Drawing.Size(128, 128);
            this.btnBishop.TabIndex = 1;
            this.btnBishop.UseVisualStyleBackColor = true;
            // 
            // btnRook
            // 
            this.btnRook.Location = new System.Drawing.Point(176, 62);
            this.btnRook.Name = "btnRook";
            this.btnRook.Size = new System.Drawing.Size(128, 128);
            this.btnRook.TabIndex = 2;
            this.btnRook.UseVisualStyleBackColor = true;
            // 
            // btnKnight
            // 
            this.btnKnight.Location = new System.Drawing.Point(176, 224);
            this.btnKnight.Name = "btnKnight";
            this.btnKnight.Size = new System.Drawing.Size(128, 128);
            this.btnKnight.TabIndex = 3;
            this.btnKnight.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(292, 50);
            this.label1.TabIndex = 4;
            this.label1.Text = "Select Promotion";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // PromoteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 365);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnKnight);
            this.Controls.Add(this.btnRook);
            this.Controls.Add(this.btnBishop);
            this.Controls.Add(this.btnQueen);
            this.Name = "PromoteForm";
            this.Text = "Promote a Pawn";
            this.Load += new System.EventHandler(this.PromoteForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnQueen;
        private System.Windows.Forms.Button btnBishop;
        private System.Windows.Forms.Button btnRook;
        private System.Windows.Forms.Button btnKnight;
        private System.Windows.Forms.Label label1;
    }
}