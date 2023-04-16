namespace Shark_Remote
{
    partial class Downloader
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Downloader));
            visualProgress = new VitNX2.UI.ControlsV2.VitNX2_ProgressBarRoundedType1();
            vitnX2_Button1 = new VitNX2.UI.ControlsV2.VitNX2_Button();
            SuspendLayout();
            // 
            // visualProgress
            // 
            visualProgress.CustomText = "";
            visualProgress.Location = new Point(12, 12);
            visualProgress.Name = "visualProgress";
            visualProgress.ProgressColor = Color.FromArgb(0, 90, 181);
            visualProgress.Size = new Size(189, 10);
            visualProgress.Style = ProgressBarStyle.Marquee;
            visualProgress.TabIndex = 32;
            visualProgress.TextColor = Color.Black;
            visualProgress.TextFont = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            visualProgress.VisualMode = VitNX2.UI.ControlsV2.Helper.VitNX2_ProgressBarDisplayMode.NoText;
            // 
            // vitnX2_Button1
            // 
            vitnX2_Button1.BackColor = Color.FromArgb(38, 43, 59);
            vitnX2_Button1.BackgroundColor = Color.FromArgb(38, 43, 59);
            vitnX2_Button1.BorderColor = Color.FromArgb(21, 29, 38);
            vitnX2_Button1.BorderRadius = 5;
            vitnX2_Button1.BorderSize = 2;
            vitnX2_Button1.FlatAppearance.BorderSize = 0;
            vitnX2_Button1.FlatStyle = FlatStyle.Flat;
            vitnX2_Button1.ForeColor = Color.FromArgb(247, 247, 248);
            vitnX2_Button1.Location = new Point(52, 28);
            vitnX2_Button1.Name = "vitnX2_Button1";
            vitnX2_Button1.Size = new Size(107, 27);
            vitnX2_Button1.TabIndex = 33;
            vitnX2_Button1.Text = "Отмена";
            vitnX2_Button1.TextColor = Color.FromArgb(247, 247, 248);
            vitnX2_Button1.UseVisualStyleBackColor = true;
            vitnX2_Button1.Click += vitnX2_Button1_Click;
            // 
            // Downloader
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(26, 32, 48);
            ClientSize = new Size(212, 65);
            Controls.Add(vitnX2_Button1);
            Controls.Add(visualProgress);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Downloader";
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Подготовка";
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private VitNX2.UI.ControlsV2.VitNX2_ProgressBarRoundedType1 visualProgress;
        private VitNX2.UI.ControlsV2.VitNX2_Button vitnX2_Button1;
    }
}