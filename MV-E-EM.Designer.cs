namespace 图像识别
{
    partial class MV_E_EM
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
            this.butOpen = new System.Windows.Forms.Button();
            this.butGrab = new System.Windows.Forms.Button();
            this.butClose = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // butOpen
            // 
            this.butOpen.Location = new System.Drawing.Point(12, 12);
            this.butOpen.Name = "butOpen";
            this.butOpen.Size = new System.Drawing.Size(75, 23);
            this.butOpen.TabIndex = 0;
            this.butOpen.Text = "打开相机";
            this.butOpen.UseVisualStyleBackColor = true;
            this.butOpen.Click += new System.EventHandler(this.butOpen_Click);
            // 
            // butGrab
            // 
            this.butGrab.Location = new System.Drawing.Point(111, 12);
            this.butGrab.Name = "butGrab";
            this.butGrab.Size = new System.Drawing.Size(75, 23);
            this.butGrab.TabIndex = 1;
            this.butGrab.Text = "开始采集";
            this.butGrab.UseVisualStyleBackColor = true;
            this.butGrab.Click += new System.EventHandler(this.butGrab_Click);
            // 
            // butClose
            // 
            this.butClose.Location = new System.Drawing.Point(211, 12);
            this.butClose.Name = "butClose";
            this.butClose.Size = new System.Drawing.Size(75, 23);
            this.butClose.TabIndex = 2;
            this.butClose.Text = "关闭相机";
            this.butClose.UseVisualStyleBackColor = true;
            this.butClose.Click += new System.EventHandler(this.butClose_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(12, 41);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(871, 619);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // MV_E_EM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 667);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.butClose);
            this.Controls.Add(this.butGrab);
            this.Controls.Add(this.butOpen);
            this.Name = "MV_E_EM";
            this.Text = "MV_E_EM";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MV_E_EM_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MV_E_EM_FormClosed);
            this.Load += new System.EventHandler(this.MV_E_EM_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button butOpen;
        private System.Windows.Forms.Button butGrab;
        private System.Windows.Forms.Button butClose;
        private System.Windows.Forms.PictureBox pictureBox1;

    }
}