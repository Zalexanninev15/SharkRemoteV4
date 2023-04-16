namespace Shark_Remote
{
    partial class Preparing
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preparing));
            windowTitle = new Panel();
            progressVisual = new VitNX2.UI.ControlsV2.VitNX2_PictureBox();
            titleExit = new Label();
            titleLabel = new Label();
            first = new Label();
            preparingTab = new VitNX2.UI.ControlsV2.VitNX2_Tab();
            firstOne = new TabPage();
            label10 = new Label();
            label4 = new Label();
            sharkOne = new VitNX2.UI.ControlsV2.VitNX2_ToogleButton();
            label11 = new Label();
            sharkMessage = new TextBox();
            firstTwo = new TabPage();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            label3 = new Label();
            botFatherLink = new LinkLabel();
            label2 = new Label();
            label1 = new Label();
            getBotTokenNow = new VitNX2.UI.ControlsV2.VitNX2_Button();
            firstThree = new TabPage();
            label14 = new Label();
            username = new VitNX2.UI.ControlsV2.VitNX2_TextBox();
            label13 = new Label();
            vitnX2_Button3 = new VitNX2.UI.ControlsV2.VitNX2_Button();
            setUsername = new VitNX2.UI.ControlsV2.VitNX2_Button();
            label8 = new Label();
            finalStage = new TabPage();
            pictureBox4 = new PictureBox();
            vitnX2_Button2 = new VitNX2.UI.ControlsV2.VitNX2_Button();
            progress = new VitNX2.UI.ControlsV2.VitNX2_ProgressBarRoundedType1();
            vitnX2_Button1 = new VitNX2.UI.ControlsV2.VitNX2_Button();
            label12 = new Label();
            label9 = new Label();
            second = new Label();
            secondSecond = new Label();
            visualProgress = new VitNX2.UI.ControlsV2.VitNX2_ProgressBarRoundedType1();
            windowTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)progressVisual).BeginInit();
            preparingTab.SuspendLayout();
            firstOne.SuspendLayout();
            firstTwo.SuspendLayout();
            firstThree.SuspendLayout();
            finalStage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            SuspendLayout();
            // 
            // windowTitle
            // 
            windowTitle.BackColor = Color.FromArgb(21, 29, 38);
            windowTitle.Controls.Add(progressVisual);
            windowTitle.Controls.Add(titleExit);
            windowTitle.Controls.Add(titleLabel);
            windowTitle.Location = new Point(-77, -14);
            windowTitle.Margin = new Padding(4, 3, 4, 3);
            windowTitle.Name = "windowTitle";
            windowTitle.Size = new Size(741, 46);
            windowTitle.TabIndex = 5;
            windowTitle.MouseDown += windowTitle_MouseDown;
            // 
            // progressVisual
            // 
            progressVisual.BorderCapStyle = System.Drawing.Drawing2D.DashCap.Flat;
            progressVisual.BorderColor = Color.RoyalBlue;
            progressVisual.BorderColor2 = Color.HotPink;
            progressVisual.BorderLineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            progressVisual.BorderSize = 0;
            progressVisual.GradientAngle = 0F;
            progressVisual.Image = Properties.Resources.loading;
            progressVisual.Location = new Point(326, 17);
            progressVisual.Name = "progressVisual";
            progressVisual.Size = new Size(25, 25);
            progressVisual.SizeMode = PictureBoxSizeMode.StretchImage;
            progressVisual.TabIndex = 21;
            progressVisual.TabStop = false;
            progressVisual.Visible = false;
            // 
            // titleExit
            // 
            titleExit.AutoSize = true;
            titleExit.FlatStyle = FlatStyle.Flat;
            titleExit.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            titleExit.ForeColor = Color.FromArgb(183, 185, 191);
            titleExit.Location = new Point(597, 18);
            titleExit.Margin = new Padding(4, 0, 4, 0);
            titleExit.Name = "titleExit";
            titleExit.Size = new Size(23, 25);
            titleExit.TabIndex = 15;
            titleExit.Text = "X";
            titleExit.TextAlign = ContentAlignment.MiddleCenter;
            titleExit.Click += titleExit_Click;
            titleExit.MouseEnter += titleExit_MouseEnter;
            titleExit.MouseLeave += titleExit_MouseLeave;
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            titleLabel.ForeColor = Color.FromArgb(247, 247, 248);
            titleLabel.Location = new Point(86, 19);
            titleLabel.Margin = new Padding(4, 0, 4, 0);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(239, 17);
            titleLabel.TabIndex = 11;
            titleLabel.Text = "Shark Remote CE — Мастер настройки";
            // 
            // first
            // 
            first.AutoSize = true;
            first.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            first.ForeColor = Color.FromArgb(247, 247, 248);
            first.Location = new Point(18, 39);
            first.Margin = new Padding(4, 0, 4, 0);
            first.Name = "first";
            first.Size = new Size(152, 17);
            first.TabIndex = 12;
            first.Text = "Условия использования";
            first.Click += first_Click;
            // 
            // preparingTab
            // 
            preparingTab.Controls.Add(firstOne);
            preparingTab.Controls.Add(firstTwo);
            preparingTab.Controls.Add(firstThree);
            preparingTab.Controls.Add(finalStage);
            preparingTab.Location = new Point(9, 78);
            preparingTab.Name = "preparingTab";
            preparingTab.SelectedIndex = 0;
            preparingTab.Size = new Size(538, 326);
            preparingTab.TabIndex = 13;
            // 
            // firstOne
            // 
            firstOne.BackColor = Color.FromArgb(26, 32, 48);
            firstOne.Controls.Add(label10);
            firstOne.Controls.Add(label4);
            firstOne.Controls.Add(sharkOne);
            firstOne.Controls.Add(label11);
            firstOne.Controls.Add(sharkMessage);
            firstOne.Location = new Point(4, 24);
            firstOne.Name = "firstOne";
            firstOne.Padding = new Padding(3);
            firstOne.Size = new Size(530, 298);
            firstOne.TabIndex = 0;
            firstOne.Text = "fO";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label10.ForeColor = Color.FromArgb(247, 247, 248);
            label10.Location = new Point(344, 281);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(179, 34);
            label10.TabIndex = 24;
            label10.Text = "Версия: 4\r\nРазработчик: Zalexanninev15";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label4.ForeColor = Color.FromArgb(247, 247, 248);
            label4.Location = new Point(62, 281);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(268, 34);
            label4.TabIndex = 23;
            label4.Text = "Я соглашаюсь с условиями использования\r\nи обязуюсь исполнять их без нарушений";
            label4.Click += label4_Click;
            // 
            // sharkOne
            // 
            sharkOne.FlatAppearance.BorderColor = Color.FromArgb(192, 0, 0);
            sharkOne.Location = new Point(7, 285);
            sharkOne.Margin = new Padding(4, 3, 4, 3);
            sharkOne.MinimumSize = new Size(26, 25);
            sharkOne.Name = "sharkOne";
            sharkOne.OffBackColor = Color.FromArgb(21, 29, 38);
            sharkOne.OffToggleColor = Color.LightGray;
            sharkOne.OnBackColor = Color.FromArgb(0, 90, 181);
            sharkOne.OnToggleColor = Color.LightGray;
            sharkOne.Size = new Size(47, 25);
            sharkOne.TabIndex = 22;
            sharkOne.TabStop = false;
            sharkOne.UseVisualStyleBackColor = true;
            sharkOne.CheckedChanged += sharkOne_CheckedChanged;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label11.ForeColor = Color.FromArgb(247, 247, 248);
            label11.Location = new Point(171, 0);
            label11.Margin = new Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new Size(152, 17);
            label11.TabIndex = 12;
            label11.Text = "Условия использования";
            label11.Click += first_Click;
            // 
            // sharkMessage
            // 
            sharkMessage.BackColor = Color.FromArgb(38, 43, 59);
            sharkMessage.BorderStyle = BorderStyle.None;
            sharkMessage.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            sharkMessage.ForeColor = Color.FromArgb(247, 247, 248);
            sharkMessage.Location = new Point(6, 22);
            sharkMessage.Multiline = true;
            sharkMessage.Name = "sharkMessage";
            sharkMessage.ReadOnly = true;
            sharkMessage.ScrollBars = ScrollBars.Vertical;
            sharkMessage.Size = new Size(516, 248);
            sharkMessage.TabIndex = 20;
            sharkMessage.TabStop = false;
            sharkMessage.Text = resources.GetString("sharkMessage.Text");
            // 
            // firstTwo
            // 
            firstTwo.BackColor = Color.FromArgb(26, 32, 48);
            firstTwo.Controls.Add(label7);
            firstTwo.Controls.Add(label6);
            firstTwo.Controls.Add(label5);
            firstTwo.Controls.Add(label3);
            firstTwo.Controls.Add(botFatherLink);
            firstTwo.Controls.Add(label2);
            firstTwo.Controls.Add(label1);
            firstTwo.Controls.Add(getBotTokenNow);
            firstTwo.Location = new Point(4, 24);
            firstTwo.Name = "firstTwo";
            firstTwo.Padding = new Padding(3);
            firstTwo.Size = new Size(530, 298);
            firstTwo.TabIndex = 1;
            firstTwo.Text = "fT1";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Point);
            label7.ForeColor = Color.FromArgb(247, 247, 248);
            label7.Location = new Point(74, 222);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(47, 21);
            label7.TabIndex = 15;
            label7.Text = "/start";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Point);
            label6.ForeColor = Color.FromArgb(247, 247, 248);
            label6.Location = new Point(269, 102);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(47, 21);
            label6.TabIndex = 15;
            label6.Text = "/start";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F, FontStyle.Italic, GraphicsUnit.Point);
            label5.ForeColor = Color.FromArgb(247, 247, 248);
            label5.Location = new Point(93, 122);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(68, 21);
            label5.TabIndex = 15;
            label5.Text = "/newbot";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            label3.ForeColor = Color.FromArgb(247, 247, 248);
            label3.Location = new Point(327, 164);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(163, 17);
            label3.TabIndex = 15;
            label3.Text = "моножирным шрифтом";
            // 
            // botFatherLink
            // 
            botFatherLink.AutoSize = true;
            botFatherLink.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            botFatherLink.LinkColor = Color.Yellow;
            botFatherLink.Location = new Point(354, 82);
            botFatherLink.Name = "botFatherLink";
            botFatherLink.Size = new Size(91, 21);
            botFatherLink.TabIndex = 17;
            botFatherLink.TabStop = true;
            botFatherLink.Text = "@BotFather";
            botFatherLink.VisitedLinkColor = Color.Gainsboro;
            botFatherLink.LinkClicked += botFatherLink_LinkClicked;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label2.ForeColor = Color.FromArgb(247, 247, 248);
            label2.Location = new Point(14, 43);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(489, 200);
            label2.TabIndex = 16;
            label2.Text = resources.GetString("label2.Text");
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ForeColor = Color.FromArgb(247, 247, 248);
            label1.Location = new Point(73, 19);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(375, 20);
            label1.TabIndex = 15;
            label1.Text = "Для продолжения необходимо вставить токен бота!";
            // 
            // getBotTokenNow
            // 
            getBotTokenNow.BackColor = Color.MediumSlateBlue;
            getBotTokenNow.BackgroundColor = Color.MediumSlateBlue;
            getBotTokenNow.BorderColor = Color.MediumSlateBlue;
            getBotTokenNow.BorderRadius = 6;
            getBotTokenNow.BorderSize = 4;
            getBotTokenNow.FlatAppearance.BorderColor = Color.FromArgb(105, 88, 205);
            getBotTokenNow.FlatAppearance.BorderSize = 0;
            getBotTokenNow.FlatAppearance.MouseDownBackColor = Color.FromArgb(105, 88, 205);
            getBotTokenNow.FlatAppearance.MouseOverBackColor = Color.FromArgb(105, 88, 205);
            getBotTokenNow.FlatStyle = FlatStyle.Flat;
            getBotTokenNow.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            getBotTokenNow.ForeColor = Color.White;
            getBotTokenNow.Location = new Point(186, 256);
            getBotTokenNow.Name = "getBotTokenNow";
            getBotTokenNow.Size = new Size(135, 32);
            getBotTokenNow.TabIndex = 18;
            getBotTokenNow.TabStop = false;
            getBotTokenNow.Text = "Вставить токен";
            getBotTokenNow.TextColor = Color.White;
            getBotTokenNow.UseVisualStyleBackColor = false;
            getBotTokenNow.Click += getBotTokenNow_Click;
            // 
            // firstThree
            // 
            firstThree.BackColor = Color.FromArgb(26, 32, 48);
            firstThree.Controls.Add(label14);
            firstThree.Controls.Add(username);
            firstThree.Controls.Add(label13);
            firstThree.Controls.Add(vitnX2_Button3);
            firstThree.Controls.Add(setUsername);
            firstThree.Controls.Add(label8);
            firstThree.Location = new Point(4, 24);
            firstThree.Name = "firstThree";
            firstThree.Padding = new Padding(3);
            firstThree.Size = new Size(530, 298);
            firstThree.TabIndex = 2;
            firstThree.Text = "fT0";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label14.ForeColor = Color.FromArgb(247, 247, 248);
            label14.Location = new Point(75, 70);
            label14.Margin = new Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new Size(401, 40);
            label14.TabIndex = 23;
            label14.Text = "Для продолжения необходимо написать свой username\r\n(можно также вставить ссылку на свой профиль)";
            label14.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // username
            // 
            username.BackColor = Color.FromArgb(21, 29, 38);
            username.BorderColor = Color.FromArgb(25, 80, 135);
            username.BorderFocusColor = Color.FromArgb(25, 80, 135);
            username.BorderRadius = 3;
            username.BorderSize = 1;
            username.CausesValidation = false;
            username.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            username.ForeColor = Color.FromArgb(247, 247, 248);
            username.Location = new Point(184, 228);
            username.Margin = new Padding(0);
            username.Multiline = false;
            username.Name = "username";
            username.Padding = new Padding(4);
            username.PasswordChar = false;
            username.PlaceholderColor = Color.FromArgb(247, 247, 248);
            username.PlaceholderText = "";
            username.ReadOnly = false;
            username.Size = new Size(142, 24);
            username.TabIndex = 27;
            username.TabStop = false;
            username.Texts = "";
            username.UnderlinedStyle = false;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label13.ForeColor = Color.FromArgb(247, 247, 248);
            label13.Location = new Point(59, 94);
            label13.Margin = new Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new Size(426, 120);
            label13.TabIndex = 24;
            label13.Text = resources.GetString("label13.Text");
            label13.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // vitnX2_Button3
            // 
            vitnX2_Button3.BackColor = Color.MediumSlateBlue;
            vitnX2_Button3.BackgroundColor = Color.MediumSlateBlue;
            vitnX2_Button3.BorderColor = Color.MediumSlateBlue;
            vitnX2_Button3.BorderRadius = 6;
            vitnX2_Button3.BorderSize = 4;
            vitnX2_Button3.FlatAppearance.BorderColor = Color.FromArgb(105, 88, 205);
            vitnX2_Button3.FlatAppearance.BorderSize = 0;
            vitnX2_Button3.FlatAppearance.MouseDownBackColor = Color.FromArgb(105, 88, 205);
            vitnX2_Button3.FlatAppearance.MouseOverBackColor = Color.FromArgb(105, 88, 205);
            vitnX2_Button3.FlatStyle = FlatStyle.Flat;
            vitnX2_Button3.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            vitnX2_Button3.ForeColor = Color.White;
            vitnX2_Button3.Location = new Point(329, 228);
            vitnX2_Button3.Name = "vitnX2_Button3";
            vitnX2_Button3.Size = new Size(25, 24);
            vitnX2_Button3.TabIndex = 26;
            vitnX2_Button3.TabStop = false;
            vitnX2_Button3.Text = "V";
            vitnX2_Button3.TextColor = Color.White;
            vitnX2_Button3.UseVisualStyleBackColor = false;
            vitnX2_Button3.Click += vitnX2_Button3_Click;
            // 
            // setUsername
            // 
            setUsername.BackColor = Color.MediumSlateBlue;
            setUsername.BackgroundColor = Color.MediumSlateBlue;
            setUsername.BorderColor = Color.MediumSlateBlue;
            setUsername.BorderRadius = 6;
            setUsername.BorderSize = 4;
            setUsername.FlatAppearance.BorderColor = Color.FromArgb(105, 88, 205);
            setUsername.FlatAppearance.BorderSize = 0;
            setUsername.FlatAppearance.MouseDownBackColor = Color.FromArgb(105, 88, 205);
            setUsername.FlatAppearance.MouseOverBackColor = Color.FromArgb(105, 88, 205);
            setUsername.FlatStyle = FlatStyle.Flat;
            setUsername.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            setUsername.ForeColor = Color.White;
            setUsername.Location = new Point(184, 259);
            setUsername.Name = "setUsername";
            setUsername.Size = new Size(170, 32);
            setUsername.TabIndex = 26;
            setUsername.TabStop = false;
            setUsername.Text = "Завершить настройку";
            setUsername.TextColor = Color.White;
            setUsername.UseVisualStyleBackColor = false;
            setUsername.Click += setUsername_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label8.ForeColor = Color.FromArgb(247, 247, 248);
            label8.Location = new Point(147, 70);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(20, 20);
            label8.TabIndex = 16;
            label8.Text = "ы";
            // 
            // finalStage
            // 
            finalStage.BackColor = Color.FromArgb(26, 32, 48);
            finalStage.Controls.Add(pictureBox4);
            finalStage.Controls.Add(vitnX2_Button2);
            finalStage.Controls.Add(progress);
            finalStage.Controls.Add(vitnX2_Button1);
            finalStage.Controls.Add(label12);
            finalStage.Controls.Add(label9);
            finalStage.Location = new Point(4, 24);
            finalStage.Name = "finalStage";
            finalStage.Padding = new Padding(3);
            finalStage.Size = new Size(530, 298);
            finalStage.TabIndex = 3;
            finalStage.Text = "finalStage";
            // 
            // pictureBox4
            // 
            pictureBox4.Image = Properties.Resources.settings;
            pictureBox4.Location = new Point(488, 6);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(34, 30);
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.TabIndex = 34;
            pictureBox4.TabStop = false;
            pictureBox4.Click += pictureBox4_Click;
            // 
            // vitnX2_Button2
            // 
            vitnX2_Button2.BackColor = Color.FromArgb(38, 43, 59);
            vitnX2_Button2.BackgroundColor = Color.FromArgb(38, 43, 59);
            vitnX2_Button2.BorderColor = Color.FromArgb(21, 29, 38);
            vitnX2_Button2.BorderRadius = 5;
            vitnX2_Button2.BorderSize = 2;
            vitnX2_Button2.FlatAppearance.BorderColor = Color.FromArgb(21, 29, 38);
            vitnX2_Button2.FlatAppearance.MouseDownBackColor = Color.FromArgb(39, 45, 59);
            vitnX2_Button2.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 45, 59);
            vitnX2_Button2.FlatStyle = FlatStyle.Flat;
            vitnX2_Button2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            vitnX2_Button2.ForeColor = Color.FromArgb(247, 247, 248);
            vitnX2_Button2.Location = new Point(152, 201);
            vitnX2_Button2.Margin = new Padding(4, 3, 4, 3);
            vitnX2_Button2.Name = "vitnX2_Button2";
            vitnX2_Button2.Size = new Size(212, 46);
            vitnX2_Button2.TabIndex = 33;
            vitnX2_Button2.TabStop = false;
            vitnX2_Button2.Text = "Использовать Shark Remote\r\nв виде Службы Windows";
            vitnX2_Button2.TextColor = Color.FromArgb(247, 247, 248);
            vitnX2_Button2.UseVisualStyleBackColor = false;
            vitnX2_Button2.Click += vitnX2_Button2_Click;
            // 
            // progress
            // 
            progress.CustomText = "";
            progress.Location = new Point(152, 253);
            progress.Name = "progress";
            progress.ProgressColor = Color.FromArgb(0, 90, 181);
            progress.Size = new Size(212, 10);
            progress.Style = ProgressBarStyle.Marquee;
            progress.TabIndex = 32;
            progress.TextColor = Color.Black;
            progress.TextFont = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            progress.Visible = false;
            progress.VisualMode = VitNX2.UI.ControlsV2.Helper.VitNX2_ProgressBarDisplayMode.NoText;
            // 
            // vitnX2_Button1
            // 
            vitnX2_Button1.BackColor = Color.MediumSlateBlue;
            vitnX2_Button1.BackgroundColor = Color.MediumSlateBlue;
            vitnX2_Button1.BorderColor = Color.MediumSlateBlue;
            vitnX2_Button1.BorderRadius = 6;
            vitnX2_Button1.BorderSize = 4;
            vitnX2_Button1.FlatAppearance.BorderColor = Color.FromArgb(105, 88, 205);
            vitnX2_Button1.FlatAppearance.BorderSize = 0;
            vitnX2_Button1.FlatAppearance.MouseDownBackColor = Color.FromArgb(105, 88, 205);
            vitnX2_Button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(105, 88, 205);
            vitnX2_Button1.FlatStyle = FlatStyle.Flat;
            vitnX2_Button1.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            vitnX2_Button1.ForeColor = Color.White;
            vitnX2_Button1.Location = new Point(143, 136);
            vitnX2_Button1.Name = "vitnX2_Button1";
            vitnX2_Button1.Size = new Size(233, 32);
            vitnX2_Button1.TabIndex = 27;
            vitnX2_Button1.TabStop = false;
            vitnX2_Button1.Text = "Запустить и использовать Shark Remote";
            vitnX2_Button1.TextColor = Color.White;
            vitnX2_Button1.UseVisualStyleBackColor = false;
            vitnX2_Button1.Click += vitnX2_Button1_Click;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label12.ForeColor = Color.FromArgb(247, 247, 248);
            label12.Location = new Point(240, 175);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new Size(35, 20);
            label12.TabIndex = 24;
            label12.Text = "или";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label9.ForeColor = Color.FromArgb(247, 247, 248);
            label9.Location = new Point(143, 109);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(233, 20);
            label9.TabIndex = 24;
            label9.Text = "Настройка успешно завершена!";
            // 
            // second
            // 
            second.AutoSize = true;
            second.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            second.ForeColor = Color.FromArgb(247, 247, 248);
            second.Location = new Point(217, 39);
            second.Margin = new Padding(4, 0, 4, 0);
            second.Name = "second";
            second.Size = new Size(75, 17);
            second.TabIndex = 14;
            second.Text = "Токен бота";
            second.Click += second_Click;
            // 
            // secondSecond
            // 
            secondSecond.AutoSize = true;
            secondSecond.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            secondSecond.ForeColor = Color.FromArgb(247, 247, 248);
            secondSecond.Location = new Point(356, 39);
            secondSecond.Margin = new Padding(4, 0, 4, 0);
            secondSecond.Name = "secondSecond";
            secondSecond.Size = new Size(169, 17);
            secondSecond.TabIndex = 14;
            secondSecond.Text = "Добавление пользователя";
            secondSecond.Click += secondSecond_Click;
            // 
            // visualProgress
            // 
            visualProgress.CustomText = "";
            visualProgress.Location = new Point(9, 62);
            visualProgress.Name = "visualProgress";
            visualProgress.ProgressColor = Color.FromArgb(0, 90, 181);
            visualProgress.Size = new Size(526, 10);
            visualProgress.Style = ProgressBarStyle.Marquee;
            visualProgress.TabIndex = 31;
            visualProgress.TextColor = Color.Black;
            visualProgress.TextFont = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            visualProgress.VisualMode = VitNX2.UI.ControlsV2.Helper.VitNX2_ProgressBarDisplayMode.NoText;
            // 
            // Preparing
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(26, 32, 48);
            ClientSize = new Size(547, 409);
            Controls.Add(visualProgress);
            Controls.Add(secondSecond);
            Controls.Add(second);
            Controls.Add(preparingTab);
            Controls.Add(first);
            Controls.Add(windowTitle);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Preparing";
            Opacity = 0.98D;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Мастер настройки";
            windowTitle.ResumeLayout(false);
            windowTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)progressVisual).EndInit();
            preparingTab.ResumeLayout(false);
            firstOne.ResumeLayout(false);
            firstOne.PerformLayout();
            firstTwo.ResumeLayout(false);
            firstTwo.PerformLayout();
            firstThree.ResumeLayout(false);
            firstThree.PerformLayout();
            finalStage.ResumeLayout(false);
            finalStage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel windowTitle;
        private Label first;
        private Label second;
        private Label titleLabel;
        private VitNX2.UI.ControlsV2.VitNX2_Tab preparingTab;
        private TabPage firstOne;
        private TabPage firstTwo;
        private TextBox sharkMessage;
        private Label secondSecond;
        private VitNX2.UI.ControlsV2.VitNX2_ToogleButton sharkOne;
        private Label label4;
        private Label titleExit;
        private Label label1;
        private Label label2;
        private LinkLabel botFatherLink;
        private Label label5;
        private Label label3;
        private Label label6;
        private Label label7;
        private VitNX2.UI.ControlsV2.VitNX2_Button getBotTokenNow;
        private TabPage firstThree;
        private Label label8;
        private TabPage finalStage;
        private Label label13;
        private Label label14;
        private VitNX2.UI.ControlsV2.VitNX2_Button setUsername;
        private Label label9;
        private VitNX2.UI.ControlsV2.VitNX2_Button vitnX2_Button1;
        private Label label10;
        private Label label11;
        private VitNX2.UI.ControlsV2.VitNX2_TextBox username;
        private VitNX2.UI.ControlsV2.VitNX2_Button vitnX2_Button3;
        private Label label12;
        private VitNX2.UI.ControlsV2.VitNX2_ProgressBarRoundedType1 visualProgress;
        private VitNX2.UI.ControlsV2.VitNX2_ProgressBarRoundedType1 progress;
        private VitNX2.UI.ControlsV2.VitNX2_Button vitnX2_Button2;
        private VitNX2.UI.ControlsV2.VitNX2_PictureBox progressVisual;
        private PictureBox pictureBox4;
    }
}