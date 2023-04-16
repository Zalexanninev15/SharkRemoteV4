using LibreHardwareMonitor.Hardware;

using Microsoft.Win32;
using Shark_Remote.Engine;
using Shark_Remote.Engine.Bot;
using Shark_Remote.Helpers;
using Shark_Remote.Helpers.imgBB;
using Shark_Remote.Properties;

using System.Data;
using System.Drawing.Printing;
using System.Globalization;
using System.IO.Compression;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.RegularExpressions;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

using Tommy;

using VitNX2.UI.ControlsV2;
using VitNX3.Functions.Win32;
using VitNX3.Functions.WinControllers;

using WindowsInput;

using static Shark_Remote.Engine.API.Functions;

using Constants = VitNX3.Functions.Win32.Constants;
using DateTime = System.DateTime;
using Dir = VitNX3.Functions.FileSystem.Folder;
using File = VitNX3.Functions.FileSystem.File;
using Network = Shark_Remote.Helpers.Network;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using Processes = VitNX3.Functions.AppsAndProcesses.Processes;
using SysFile = System.IO.File;
using SystemInformation = System.Windows.Forms.SystemInformation;

namespace Shark_Remote
{
    public partial class MainForm : Form
    {
        public bool
            taskMgrOff = true;

        public string
            tempPath = $@"{FileSystem.GetDataPath()}\temp",
            ip = VitNX3.Functions.Information.Internet.GetPublicIP(),
            tfileStr,
            tmp;

        public string[]
            plugins,
            pluginsAction;

        private VitNX3.Functions.SettingsAndLog.Log log = new VitNX3.Functions.SettingsAndLog.Log($@"{FileSystem.GetDataPath()}\shark_remote.log");

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= Constants.CS_DROPSHADOW;
                return cp;
            }
        }

        public MainForm()
        {
            InitializeComponent();
            KeyPreview = true;
            Values.AppModes.preparing_enabled = false;
            versionLabel.Text = versionLabelText;
            ServicePointManager.SecurityProtocol = VitNX3.Functions.Web.Config.UseProtocols();
            Import.ReleaseCapture();
            try { Values.AppModes.mini = Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_mini_mode")); } catch { }
            if (Values.AppModes.mini)
            {
                ClientSize = new Size(UI.Window.Dpi(210), UI.Window.Dpi(229));
                Size = new Size(UI.Window.Dpi(210), UI.Window.Dpi(229));
                label21.Visible = true;
                label22.Visible = true;
                modeChanger.Visible = true;
                vitnX2_Panel4.Visible = true;
            }
            else
            {
                ClientSize = new Size(UI.Window.Dpi(757), UI.Window.Dpi(362));
                Size = new Size(UI.Window.Dpi(757), UI.Window.Dpi(362));
                label21.Visible = false;
                label22.Visible = false;
                modeChanger.Visible = false;
                vitnX2_Panel4.Visible = false;
            }
            try { VitNX3.Functions.WindowAndControls.Controls.SetNativeThemeForControls(eventsLog.Handle); } catch { }
            try { VitNX3.Functions.WindowAndControls.Controls.SetNativeThemeForControls(varsList.Handle); } catch { }
            try { VitNX3.Functions.WindowAndControls.Controls.SetNativeThemeForControls(pluginsManagerList.Handle); } catch { }
            Values.AppUI.use_forced_performance = Convert.ToInt32(Engine.Settings.Read("UI", "use_forced_performance")) == 1 ? true : false;
            if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_rounded_window_frame_style")) == true && !Values.AppUI.use_forced_performance)
            {
                try
                {
                    if (Convert.ToInt64(VitNX3.Functions.Information.Windows.GetWindowsCurrentBuildNumberFromRegistry()) >= 2200)
                        Region = VitNX3.Functions.WindowAndControls.Window.SetWindowsElevenStyleForWinForm(Handle, Width, Height);
                    else
                        Region = Region.FromHrgn(Import.CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
                }
                catch { Region = Region.FromHrgn(Import.CreateRoundRectRgn(0, 0, Width, Height, 15, 15)); }
            }
            Import.SetProcessDpiAwareness(Enums.PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE);
            Home.MouseEnter += new EventHandler(UI.MyButton_MouseEnter);
            Home.MouseLeave += new EventHandler(UI.MyButton_MouseLeave);
            Settings.MouseEnter += new EventHandler(UI.MyButton_MouseEnter);
            Settings.MouseLeave += new EventHandler(UI.MyButton_MouseLeave);
            Plugins.MouseEnter += new EventHandler(UI.MyButton_MouseEnter);
            Plugins.MouseLeave += new EventHandler(UI.MyButton_MouseLeave);
            Help.MouseEnter += new EventHandler(UI.MyButton_MouseEnter);
            Help.MouseLeave += new EventHandler(UI.MyButton_MouseLeave);
            GetSettings();
            Import.SetThreadExecutionState(Enums.EXECUTION_STATE.ES_CONTINUOUS |
                Enums.EXECUTION_STATE.ES_SYSTEM_REQUIRED |
                Enums.EXECUTION_STATE.ES_DISPLAY_REQUIRED);
            if (Network.InternetOk() == false)
            {
                if (Values.AppModes.service)
                    AddEvent("Cannot startup because no Internet connection was detected!", true);
                else
                    VitNX2_MessageBox.Show("Нет доступа в сеть!",
                        "Ошибка соединения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                log.Write("Нет доступа в сеть!");
                Processes.KillNative($"Shark Remote.exe");
            }
            botPowerControl.Checked = true;
            if (!Values.AppUI.use_forced_performance)
            {
                if (!Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_transparency")))
                {
                    if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_animation")))
                    {
                        Opacity = 0;
                        System.Windows.Forms.Timer launch = new System.Windows.Forms.Timer();
                        launch.Tick += new EventHandler((sender, e) =>
                        {
                            if ((Opacity += 0.05d) == 1)
                                launch.Stop();
                        });
                        launch.Interval = 20;
                        launch.Start();
                    }
                }
                else
                    Opacity = 0.96;
            }
            if (!VitNX3.Functions.Web.DataFromSites.IsValidTelegramBotToken(Engine.Settings.MemoryValues.token))
            {
                botPowerControlStatus.ForeColor = Color.FromArgb(215, 99, 90);
                botPowerControlStatus.Texts = "Деактивирован";
                statusBot.Text = "Откл.";
                eventsLog.Items.Clear();
                AddEvent("Ожидаю включения...");
            }
            botPowerControlStatus.ReadOnly = true;
            botName.ReadOnly = true;
            botId.ReadOnly = true;
            botUsername.ReadOnly = true;
            TopMost = Values.AppHiddenParameters.W_TOP;
            if (Values.AppHiddenParameters.W_HIDE)
                Hide();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_rounded_window_frame_style")) == false)
            {
                base.OnPaint(e);
                Graphics g = e.Graphics;
                Rectangle rect = new Rectangle(new Point(0, 0), new Size(Width, Height));
                Pen pen = new Pen(Color.FromArgb(26, 32, 48));
                g.DrawRectangle(pen, rect);
            }
        }

        private void GetSettings(bool readUiSettings = true)
        {
            Engine.Settings.MemoryValues.token = Engine.Settings.Read("BOT", "token");
            username.Texts = Values.Config.ReadUsername(true);
            if (Engine.Settings.MemoryValues.token.Length < 15)
                Engine.Settings.MemoryValues.token = "";
            if (readUiSettings)
            {
                if (Values.AppUI.use_forced_performance)
                {
                    selectedMenu.BackColor = Color.FromArgb(33, 61, 92);
                    Values.AppModes.mini = true;
                    label21.Visible = true;
                    label22.Visible = true;
                    modeChanger.Visible = true;
                    vitnX2_Panel4.Visible = true;
                    ClientSize = new Size(UI.Window.Dpi(210), UI.Window.Dpi(229));
                    Size = new Size(UI.Window.Dpi(210), UI.Window.Dpi(229));
                }
                else
                {
                    string color_name = Engine.Settings.Read("UI", "menu_color");
                    switch (color_name)
                    {
                        case "default":
                            selectedMenu.BackColor = Color.FromArgb(33, 61, 92);
                            break;

                        case "keyboard":
                            selectedMenu.BackColor = Color.FromArgb(214, 222, 228);
                            break;

                        case "unigram":
                            selectedMenu.BackColor = Color.FromArgb(69, 105, 147);
                            break;

                        case "vivaldi":
                            selectedMenu.BackColor = Color.FromArgb(234, 56, 56);
                            break;

                        case "github":
                            selectedMenu.BackColor = Color.FromArgb(117, 47, 156);
                            break;

                        case "μtorrent":
                            selectedMenu.BackColor = Color.FromArgb(141, 196, 95);
                            break;

                        case "native":
                            selectedMenu.BackColor = VitNX3.Functions.Information.Windows.GetWindowsAccentColor();
                            botPowerControl.OnBackColor = selectedMenu.BackColor;
                            break;

                        case "happy_new_year":
                            selectedMenu.BackColor = Color.FromArgb(197, 66, 69);
                            botPowerControl.OnBackColor = selectedMenu.BackColor;
                            break;

                        case "happy_new_year_with_icons":
                            sc(false);
                            break;

                        case "happy_new_year_with_icons_and_hide_log":
                            sc();
                            break;

                        default:
                            selectedMenu.BackColor = Color.FromArgb(33, 61, 92);
                            break;
                    }
                }
            }
            try
            {
                if (!Directory.Exists($@"{FileSystem.GetDataPath()}\plugins"))
                    Directory.CreateDirectory($@"{FileSystem.GetDataPath()}\plugins");
                if (!SysFile.Exists($@"{FileSystem.GetDataPath()}\plugins\installed.cfg"))
                    SysFile.WriteAllText($@"{FileSystem.GetDataPath()}\plugins\installed.cfg", "");
            }
            catch (Exception ex)
            {
                if (Values.AppModes.service)
                    Console.WriteLine($"Error list with plugins!\n{ex.Message}");
                else
                    VitNX2_MessageBox.Show($"Ошибка списка плагинов:\n{ex.Message}", "Невозможно загрузить плагины", MessageBoxButtons.OK, MessageBoxIcon.Error);
                log.Write($"Ошибка списка плагинов: {ex.Message}");
            }
        }

        public void AddEvent(string text,
            bool onlyLog = false)
        {
            text = text.Trim();
            if (!onlyLog)
            {
                eventsLog.Items.Add(text);
                log.Write(text);
            }
            else
                log.Write($"[H] {text}");
        }

        public static bool IsChanged = false;

        private void Home_Click(object sender, EventArgs e)
        {
            if (IsChanged)
                VitNX2_MessageBox.Show("Вы забыли применить настройки!",
                     "Невозможно применить настройки!",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Warning);
            else
            {
                selectedMenu.Location = new Point(11, 166);
                burgerControl.SelectedIndex = 0;
            }
        }

        public bool firstScCheck = false;

        private void Settings_Click(object sender, EventArgs? e)
        {
            if (botPowerControl.Checked)
                VitNX2_MessageBox.Show("Выключите бота, чтобы воспользоваться настройками!",
                "Требуется выключить бота!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            else
            {
                selectedMenu.Location = new Point(11, 200);
                if (Directory.Exists(Values.AppInfo.service_path_tool) && Service.IsInstalled())
                {
                    firstScCheck = true;
                    vitnX2_ToogleButton2.Checked = true;
                }
                GetSettings(false);
                varsList.Items.Clear();
                varsList.Items.AddRange(SysFile.ReadAllLines($@"{FileSystem.GetDataPath()}\settings\variables.txt"));
                burgerControl.SelectedIndex = 1;
            }
        }

        private void Plugins_Click(object sender, EventArgs? e)
        {
            if (IsChanged)
                VitNX2_MessageBox.Show("Вы забыли применить настройки!",
                     "Применение настроек!",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Warning);
            else
            {
                selectedMenu.Location = new Point(11, 235);
                burgerControl.SelectedIndex = 2;
            }
        }

        private void Help_Click(object sender, EventArgs? e)
        {
            UI.OpenLink("https://teletype.in/@zalexanninev15/Shark-Remote-Documentation");
        }

        private async void BotPower_CheckedChangedAsync(object sender, EventArgs e)
        {
            botName.Texts = "";
            botUsername.Texts = "";
            botId.Texts = "";
            botPowerControlStatus.Texts = "";
            GetSettings(false);
            var botClient = new TelegramBotClient(Engine.Settings.MemoryValues.token);
            using (var cts = new CancellationTokenSource())
            {
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = Array.Empty<UpdateType>()
                };
                if (VitNX3.Functions.Web.DataFromSites.IsValidTelegramBotToken(Engine.Settings.MemoryValues.token))
                {
                    if (botPowerControl.Checked)
                    {
                        botPowerControl.Enabled = false;
                        statusBot.Text = "Подкл.?";
                        try
                        {
                            botClient.StartReceiving(
                            HandleUpdateAsync,
                            HandlePollingErrorAsync,
                            receiverOptions,
                            cts.Token);
                            statusBot.Text = "Вкл.";
                            show_first_message = true;
                            botPowerControl.Enabled = true;
                            var botInfo = botClient.GetMeAsync().Result;
                            botName.Texts = botInfo.FirstName;
                            botUsername.Texts = botInfo.Username;
                            botId.Texts = botInfo.Id.ToString();
                            AddEvent($"Имя бота = {botName.Texts}", true);
                            AddEvent($"Username бота = {botUsername.Texts}", true);
                            AddEvent($"ID бота = {botId.Texts}", true);
                            botPowerControlStatus.ForeColor = Color.FromArgb(153, 230, 153);
                            botPowerControlStatus.Texts = "Активирован";
                            if (show_first_message)
                            {
                                try
                                {
                                    string s = Values.Config.ReadUsername(true);
                                    if (s.Contains('|'))
                                        await SendNotification(botClient, s.Split('|')[1], $"🟢 Компьютер <i>{Dns.GetHostName()}</i> (<b>{ip}</b>) включён!");
                                }
                                catch { AddEvent("Уведомления отключены!"); }
                            }
                            else
                                AddEvent("Уведомления отключены!");
                            AddEvent($"Ожидаю ввода команд...");
                        }
                        catch (Exception ex)
                        {
                            try { cts.Cancel(); } catch { }
                            botPowerControlStatus.ForeColor = Color.FromArgb(198, 87, 96);
                            botPowerControlStatus.Texts = "Ошибка";
                            show_first_message = false;
                            botPowerControl.Enabled = true;
                            statusBot.Text = "Откл.";
                            AddEvent(ex.Message, true);
                            botPowerControl.Checked = false;
                        }
                    }
                    else
                    {
                        if (show_first_message)
                        {
                            try
                            {
                                string s = Values.Config.ReadUsername(true);
                                if (s.Contains('|'))
                                    await SendNotification(botClient, s.Split('|')[1], $"🛑 Компьютер <i>{Dns.GetHostName()}</i> (<b>{ip}</b>) отключён!");
                            }
                            catch { AddEvent("Уведомления отключены!"); }
                        }
                        else
                            AddEvent("Уведомления отключены!");
                        try { cts.Cancel(); } catch { }
                        show_first_message = false;
                        botPowerControlStatus.ForeColor = Color.FromArgb(215, 99, 90);
                        botPowerControlStatus.Texts = "Деактивирован";
                        statusBot.Text = "Откл.";
                        eventsLog.Items.Clear();
                        AddEvent("Ожидаю включения...");
                    }
                }
                else
                {
                    try { cts.Cancel(); } catch { }
                    show_first_message = false;
                    botPowerControlStatus.ForeColor = Color.FromArgb(198, 87, 96);
                    botPowerControlStatus.Texts = "Ошибка";
                    statusBot.Text = "Откл.";
                    Engine.Settings.MemoryValues.token = "";
                    eventsLog.Items.Clear();
                    AddEvent("Ожидаю включения...");
                }
            }
        }

        public bool show_first_message = false;

        public async Task HandleUpdateAsync(ITelegramBotClient botClient,
            Update update,
            CancellationToken cancellationToken)
        {
            try
            {
                if (botPowerControl.Checked)
                {
                    if (update.Message is not { } message)
                        return;

                    var chatId = update.Message.Chat.Id;
                    var username = update.Message.Chat.Username;

                    if ((Values.Config.ReadUsername() == username) || (Convert.ToString(chatId).Contains(Values.Config.ReadUserId())))
                    {
                        if (update.Message.Type == MessageType.Text)
                        {
                            await HandleMessageAsync(botClient,
                                update.Message,
                                cancellationToken);
                            return;
                        }
                        if (update.Message.Type == MessageType.Document)
                        {
                            await HandleFileAsync(botClient,
                                update.Message);
                            return;
                        }
                        if (update.Message.Type == MessageType.Audio)
                        {
                            await HandleAudioAsync(botClient,
                                update.Message);
                            return;
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(chatId: chatId,
                        text: "🔒 <b>Access to the bot is only allowed to users whose <b>username</b> is entered in the list of users in the application settings!</b>",
                        parseMode: ParseMode.Html, cancellationToken: cancellationToken);
                        BeginInvoke(new Action(() => { AddEvent($"Пользователь @{username} попытался получить доступ к боту!"); }));
                    }
                }
            }
            catch (Exception ex) { AddEvent($"Ошибка: {ex.Message}"); }
        }

        private async Task HandleFileAsync(ITelegramBotClient botClient,
            Telegram.Bot.Types.Message message)
        {
            var chatId = message.Chat.Id;
            var username = message.Chat.Username;
            var messageDocument = message.Document;
            var messageCaption = message.Caption;
            if (!Directory.Exists("saved"))
                Directory.CreateDirectory("saved");
            if (!Directory.Exists($@"{FileSystem.GetDataPath()}\cache"))
                Directory.CreateDirectory(@$"{FileSystem.GetDataPath()}\cache");
            BeginInvoke(new Action(() =>
            {
                AddEvent($"Получаю файл '{messageDocument.FileName}' от @{username}...");
            }));
            try
            {
                var documentMessageTypeFileId = messageDocument.FileId;
                var documentMessageTypeFileInfo = await botClient.GetFileAsync(documentMessageTypeFileId);
                var documentMessageTypeFilePath = documentMessageTypeFileInfo.FilePath;
                var documentMessageTypeFileSize = documentMessageTypeFileInfo.FileSize;
                var documentMessageTypeFileName = messageDocument.FileName;
                if (messageCaption == "" || messageCaption == null)
                {
                    await botClient.SendChatActionAsync(chatId: chatId,
                    ChatAction.Typing);
                    var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                        text: "🟡 Сохраняю файл...");
                    string destinationFilePath = @$"{Values.AppInfo.startup_path}saved\{documentMessageTypeFileName}";
                    try
                    {
                        if (SysFile.Exists(destinationFilePath))
                            File.DeleteForever(destinationFilePath);
                    }
                    catch { }
                    await DownloadContentManager(botClient,
                        message,
                        destinationFilePath,
                        ContentType.File);
                    await botClient.EditMessageTextAsync(chatId: chatId,
                            messageId: processMessage.MessageId,
                            text: "😋 <b>Файл сохранён в папку <code>saved</code>!</b>",
                            parseMode: ParseMode.Html);
                }
                else
                {
                    if (Convert.ToString(messageCaption).ToLower() == "desktop")
                    {
                        await botClient.SendChatActionAsync(chatId: chatId,
                        ChatAction.Typing);
                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                        text: "🟡 Сохраняю файл и применяю как фон рабочего стола...");
                        string destinationFilePath = @$"{FileSystem.GetDataPath()}\cache\desktop_wallpaper.image";
                        try
                        {
                            if (SysFile.Exists(destinationFilePath))
                                File.DeleteForever(destinationFilePath);
                        }
                        catch { }
                        await DownloadContentManager(botClient,
                            message,
                            destinationFilePath,
                            ContentType.File);
                        await botClient.EditMessageTextAsync(chatId: chatId,
                            messageId: processMessage.MessageId,
                            text: "😋 <b>Файл сохранён в кэше и применён как фоновое изображение для Рабочего стола!</b>",
                            parseMode: ParseMode.Html);
                        DesktopWallpaper.Set(destinationFilePath);
                    }
                    if (Convert.ToString(messageCaption).ToLower() == "tprint")
                    {
                        await botClient.SendChatActionAsync(chatId: chatId, ChatAction.Typing);
                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                        text: "🟡 Печатаю...");
                        await botClient.SendChatActionAsync(chatId: chatId,
                            ChatAction.Typing);
                        string destinationFilePath = @$"{FileSystem.GetDataPath()}\cache\{File.NameGenerator("print", "text")}";
                        try
                        {
                            if (SysFile.Exists(destinationFilePath))
                                File.DeleteForever(destinationFilePath);
                        }
                        catch { }
                        await DownloadContentManager(botClient,
                            message,
                            destinationFilePath,
                            ContentType.File);
                        tfileStr = SysFile.ReadAllText(destinationFilePath);

                        PrintDocument printDocument = new PrintDocument();
                        printDocument.PrintPage += PrintPageHandler;
                        PrintDialog printDialog = new PrintDialog();
                        printDialog.Document = printDocument;
                        printDialog.Document.Print();
                        await botClient.EditMessageTextAsync(chatId: chatId,
                            messageId: processMessage.MessageId,
                            text: "😋 <b>Файл сохранён в кэше и распечатан!</b>",
                            parseMode: ParseMode.Html);
                    }
                }
                BeginInvoke(new Action(() =>
                {
                    AddEvent($"Получен файл '{messageDocument.FileName}' от @{username}");
                }));
            }
            catch (Exception ex)
            {
                BeginInvoke(new Action(() =>
                {
                    AddEvent($"Файл '{messageDocument.FileName}' от @{username} недоступен!");
                    if (ex.Message.Contains("Bad Request: file is too big"))
                        AddEvent($"[TelegramBotAPI] Размер файла слишком большой!");
                }));
                AddEvent(ex.Message, true);
            }
        }

        private async Task HandleAudioAsync(ITelegramBotClient botClient,
            Telegram.Bot.Types.Message message)
        {
            var chatId = message.Chat.Id;
            var username = message.Chat.Username;
            var messageAudio = message.Audio;
            var messageCaption = message.Caption;
            if (!Directory.Exists("saved"))
                Directory.CreateDirectory("saved");
            if (!Directory.Exists($@"{FileSystem.GetDataPath()}\cache"))
                Directory.CreateDirectory(@$"{FileSystem.GetDataPath()}\cache");
            BeginInvoke(new Action(() =>
            {
                AddEvent($"Получаю аудио '{messageAudio.FileName}' от @{username}...");
            }));
            try
            {
                var audioMessageTypeFileId = messageAudio.FileId;
                var audioMessageTypeFileInfo = await botClient.GetFileAsync(audioMessageTypeFileId);
                var audioMessageTypeFilePath = audioMessageTypeFileInfo.FilePath;
                var audioMessageTypeFileSize = audioMessageTypeFileInfo.FileSize;
                var audioMessageTypeFileName = messageAudio.FileName;
                string destinationFilePath = @$"{Values.AppInfo.startup_path}saved\{audioMessageTypeFileName}";
                if (messageCaption == "" || messageCaption == null)
                {
                    await botClient.SendChatActionAsync(chatId: chatId,
                    ChatAction.Typing);
                    var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                        text: "🟡 Сохраняю аудио...");
                    try
                    {
                        if (SysFile.Exists(destinationFilePath))
                            File.DeleteForever(destinationFilePath);
                    }
                    catch { }
                    await DownloadContentManager(botClient,
                        message,
                        destinationFilePath,
                        ContentType.Audio);
                    await botClient.EditMessageTextAsync(chatId: chatId,
                            messageId: processMessage.MessageId,
                            text: "😋 <b>Аудио сохранено в папку <code>saved</code>!</b>",
                            parseMode: ParseMode.Html);
                }
                else
                {
                    if (Convert.ToString(messageCaption).ToLower() == "play")
                    {
                        await botClient.SendChatActionAsync(chatId: chatId,
                        ChatAction.Typing);
                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                        text: "🟡 Сохраняю аудио и воспроизвожу...");
                        try
                        {
                            if (SysFile.Exists(destinationFilePath))
                                File.DeleteForever(destinationFilePath);
                        }
                        catch { }
                        await DownloadContentManager(botClient,
                            message,
                            destinationFilePath,
                            ContentType.Audio);
                        Processes.Open(destinationFilePath);
                        await botClient.EditMessageTextAsync(chatId: chatId,
                            messageId: processMessage.MessageId,
                            text: "😋 <b>Аудио сохранено в папку <code>saved</code> и воспроизведено!</b>",
                            parseMode: ParseMode.Html);
                        DesktopWallpaper.Set(destinationFilePath);
                    }
                }
                BeginInvoke(new Action(() =>
                {
                    AddEvent($"Получено аудио '{messageAudio.FileName}' от @{username}");
                }));
            }
            catch (Exception ex)
            {
                BeginInvoke(new Action(() =>
                {
                    AddEvent($"Аудио '{messageAudio.FileName}' от @{username} недоступно!");
                    if (ex.Message.Contains("Bad Request: audio is too big"))
                        AddEvent($"[TelegramBotAPI] Размер аудио слишком большой!");
                }));
                AddEvent(ex.Message, true);
            }
        }

        private async Task HandleMessageAsync(ITelegramBotClient botClient,
        Telegram.Bot.Types.Message message,
        CancellationToken cancellationToken)
        {
            try
            {
                string[] command = new string[2];
                var chatId = message.Chat.Id;
                var username = message.Chat.Username;
                var messageText = message.Text;
                command[0] = TelegramBot.GetCommand(messageText);
                command[1] = TelegramBot.GetArguments(messageText, command[0]);
                var _command = TelegramBot.IsMyCommand(command[0]);
                if (_command == TelegramBot.BotCommandType.NATIVE || _command == TelegramBot.BotCommandType.PLUGIN)
                {
                    if (command[0] != command[1].Replace(@"/", "") && command[1] != "")
                        BeginInvoke(new Action(() =>
                        {
                            AddEvent($"Принята команда '{command[0]}' с аргументом '{command[1].Replace("'", "").Replace("\"", "")}'");
                        }));
                    else BeginInvoke(new Action(() =>
                    {
                        AddEvent($"Принята команда '{command[0]}'");
                    }));
                }
                else
                {
                    if (_command == TelegramBot.BotCommandType.MENU)
                        BeginInvoke(new Action(() =>
                        {
                            AddEvent($"Выбрано меню '{command[0]}'");
                        }));
                    else BeginInvoke(new Action(() =>
                    {
                        AddEvent($"Введённой команды '{command[0]}' не существует!");
                    }));
                }
                switch (_command)
                {
                    case TelegramBot.BotCommandType.MENU:
                        {
                            switch (command[0])
                            {
                                case "📩 Отправка и сохранение":
                                    {
                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                        text: $"<b>📩 Отправка и сохранение</b>\n\n" +
                                        "💾 Сохраните файл (документ) на PC просто отправив его боту\n" +
                                        "* отправка без подписи - скачать файл в папку <code>saved</code>\n" +
                                        "* подпись <code>desktop</code> - установить файл (необходимо отправить картинку как документ) как фоновое изображение для Рабочего стола\n" +
                                        "* подпись <code>tprint</code> - распечатать текстовый файл\n\n" +
                                        "🎧 Также можно сохранять аудио\n" +
                                        "* отправка без подписи - скачать аудио в папку <code>saved</code>\n" +
                                        "* подпись <code>play</code> - скачать аудио в папку <code>saved</code> и воспроизвести",
                                         parseMode: ParseMode.Html,
                                         cancellationToken: cancellationToken);
                                        break;
                                    }
                                case "🗃 Файлы и папки":
                                    {
                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                        text: $"<b>🗃 Файлы и папки</b>" +
                                        "\n/ls [папка] - содержимое папки" +
                                         "\n/lst [папка] - запись содержимого папки в текстовый файл" +
                                        "\n/md [папка] - создать папку" +
                                        "\n/clean - очистить Корзину" +
                                        "\n/send [объект] - загрузить файл/папку в Telegram" +
                                        "\n/tprint [файл] - печать текстового файла (имеются настройки для Shark Remote)" +
                                        "\n/touch [файл] - создать новый файл" +
                                        "\n/touch [файл]|[текст] - создать новый файл с указанным текстом (поддерживается многострочный текст)" +
                                        "\n/cat [файл] - показ текстового содержимого файла" +
                                        "\n/file [файл] - информация о файле" +
                                        "\n/dir [папка] - информация о папке" +
                                        "\n/del [файл] - удалить файл в Корзину" +
                                        "\n/rd [папка] - удалить папку в Корзину",
                                         parseMode: ParseMode.Html,
                                         cancellationToken: cancellationToken);
                                        break;
                                    }
                                case "🕹 Управление":
                                    {
                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                        text: $"<b>🕹 Управление</b>" +
                                        "\n/screen - сделать скриншот" +
                                        "\n\t<code>/screen w</code> - сделать скриншот, загрузить его на <a href=\"https://ru.imgbb.com/\">ImgBB</a> и получить ссылку на изображение" +
                                        "\n\t<code>/screen u</code> - сделать скриншот, загрузить его на <a href=\"https://ru.imgbb.com/\">ImgBB</a> и получить прямую ссылку на изображение" +
                                        "\n/wh - переключить режим показа окон" +
                                        "\n/power [действие] - управление питанием:" +
                                        "\n\t<code>loff</code> - выход из системы" +
                                        "\n\t<code>off</code> - выключение" +
                                        "\n\t<code>r</code> - перезагрузка" +
                                        "\n\t<code>lck</code> - блокировка" +
                                        "\n\t<code>mon</code> - включение монитора" +
                                        "\n\t<code>moff</code> - выключение монитора" +
                                        "\n\t<code>g</code> - информация о батарее" +
                                        "\n/vset [0-100] - задать уровень громкости звука в %" +
                                        "\n/vget - получить уровень громкости звука в %" +
                                        "\n/get - получить текст из буфера обмена" +
                                        "\n/set [текст] - задать текст для буфера обмена" +
                                        "\n/apps - установленные приложения" +
                                        "\n/info - информация о PC" +
                                        "\n/uptime - время работы системы с момента включения PC" +
                                        "\n/input [тип ввода] [значение] - управление клавиатурой:" +
                                        "\n\t<code>0</code> - использовать сочетание клавиш (<a href=\"https://teletype.in/@zalexanninev15/Shark-Remote-Documentation#K569\">информация</a>)" +
                                        "\n\t<code>1</code> - нажать одну клавмшу на клавиатуре (<a href=\"https://teletype.in/@zalexanninev15/Shark-Remote-Documentation#K569\">информация</a>)" +
                                        "\n\t<code>2</code> - написать любой текст" +
                                        "\n/move [x],[y] - перемещение курсора мыши по координатам XY" +
                                        "\n/click [кнопка] - нажатие кнопки на мыши:" +
                                        "\n\t<code>1</code> - левая кнопка" +
                                        "\n\t<code>2</code> - средняя кнопка" +
                                        "\n\t<code>3</code> - правая кнопка" +
                                        "\n/dclick [кнопка] - двойное нажатие кнопки на мыши:" +
                                        "\n\t<code>1</code> - левая кнопка" +
                                        "\n\t<code>2</code> - средняя кнопка" +
                                        "\n\t<code>3</code> - правая кнопка" +
                                        "\n/msg [тип сообщения] [текст] - отправка сообщения:" +
                                        "\n\t<code>0</code> - обычное сообщение" +
                                        "\n\t<code>1</code> - информация" +
                                        "\n\t<code>2</code> - предупреждение" +
                                        "\n\t<code>3</code> - ошибка",
                                        parseMode: ParseMode.Html,
                                        cancellationToken: cancellationToken,
                                        disableWebPagePreview: true);
                                        break;
                                    }
                                case "🌐 Сеть":
                                    {
                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                        text: $"<b>🌐 Сеть</b>" +
                                        "\n/geo - получение местоположения PC" +
                                        "\n/net - сетевая информация" +
                                        "\n/ping [сайт/IP адрес] - пропинговка сайта или IP-адреса",
                                         parseMode: ParseMode.Html,
                                         cancellationToken: cancellationToken);
                                        break;
                                    }
                                case "📜 Задачи":
                                    {
                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                        text: $"<b>📜 Задачи</b>" +
                                        "\n/usage - загруженность и температура комплектующих" +
                                         "\n/run [объект] - запуск файла/папки/приложения" +
                                        "\n/run [файл приложения]|[аргументы] - запуск приложения с аргументом(ами)" +
                                         "\n/tasks - запущенные процессы" +
                                        "\n/kill [файл приложения (можно без EXE)] - завершить все процессы приложения" +
                                         "\n/kcmd - завершить все процессы Cmd" +
                                         "\n/kps - завершить все процессы PowerShell" +
                                        "\n/sc [действие] [название службы] - управление службами:" +
                                         "\n\t<code>g</code> - получить список служб (название службы не требуется)" +
                                         "\n\t<code>sta</code> - запустить службу" +
                                         "\n\t<code>stp</code> - остановить службу" +
                                         "\n\t<code>r</code> - перезапустить службу",
                                         parseMode: ParseMode.Html,
                                         cancellationToken: cancellationToken);
                                        break;
                                    }
                                case "🤏 Другое":
                                    {
                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "<b>🤏 Другое</b>" +
                                            "\n/start - запуск бота" +
                                            "\n/pwg - генератор паролей" +
                                            "\n/bot - информация о боте",
                                            parseMode: ParseMode.Html,
                                            cancellationToken: cancellationToken);
                                        break;
                                    }
                                case "📦+🪴 Пользовательские":
                                    {
                                        bool havePlg = false;
                                        string iPlg = "<b>📦 Плагины</b>\n";
                                        string[] listCFG = SysFile.ReadAllLines($@"{FileSystem.GetDataPath()}\plugins\installed.cfg");
                                        foreach (string item in listCFG)
                                        {
                                            if (item != "")
                                            {
                                                if (!item.StartsWith('#'))
                                                {
                                                    string args = "";
                                                    int agumentsPlugin = Convert.ToInt32(Engine.Settings.Read("", "arguments_count", Engine.Settings.TomlTypeRead.OnlyOneKey, @$"plugins\{item.Split(", ")[0].Remove(0, 4)}\main.manifest"));
                                                    if (agumentsPlugin > 0 && agumentsPlugin <= 4)
                                                        if (agumentsPlugin == 1)
                                                            args += "[аргумент 1] ";
                                                        else if (agumentsPlugin == 2)
                                                            args += "[аргумент 1]|[аргумент 2] ";
                                                        else if (agumentsPlugin == 3)
                                                            args += "[аргумент 1]|[аргумент 2]|[аргумент 3] ";
                                                        else if (agumentsPlugin == 4)
                                                            args += "[аргумент 1]|[аргумент 2]|[аргумент 3]|[аргумент 4] ";
                                                    iPlg += item.Split(", ")[3].Remove(0, 8) + $" {args}- " + item.Split(", ")[0].Remove(0, 4) + "\n";
                                                    havePlg = true;
                                                }
                                            }
                                        }
                                        if (!havePlg)
                                            iPlg += "Не установлены!\n";
                                        string[] listTXT = SysFile.ReadAllLines($@"{FileSystem.GetDataPath()}\settings\variables.txt");
                                        string varList = "\n<b>🪴 Пользовательские переменные</b>";
                                        foreach (string item in listTXT)
                                        {
                                            if (item != "")
                                                varList += $"\n<code>{item.Split('=')[0]}</code>={item.Split("=")[1]}";
                                        }
                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: $"{iPlg}{varList}",
                                        parseMode: ParseMode.Html,
                                        cancellationToken: cancellationToken,
                                        replyMarkup: BotKeyboards.DocVarKeyboard,
                                        disableWebPagePreview: true);
                                        break;
                                    }
                            }
                            break;
                        }
                    case TelegramBot.BotCommandType.NATIVE:
                        {
                            switch (command[0])
                            {
                                case "start":
                                    {
                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "Привет!\n\n" +
                                            "<b>Я твой персональный бот для удалённого управления PC под руководством приложения Shark Remote</b>\n" +
                                            "Больше информации о боте смотри тут /bot" +
                                            "\n\n👇 <b>С помощью меню ниже ты сможешь приступить к управлению своим PC</b>",
                                            parseMode: ParseMode.Html,
                                            cancellationToken: cancellationToken,
                                            replyMarkup: BotKeyboards.MainKeyboard());
                                        break;
                                    }
                                case "uptime":
                                    {
                                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Получаю время работы системы...");
                                        await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.Typing);
                                        try
                                        {
                                            TimeSpan time = TimeSpan.FromMilliseconds(Environment.TickCount);
                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                    processMessage.MessageId,
                                                    $"🕑 Система работает уже:\n" +
                                                    $"{time.Days} дней\n" +
                                                    $"{time.Hours} часов\n" +
                                                    $"{time.Minutes} минут\n" +
                                                    $"{time.Seconds} секунд",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                        }
                                        catch
                                        {
                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                    processMessage.MessageId,
                                                    "🔴 <b>Невозможно получить данные!</b>\n" +
                                                    "Вполне возможно, что у вашего PC надо заменить батарейку на материнской плате или неправильно настроено время в Windows",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "move":
                                    {
                                        if ((command[1].Length > 0) && (command[1].Contains(',')))
                                        {
                                            try
                                            {
                                                string[] xy = command[1].Split(",");
                                                new InputSimulator().Mouse.MoveMouseTo(0, 0).MoveMouseBy(Convert.ToInt32(xy[0]), Convert.ToInt32(xy[1]));
                                                await botClient.SendTextMessageAsync(chatId: chatId,
                                                    "🖱 Курсор мыши перемещён по заданным координатам!",
                                                    parseMode: ParseMode.Html);
                                            }
                                            catch
                                            {
                                                await botClient.SendTextMessageAsync(chatId: chatId,
                                                    "🔴 <b>Ошибка применения коородинат!</b>\n" +
                                                    "Необходимо отправить точные координаты X,Y для курсора мыши в формате <code>x,y</code>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                            }
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                    "🔴 <b>Невозможно задать координаты курсора мыши!</b>\n" +
                                                    "Необходимо отправить точные координаты X,Y для курсора мыши в формате <code>x,y</code>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "click":
                                    {
                                        switch (command[1])
                                        {
                                            case "1":
                                                {
                                                    new InputSimulator().Mouse.LeftButtonClick();
                                                    await botClient.SendTextMessageAsync(chatId: chatId,
                                                       "🐁 <Левая кнопка мыши нажата!",
                                                       parseMode: ParseMode.Html);
                                                    break;
                                                }
                                            case "2":
                                                {
                                                    new InputSimulator().Mouse.MiddleButtonClick();
                                                    await botClient.SendTextMessageAsync(chatId: chatId,
                                                       "🐁 Средняя кнопка мыши нажата!",
                                                       parseMode: ParseMode.Html);
                                                    break;
                                                }
                                            case "3":
                                                {
                                                    new InputSimulator().Mouse.RightButtonClick();
                                                    await botClient.SendTextMessageAsync(chatId: chatId,
                                                       "🐁 Правая кнопка мыши нажата!",
                                                       parseMode: ParseMode.Html);
                                                    break;
                                                }
                                            default:
                                                {
                                                    await botClient.SendTextMessageAsync(chatId: chatId,
                                                   "🔴 <b>Необходимо указать кнопку для нажатия на мышке!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                                case "dclick":
                                    {
                                        switch (command[1])
                                        {
                                            case "1":
                                                {
                                                    new InputSimulator().Mouse.LeftButtonDoubleClick();
                                                    await botClient.SendTextMessageAsync(chatId: chatId,
                                                       "🐁 Левая кнопка мыши нажата 2 раза!",
                                                       parseMode: ParseMode.Html);
                                                    break;
                                                }
                                            case "2":
                                                {
                                                    new InputSimulator().Mouse.MiddleButtonDoubleClick();
                                                    await botClient.SendTextMessageAsync(chatId: chatId,
                                                       "🐁 Средняя кнопка мыши нажата 2 раза!",
                                                       parseMode: ParseMode.Html);
                                                    break;
                                                }
                                            case "3":
                                                {
                                                    new InputSimulator().Mouse.RightButtonDoubleClick();
                                                    await botClient.SendTextMessageAsync(chatId: chatId,
                                                       "🐁 Правая кнопка мыши нажата 2 раза!",
                                                       parseMode: ParseMode.Html);
                                                    break;
                                                }
                                            default:
                                                {
                                                    await botClient.SendTextMessageAsync(chatId: chatId,
                                                   "🔴 <b>Необходимо указать кнопку для двойного нажатия на мышке!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                                case "wheel":
                                    {
                                        if (command[1] != "")
                                        {
                                            try
                                            {
                                                int click_count = Convert.ToInt32(command[1]);
                                                new InputSimulator().Mouse.VerticalScroll(click_count);
                                                string up_or_down = click_count > 0 ? "вверх" : "вниз";
                                                await botClient.SendTextMessageAsync(chatId: chatId,
                                                           $"🛞 Колёсико мыши прокрутило необходимое количество кликов {up_or_down}!",
                                                           parseMode: ParseMode.Html);
                                            }
                                            catch
                                            {
                                                await botClient.SendTextMessageAsync(chatId: chatId,
                                               "🔴 <b>Необходимо указать на какое количество кликов (положительное число - вверх, отрицательное число - вниз) крутить колёсико мыши!</b>",
                                               parseMode: ParseMode.Html,
                                               cancellationToken: cancellationToken);
                                            }
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                               "🔴 <b>Необходимо указать на какое количество кликов (положительное число - вверх, отрицательное число - вниз) крутить колёсико мыши!</b>",
                                               parseMode: ParseMode.Html,
                                               cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "usage":
                                    {
                                        string returnStr = "";
                                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Считываю информацию с датчиков...");
                                        await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.Typing);
                                        try
                                        {
                                            Computer computer = new Computer
                                            {
                                                IsCpuEnabled = true,
                                                IsGpuEnabled = true,
                                                IsMemoryEnabled = true,
                                            };
                                            computer.Open();
                                            computer.Accept(new HardwareUsage());
                                            foreach (IHardware hardware in computer.Hardware)
                                            {
                                                foreach (ISensor sensor in hardware.Sensors)
                                                {
                                                    if (sensor.SensorType.ToString() == "Temperature" || sensor.SensorType.ToString() == "Fan" ||
                                                        sensor.SensorType.ToString() == "Control" || sensor.SensorType.ToString() == "Clock" ||
                                                        sensor.SensorType.ToString() == "Load")
                                                    {
                                                        if (sensor.Name.ToString().Contains("CPU"))
                                                        {
                                                            if (sensor.Value != null && sensor.Value > 0.0 && sensor.Name.Contains("CPU Total"))
                                                                returnStr += string.Format("<b>CPU:</b> Процессор загружен на {0}% (примерно)\n", string.Format("{0:0.0}", sensor.Value - 5));
                                                        }
                                                        if (sensor.Name.ToString().Contains("GPU"))
                                                        {
                                                            if (sensor.Value != null && sensor.Value > 0.0)
                                                            {
                                                                if (sensor.Name.Contains("Core") && sensor.Value < 120.0 && sensor.Value > 10)
                                                                    returnStr += string.Format("<b>GPU:</b> Температура видеокарты {0}°C\n", string.Format("{0:0.0}", sensor.Value));
                                                                if (sensor.Name.Contains("Fan"))
                                                                {
                                                                    if (sensor.Value <= 100)
                                                                        returnStr += string.Format("<b>GPU:</b> Вентмлятор(ы) видеокарты работает(ют) на {0}%\n", string.Format("{0:0.0}", sensor.Value));
                                                                    else
                                                                        returnStr += string.Format("<b>GPU:</b> Скорость вентмлятора(ов) видеокарты - {0} RPM (об/мин)\n", string.Format("{0:0.0}", sensor.Value));
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if ((sensor.Name == "Memory Used" || sensor.Name == "Memory Available") && !sensor.Name.Contains("Virtual") && !sensor.Name.Contains("D3D"))
                                                        returnStr += string.Format("<b>RAM:</b> {0} - {1} ГБ\n", sensor.Name == "Memory Used" ? "Оперативной памяти используется" : "Оперативной памяти свободно", string.Format("{0:0.0}", sensor.Value));
                                                }
                                            }
                                            computer.Close();
                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                    processMessage.MessageId,
                                                    $"🤯 Загруженность и температура комплектующих:\n{returnStr}",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                        }
                                        catch
                                        {
                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                    processMessage.MessageId,
                                                    "🔴 <b>Невозможно получить данные!</b>\n" +
                                                    "Скорее всего датчики системы работают неправильно",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "input":
                                    {
                                        if (command[1].Split(" ")[0] == "0" || command[1].Split(" ")[0] == "1"
                                            || command[1].Split(" ")[0] == "2")
                                        {
                                            var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Ввожу...");
                                            await botClient.SendChatActionAsync(chatId: chatId,
                                                ChatAction.Typing);
                                            switch (command[1].Split(" ")[0])
                                            {
                                                case "0":
                                                    {
                                                        try
                                                        {
                                                            string[] new_hotkey = command[1].Remove(0, 2).Replace("+", ",").Split(",");
                                                            VirtualKeyCode[] vkc = new VirtualKeyCode[new_hotkey.Count()];
                                                            for (int i = 0; i < new_hotkey.Length; i++)
                                                                vkc[i] = Helpers.Keyboard.ConvertToVirtualKeyCode(new_hotkey[i]);
                                                            if (new_hotkey.Count() == 2)
                                                                new InputSimulator().Keyboard.ModifiedKeyStroke(vkc[0], vkc[1]);
                                                            if (new_hotkey.Count() == 3)
                                                                new InputSimulator().Keyboard.ModifiedKeyStroke(vkc[0], vkc[1..2]);
                                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                            processMessage.MessageId,
                                                            "⌨️ Сочетание клавиш применено!");
                                                        }
                                                        catch
                                                        {
                                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                            processMessage.MessageId,
                                                            "🔴 <b>Сочетание клавиш невозможно применить!</b>\n" +
                                                            "<a href=\"https://teletype.in/@zalexanninev15/Shark-Remote-Documentation#K569\">Ознакомьтесь с инструкцией</a>",
                                                            parseMode: ParseMode.Html,
                                                            cancellationToken: cancellationToken);
                                                        }
                                                        break;
                                                    }
                                                case "1":
                                                    {
                                                        try
                                                        {
                                                            string new_key = command[1].Remove(0, 2);
                                                            VirtualKeyCode vkc_button = Helpers.Keyboard.ConvertToVirtualKeyCode(new_key);
                                                            new InputSimulator().Keyboard.KeyPress(vkc_button);
                                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                            processMessage.MessageId,
                                                             "🔲 Клавиша нажата!");
                                                        }
                                                        catch
                                                        {
                                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                            processMessage.MessageId,
                                                            "🔴 <b>Введённой клавиши не существует!</b>\n" +
                                                            "<a href=\"https://teletype.in/@zalexanninev15/Shark-Remote-Documentation#K569\">Ознакомьтесь с инструкцией</a>",
                                                            parseMode: ParseMode.Html);
                                                        }
                                                        break;
                                                    }
                                                case "2":
                                                    {
                                                        try
                                                        {
                                                            command[1] = command[1].Remove(0, 2);
                                                            new InputSimulator().Keyboard.TextEntry(command[1]);
                                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                            processMessage.MessageId,
                                                            "🈵 Текст введён с клавиатуры!",
                                                            cancellationToken: cancellationToken);
                                                        }
                                                        catch
                                                        {
                                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                            processMessage.MessageId,
                                                            "🔴 <b>Введённых символов не существует!</b>",
                                                            parseMode: ParseMode.Html,
                                                            cancellationToken: cancellationToken);
                                                        }
                                                        break;
                                                    }
                                            }
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                               "🔴 <b>Необходимо указать тип ввода и что надо ввести!</b>",
                                               parseMode: ParseMode.Html,
                                               cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "geo":
                                    {
                                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Получаю местоположение...");
                                        await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.FindLocation);
                                        try
                                        {
                                            int ipgeo = Convert.ToInt32(Engine.Settings.Read("GEOLOCATION", "selected_service"));
                                            string ipgeolocation_api_key = "";
                                            string[] geo = new string[] { "", "", "" };
                                            if (ipgeo == 2)
                                            {
                                                ipgeolocation_api_key = Engine.Settings.Read("GEOLOCATION", "ipgeolocationio_api_key");
                                                if (ipgeolocation_api_key != "")
                                                    geo = await Network.GetGeoAsync(ip, 2, ipgeolocation_api_key);
                                                else
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                     text: "🔐 <b>Требуется API ключ!</b>" +
                                                    "\nДля использования сервиса <a href=\"https://ipgeolocation.io/\">ipgeolocation.io</a> требуется <a href=\"https://app.ipgeolocation.io/\">API ключ</a>, который необходимо вставить в файл настроек.",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                            }
                                            else
                                                geo = await Network.GetGeoAsync(ip, ipgeo);
                                            var keyboardGeo = new InlineKeyboardMarkup(new[]
                                            {
                                                new[]
                                                {
                                                    InlineKeyboardButton.WithUrl("ℹ Получить больше информации",
                                                    $"https://ipgeolocation.io/ip-location/{ip}"),
                                                },
                                                new[]
                                                {
                                                    InlineKeyboardButton.WithUrl("🗺 Показать на карте",
                                                    $"https://www.google.com/maps/search/?api=1&query={geo[1]},{geo[2]}"),
                                                }
                                            });
                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                messageId: processMessage.MessageId,
                                                text: $"📍 <b>Местоположение</b>\n{geo[0]}",
                                                parseMode: ParseMode.Html,
                                                replyMarkup: keyboardGeo);
                                            try
                                            {
                                                if (geo[1] != "" && geo[2] != "")
                                                {
                                                    float la_map;
                                                    float.TryParse(geo[1], NumberStyles.Any,
                                                        new CultureInfo("en-US"),
                                                        out la_map);
                                                    float lo_map; float.TryParse(geo[2],
                                                        NumberStyles.Any,
                                                        new CultureInfo("en-US"),
                                                        out lo_map);
                                                    await botClient.SendVenueAsync(chatId: chatId,
                                                        latitude: la_map,
                                                        longitude: lo_map,
                                                        title: "📍 Местоположение компьютера",
                                                        address: "Подробности смотрите в сообщении выше");
                                                }
                                            }
                                            catch { }
                                        }
                                        catch (Exception ex)
                                        {
                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                messageId: processMessage.MessageId,
                                                text: "🔴 <b>Сервис не отвечает!</b>" +
                                                "\nПовторите попытку позже.\n" +
                                                $"Ошибка: {ex.ToString()}",
                                            parseMode: ParseMode.Html,
                                            cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "net":
                                    {
                                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Получаю сетевую информацию...");
                                        await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.Typing);
                                        try
                                        {
                                            var keyboard = new InlineKeyboardMarkup(new[]
                                            {
                                            new[]
                                            {
                                                InlineKeyboardButton.WithUrl("ℹ Получить больше информации",
                                                $"https://2ip.ru/whois/?ip={ip}"),
                                            }
                                        });
                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                messageId: processMessage.MessageId,
                                                     text: $"🌐 <b>Сетевая информация</b>" +
                                                     $"\nИмя хоста: <code>{Dns.GetHostName()}</code>" +
                                                     $"\nПубличный IP-адрес: <code>{ip}</code>" +
                                                     $"\nIPv4: <code>{Network.Addresses.IPv4()}</code>" +
                                                     $"\nIPv6: <code>{Network.Addresses.IPv6()}</code>" +
                                                     $"\nMAC-адрес: <code>{Network.Addresses.MAC()}</code>",
                                                     parseMode: ParseMode.Html,
                                                     replyMarkup: keyboard,
                                                     cancellationToken: cancellationToken);
                                        }
                                        catch
                                        {
                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                messageId: processMessage.MessageId,
                                                text: "🔴 <b>Сервис не отвечает!</b>" +
                                                "\nПовторите попытку позже.",
                                            parseMode: ParseMode.Html,
                                            cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "ls":
                                    {
                                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Формирую вывод содержимого папки...");
                                        await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.Typing);
                                        command[1] = TelegramBot.ArgumentsAsText(command[1]);
                                        if (command[1] != "" && command[1].ToLower() != "/ls")
                                        {
                                            if (Directory.Exists(command[1]))
                                            {
                                                if (!command[1].EndsWith(@"\"))
                                                    command[1] = @$"{command[1]}\";
                                                try
                                                {
                                                    List<string> ls = FileSystem.ReturnRecursFList(command[1]);
                                                    string message1 = "";
                                                    foreach (string fname in ls)
                                                        message1 += $"{fname}\n";
                                                    bool IsDisk = false;
                                                    foreach (Match m in Regex.Matches(command[1],
                                                        @"[A-Z]:\\"))
                                                    {
                                                        if (command[1].EndsWith(m.Value))
                                                            IsDisk = true;
                                                    }
                                                    if (IsDisk)
                                                        await botClient.EditMessageTextAsync(chatId: chatId,
                                                            messageId: processMessage.MessageId,
                                                            text: "📁 <b>Содержимое корня диска</b>" +
                                                            $"\n{message1.Trim('\n')}",
                                                            parseMode: ParseMode.Html,
                                                            cancellationToken: cancellationToken);
                                                    else
                                                        await botClient.EditMessageTextAsync(chatId: chatId,
                                                            messageId: processMessage.MessageId,
                                                            text: "📁 <b>Содержимое папки</b>" +
                                                            $"\n{message1.Trim('\n')}",
                                                            parseMode: ParseMode.Html,
                                                            cancellationToken: cancellationToken);
                                                }
                                                catch
                                                {
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "🔴 <b>Папка пустая или элементов в папке слишком много!</b>" +
                                                    "\nЕсли вы уверены, что папка не пустая, то воспользуйтесь командой /lst, " +
                                                    "чтобы получить элементы расположенные в папке в виде текстового файла.",
                                                parseMode: ParseMode.Html,
                                                cancellationToken: cancellationToken);
                                                }
                                            }
                                            else
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                messageId: processMessage.MessageId,
                                                text: "🔴 <b>Данного пути не существует!</b>",
                                                parseMode: ParseMode.Html,
                                                cancellationToken: cancellationToken);
                                        }
                                        else
                                        {
                                            command[1] = Application.StartupPath;
                                            try
                                            {
                                                List<string> ls = FileSystem.ReturnRecursFList(command[1]);
                                                string message1 = "";
                                                foreach (string fname in ls)
                                                    message1 += $"{fname}\n";
                                                bool IsDisk = false;
                                                foreach (Match m in Regex.Matches(command[1],
                                                    @"[A-Z]:\\"))
                                                {
                                                    if (command[1].EndsWith(m.Value))
                                                        IsDisk = true;
                                                }
                                                if (IsDisk)
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                        messageId: processMessage.MessageId,
                                                        text: "📁 <b>Содержимое корня диска</b>" +
                                                        $"\n{message1.Trim('\n')}",
                                                        parseMode: ParseMode.Html,
                                                        cancellationToken: cancellationToken);
                                                else
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                        messageId: processMessage.MessageId,
                                                        text: "📁 <b>Содержимое папки</b>" +
                                                        $"\n{message1.Trim('\n')}",
                                                        parseMode: ParseMode.Html,
                                                        cancellationToken: cancellationToken);
                                            }
                                            catch
                                            {
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                messageId: processMessage.MessageId,
                                                text: "🔴 <b>Папка пустая или элементов в папке слишком много!</b>" +
                                                "\nЕсли вы уверены, что папка не пустая, то воспользуйтесь командой /lst, " +
                                                "чтобы получить элементы расположенные в папке в виде текстового файла.",
                                            parseMode: ParseMode.Html,
                                            cancellationToken: cancellationToken);
                                            }
                                        }
                                        break;
                                    }
                                case "lst":
                                    {
                                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Формирую вывод содержимого папки...");
                                        await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.UploadDocument);
                                        command[1] = TelegramBot.ArgumentsAsText(command[1]);
                                        if (command[1] != "" && command[1].ToLower() != "/lst")
                                        {
                                            if (Directory.Exists(command[1]))
                                            {
                                                if (!command[1].EndsWith(@"\"))
                                                    command[1] = $@"{command[1]}\";
                                                try
                                                {
                                                    List<string> ls = FileSystem.ReturnRecursFList(command[1]);
                                                    string message1 = "";
                                                    foreach (string fname in ls)
                                                        message1 += $"{fname}\n";
                                                    bool IsDisk = false;
                                                    foreach (Match m in Regex.Matches(command[1],
                                                        @"[A-Z]:\\"))
                                                    {
                                                        if (command[1].EndsWith(m.Value))
                                                            IsDisk = true;
                                                    }
                                                    if (IsDisk)
                                                    {
                                                        await botClient.DeleteMessageAsync(chatId: chatId,
                                                        messageId: processMessage.MessageId);
                                                        SysFile.WriteAllText($@"{tempPath}\Папка.txt", $"Содержимое корня диска {command[1].Remove(2, 1)}\n{message1}");
                                                        using (FileStream stream = SysFile.OpenRead($@"{tempPath}\Папка.txt"))
                                                        {
                                                            InputOnlineFile inputOnlineFile = new InputOnlineFile(stream,
                                                                "Папка.txt");
                                                            await botClient.SendDocumentAsync(chatId,
                                                                inputOnlineFile,
                                                                caption: "🖴 <b>Список содержимого корня диска загружен!</b>",
                                                                parseMode: ParseMode.Html, cancellationToken: cancellationToken);
                                                        }
                                                        try { File.DeleteForever($@"{tempPath}\Папка.txt"); } catch { }
                                                    }
                                                    else
                                                    {
                                                        await botClient.DeleteMessageAsync(chatId: chatId,
                                                        messageId: processMessage.MessageId);
                                                        SysFile.WriteAllText($@"{tempPath}\Папка.txt", $"Содержимое папки \"{command[1]}\":\n{message1}");
                                                        using (FileStream stream = SysFile.OpenRead($@"{tempPath}\Папка.txt"))
                                                        {
                                                            InputOnlineFile inputOnlineFile = new InputOnlineFile(stream, "Папка.txt");
                                                            await botClient.SendDocumentAsync(chatId,
                                                                inputOnlineFile,
                                                                caption: "📁 <b>Список содержимого папки загружен!</b>",
                                                                parseMode: ParseMode.Html,
                                                                cancellationToken: cancellationToken);
                                                        }
                                                        try { File.DeleteForever($@"{tempPath}\Папка.txt"); } catch { }
                                                    }
                                                }
                                                catch
                                                {
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "🔴 <b>Ошибка чтения папки!</b>",
                                                parseMode: ParseMode.Html,
                                                cancellationToken: cancellationToken);
                                                }
                                            }
                                            else
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                messageId: processMessage.MessageId,
                                                text: "🔴 <b>Данного пути не существует!</b>",
                                                parseMode: ParseMode.Html,
                                                cancellationToken: cancellationToken);
                                        }
                                        else
                                        {
                                            command[1] = Application.StartupPath;
                                            try
                                            {
                                                List<string> ls = FileSystem.ReturnRecursFList(command[1]);
                                                string message1 = "";
                                                foreach (string fname in ls)
                                                    message1 += $"{fname}\n";
                                                bool IsDisk = false;
                                                foreach (Match m in Regex.Matches(command[1],
                                                    @"[A-Z]:\\"))
                                                {
                                                    if (command[1].EndsWith(m.Value))
                                                        IsDisk = true;
                                                }
                                                if (IsDisk)
                                                {
                                                    await botClient.DeleteMessageAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId);
                                                    SysFile.WriteAllText($@"{tempPath}\Папка.txt", $"Содержимое корня диска {command[1].Remove(2, 1)}\n{message1}");
                                                    using (FileStream stream = SysFile.OpenRead($@"{tempPath}\Папка.txt"))
                                                    {
                                                        InputOnlineFile inputOnlineFile = new InputOnlineFile(stream,
                                                            "Папка.txt");
                                                        await botClient.SendDocumentAsync(chatId,
                                                            inputOnlineFile,
                                                            caption: "🖴 <b>Список содержимого корня диска загружен!</b>",
                                                            parseMode: ParseMode.Html,
                                                            cancellationToken: cancellationToken);
                                                    }
                                                    try { File.DeleteForever($@"{tempPath}\Папка.txt"); } catch { }
                                                }
                                                else
                                                {
                                                    await botClient.DeleteMessageAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId);
                                                    SysFile.WriteAllText($@"{tempPath}\Папка.txt", $"Содержимое папки \"{command[1]}\":\n{message1}");
                                                    using (FileStream stream = SysFile.OpenRead($@"{tempPath}\Папка.txt"))
                                                    {
                                                        InputOnlineFile inputOnlineFile = new InputOnlineFile(stream, "Папка.txt");
                                                        await botClient.SendDocumentAsync(chatId,
                                                            inputOnlineFile,
                                                            caption: "📁 <b>Список содержимого папки загружен!</b>",
                                                            parseMode: ParseMode.Html,
                                                            cancellationToken: cancellationToken);
                                                    }
                                                    try { File.DeleteForever($@"{tempPath}\Папка.txt"); } catch { }
                                                }
                                            }
                                            catch
                                            {
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                messageId: processMessage.MessageId,
                                                text: "🔴 <b>Ошибка чтения папки!</b>",
                                            parseMode: ParseMode.Html, cancellationToken: cancellationToken);
                                            }
                                        }
                                        break;
                                    }
                                case "wh":
                                    {
                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                                        text: "🪟 <b>Режим показа окон переключён!</b>",
                                                        parseMode: ParseMode.Html, cancellationToken: cancellationToken);
                                        new InputSimulator().Keyboard.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.VK_D);
                                        break;
                                    }
                                case "bot":
                                    {
                                        var keyboardBot = new InlineKeyboardMarkup(new[]
                                            {
                                        new[]
                                        {
                                            InlineKeyboardButton.WithUrl("🦈 Сайт",
                                            $"https://sharkremote.neocities.org"),
                                        },
                                        new[]
                                        {
                                            InlineKeyboardButton.WithUrl("🆕 Новостной канал",
                                            $"https://t.me/NewsWiT"),
                                        },
                                        new[]
                                        {
                                            InlineKeyboardButton.WithUrl("✍️ Связаться с автором",
                                            $"https://t.me/Zalexanninev15"),
                                        },
                                    });
                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: $"ℹ️ Shark Remote CE - приложение для создания Telegram бота для управления PC с Windows. Сейчас используется версия {Application.ProductVersion}\n\n" +
                                            "🤖 <b>Токен бота:</b>" +
                                            $"\n<code>{Engine.Settings.MemoryValues.token}</code>\n\n" +
                                            $"📁 <b>Shark Remote расположен по пути:</b>\n" +
                                            $"<code>{Values.AppInfo.startup_path}</code>\n\n" +
                                            $"⚙️ <b>Настройки и данные расположены по пути:</b>\n" +
                                            $"<code>{FileSystem.GetDataPath()}</code>\n" +
                                            "\n👤 <b>Ваш UserID:</b>" +
                                            $"\n<code>{chatId}</code>",
                                            parseMode: ParseMode.Html,
                                            replyMarkup: keyboardBot);
                                        break;
                                    }
                                case "pwg":
                                    {
                                        var keyboardBot = new InlineKeyboardMarkup(new[]
                                        {
                                            new[]
                                            {
                                                InlineKeyboardButton.WithWebApp(text: "🔣 Сгенерировать пароль",
                                                new WebAppInfo{Url = "https://zalexanninev15.github.io/PassGen2"}),
                                            },
                                        });
                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: $"🔣 Генератор паролей с веб-интерфейсом",
                                            replyMarkup: keyboardBot);
                                        break;
                                    }
                                case "power":
                                    {
                                        switch (command[1].ToLower())
                                        {
                                            case "loff":
                                                {
                                                    await botClient.SendTextMessageAsync(chatId: chatId,
                                                        text: "🛑 <b>Выхожу из системы...</b>",
                                                        parseMode: ParseMode.Html,
                                                        cancellationToken: cancellationToken);
                                                    PowerControl.Computer(PowerControl.SYSTEM_POWER_CONTROL.SYSTEM_LOGOFF);
                                                    break;
                                                }
                                            case "off":
                                                {
                                                    await botClient.SendTextMessageAsync(chatId: chatId,
                                                        text: "🔌 <b>Выключаю компьютер...</b>",
                                                        parseMode: ParseMode.Html,
                                                        cancellationToken: cancellationToken);
                                                    PowerControl.Computer(PowerControl.SYSTEM_POWER_CONTROL.SYSTEM_SHUTDOWN);
                                                    break;
                                                }
                                            case "r":
                                                {
                                                    await botClient.SendTextMessageAsync(chatId: chatId,
                                                        text: "🔁 <b>Перезагружаю компьютер...</b>",
                                                        parseMode: ParseMode.Html,
                                                        cancellationToken: cancellationToken);
                                                    PowerControl.Computer(PowerControl.SYSTEM_POWER_CONTROL.SYSTEM_REBOOT);
                                                    break;
                                                }
                                            case "lck":
                                                {
                                                    await botClient.SendTextMessageAsync(chatId: chatId,
                                                        text: "🔒 <b>Блокирую систему...</b>",
                                                        parseMode: ParseMode.Html,
                                                        cancellationToken: cancellationToken);
                                                    PowerControl.Computer(PowerControl.SYSTEM_POWER_CONTROL.SYSTEM_LOCK);
                                                    break;
                                                }
                                            case "mon":
                                                {
                                                    await botClient.SendTextMessageAsync(chatId: chatId,
                                                        text: "🖥 <b>Включаю монитор...</b>",
                                                        parseMode: ParseMode.Html,
                                                        cancellationToken: cancellationToken);
                                                    PowerControl.Monitor(true);
                                                    break;
                                                }
                                            case "moff":
                                                {
                                                    await botClient.SendTextMessageAsync(chatId: chatId,
                                                        text: "💤 <b>Выключаю монитор...</b>",
                                                        parseMode: ParseMode.Html,
                                                        cancellationToken: cancellationToken);
                                                    PowerControl.Monitor(false);
                                                    break;
                                                }
                                            case "g":
                                                {
                                                    var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                                text: "🟡 Получаю данные о батарее...");
                                                    await botClient.SendChatActionAsync(chatId: chatId,
                                                    ChatAction.Typing);
                                                    string battery_status = Convert.ToString(SystemInformation.PowerStatus.BatteryChargeStatus),
                                                    per = Convert.ToString(SystemInformation.PowerStatus.BatteryLifePercent * 100) + "%";
                                                    if (battery_status == "NoSystemBattery")
                                                    {
                                                        battery_status = "Не обнаружена";
                                                        per = "Нет данных";
                                                    }
                                                    if (battery_status == "Unknown")
                                                        battery_status = "Неизвестно";
                                                    if (battery_status == "Charging")
                                                        battery_status = "Заряжается...";
                                                    if (battery_status == "High")
                                                        battery_status = "Всё отлично";
                                                    if ((battery_status == "Low") || (battery_status == "0"))
                                                        battery_status = "Пока нормально";
                                                    if (battery_status == "Critical")
                                                        battery_status = "Ставь на зарядку!";
                                                    if (battery_status == "Low, Critical, Charging")
                                                        battery_status = "Не работает";
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                        messageId: processMessage.MessageId,
                                                        text: "🔋 <b>Батерея:</b>" +
                                                        "\nСтатус: <code>" + battery_status + "</code>" +
                                                        "\nУровень заряда: <code>" + per + "</code>",
                                                        parseMode: ParseMode.Html,
                                                        cancellationToken: cancellationToken);
                                                    break;
                                                }
                                            default:
                                                {
                                                    await botClient.SendTextMessageAsync(chatId: chatId,
                                                   "🔴 <b>Необходимо указать действие для питания!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                                case "touch":
                                    {
                                        if (command[1] != "")
                                        {
                                            await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.Typing);
                                            try
                                            {
                                                if (command[1].Contains("|"))
                                                {
                                                    string[] file_touch = command[1].Split('|');
                                                    SysFile.WriteAllText(file_touch[0], file_touch[1]);
                                                }
                                                else
                                                    SysFile.Create(command[1]).Close();
                                                await botClient.SendTextMessageAsync(chatId: chatId,
                                                text: "✅ <b>Файл создан или перезаписан!</b>",
                                                parseMode: ParseMode.Html,
                                                cancellationToken: cancellationToken);
                                            }
                                            catch
                                            {
                                                await botClient.SendTextMessageAsync(chatId: chatId,
                                                text: "🔴 <b>Файл невозможно создать или перезаписать!</b>\n" +
                                                "Возможно файл уже используется каким-то другим приложением или недоступен для текущего пользователя Windows",
                                                parseMode: ParseMode.Html,
                                                cancellationToken: cancellationToken);
                                            }
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🔴 <b>Файл невозможно создать или перезаписать!</b>\n" +
                                            "Требуется указать хотя бы файл, который нужно создать",
                                            parseMode: ParseMode.Html,
                                            cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "screen":
                                    {
                                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Делаю скриншот...");
                                        await botClient.SendChatActionAsync(chatId: chatId,
                                        ChatAction.UploadPhoto);
                                        string fileName = string.Format("Скриншот_{0}_{1}.png",
                                            DateTime.Now.ToString("yyyyMMddHHmm"),
                                            Guid.NewGuid());
                                        await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.UploadDocument);
                                        if (command[1].ToLower() != "w" && command[1].ToLower() != "u")
                                        {
                                            InputOnlineFile inputOnlineFile = new InputOnlineFile(VitNX3.Functions.Information.Monitor.CaptureScreenToMemoryStream(),
                                                fileName);
                                            await botClient.DeleteMessageAsync(chatId: chatId,
                                                messageId: processMessage.MessageId);
                                            await botClient.SendDocumentAsync(chatId: chatId,
                                                inputOnlineFile,
                                                caption: "🖥 <b>Скриншот загружен!</b>",
                                                parseMode: ParseMode.Html,
                                                cancellationToken: cancellationToken);
                                        }
                                        else
                                        {
                                            string api_key = Engine.Settings.Read("BOT", "imgbb_api_key");
                                            if (!string.IsNullOrEmpty(api_key))
                                            {
                                                VitNX3.Functions.Information.Monitor.CaptureScreenToFile($@"{tempPath}\{fileName}", System.Drawing.Imaging.ImageFormat.Png);
                                                try
                                                {
                                                    var imgBB = new Uploader(api_key);
                                                    var result = imgBB.UploadImageFileAsync($@"{tempPath}\{fileName}").Result;
                                                    await botClient.DeleteMessageAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId);
                                                    if (command[1].ToLower() == "w")
                                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                                        text: $"🖥 <b>Скриншот загружен!</b>\n" +
                                                        $"Сслыка на изображение: {result.data.url_viewer}",
                                                        parseMode: ParseMode.Html,
                                                        cancellationToken: cancellationToken);
                                                    else if (command[1].ToLower() == "u")
                                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                                        text: $"🖥 <b>Скриншот загружен!</b>\n" +
                                                        $"Прямая ссылка на изображение: {result.data.url}",
                                                        parseMode: ParseMode.Html,
                                                        cancellationToken: cancellationToken);
                                                }
                                                catch
                                                {
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "🔴 <b>Сервис не отвечает!</b>" +
                                                    "\nПовторите попытку позже.",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                                }
                                                try { File.DeleteForever($@"{tempPath}\{fileName}"); } catch { }
                                            }
                                            else
                                            {
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                messageId: processMessage.MessageId,
                                                text: "🔐 <b>Требуется API ключ!</b>" +
                                                "\nДля использования сервиса <a href=\"https://ru.imgbb.com/\">ImgBB</a> требуется <a href=\"https://api.imgbb.com/\">API ключ</a>, который необходимо вставить в файл настроек.",
                                                parseMode: ParseMode.Html,
                                                cancellationToken: cancellationToken);
                                            }
                                        }
                                        break;
                                    }
                                case "vset":
                                    {
                                        if (command[1] != "" && VitNX3.Functions.Data.Text.ContainsOnlyNumbers(command[1]))
                                        {
                                            var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Изменяю значение уровня громкости звука...");
                                            try
                                            {
                                                float newVolume = float.Parse(command[1]);
                                                if (newVolume > 0) newVolume = newVolume / 100;
                                                if (newVolume >= 0 && newVolume <= 1)
                                                {
                                                    VolumeControl audio = new VolumeControl();
                                                    audio.Set(newVolume);
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                        messageId: processMessage.MessageId,
                                                        text: "🔊 <b>Задано новое значение уровня громкости звука!</b>",
                                                        parseMode: ParseMode.Html,
                                                        cancellationToken: cancellationToken);
                                                }
                                                else await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "🔴 <b>Значение не может быть ниже 0 или выше 100!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                            }
                                            catch (Exception ex)
                                            {
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "🔴 <b>Невозможно изменить уровень громкость звука!</b>" +
                                                $"\nСообщение ошибки: <code>{ex.Message}</code>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                            }
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                   "🔴 <b>Необходимо указать новое значение громкости звука! От 0 до 100</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "vget":
                                    {
                                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Получаю текущее значение уровня громкости звука...");
                                        try
                                        {
                                            VolumeControl audio = new VolumeControl();
                                            float currentVolume = audio.Get();
                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                messageId: processMessage.MessageId,
                                                text: $"🔊 <b>Текущее значение уровня громкости звука:</b> " +
                                                $"{currentVolume * 100}%",
                                                parseMode: ParseMode.Html,
                                                cancellationToken: cancellationToken);
                                        }
                                        catch (Exception ex)
                                        {
                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                messageId: processMessage.MessageId,
                                                text: "🔴 <b>Невозможно получить уровень громкости звука!</b>" +
                                            $"\nСообщение ошибки: <code>{ex.Message}</code>",
                                                parseMode: ParseMode.Html,
                                                cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "get":
                                    {
                                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Получаю текущий текст из буфера обмена...");
                                        await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.Typing);
                                        try
                                        {
                                            GetClipboardText GetClipboard = new GetClipboardText();
                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                messageId: processMessage.MessageId,
                                                text: "📋 <b>Текст в буфере обмена</b>\n<code>" + GetClipboard.GetText() + "</code>",
                                                parseMode: ParseMode.Html,
                                                cancellationToken: cancellationToken);
                                        }
                                        catch
                                        {
                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                        messageId: processMessage.MessageId,
                                        text: "🔴 <b>Невозможно получить текст из буфера обмена!</b>",
                                        parseMode: ParseMode.Html,
                                        cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "set":
                                    {
                                        if (command[1] != "")
                                        {
                                            var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Добавляю новый текст в буфер обмена...");
                                            try
                                            {
                                                await SetClipboardText.Run(() => Clipboard.SetText(command[1]));
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "📋 <b>В буфер обмена добавлен новый текст!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                            }
                                            catch
                                            {
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                            messageId: processMessage.MessageId,
                                            text: "🔴 <b>Невозможно задать текст для буфера обмена!</b>",
                                            parseMode: ParseMode.Html,
                                            cancellationToken: cancellationToken);
                                            }
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                   "🔴 <b>Необходимо указать текст для буфера обмена!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "md":
                                    {
                                        if (command[1] != "")
                                        {
                                            command[1] = TelegramBot.ArgumentsAsText(command[1]);
                                            var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Создаю новую папку...");
                                            if (!Directory.Exists(command[1]))
                                            {
                                                Directory.CreateDirectory(command[1]);
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "📁 <b>Папка успешно создана!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                            }
                                            else await botClient.EditMessageTextAsync(chatId: chatId,
                                                messageId: processMessage.MessageId,
                                                text: "🔴 <b>Папка с таким именем уже сушествует!</b>",
                                                parseMode: ParseMode.Html,
                                                cancellationToken: cancellationToken);
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                   "🔴 <b>Необходимо указать новую папку!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "clean":
                                    {
                                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Очищаю Корзину...");
                                        Import.SHEmptyRecycleBin(IntPtr.Zero,
                                            null,
                                            Enums.SHERB_RECYCLE.SHERB_NO_SOUND |
                                            Enums.SHERB_RECYCLE.SHERB_NO_CONFIRMATION);
                                        await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.Typing);
                                        await botClient.EditMessageTextAsync(chatId: chatId,
                                        messageId: processMessage.MessageId,
                                            text: "🗑 <b>Корзина очищена!</b>",
                                            parseMode: ParseMode.Html,
                                            cancellationToken: cancellationToken);
                                        break;
                                    }
                                case "kill":
                                    {
                                        if (command[1] != "")
                                        {
                                            await botClient.SendChatActionAsync(chatId: chatId, ChatAction.Typing);
                                            command[1] = TelegramBot.ArgumentsAsText(command[1]);
                                            var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Завершаю процесс(ы)...");
                                            if (!command[1].Contains(".exe"))
                                                command[1] += ".exe";
                                            if (command[1] == "cmd.exe")
                                            {
                                                Processes.KillNative("WindowsTerminal.exe");
                                                Processes.KillNative("OpenConsole.exe");
                                                Processes.KillNative("cmd.exe");
                                            }
                                            if (command[1] == "powershell.exe")
                                            {
                                                Processes.KillNative("WindowsTerminal.exe");
                                                Processes.KillNative("OpenConsole.exe");
                                                Processes.KillNative("powershell.exe");
                                            }
                                            Processes.KillNative(command[1]);
                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                processMessage.MessageId,
                                                text: $"⛏ <b>Все процессы завершены'!</b>",
                                                parseMode: ParseMode.Html,
                                                cancellationToken: cancellationToken);
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                   "🔴 <b>Необходимо указать имя процесса (можно без EXE)!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "kcmd":
                                    {
                                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Завершаю процесс(ы) Cmd...");
                                        await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.Typing);
                                        Processes.KillNative("WindowsTerminal.exe");
                                        Processes.KillNative("OpenConsole.exe");
                                        Processes.KillNative("cmd.exe");
                                        await botClient.EditMessageTextAsync(chatId: chatId,
                                            processMessage.MessageId,
                                            text: "⛏ <b>Все процессы Cmd завершены!</b>",
                                            parseMode: ParseMode.Html,
                                            cancellationToken: cancellationToken);
                                        break;
                                    }
                                case "kps":
                                    {
                                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                               text: "🟡 Завершаю процесс(ы) PowerShell...");
                                        await botClient.SendChatActionAsync(chatId: chatId,
                                        ChatAction.Typing);
                                        Processes.KillNative("WindowsTerminal.exe");
                                        Processes.KillNative("OpenConsole.exe");
                                        Processes.KillNative("powershell.exe");
                                        await botClient.EditMessageTextAsync(chatId: chatId,
                                            processMessage.MessageId,
                                            text: "⛏ <b>Все процессы PowerShell завершены!</b>",
                                            parseMode: ParseMode.Html,
                                            cancellationToken: cancellationToken);
                                        break;
                                    }
                                case "curl":
                                    {
                                        if (command[1] != "")
                                        {
                                            var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Посылаю curl запрос...");
                                            await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.Typing);
                                            command[1] = TelegramBot.ArgumentsAsText(command[1]);
                                            try
                                            {
                                                string curlR = Processes.Execute("curl", command[1]);
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                    processMessage.MessageId,
                                                    text: "📩 <b>Curl запрос отправлен!</b>\nРезультат:\n" +
                                                    $"<code>{curlR}</code>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                            }
                                            catch
                                            {
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "🔴 <b>Невозможно выполнить curl запрос!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                            }
                                        }
                                        else
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                    text: "🔴 <b>Необходимо запрос для отправки!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                        break;
                                    }
                                case "send":
                                    {
                                        if (command[1] != "")
                                        {
                                            var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Создаю ZIP архив с указанным объектом...");
                                            await botClient.SendChatActionAsync(chatId: chatId,
                                                ChatAction.UploadDocument);
                                            command[1] = TelegramBot.ArgumentsAsText(command[1]);
                                            if (Directory.Exists(command[1]))
                                            {
                                                int bakCheck;
                                                DirectoryInfo dirInfo = new DirectoryInfo(command[1]);
                                                string fileName = $"{dirInfo.Name}.zip";
                                                await botClient.DeleteMessageAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId);
                                                processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                                        text: "🟡 Упаковываю папку...");
                                                long dirLength = Dir.GetSize(new DirectoryInfo(dirInfo.FullName));
                                                if (dirLength > 0)
                                                {
                                                    try
                                                    {
                                                        ZipFile.CreateFromDirectory(@command[1],
                                                            $@"{tempPath}\{fileName}",
                                                            CompressionLevel.SmallestSize,
                                                            true);
                                                        bakCheck = 1;
                                                    }
                                                    catch
                                                    {
                                                        await botClient.EditMessageTextAsync(chatId: chatId,
                                                            processMessage.MessageId,
                                                            text: "🔴 <b>Папку невозможно заархивировать!</b>",
                                                            parseMode: ParseMode.Html,
                                                            cancellationToken: cancellationToken);
                                                        bakCheck = 0;
                                                    }
                                                    if (bakCheck == 1)
                                                    {
                                                        await botClient.EditMessageTextAsync(chatId: chatId,
                                                            processMessage.MessageId,
                                                            text: "🟡 Загружаю архив с папкой в бота...");
                                                        try
                                                        {
                                                            using (FileStream fs = SysFile.OpenRead($@"{tempPath}\{fileName}"))
                                                            {
                                                                InputOnlineFile inputOnlineFile = new InputOnlineFile(fs,
                                                                    fileName);
                                                                await botClient.SendDocumentAsync(chatId: chatId,
                                                                    inputOnlineFile,
                                                                    caption: "🗄 <b>Папка загружена!</b>",
                                                                    parseMode: ParseMode.Html,
                                                                    cancellationToken: cancellationToken);
                                                            }
                                                            await botClient.DeleteMessageAsync(chatId: chatId,
                                                                messageId: processMessage.MessageId);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            await botClient.DeleteMessageAsync(chatId: chatId,
                                                            messageId: processMessage.MessageId);
                                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                            text: "🔴 <b>Итоговый архив получился слишком большим (больше 50 МБ) или у компьютера нестабильное соединение с Telegram для загрузки файлов!</b>" +
                                                                $"\nСообщение ошибки: <code>{ex.Message}</code>",
                                                                parseMode: ParseMode.Html,
                                                                cancellationToken: cancellationToken);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                        messageId: processMessage.MessageId,
                                                        text: "🔴 <b>Папка пустая!</b>",
                                                        parseMode: ParseMode.Html, cancellationToken: cancellationToken);
                                                }
                                                try { File.DeleteForever(fileName); } catch { }
                                                break;
                                            }
                                            else
                                            {
                                                if (SysFile.Exists(command[1]))
                                                {
                                                    FileInfo fileInf = new FileInfo(command[1]);
                                                    if ((double)fileInf.Length > 0 && (double)fileInf.Length < 52428800)
                                                    {
                                                        string fileName = Path.GetFileName(command[1]);
                                                        await botClient.EditMessageTextAsync(chatId: chatId,
                                                            processMessage.MessageId,
                                                            text: "🟡 Загружаю файл в бота...");
                                                        try
                                                        {
                                                            using (FileStream fs = SysFile.OpenRead(command[1]))
                                                            {
                                                                InputOnlineFile inputOnlineFile = new InputOnlineFile(fs,
                                                                $"{fileName}{fileInf.Extension}");
                                                                await botClient.SendDocumentAsync(chatId: chatId,
                                                                    inputOnlineFile,
                                                                    caption: "🗄 <b>Файл загружен!</b>",
                                                                    parseMode: ParseMode.Html,
                                                                    cancellationToken: cancellationToken);
                                                            }
                                                            await botClient.DeleteMessageAsync(chatId: chatId,
                                                                messageId: processMessage.MessageId);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            await botClient.DeleteMessageAsync(chatId: chatId,
                                                            messageId: processMessage.MessageId);
                                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                            text: "🔴 <b>Файл слишком большой (более 50 МБ) или у компьютера нестабильное соединение с Telegram для загрузки файлов!</b>" +
                                                                $"\nСообщение ошибки: <code>{ex.Message}</code>",
                                                                parseMode: ParseMode.Html,
                                                                cancellationToken: cancellationToken);
                                                        }
                                                        try { File.DeleteForever(fileName); } catch { }
                                                    }
                                                    else
                                                    {
                                                        await botClient.EditMessageTextAsync(chatId: chatId,
                                                            messageId: processMessage.MessageId,
                                                            text: "🔴 <b>Файл пустой или слишком большой (более 50 МБ)!</b>",
                                                            parseMode: ParseMode.Html,
                                                            cancellationToken: cancellationToken);
                                                    }
                                                }
                                                else
                                                {
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                        messageId: processMessage.MessageId,
                                                        text: "🔴 <b>Объект не найден!</b>",
                                                        parseMode: ParseMode.Html,
                                                        cancellationToken: cancellationToken);
                                                }
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                    text: "🔴 <b>Необходимо указать объект для отправки!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "apps":
                                    {
                                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                                text: "🟡 Собираю список установленных приложений...");
                                        await botClient.SendChatActionAsync(chatId: chatId,
                                        ChatAction.UploadDocument);
                                        await Task.Run(() =>
                                        {
                                            string toFile = $"Установленные приложения:\n{VitNX3.Functions.AppsAndProcesses.Installed.GetList()}";
                                            SysFile.WriteAllText($@"{tempPath}\Приложения.txt", toFile);
                                        });
                                        using (FileStream stream = SysFile.OpenRead($@"{tempPath}\Приложения.txt"))
                                        {
                                            await botClient.SendChatActionAsync(chatId: chatId,
                                                ChatAction.UploadDocument);
                                            InputOnlineFile inputOnlineFile = new InputOnlineFile(stream,
                                                "Приложения.txt");
                                            await botClient.SendDocumentAsync(chatId,
                                                inputOnlineFile,
                                                caption: "📁 <b>Подготовлен список установленных приложений!</b>",
                                                parseMode: ParseMode.Html,
                                                cancellationToken: cancellationToken);
                                        }
                                        await botClient.DeleteMessageAsync(chatId: chatId,
                                            messageId: processMessage.MessageId);
                                        try { File.DeleteForever($@"{tempPath}\Приложения.txt"); } catch { }
                                        break;
                                    }
                                case "tasks":
                                    {
                                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Получаю список процессов...");
                                        await botClient.SendChatActionAsync(chatId: chatId,
                                        ChatAction.Typing);
                                        string toFile = $"Все процессы:{Processes.GetListWithInformation()}";
                                        SysFile.WriteAllText(@$"{tempPath}\Процессы.txt", toFile);
                                        await botClient.SendChatActionAsync(chatId: chatId,
                                        ChatAction.UploadDocument);
                                        await botClient.DeleteMessageAsync(chatId: chatId,
                                        messageId: processMessage.MessageId);
                                        using (FileStream stream = SysFile.OpenRead(@$"{tempPath}\Процессы.txt"))
                                        {
                                            await botClient.SendChatActionAsync(chatId: chatId,
                                                ChatAction.UploadDocument);
                                            InputOnlineFile inputOnlineFile = new InputOnlineFile(stream,
                                               "Процессы.txt");
                                            await botClient.SendDocumentAsync(chatId,
                                                inputOnlineFile,
                                                caption: "⏳ <b>Подготовлен список процессов!</b>",
                                                parseMode: ParseMode.Html,
                                                cancellationToken: cancellationToken);
                                        }
                                        try { File.DeleteForever(@$"{tempPath}\Процессы.txt"); } catch { }
                                        break;
                                    }
                                case "cat":
                                    {
                                        if (command[1] != "")
                                        {
                                            var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Читаю файл...");
                                            await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.Typing);
                                            try
                                            {
                                                string fileText = File.GetText(TelegramBot.ArgumentsAsText(command[1]));
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                   messageId: processMessage.MessageId,
                                                   "📜 <b>Содержимое файла:</b>\n" +
                                                   $"<code>{fileText}</code>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                            }
                                            catch
                                            {
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    "🔴 <b>Чтение файла не может быть завершено, т.к текста слишком много для одного сообщения Telegram!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                            }
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                   "🔴 <b>Необходимо указать путь к файлу!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "msg":
                                    {
                                        if (command[1].Split(" ")[0] == "0" || command[1].Split(" ")[0] == "1" ||
                                            command[1].Split(" ")[0] == "2" || command[1].Split(" ")[0] == "3")
                                        {
                                            await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.Typing);
                                            switch (command[1].Split(" ")[0])
                                            {
                                                case "0":
                                                    {
                                                        command[1] = command[1].Remove(0, 2);
                                                        BeginInvoke(new Action(() =>
                                                        {
                                                            MessageBox.Show(command[1],
                                                            "Сообщение",
                                                            MessageBoxButtons.OK,
                                                            MessageBoxIcon.None,
                                                            MessageBoxDefaultButton.Button1,
                                                            MessageBoxOptions.DefaultDesktopOnly);
                                                        }));
                                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                                           "💬 <b>Сообщение отправлено!</b>",
                                                           parseMode: ParseMode.Html,
                                                           cancellationToken: cancellationToken);
                                                        break;
                                                    }
                                                case "1":
                                                    {
                                                        command[1] = command[1].Remove(0, 2);
                                                        BeginInvoke(new Action(() =>
                                                        {
                                                            MessageBox.Show(command[1],
                                                            "Информация",
                                                            MessageBoxButtons.OK,
                                                            MessageBoxIcon.Information,
                                                            MessageBoxDefaultButton.Button1,
                                                            MessageBoxOptions.DefaultDesktopOnly);
                                                        }));
                                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                                           "💬 <b>Сообщение с информацией отправлено!</b>",
                                                           parseMode: ParseMode.Html,
                                                           cancellationToken: cancellationToken);
                                                        break;
                                                    }
                                                case "2":
                                                    {
                                                        command[1] = command[1].Remove(0, 2);
                                                        BeginInvoke(new Action(() =>
                                                        {
                                                            MessageBox.Show(command[1],
                                                                "Ошибка",
                                                                MessageBoxButtons.OK,
                                                                MessageBoxIcon.Error,
                                                                MessageBoxDefaultButton.Button1,
                                                                MessageBoxOptions.DefaultDesktopOnly);
                                                        }));
                                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                                           "💬 <b>Сообщение с ошибкой отправлено!</b>",
                                                           parseMode: ParseMode.Html,
                                                           cancellationToken: cancellationToken);
                                                        break;
                                                    }
                                                case "3":
                                                    {
                                                        command[1] = command[1].Remove(0, 2);
                                                        BeginInvoke(new Action(() =>
                                                        {
                                                            MessageBox.Show(command[1],
                                                            "Предупреждение",
                                                            MessageBoxButtons.OK,
                                                            MessageBoxIcon.Warning,
                                                            MessageBoxDefaultButton.Button1,
                                                            MessageBoxOptions.DefaultDesktopOnly);
                                                        }));
                                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                                           "💬 <b>Сообщение с предупреждением отправлено!</b>",
                                                           parseMode: ParseMode.Html,
                                                           cancellationToken: cancellationToken);
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                                       "🔴 <b>Необходимо указать тип сообщения!</b>",
                                                       parseMode: ParseMode.Html,
                                                       cancellationToken: cancellationToken);
                                                        break;
                                                    }
                                            }
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                               "🔴 <b>Необходимо указать тип сообщения!</b>",
                                               parseMode: ParseMode.Html,
                                               cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "file":
                                    {
                                        if (command[1] != "")
                                        {
                                            command[1] = TelegramBot.ArgumentsAsText(command[1]);
                                            var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                                text: "🟡 Получаю информацию о файле...");
                                            await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.Typing);
                                            FileInfo fileInf = new FileInfo(command[1]);
                                            if (fileInf.Exists)
                                            {
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "📄 <b>Файл:</b>" +
                                                    "\nИмя: <code>" + fileInf.Name + "</code>" +
                                                    "\nПуть: <code>" + fileInf.FullName + "</code>" +
                                                    "\nДата и время создания: " + fileInf.CreationTime +
                                                    "\nMD5: <code>" + File.GetMD5(command[1]) + "</code>" +
                                                    "\nРазмер: " + ((double)fileInf.Length / 1048576).ToString("#.# МБ") + " (<code>" + fileInf.Length + "</code> Байт)",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                            }
                                            else
                                            {
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                   messageId: processMessage.MessageId,
                                                   "🔴 <b>Файл не найден!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                            }
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                   "🔴 <b>Необходимо указать путь к файлу!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "dir":
                                    {
                                        if (command[1] != "")
                                        {
                                            command[1] = TelegramBot.ArgumentsAsText(command[1]);
                                            var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                                text: "🟡 Получаю информацию о папке...");
                                            await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.Typing);
                                            DirectoryInfo dirInfo = new DirectoryInfo(command[1]);
                                            if (Directory.Exists(command[1]))
                                            {
                                                long length = Dir.GetSize(new DirectoryInfo(dirInfo.FullName));
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                   messageId: processMessage.MessageId,
                                                   text: "📁 <b>Папка:</b>" +
                                                   "\nИмя: <code>" + dirInfo.Name + "</code>" +
                                                   "\nПуть: <code>" + dirInfo.FullName + "</code>" +
                                                   "\nДата и время создания: " + dirInfo.CreationTime +
                                                   "\nРазмер: " + ((double)length / 1048576).ToString("#.# МБ") + " (<code>" + length + "</code> Байт)",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                            }
                                            else
                                            {
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                   messageId: processMessage.MessageId,
                                                   "🔴 <b>Папка не найдена!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                            }
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                   "🔴 <b>Необходимо указать путь к папке!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "del":
                                    {
                                        if (command[1] != "")
                                        {
                                            string filepath = TelegramBot.ArgumentsAsText(command[1]);
                                            var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                                text: "🟡 Удаляю файл в Корзину...");
                                            await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.Typing);
                                            try
                                            {
                                                if (SysFile.Exists(filepath))
                                                {
                                                    BeginInvoke(new Action(() => { File.DeleteToRecycleBin(filepath); }));
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                       messageId: processMessage.MessageId,
                                                       "🗑 <b>Файл удалён в Корзину!</b>",
                                                       parseMode: ParseMode.Html,
                                                       cancellationToken: cancellationToken);
                                                }
                                                else
                                                {
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                       messageId: processMessage.MessageId,
                                                       "🔴 <b>Файл не найден!</b>",
                                                       parseMode: ParseMode.Html,
                                                       cancellationToken: cancellationToken);
                                                }
                                            }
                                            catch
                                            {
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                   messageId: processMessage.MessageId,
                                                   "🔴 <b>Удаление файла невозможно!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                            }
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                   "🔴 <b>Необходимо указать путь к файлу!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "rd":
                                    {
                                        if (command[1] != "")
                                        {
                                            string dirpath = TelegramBot.ArgumentsAsText(command[1]);
                                            var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                                text: "🟡 Удаляю папку в Корзину...");
                                            await botClient.SendChatActionAsync(chatId: chatId,
                                            ChatAction.Typing);
                                            try
                                            {
                                                if (Directory.Exists(dirpath))
                                                {
                                                    BeginInvoke(new Action(() => { Dir.DeleteToRecycleBin(dirpath); }));
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                       messageId: processMessage.MessageId,
                                                       "🗑 <b>Папка удалена в Корзину!</b>",
                                                       parseMode: ParseMode.Html,
                                                       cancellationToken: cancellationToken);
                                                }
                                                else
                                                {
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                       messageId: processMessage.MessageId,
                                                       "🔴 <b>Папка не найдена!</b>",
                                                       parseMode: ParseMode.Html,
                                                       cancellationToken: cancellationToken);
                                                }
                                            }
                                            catch
                                            {
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                   messageId: processMessage.MessageId,
                                                   "🔴 <b>Удаление папки невозможно!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                            }
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                   "🔴 <b>Необходимо указать путь к папке!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "run":
                                    {
                                        if (command[1] != "")
                                        {
                                            var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🟡 Создаю процесс...");
                                            await botClient.SendChatActionAsync(chatId: chatId,
                                                ChatAction.Typing);
                                            try
                                            {
                                                string obj = command[1].Split('|')[0];
                                                string objData = "";
                                                try { objData = command[1].Split('|')[1]; } catch { }
                                                if (objData == "")
                                                    Processes.Open(obj);
                                                else
                                                    Processes.Run(obj, objData);
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                messageId: processMessage.MessageId,
                                                $"⛏ <b>Процесс создан!</b>",
                                                parseMode: ParseMode.Html,
                                                cancellationToken: cancellationToken);
                                            }
                                            catch
                                            {
                                                await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    "🔴 <b>Процесс не может быть создан, т.к. объекта для запуска не существует!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                            }
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                   "🔴 <b>Необходимо указать объект запуска!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "ping":
                                    {
                                        if (command[1] != "")
                                        {
                                            string address = "";
                                            try { address = TelegramBot.ArgumentsAsText(command[1].Split()[1].Replace("https://", "").Replace("http://", "").Replace("ftp://", "").Replace("ftps://", "")); }
                                            catch { address = TelegramBot.ArgumentsAsText(command[1].Replace("https://", "").Replace("http://", "").Replace("ftp://", "").Replace("ftps://", "")); }
                                            address = address.ToLower();
                                            if (address != "/ping")
                                            {
                                                Ping ping = new Ping();
                                                PingReply pingReply = null;
                                                var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                                text: "🟡 Пингую...");
                                                await botClient.SendChatActionAsync(chatId: chatId,
                                                ChatAction.Typing);
                                                try
                                                {
                                                    pingReply = ping.Send(address);
                                                    if (pingReply.Status != IPStatus.TimedOut)
                                                        await botClient.EditMessageTextAsync(chatId: chatId,
                                                            messageId: processMessage.MessageId,
                                                            text: $"🌐 <b>Сервер {address}</b>\n" +
                                                            $"IP-адрес: <code>{pingReply.Address}</code>\n" +
                                                            $"Статус: {Convert.ToString(pingReply.Status).Replace("Success", "Успех")}\n" +
                                                            $"Время ответа: {pingReply.RoundtripTime} сек.\n" +
                                                            $"TTL: <code>{pingReply.Options.Ttl}</code>\n" +
                                                            $"Не фрагментируется: <code>{Convert.ToString(pingReply.Options.DontFragment).Replace("True", "Да").Replace("False", "Нет")}</code>\n" +
                                                            $"Размер буфера: <code>{pingReply.Buffer.Length}</code>",
                                                            parseMode: ParseMode.Html,
                                                            cancellationToken: cancellationToken);
                                                    else
                                                    {
                                                        if (Convert.ToString(pingReply.Status) == "TimedOut")
                                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                                messageId: processMessage.MessageId,
                                                                text: $"🌐 <b>Сервер {address}</b>\n" +
                                                                $"Статус: Истекло время ожидания ответа",
                                                                parseMode: ParseMode.Html,
                                                                cancellationToken: cancellationToken);
                                                        else
                                                            await botClient.EditMessageTextAsync(chatId: chatId,
                                                                messageId: processMessage.MessageId,
                                                                text: $"🌐 <b>Сервер {address}</b>\n" +
                                                                $"Статус: {pingReply.Status}",
                                                                parseMode: ParseMode.Html,
                                                                cancellationToken: cancellationToken);
                                                    }
                                                }
                                                catch
                                                {
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                        messageId: processMessage.MessageId,
                                                        text: "🔴 <b>Ошибка получения данных!</b>\n" +
                                                        $"Сервер: <code>{address}</code>",
                                                        parseMode: ParseMode.Html,
                                                        cancellationToken: cancellationToken);
                                                }
                                            }
                                            else
                                                await botClient.SendTextMessageAsync(chatId: chatId,
                                                        text: $"🔴 <b>Требуется сайт или IP адрес!</b>",
                                                        parseMode: ParseMode.Html,
                                                        cancellationToken: cancellationToken);
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                       "🔴 <b>Необходимо указать действие!</b>",
                                                       parseMode: ParseMode.Html,
                                                       cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "sc":
                                    {
                                        if (command[1].ToLower().StartsWith("g") || command[1].ToLower().StartsWith("sta") ||
                                            command[1].ToLower().StartsWith("stp") || command[1].ToLower().StartsWith("r"))
                                        {
                                            if (command[1].ToLower().StartsWith("g"))
                                            {
                                                await botClient.SendChatActionAsync(chatId: chatId,
                                                ChatAction.Typing);
                                                var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                                text: "🟡 Получаю список служб...");
                                                string std = "";
                                                using (StreamWriter sw = new StreamWriter($@"{tempPath}\Службы.txt"))
                                                {
                                                    sw.WriteLine("Службы:");
                                                    ManagementObjectSearcher services = new ManagementObjectSearcher("SELECT * FROM Win32_Service");
                                                    ManagementObjectCollection services_collection = services.Get();
                                                    foreach (ManagementObject service in services_collection)
                                                    {
                                                        std = Convert.ToBoolean(service["Started"]) == true ? "Да" : "Нет";
                                                        sw.WriteLine($"\nСлужба: {service["Name"]}\n" +
                                                            $"Название: {service["Caption"]}\n" +
                                                            $"Описание: {service["Description"]}\n" +
                                                            $"Команда запуска: {service["PathName"]}\n" +
                                                            $"Запущена: {std}");
                                                    }
                                                }
                                                await botClient.SendChatActionAsync(chatId: chatId,
                                                ChatAction.UploadDocument);
                                                await botClient.DeleteMessageAsync(chatId: chatId, processMessage.MessageId);
                                                using (FileStream fs = SysFile.OpenRead($@"{tempPath}\Службы.txt"))
                                                {
                                                    InputOnlineFile inputOnlineFile = new InputOnlineFile(fs, "Службы.txt");
                                                    await botClient.SendDocumentAsync(chatId: chatId, inputOnlineFile, "🛎 <b>Подготовлен список служб!</b>");
                                                }
                                                try { File.DeleteForever($@"{tempPath}\Службы.txt"); } catch { }
                                            }
                                            if (command[1].ToLower().StartsWith("sta"))
                                            {
                                                var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                                text: "🟡 Запускаю службу...");
                                                var sc = ServicesControl.Start(TelegramBot.ArgumentsAsText(command[1].Remove(0, 4)));
                                                if (sc == ServicesControl.ServiceStatus.ALREADY_RUNNING)
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "🛎 <b>Служба уже запущена!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                                if (sc == ServicesControl.ServiceStatus.RUNNING)
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "🛎 <b>Служба запущена!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                                if (sc == ServicesControl.ServiceStatus.UNKNOWN_ERROR)
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "🔴 <b>Ошибка запуска службы!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                            }
                                            if (command[1].ToLower().StartsWith("stp"))
                                            {
                                                var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                                text: "🟡 Останавливаю службу...");
                                                var sc = ServicesControl.Stop(TelegramBot.ArgumentsAsText(command[1].Remove(0, 4)));
                                                if (sc == ServicesControl.ServiceStatus.ALREADY_STOPPED)
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "🛎 <b>Служба уже остановлена!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                                if (sc == ServicesControl.ServiceStatus.STOPPED)
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "🛎 <b>Служба остановлена!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                                if (sc == ServicesControl.ServiceStatus.UNKNOWN_ERROR)
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "🔴 <b>Ошибка остановки службы!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                            }
                                            if (command[1].ToLower().StartsWith("r"))
                                            {
                                                var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                                text: "🟡 Перезапускаю службу...");
                                                var sc = ServicesControl.Restart(TelegramBot.ArgumentsAsText(command[1].Remove(0, 2)));
                                                if (sc == ServicesControl.ServiceStatus.CONFLICT_RESTARTED)
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "🛎 <b>Служба не может быть перезапущена!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                                if (sc == ServicesControl.ServiceStatus.RESTARTED)
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "🛎 <b>Служба перезапущена!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                                if (sc == ServicesControl.ServiceStatus.UNKNOWN_ERROR)
                                                    await botClient.EditMessageTextAsync(chatId: chatId,
                                                    messageId: processMessage.MessageId,
                                                    text: "🔴 <b>Ошибка перезапуска службы!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                            }
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                   "🔴 <b>Необходимо указать действие!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "tprint":
                                    {
                                        if (command[1] != "")
                                        {
                                            string tfilePath = TelegramBot.ArgumentsAsText(command[1]);
                                            if (SysFile.Exists(tfilePath))
                                            {
                                                tfileStr = SysFile.ReadAllText(tfilePath);
                                                await botClient.SendChatActionAsync(chatId: chatId, ChatAction.Typing);
                                                PrintDocument printDocument = new PrintDocument();
                                                printDocument.PrintPage += PrintPageHandler;
                                                PrintDialog printDialog = new PrintDialog();
                                                printDialog.Document = printDocument;
                                                printDialog.Document.Print();
                                                await botClient.SendTextMessageAsync(chatId: chatId, text: "🖨 <b>Идёт печать содержимого текстового файла!</b>",
                                                    parseMode: ParseMode.Html,
                                                    cancellationToken: cancellationToken);
                                            }
                                            else
                                            {
                                                await botClient.SendTextMessageAsync(chatId: chatId,
                                                       "🔴 <b>Текстовый файл для печати не найден!</b>",
                                                       parseMode: ParseMode.Html,
                                                       cancellationToken: cancellationToken);
                                            }
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(chatId: chatId,
                                                   "🔴 <b>Необходимо указать текстовый файл!</b>",
                                                   parseMode: ParseMode.Html,
                                                   cancellationToken: cancellationToken);
                                        }
                                        break;
                                    }
                                case "info":
                                    {
                                        var processMessage = await botClient.SendTextMessageAsync(chatId: chatId,
                                               text: "🟡 Собираю информацию...");
                                        await botClient.SendChatActionAsync(chatId: chatId, ChatAction.UploadDocument);
                                        string monitor_data = "Невозможно опознать (отсутствует WMIC)";
                                        try
                                        {
                                            SysFile.WriteAllText($@"{tempPath}\monitor_data.bat", Resources.getmon);
                                            Processes.RunAW("cmd", $"/c \"{tempPath}\\monitor_data.bat\" >\"{tempPath}\\monitor_data.log\"", false);
                                            monitor_data = SysFile.ReadLines($@"{tempPath}\monitor_data.log").Skip(2).First();
                                            File.DeleteForever($@"{tempPath}\monitor_data.log");
                                        }
                                        catch { }
                                        var assemblyName = AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location);
                                        using (StreamWriter sw = new StreamWriter($@"{tempPath}\Компьютер.txt"))
                                        {
                                            sw.WriteLine("Информация о PC:\n");
                                            sw.WriteLine("------------- Система -------------");
                                            sw.WriteLine($"Версия Windows: {VitNX3.Functions.Information.Windows.GetWindowsVersion()} (x64) - {VitNX3.Functions.Information.Windows.GetWindowsProductNameFromRegistry()}");
                                            ManagementObjectSearcher _WinOS = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                                            ManagementObjectCollection WinOS = _WinOS.Get();
                                            foreach (ManagementObject WinOSobj in WinOS)
                                                sw.WriteLine("Серийный номер: " + WinOSobj["SerialNumber"]);
                                            sw.WriteLine($"Ключ Windows: {VitNX3.Functions.Information.Windows.GetWindowsProductKeyFromRegistry()}");
                                            sw.WriteLine($"Системная папка: {Environment.SystemDirectory}");
                                            sw.WriteLine($"Имя компьютера (хост): {Environment.MachineName}");
                                            sw.WriteLine($"Имя текущего пользователя: {SystemInformation.UserName}\n");
                                            sw.WriteLine("=====================================================================================");
                                            sw.WriteLine("------------- Материнская плата -------------");
                                            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                                            ManagementObjectCollection information = searcher.Get();
                                            try
                                            {
                                                foreach (ManagementObject obj in information)
                                                {
                                                    sw.WriteLine("Производитель: " + obj["Manufacturer"].ToString().Replace("To Be Filled By O.E.M.", "Производитель не указал"));
                                                    sw.WriteLine("Название: " + obj["Product"]);
                                                    sw.WriteLine("Модель: " + obj["Model"].ToString().Replace("To Be Filled By O.E.M.", "Производитель не указал"));
                                                    sw.WriteLine("Серийный номер: " + obj["SerialNumber"].ToString().Replace("To Be Filled By O.E.M.", "Производитель не указал"));
                                                }
                                            }
                                            catch { }
                                            sw.WriteLine("\n------------- UEFI/BIOS -------------");
                                            ManagementObjectSearcher mysBios = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
                                            foreach (ManagementObject myBios in mysBios.Get())
                                            {
                                                sw.WriteLine("Производитель: " + myBios["Manufacturer"].ToString().Replace("To Be Filled By O.E.M.", "Производитель не указал"));
                                                sw.WriteLine("Название: " + myBios["Name"]);
                                                sw.WriteLine("Версия: " + myBios["Version"].ToString().Replace("To Be Filled By O.E.M.", "Производитель не указал"));
                                                string bios_rdate = Convert.ToString(myBios["ReleaseDate"]).Replace("000000.000000+000", "");
                                                try
                                                {
                                                    DateTime dt = new DateTime();
                                                    if (DateTime.TryParseExact(bios_rdate, "yyyyMMdd",
                                                                              CultureInfo.InvariantCulture,
                                                                              DateTimeStyles.None, out dt))
                                                        bios_rdate = dt.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture);
                                                }
                                                catch { }
                                                sw.WriteLine($"Дата выпуска: {bios_rdate}");
                                                sw.WriteLine("Серийный номер: " + myBios["SerialNumber"].ToString().Replace("To Be Filled By O.E.M.", "Производитель не указал"));
                                            }
                                            sw.WriteLine($"Ключ Windows (OEM): {VitNX3.Functions.Information.Windows.GetWindowsProductKeyFromUefi()}\n");
                                            sw.WriteLine("=====================================================================================");
                                            sw.WriteLine("------------- Процессор (ЦПУ/CPU) -------------");
                                            ManagementObjectSearcher searcher8 = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_Processor");
                                            foreach (ManagementObject queryObj in searcher8.Get())
                                            {
                                                sw.WriteLine("Название: {0}", queryObj["Name"]);
                                                sw.WriteLine("Сокет: {0}", queryObj["SocketDesignation"]);
                                                try { sw.WriteLine("Серийный номер: {0}", queryObj["SerialNumber"].ToString().Replace("To Be Filled By O.E.M.", "Производитель не указал")); } catch { }
                                                sw.WriteLine("Число ядер: {0}", queryObj["NumberOfCores"]);
                                                sw.WriteLine("Число потоков: {0}", queryObj["NumberOfLogicalProcessors"]);
                                                sw.WriteLine("ID: {0}", queryObj["ProcessorId"]);
                                            }
                                            sw.WriteLine("\n------------- Оперативная память (ОЗУ/RAM) -------------");
                                            ManagementObjectSearcher searcher12 = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_PhysicalMemory");
                                            string ddr = "", ff = "";
                                            foreach (ManagementObject queryObj in searcher12.Get())
                                            {
                                                ddr = Convert.ToString(queryObj["MemoryType"]);
                                                ff = Convert.ToString(queryObj["FormFactor"]);
                                                switch (ddr)
                                                {
                                                    case "26":
                                                        ddr = "DDR4";
                                                        break;

                                                    case "25":
                                                        ddr = "FBD2 (серверная)";
                                                        break;

                                                    case "24":
                                                        ddr = "DDR3";
                                                        break;

                                                    case "22":
                                                        ddr = "DDR2 FB-DIMM (серверная)";
                                                        break;

                                                    case "21" or "17":
                                                        ddr = "DDR2";
                                                        break;

                                                    case "20":
                                                        ddr = "DDR";
                                                        break;

                                                    case "12":
                                                        ddr = "SO-DIMM";
                                                        break;

                                                    case "9":
                                                        ddr = "RAM";
                                                        break;

                                                    case "8":
                                                        ddr = "SRAM";
                                                        break;

                                                    case "7":
                                                        ddr = "VRAM";
                                                        break;

                                                    case "0" or "1":
                                                        ddr = "Невозможно определить";
                                                        break;
                                                }
                                                switch (ff)
                                                {
                                                    case "23":
                                                        ff = "LGA";
                                                        break;

                                                    case "12":
                                                        ff = "SO-DIMM";
                                                        break;

                                                    case "8":
                                                        ff = "DIMM";
                                                        break;

                                                    case "6":
                                                        ff = "Проприетарный";
                                                        break;

                                                    case "0" or "1":
                                                        ff = "Невозможно определить";
                                                        break;
                                                }
                                                sw.WriteLine("Производитель: {0}", queryObj["Manufacturer"]);
                                                sw.WriteLine("Описание: {0}", queryObj["Description"]);
                                                sw.WriteLine("Серийный номер: {0}", queryObj["SerialNumber"]);
                                                sw.WriteLine("Объём: {0} ГБ", Math.Round(Convert.ToDouble(queryObj["Capacity"]) / 1024 / 1024 / 1024, 2));
                                                sw.WriteLine("Скорость: {0} МГц", queryObj["Speed"]);
                                                sw.WriteLine($"Тип памяти: {ddr}");
                                                sw.WriteLine($"Форм-фактор: {ff}\n");
                                            }
                                            sw.WriteLine("------------- Видеокарта (GPU) -------------");
                                            string ggz = "Невозможно определить";
                                            ManagementObjectSearcher searcher11 = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_VideoController");
                                            foreach (ManagementObject queryObj in searcher11.Get())
                                            {
                                                string videoCardName = queryObj["Caption"].ToString();
                                                if (videoCardName.StartsWith("Radeon") || videoCardName.StartsWith("ATI"))
                                                    videoCardName = $"AMD {videoCardName}";
                                                else if (videoCardName.Contains("GT"))
                                                    videoCardName = $"NVIDIA {videoCardName}";
                                                sw.WriteLine("Название: {0}", videoCardName);
                                                if ((Convert.ToString(queryObj["AdapterRAM"]) == "536870912") || (Convert.ToString(queryObj["AdapterRAM"]) == "268435456") ||
                                                    (Convert.ToString(queryObj["AdapterRAM"]) == "268435456"))
                                                    sw.WriteLine("Количество памяти: {0} МБ", Math.Round(Convert.ToDouble(queryObj["AdapterRAM"]) / 1024 / 1024, 2));
                                                else
                                                    sw.WriteLine("Количество видеопамяти: {0} ГБ", Math.Round(Convert.ToDouble(queryObj["AdapterRAM"]) / 1024 / 1024 / 1024, 2));
                                                sw.WriteLine("Видеопроцессор: {0}", queryObj["VideoProcessor"]);
                                                ggz = Convert.ToString(queryObj["CurrentRefreshRate"]);
                                            }
                                            sw.WriteLine("\n------------- Монитор -------------");
                                            ManagementObjectSearcher searcher1113 = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_DesktopMonitor");
                                            var monitors = VitNX3.Functions.Information.Monitor.GetMergedFriendlyNames();
                                            foreach (var monitor in monitors)
                                            {
                                                sw.WriteLine("Модель: " + monitor);
                                                if (monitor == "")
                                                {
                                                    var monitorsByMonitorIds = VitNX3.Functions.Information.Monitor.GetNamesByMonitorIds();
                                                    foreach (var monitorByMonitorId in monitorsByMonitorIds)
                                                        sw.WriteLine($"Модель: {monitorByMonitorId}");
                                                }
                                                foreach (ManagementObject queryObj1132 in searcher1113.Get())
                                                {
                                                    sw.WriteLine("Тип: " + queryObj1132["MonitorType"]);
                                                    sw.WriteLine("ID: " + queryObj1132["PNPDeviceID"]);
                                                }
                                                sw.WriteLine($"Разрешение экрана: {Screen.PrimaryScreen.Bounds.Width}x{Screen.PrimaryScreen.Bounds.Height}");
                                                if (ggz != "Невозможно определить")
                                                    sw.WriteLine($"Частота обновления экрана: {ggz} Гц");
                                                else
                                                    sw.WriteLine($"Частота обновления экрана: {ggz}");
                                                if (monitor_data == "")
                                                    monitor_data = "Невозможно определить (отсутствует WMIC)";
                                                sw.WriteLine($"Год выпуска: {monitor_data}");
                                            }
                                            sw.WriteLine("\n=====================================================================================");
                                            sw.WriteLine("------------- Диски (физические) -------------");
                                            ManagementObjectSearcher mosDisks = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
                                            foreach (ManagementObject moDisk in mosDisks.Get())
                                            {
                                                sw.WriteLine("Модель: {0}", moDisk["Model"]);
                                                sw.WriteLine("Серийный номер: {0}", Convert.ToString(moDisk["SerialNumber"]).Replace(" ", ""));
                                                sw.WriteLine("ID: {0}", Convert.ToString(moDisk["DeviceID"]).Replace(@"\\.\", ""));
                                                if (Convert.ToString(moDisk["InterfaceType"]) == "SCSI")
                                                    sw.WriteLine("Интерфейс подключения: SATA");
                                                else
                                                {
                                                    if (Convert.ToString(moDisk["InterfaceType"]) == "IDE")
                                                        sw.WriteLine("Интерфейс подключения: IDE/SATA");
                                                    else
                                                        sw.WriteLine("Интерфейс подключения: {0}", moDisk["InterfaceType"]);
                                                }
                                                if (Convert.ToString(moDisk["MediaType"]) == "Fixed hard disk media")
                                                    sw.WriteLine("Тип: HDD/SSD");
                                                if (Convert.ToString(moDisk["MediaType"]) == "Removable Media")
                                                    sw.WriteLine("Тип: Флеш-накопитель (переносное устройство)");
                                                sw.WriteLine("Размер: {0} ГБ", Math.Round(Convert.ToDouble(moDisk["Size"]) / 1024 / 1024 / 1024, 2));
                                                sw.WriteLine("Разделы: {0}", moDisk["Partitions"]);
                                                sw.WriteLine("Прошивка: {0}", moDisk["FirmwareRevision"]);
                                                sw.WriteLine("Cylinders (цилиндры, совокупность всех дорожек в заданном положении привода): {0}", moDisk["TotalCylinders"]);
                                                sw.WriteLine("Сектора: {0}", moDisk["TotalSectors"]);
                                                sw.WriteLine("Головки: {0}", moDisk["TotalHeads"]);
                                                sw.WriteLine("Всего треков: {0}\n", moDisk["TotalTracks"]);
                                            }
                                            sw.WriteLine("------------- Локальные и сетевые диски, извлекаемые диски (флешки), дисководы -------------");
                                            ManagementObjectSearcher searcher44 = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_Volume");
                                            foreach (ManagementObject queryObj in searcher44.Get())
                                            {
                                                sw.WriteLine("Имя диска: {0}", queryObj["Caption"]);
                                                sw.WriteLine("Буква диска: {0}", queryObj["DriveLetter"]);
                                                string typeOfDisk = "Тип диска: ";
                                                int type = Convert.ToInt32(queryObj["DriveType"]);
                                                switch (type)
                                                {
                                                    case 0:
                                                        sw.WriteLine($"{typeOfDisk}Неизвестный диск");
                                                        break;

                                                    case 2:
                                                        sw.WriteLine($"{typeOfDisk}Извлекаемый диск (флешка)");
                                                        break;

                                                    case 3:
                                                        sw.WriteLine($"{typeOfDisk}Локальный диск");
                                                        break;

                                                    case 4:
                                                        sw.WriteLine($"{typeOfDisk}Сетевой диск");
                                                        break;

                                                    case 5:
                                                        sw.WriteLine($"{typeOfDisk}Дисковод");
                                                        break;

                                                    case 6:
                                                        sw.WriteLine($"{typeOfDisk}RAM-диск");
                                                        break;
                                                }
                                                sw.WriteLine("Размер: {0} ГБ", Math.Round(Convert.ToDouble(queryObj["Capacity"]) / 1024 / 1024 / 1024, 2));
                                                sw.WriteLine("Файловая система: {0}", queryObj["FileSystem"]);
                                                sw.WriteLine("Свободное место: {0} ГБ\n", Math.Round(Convert.ToDouble(queryObj["FreeSpace"]) / 1024 / 1024 / 1024, 2));
                                            }
                                            sw.WriteLine("=====================================================================================");
                                            sw.WriteLine("------------- USB устройства -------------");
                                            sw.Write(VitNX3.Functions.Information.UsbDevices.UsbToString());
                                            sw.WriteLine("=====================================================================================\n------------- Сетевые устройства -------------");
                                            ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_NetworkAdapterConfiguration");
                                            foreach (ManagementObject queryObj in searcher1.Get())
                                            {
                                                sw.WriteLine("Название: {0}", queryObj["Caption"]);
                                                if (queryObj["DefaultIPGateway"] == null)
                                                    sw.WriteLine("IP-шлюз по умолчанию: {0}", queryObj["DefaultIPGateway"]);
                                                else
                                                {
                                                    string[] arrDefaultIPGateway = (string[])queryObj["DefaultIPGateway"];
                                                    foreach (string arrValue in arrDefaultIPGateway)
                                                        sw.WriteLine("IP-шлюз по умолчанию: {0}", arrValue);
                                                }
                                                if (queryObj["DNSServerSearchOrder"] == null)
                                                    sw.WriteLine("Порядок поиска DNS-сервера: {0}", queryObj["DNSServerSearchOrder"]);
                                                else
                                                {
                                                    string[] arrDNSServerSearchOrder = (string[])queryObj["DNSServerSearchOrder"];
                                                    foreach (string arrValue in arrDNSServerSearchOrder)
                                                        sw.WriteLine("Порядок поиска DNS сервера: {0}", arrValue);
                                                }
                                                if (queryObj["IPAddress"] == null)
                                                    sw.WriteLine("IP-адрес: {0}", queryObj["IPAddress"]);
                                                else
                                                {
                                                    string[] arrIPAddress = (string[])queryObj["IPAddress"];
                                                    foreach (string arrValue in arrIPAddress)
                                                        sw.WriteLine("IP-адрес: {0}", arrValue);
                                                }
                                                if (queryObj["IPSubnet"] == null)
                                                    sw.WriteLine("IP подсеть: {0}", queryObj["IPSubnet"]);
                                                else
                                                {
                                                    string[] arrIPSubnet = (string[])queryObj["IPSubnet"];
                                                    foreach (string arrValue in arrIPSubnet)
                                                        sw.WriteLine("IP-подсеть: {0}", arrValue);
                                                }
                                                sw.WriteLine("MAC-адрес: {0}", queryObj["MACAddress"]);
                                                sw.WriteLine("Сервисное название: {0}\n", queryObj["ServiceName"]);
                                            }
                                        }
                                        await botClient.DeleteMessageAsync(chatId: chatId,
                                            messageId: processMessage.MessageId);
                                        using (FileStream fs = SysFile.OpenRead($@"{tempPath}\Компьютер.txt"))
                                        {
                                            InputOnlineFile inputOnlineFile = new InputOnlineFile(fs, @"Компьютер.txt");
                                            await botClient.SendDocumentAsync(chatId,
                                                 inputOnlineFile,
                                                 caption: "💻 <b>Информация о PC подготовлена!</b>",
                                                 parseMode: ParseMode.Html,
                                                 cancellationToken: cancellationToken);
                                        }
                                        try { File.DeleteForever($@"{tempPath}\Компьютер.txt"); } catch { }
                                        try { File.DeleteForever($@"{tempPath}\getmon.cmd"); } catch { }
                                    }
                                    break;
                            }
                            break;
                        }
                    case TelegramBot.BotCommandType.PLUGIN:
                        {
                            string plName = Engine.API.PluginsManager.SearchPluginWithCommand(command[0]);
                            try
                            {
                                var messageId = await botClient.SendTextMessageAsync(chatId: chatId,
                                text: "🔃 <b>Плагин работает...</b>",
                                parseMode: ParseMode.Html,
                                cancellationToken: cancellationToken);
                                int chatActionTypePlugin = Convert.ToInt32(Engine.Settings.Read("", "chat_action_type", Engine.Settings.TomlTypeRead.OnlyOneKey, @$"plugins\{plName}\main.manifest"));
                                int messageTypePlugin = Convert.ToInt32(Engine.Settings.Read("", "message_type", Engine.Settings.TomlTypeRead.OnlyOneKey, @$"plugins\{plName}\main.manifest"));
                                int plugin_type = Convert.ToInt32(Engine.Settings.Read("", "message_type", Engine.Settings.TomlTypeRead.OnlyOneKey, @$"plugins\{plName}\main.manifest"));
                                switch (chatActionTypePlugin)
                                {
                                    case 0:
                                        break;

                                    case 1:
                                        await botClient.SendChatActionAsync(chatId: chatId, ChatAction.Typing);
                                        break;
                                }
                                switch (messageTypePlugin)
                                {
                                    case 0:
                                        await botClient.EditMessageTextAsync(chatId: chatId,
                                        messageId.MessageId,
                                        text: Engine.API.PluginsManager.RunPluginScript(plName, command[1] != "" ? command[1] : "None", command[0], plugin_type: plugin_type));
                                        break;

                                    case 1:
                                        await botClient.EditMessageTextAsync(chatId: chatId,
                                        messageId.MessageId,
                                        text: Engine.API.PluginsManager.RunPluginScript(plName, command[1] != "" ? command[1] : "None", command[0], plugin_type: plugin_type),
                                        parseMode: ParseMode.Html,
                                        cancellationToken: cancellationToken);
                                        break;

                                    case 2:
                                        await botClient.SendTextMessageAsync(chatId: chatId,
                                            text: "🎯 <b>Плагин работает в фоне!</b>", parseMode: ParseMode.Html);
                                        await Task.Run(() => Engine.API.PluginsManager.RunPluginScript(plName, command[1] != "" ? command[1] : "None", command[0], background_worker: true, plugin_type: plugin_type));
                                        break;
                                }
                            }
                            catch (Exception ex) { AddEvent($"Ошибка плагина \"{plName}\": {ex.Message}"); }
                            break;
                        }
                    default:
                        {
                            await botClient.SendTextMessageAsync(chatId: chatId,
                    text: "❌ <b>Введённой команды не существует!</b>",
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);
                            break;
                        }
                }
            }
            catch (Exception ex) { AddEvent($"Неизвестная проблема бота: {ex.Message}"); }
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Ошибка Telegram Bot API:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            if (!ErrorMessage.Contains("[409]") && !ErrorMessage.Contains("[401]") && !ErrorMessage.Contains("1101"))
                AddEvent(ErrorMessage);
            if (!VitNX3.Functions.Web.DataFromSites.IsValidTelegramBotToken(Engine.Settings.MemoryValues.token))
                AddEvent("Ошибка токена бота!");
            return Task.CompletedTask;
        }

        private void windowTitle_MouseDown(object sender, MouseEventArgs e)
        {
            Import.ReleaseCapture();
            Import.PostMessage(Handle,
                Constants.WM_SYSCOMMAND,
                Constants.DOMOVE, 0);
        }

        public void sc(bool with_tree = true)
        {
            pictureBox3.Image = Resources.bot2023;
            pictureBox4.Image = Resources.snowflake;
            pictureBox5.Image = Resources.gift_box;
            pictureBox6.Image = Resources.docs2023;
            vitnX2_PictureBox1.Image = Resources.news2000;
            vitnX2_PictureBox2.Image = Resources.home;
            vitnX2_PictureBox3.Image = Resources.author;
            vitnX2_PictureBox4.Image = Resources.lollipops;
            vitnX2_PictureBox5.Image = Resources.chocolate_git;
            pictureBox7.Image = Resources.DevPlugin2024;
            selectedMenu.BackColor = Color.FromArgb(197, 66, 69);
            botPowerControl.OnBackColor = Color.FromArgb(197, 66, 69);
            if (with_tree)
            {
                pictureBox1.Enabled = true;
                pictureBox1.Visible = true;
            }
        }

        private async void titleExit_Click(object sender, EventArgs e)
        {
            //if (!Values.AppUI.use_forced_performance)
            //{
            //    if (!Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_transparency")))
            //    {
            //        if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_animation")))
            //        {
            //            form_x = Location.X;
            //            form_y = Location.Y;
            //            do
            //            {
            //                Opacity -= 0.2;
            //                await Task.Delay(1);
            //            }
            //            while (Opacity > 0);
            //        }
            //    }
            //}
            if (show_first_message)
            {
                try
                {
                    var botClient = new TelegramBotClient(Engine.Settings.MemoryValues.token);
                    string s = Values.Config.ReadUsername(true);
                    if (s.Contains('|'))
                        await SendNotification(botClient, s.Split('|')[1], $"🛑 Компьютер <i>{Dns.GetHostName()}</i> (<b>{ip}</b>) отключён!");
                }
                catch { }
            }
            Application.Exit();
        }

        private void titleExit_MouseEnter(object sender, EventArgs e)
        {
            titleExit.ForeColor = Color.Red;
            label21.ForeColor = Color.Red;
        }

        private void titleExit_MouseLeave(object sender, EventArgs e)
        {
            titleExit.ForeColor = Color.FromArgb(183, 185, 191);
            label21.ForeColor = Color.FromArgb(183, 185, 191);
        }

        private void eventsLog_DoubleClick(object sender, EventArgs e) => Clipboard.SetText(eventsLog.SelectedItem.ToString());

        public void PrintPageHandler(object sender, PrintPageEventArgs e) => e.Graphics.DrawString(tfileStr, new Font(Engine.Settings.Read("PRINT_OPTIONS", "font"),
                Convert.ToInt32(Engine.Settings.Read("PRINT_OPTIONS", "size"))),
                Brushes.Black, 0, 0);

        private void botPowerPanel_Click(object sender, EventArgs e) => botPowerControl.Checked = !botPowerControl.Checked;

        private void label3_Click(object sender, EventArgs e) => botPowerControl.Checked = !botPowerControl.Checked;

        private void label4_Click(object sender, EventArgs e) => botPowerControl.Checked = !botPowerControl.Checked;

        private int form_x, form_y;

        private async void titleMinimize_Click(object sender, EventArgs e)
        {
            if (!Values.AppUI.use_forced_performance)
            {
                if (!Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_transparency")))
                {
                    if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_animation")))
                    {
                        form_x = Location.X;
                        form_y = Location.Y;
                        do
                        {
                            Opacity -= 0.2;
                            Location = new Point(Location.X - 25, Location.Y + 35);
                            await Task.Delay(1);
                        }
                        while (Opacity > 0);
                    }
                }
            }
            Hide();
            ShowInTaskbar = false;
            SharkIcon.Visible = true;
        }

        private void SharkIcon_Click(object sender, EventArgs e)
        {
            ShowInTaskbar = true;
            titleMinimize.ForeColor = Color.FromArgb(183, 185, 191);
            label22.ForeColor = Color.FromArgb(183, 185, 191);
            Show();
            if (Values.AppModes.mini)
            {
                ClientSize = new Size(UI.Window.Dpi(210), UI.Window.Dpi(229));
                Size = new Size(UI.Window.Dpi(210), UI.Window.Dpi(229));
            }
            else
            {
                ClientSize = new Size(UI.Window.Dpi(757), UI.Window.Dpi(362));
                Size = new Size(UI.Window.Dpi(757), UI.Window.Dpi(362));
            }
            Location = new Point(form_x, form_y);
            StartPosition = FormStartPosition.CenterScreen;
            if (!Values.AppUI.use_forced_performance)
            {
                if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_transparency")))
                    Opacity = 0.96;
                else
                {
                    if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_animation")))
                    {
                        Opacity = 0;
                        System.Windows.Forms.Timer launch = new System.Windows.Forms.Timer();
                        launch.Tick += new EventHandler((sender, e) =>
                        {
                            if ((Opacity += 0.05d) == 1)
                                launch.Stop();
                        });
                        launch.Interval = 20;
                        launch.Start();
                    }
                }
            }
            SharkIcon.Visible = false;
        }

        private void openPluginsDir_Click(object sender, EventArgs e) => Processes.Open($"\"{FileSystem.GetDataPath()}\\plugins\"");

        private void plgInstallPicker_Click(object sender, EventArgs e)
        {
            if (!botPowerControl.Checked)
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Plugin installer (*.srp)|*.srp";
                openFileDialog1.InitialDirectory = Application.StartupPath;
                openFileDialog1.Title = "Укажите файл установщика плагина";
                var dr = openFileDialog1.ShowDialog();
                if (dr == DialogResult.Cancel || openFileDialog1.FileName == "")
                    return;
                if (dr == DialogResult.OK)
                {
                    string filename = openFileDialog1.FileName;
                    string foldername = Path.GetFileName(filename).Replace(".srp", "");
                    if ((new FileInfo(filename).Length / 1048576) < 20 || Values.AppHiddenParameters.P_ISL)
                    {
                        try
                        {
                            if (Directory.Exists($@"{FileSystem.GetDataPath()}\plugins\{foldername}"))
                                try { Dir.DeleteForever($@"{FileSystem.GetDataPath()}\plugins\{foldername}"); } catch { }
                            ZipFile.ExtractToDirectory(filename, $@"{FileSystem.GetDataPath()}\plugins\{foldername}", true);
                            string[] listPlg = SysFile.ReadAllLines($@"{FileSystem.GetDataPath()}\plugins\installed.cfg");
                            string plg = "";
                            if (SysFile.Exists($@"{FileSystem.GetDataPath()}\plugins\{foldername}\main.ps1"))
                                plg = SysFile.ReadAllLines($@"{FileSystem.GetDataPath()}\plugins\{foldername}\main.ps1")[0].Remove(0, 1);
                            if (SysFile.Exists($@"{FileSystem.GetDataPath()}\plugins\{foldername}\main.lua"))
                                plg = SysFile.ReadAllLines($@"{FileSystem.GetDataPath()}\plugins\{foldername}\main.lua")[0].Remove(0, 1);
                            var lines = SysFile.ReadAllLines($@"{FileSystem.GetDataPath()}\plugins\installed.cfg").Where(line => line.Trim().Split(", ")[0].Remove(0, 4) != plg.Trim().Split(", ")[0].Remove(0, 4)).ToArray();
                            SysFile.WriteAllLines($@"{FileSystem.GetDataPath()}\plugins\installed.cfg", lines);
                            SysFile.AppendAllText($@"{FileSystem.GetDataPath()}\plugins\installed.cfg", $"{plg.Trim()}\n");
                            VitNX2_MessageBox.Show("Установка плагина завершена!",
                            "Плагин установлен",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            if (Directory.Exists($@"{FileSystem.GetDataPath()}\plugins\{foldername}"))
                                try { Dir.DeleteForever($@"{FileSystem.GetDataPath()}\plugins\{foldername}"); } catch { }
                            VitNX2_MessageBox.Show($"Плагин не найден!\nОшибка: {ex.Message}",
                                "Ошибка установки плагина",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                    else
                        VitNX2_MessageBox.Show("Установшмк плагина имеет размер более 20 МБ!", "Установщик плагина слишком большой", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
                VitNX2_MessageBox.Show("Выключите бота, чтобы установить новый плагин!",
                     "Требуется выключить бота!",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Information);
        }

        private void plgList_Click(object sender, EventArgs e)
        {
            bool havePlg = false;
            pluginsManagerList.Items.Clear();
            string[] list = SysFile.ReadAllLines($@"{FileSystem.GetDataPath()}\plugins\installed.cfg");
            foreach (string item in list)
            {
                if (item != "")
                {
                    if (!item.StartsWith('#'))
                    {
                        pluginsManagerList.Items.Add("Название: " + item.Split(", ")[0].Remove(0, 4) +
                            "  Версия: " + item.Split(", ")[1].Remove(0, 8) + "  Автор: " + item.Split(", ")[2].Remove(0, 7) +
                            "  Команда: " + item.Split(", ")[3].Remove(0, 8));
                        havePlg = true;
                    }
                    else
                        pluginsManagerList.Items.Clear();
                }
            }
            if (!havePlg)
                VitNX2_MessageBox.Show("Не найдено установленных плагинов!", "Нет установленных плагинов", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                plgManagerListPanel.Visible = true;
        }

        private void plgManagerListClose_Click(object sender, EventArgs e) => plgManagerListPanel.Visible = false;

        private void plgManagerListClose_MouseEnter(object sender, EventArgs e) => plgManagerListClose.ForeColor = Color.FromArgb(0, 144, 242);

        private void plgManagerListClose_MouseLeave(object sender, EventArgs e) => plgManagerListClose.ForeColor = Color.FromArgb(183, 185, 191);

        private void plgBtnDelete_Click(object sender, EventArgs e)
        {
            bool isDelete = true;
            if (!botPowerControl.Checked)
            {
                var msg = VitNX2_MessageBox.Show($"Желаете удалить плагин \"{plgSelectedName.Text}\"?",
                    "Запрос на удаление плагина",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);
                if (msg == DialogResult.Yes)
                {
                    try
                    {
                        string[] delPlgs = SysFile.ReadAllLines($@"{FileSystem.GetDataPath()}\plugins\installed.cfg");
                        Thread.Sleep(50);
                        SysFile.Create($@"{FileSystem.GetDataPath()}\plugins\installed.cfg").Close();
                        Thread.Sleep(20);
                        using (StreamWriter writer = new StreamWriter($@"{FileSystem.GetDataPath()}\plugins\installed.cfg"))
                        {
                            foreach (string delPlg in delPlgs)
                            {
                                if (!delPlg.StartsWith('#'))
                                {
                                    if (plgSelectedName.Text == delPlg.Split(", ")[0].Remove(0, 4))
                                    {
                                        try { Dir.DeleteForever($@"{FileSystem.GetDataPath()}\plugins\{plgSelectedName.Text}"); }
                                        catch
                                        {
                                            isDelete = false;
                                            VitNX2_MessageBox.Show("Папка с плагином не может быть удалена!",
                                                "Невозможно удалить плагин",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                                        }
                                        if (isDelete)
                                        {
                                            pluginsManagerList.Items.Remove(pluginsManagerList.SelectedItem);
                                            plgSelected.Visible = false;
                                            VitNX2_MessageBox.Show($"Удаление плагина \"{plgSelectedName.Text}\" завершено!",
                                                "Плагин удалён",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Information);
                                            if (pluginsManagerList.Items.Count <= 0)
                                            {
                                                plgManagerListPanel.Visible = false;
                                                plgSelected.Visible = false;
                                            }
                                        }
                                        else
                                            writer.WriteLine(delPlg);
                                    }
                                    else
                                        writer.WriteLine(delPlg);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        VitNX2_MessageBox.Show($"Ошибка удаления плагина!\n{ex.Message}",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    }
                }
            }
            else
                VitNX2_MessageBox.Show("Выключите бота, чтобы удалить плагин!",
                    "Требуется выключить бота!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
        }

        private void plgSelectedExit_Click(object sender, EventArgs e) => plgSelected.Visible = false;

        private void plgSelectedExit_MouseEnter(object sender, EventArgs e) => plgSelectedExit.ForeColor = Color.FromArgb(0, 144, 242);

        private void plgSelectedExit_MouseLeave(object sender, EventArgs e) => plgSelectedExit.ForeColor = Color.FromArgb(183, 185, 191);

        private void plgBtnDir_Click(object sender, EventArgs e) => Processes.Open($"\"{FileSystem.GetDataPath()}\\plugins\\{plgSelectedName.Text}\"");

        private void TitleLabel_Click(object sender, EventArgs e) => versionPanel.Visible = !versionPanel.Visible;

        private void vitnX2_PictureBox1_Click(object sender, EventArgs e) => UI.OpenLink("https://t.me/s/NewsWiT");

        private void vitnX2_PictureBox2_Click(object sender, EventArgs e) => UI.OpenLink("https://sharkremote.neocities.org");

        private void vitnX2_PictureBox3_Click(object sender, EventArgs e)
        {
            UI.OpenLink("https://t.me/Zalexanninev15");
        }

        private void titleLabel_MouseEnter(object sender, EventArgs e)
        {
            if (!Values.AppModes.mini)
                versionPanel.Visible = !versionPanel.Visible;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Home_Click(this, null);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Settings_Click(this, null);
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Plugins_Click(this, null);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Help_Click(this, null);
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            UI.OpenLink("https://teletype.in/@zalexanninev15/Shark-Remote-Documentation#OSnJ");
        }

        private void getBotTokenNow_Click(object sender, EventArgs e)
        {
            try
            {
                IsChanged = true;
                string tmp = Clipboard.GetText();
                tmp = tmp.Replace("\"", "");
                if (tmp != "")
                {
                    if (!VitNX3.Functions.Web.DataFromSites.IsValidTelegramBotToken(tmp))
                    {
                        VitNX2_MessageBox.Show("Действительный токен не обнаружен!\nИспользуйте другой токен и бота, возможно у вас проблемы с Сетью",
                            "Ошибка проверки токена",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        Engine.Settings.MemoryValues.token = "";
                    }
                    else
                    {
                        Engine.Settings.MemoryValues.token = tmp;
                        VitNX2_MessageBox.Show("Токен принят и его работоспособность подтверждена!",
                            "Готово",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                else
                {
                    VitNX2_MessageBox.Show("Невозможно извлечь токен бота из буфера обмена!",
                    "Ошибка получения токена",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                    Engine.Settings.MemoryValues.token = "";
                }
            }
            catch
            {
                VitNX2_MessageBox.Show("Невозможно извлечь токен бота из буфера обмена!",
                "Ошибка получения токена",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                Engine.Settings.MemoryValues.token = "";
            }
        }

        private string username_old = "";

        private void vitnX2_Button1_Click(object sender, EventArgs e)
        {
            username_old = username.Texts;
            if (!Easter_Egg(username.Texts))
            {
                string user_old = Engine.Settings.Read("BOT", "admin");
                string user = username.Texts.Replace("\"", "");
                if (user != "")
                {
                    if (user.ToLower().StartsWith("https://t.me/"))
                        user = user.Replace("https://t.me/".ToLower(), "").Replace("http://t.me/".ToLower(), "").Replace("t.me/".ToLower(), "");
                    else if (user.ToLower().EndsWith(".t.me"))
                        user = user.Replace(".t.me".ToLower(), "").Replace("https://".ToLower(), "").Replace("http://".ToLower(), "");
                    else
                        user = user.StartsWith('@') ? user.Remove(0, 1) : user;
                }
                else
                {
                    VitNX2_MessageBox.Show("Не указан username пользователя!\nБудет использован старый username пользователя из настроек",
                             "Ошибка получения username",
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Error);
                    username.Texts = user_old;
                    user = user_old;
                }
                bool use_rounded_window_frame_style = Convert.ToBoolean(Engine.Settings.Read("UI", "use_rounded_window_frame_style"));
                string menu_color = Engine.Settings.Read("UI", "menu_color");
                bool use_window_transparency = Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_transparency"));
                bool use_window_animation = Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_animation"));
                int use_forced_performance = Convert.ToInt32(Engine.Settings.Read("UI", "use_forced_performance"));
                string imgbb_api_key = Engine.Settings.Read("BOT", "imgbb_api_key");
                int ipgeo = Convert.ToInt32(Engine.Settings.Read("GEOLOCATION", "selected_service"));
                string ipgeolocation_api_key = Engine.Settings.Read("GEOLOCATION", "ipgeolocationio_api_key");
                string font = Engine.Settings.Read("PRINT_OPTIONS", "font");
                int size = Convert.ToInt32(Engine.Settings.Read("PRINT_OPTIONS", "size"));
                string hbp = Engine.Settings.Read("OTHER", "hidden_bot_parameters");
                string hap = Engine.Settings.Read("OTHER", "hidden_application_parameters");
                if (vitnX2_ToogleButton2.Checked)
                {
                    use_forced_performance = 1;
                    Values.AppModes.mini = true;
                }
                try
                {
                    TomlTable toml = new TomlTable
                    {
                        ["BOT"] =
                        {
                            ["token"] = Engine.Settings.MemoryValues.token,
                            ["admin"] = user,
                            ["imgbb_api_key"] = imgbb_api_key
                        },
                        ["GEOLOCATION"] =
                        {
                            ["selected_service"] = ipgeo,
                            ["ipgeolocationio_api_key"] = ipgeolocation_api_key
                        },
                        ["PRINT_OPTIONS"] =
                        {
                            ["font"] = font,
                            ["size"] = size
                        },
                        ["UI"] =
                        {
                            ["use_rounded_window_frame_style"] = use_rounded_window_frame_style,
                            ["use_window_mini_mode"] = Values.AppModes.mini,
                            ["menu_color"] = menu_color,
                            ["use_window_transparency"] = use_window_transparency,
                            ["use_window_animation"] = use_window_animation,
                            ["use_forced_performance"] = use_forced_performance
                        },
                        ["OTHER"] =
                        {
                            ["config_version"] = Values.Config.VERSION,
                            ["hidden_bot_parameters"] = hbp,
                            ["hidden_application_parameters"] = hap
                        }
                    };
                    using (StreamWriter writer = SysFile.CreateText($@"{FileSystem.GetDataPath()}\settings\main.toml"))
                    {
                        toml.WriteTo(writer);
                        writer.Flush();
                    }
                    var Dialog = VitNX2_MessageBox.Show("Настройки будут применены после перезапуска приложения!", "Требуется перезапуск приложения", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Restart();
                }
                catch (Exception ex) { VitNX2_MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка записи настроек", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            else
                username.Texts = username_old;
        }

        private void vitnX2_Button5_Click(object sender, EventArgs e) => Processes.Open($"\"{FileSystem.GetDataPath()}\\settings\"");

        private void vitnX2_Button4_Click(object sender, EventArgs e) => botPowerControl.Checked = !botPowerControl.Checked;

        private bool Easter_Egg(string type)
        {
            if (type == "2020+3")
            {
                sc();
                return true;
            }
            else
                return false;
        }

        public bool IsNextStage = true;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.M && e.Control)
            {
                if (Values.AppModes.mini && IsNextStage)
                {
                    label21.Visible = false;
                    label22.Visible = false;
                    vitnX2_Panel4.Visible = false;
                    ClientSize = new Size(UI.Window.Dpi(757), UI.Window.Dpi(362));
                    Size = new Size(UI.Window.Dpi(757), UI.Window.Dpi(362));
                    Location = new Point(form_x, form_y);
                    StartPosition = FormStartPosition.CenterScreen;
                    if (!Values.AppUI.use_forced_performance)
                    {
                        if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_rounded_window_frame_style")) == true)
                        {
                            try
                            {
                                if (Convert.ToInt64(VitNX3.Functions.Information.Windows.GetWindowsCurrentBuildNumberFromRegistry()) >= 2200)
                                    Region = VitNX3.Functions.WindowAndControls.Window.SetWindowsElevenStyleForWinForm(Handle, Width, Height);
                                else
                                    Region = Region.FromHrgn(Import.CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
                            }
                            catch { Region = Region.FromHrgn(Import.CreateRoundRectRgn(0, 0, Width, Height, 15, 15)); }
                        }
                        if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_transparency")))
                            Opacity = 0.96;
                        else
                        {
                            if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_animation")))
                            {
                                Opacity = 0;
                                System.Windows.Forms.Timer launch = new System.Windows.Forms.Timer();
                                launch.Tick += new EventHandler((sender, e) =>
                                {
                                    if ((Opacity += 0.05d) == 1)
                                        launch.Stop();
                                });
                                launch.Interval = 20;
                                launch.Start();
                            }
                        }
                    }
                    SharkIcon.Visible = false;
                    modeChanger.Visible = false;
                    label22.Visible = false;
                    label21.Visible = false;
                    Values.AppModes.mini = false;
                    IsNextStage = false;
                }
                if (!Values.AppModes.mini && IsNextStage)
                {
                    label21.Visible = true;
                    label22.Visible = true;
                    vitnX2_Panel4.Visible = true;
                    ClientSize = new Size(UI.Window.Dpi(210), UI.Window.Dpi(229));
                    Size = new Size(UI.Window.Dpi(210), UI.Window.Dpi(229));
                    Location = new Point(form_x, form_y);
                    StartPosition = FormStartPosition.CenterScreen;
                    if (!Values.AppUI.use_forced_performance)
                    {
                        if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_rounded_window_frame_style")) == true)
                        {
                            try
                            {
                                if (Convert.ToInt64(VitNX3.Functions.Information.Windows.GetWindowsCurrentBuildNumberFromRegistry()) >= 2200)
                                    Region = VitNX3.Functions.WindowAndControls.Window.SetWindowsElevenStyleForWinForm(Handle, Width, Height);
                                else
                                    Region = Region.FromHrgn(Import.CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
                            }
                            catch { Region = Region.FromHrgn(Import.CreateRoundRectRgn(0, 0, Width, Height, 15, 15)); }
                        }
                        if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_transparency")))
                            Opacity = 0.96;
                        else
                        {
                            if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_animation")))
                            {
                                Opacity = 0;
                                System.Windows.Forms.Timer launch = new System.Windows.Forms.Timer();
                                launch.Tick += new EventHandler((sender, e) =>
                                {
                                    if ((Opacity += 0.05d) == 1)
                                        launch.Stop();
                                });
                                launch.Interval = 20;
                                launch.Start();
                            }
                        }
                    }
                    SharkIcon.Visible = false;
                    modeChanger.Visible = true;
                    label22.Visible = true;
                    label21.Visible = true;
                    Values.AppModes.mini = true;
                }
                IsNextStage = true;
                e.Handled = true;
            }
            if (e.KeyCode == Keys.P && e.Control)
            {
                if (Values.AppHiddenParameters.W_SM)
                {
                    DebugComboBox.DataSource = ProductMode.GetAllWindowHandleNames();
                    DebugComboBox.Visible = true;
                }
                else
                {
                    string selection = "Shark Remote";
                    var form = ProductMode.GetBitmapScreenshot(selection);
                    if (form == null)
                        return;
                    Clipboard.SetImage(form);
                }
            }
        }

        private void vitnX2_PictureBox4_Click(object sender, EventArgs e)
        {
            if (Values.AppModes.mini && IsNextStage)
            {
                label21.Visible = false;
                label22.Visible = false;
                vitnX2_Panel4.Visible = false;
                ClientSize = new Size(UI.Window.Dpi(757), UI.Window.Dpi(362));
                Size = new Size(UI.Window.Dpi(757), UI.Window.Dpi(362));
                Location = new Point(form_x, form_y);
                StartPosition = FormStartPosition.CenterScreen;
                if (!Values.AppUI.use_forced_performance)
                {
                    if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_rounded_window_frame_style")))
                    {
                        try
                        {
                            if (Convert.ToInt64(VitNX3.Functions.Information.Windows.GetWindowsCurrentBuildNumberFromRegistry()) >= 2200)
                                Region = VitNX3.Functions.WindowAndControls.Window.SetWindowsElevenStyleForWinForm(Handle, Width, Height);
                            else
                                Region = Region.FromHrgn(Import.CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
                        }
                        catch { Region = Region.FromHrgn(Import.CreateRoundRectRgn(0, 0, Width, Height, 15, 15)); }
                    }
                    if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_transparency")))
                        Opacity = 0.96;
                    else
                    {
                        if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_animation")))
                        {
                            Opacity = 0;
                            System.Windows.Forms.Timer launch = new System.Windows.Forms.Timer();
                            launch.Tick += new EventHandler((sender, e) =>
                            {
                                if ((Opacity += 0.05d) == 1)
                                    launch.Stop();
                            });
                            launch.Interval = 20;
                            launch.Start();
                        }
                    }
                }
                SharkIcon.Visible = false;
                modeChanger.Visible = false;
                label22.Visible = false;
                label21.Visible = false;
                Values.AppModes.mini = false;
                IsNextStage = false;
            }
            if (!Values.AppModes.mini && IsNextStage)
            {
                label21.Visible = true;
                label22.Visible = true;
                vitnX2_Panel4.Visible = true;
                ClientSize = new Size(UI.Window.Dpi(210), UI.Window.Dpi(229));
                Size = new Size(UI.Window.Dpi(210), UI.Window.Dpi(229));
                Location = new Point(form_x, form_y);
                StartPosition = FormStartPosition.CenterScreen;
                if (!Values.AppUI.use_forced_performance)
                {
                    if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_rounded_window_frame_style")))
                    {
                        try
                        {
                            if (Convert.ToInt64(VitNX3.Functions.Information.Windows.GetWindowsCurrentBuildNumberFromRegistry()) >= 2200)
                                Region = VitNX3.Functions.WindowAndControls.Window.SetWindowsElevenStyleForWinForm(Handle, Width, Height);
                            else
                                Region = Region.FromHrgn(Import.CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
                        }
                        catch { Region = Region.FromHrgn(Import.CreateRoundRectRgn(0, 0, Width, Height, 15, 15)); }
                    }
                    if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_transparency")))
                        Opacity = 0.96;
                    else
                    {
                        if (Convert.ToBoolean(Engine.Settings.Read("UI", "use_window_animation")))
                        {
                            Opacity = 0;
                            System.Windows.Forms.Timer launch = new System.Windows.Forms.Timer();
                            launch.Tick += new EventHandler((sender, e) =>
                            {
                                if ((Opacity += 0.05d) == 1)
                                    launch.Stop();
                            });
                            launch.Interval = 20;
                            launch.Start();
                        }
                    }
                }
                SharkIcon.Visible = false;
                modeChanger.Visible = true;
                label22.Visible = true;
                label21.Visible = true;
                Values.AppModes.mini = true;
            }
            IsNextStage = true;
        }

        private void versionLabel_Click(object sender, EventArgs e)
        {
            //progressVisual.Visible = true;
            //Task.Run(AppUpdater.Upgrade).Wait();
            //progressVisual.Visible = false;
        }

        private void vitnX_ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DebugComboBox.Visible = false;
            string selection = DebugComboBox.SelectedValue.ToString();
            if (string.IsNullOrEmpty(selection))
                return;
            var form = ProductMode.GetBitmapScreenshot(selection);
            if (form == null)
                return;
            Clipboard.SetImage(form);
        }

        private static string versionLabelText = Values.AppInfo.VersionLabel();

        private void versionLabel_MouseEnter(object sender, EventArgs e)
        {
            //versionLabel.ForeColor = Color.LimeGreen;
            //versionLabel.Text = "Желаете проверить на\r\n\tналичие новых версий?";
            //versionLabel.TextAlign = ContentAlignment.MiddleCenter;
        }

        private void pluginsManagerList_Click(object sender, EventArgs e)
        {
            try
            {
                plgSelectedName.Text = pluginsManagerList.SelectedItem.ToString().Split("  ")[0].Remove(0, 10);
                if (SysFile.Exists($@"{FileSystem.GetDataPath()}\plugins\{plgSelectedName.Text}\main.lua"))
                    plgSelectedPluginCodeType.Text = "Скрипт: Lua";
                if (SysFile.Exists($@"{FileSystem.GetDataPath()}\plugins\{plgSelectedName.Text}\main.ps1"))
                    plgSelectedPluginCodeType.Text = "Скрипт: PowerShell";
                plgSelected.Visible = true;
            }
            catch { }
        }

        private void vitnX2_PictureBox5_Click(object sender, EventArgs e)
        {
            UI.OpenLink("https://codeberg.org/Zalexanninev15/Shark-Remote");
        }

        private void vitnX2_Button2_Click(object sender, EventArgs e)
        {
            try
            {
                username.Texts = Clipboard.GetText();
            }
            catch { }
        }

        private void versionLabel_MouseLeave(object sender, EventArgs e)
        {
            //versionLabel.ForeColor = Color.FromArgb(247, 247, 248);
            //versionLabel.Text = versionLabelText;
        }

        private void progressVisual_Click(object sender, EventArgs e)
        {
            progressVisual.Visible = false;
        }

        private void vitnX2_ToogleButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (!firstScCheck)
            {
                progressVisual.Visible = true;
                progress.Visible = true;
                Cursor.Current = Cursors.WaitCursor;
                if (vitnX2_ToogleButton2.Checked)
                {
                    Downloader.Values.url = Values.AppInfo.WinSW_URL;
                    Downloader.Values.save_path = Values.AppInfo.service_path_tool;
                    Downloader.Values.done = false;
                    label2.Text = "Настраиваю...";
                    try
                    {
                        if (Directory.Exists(Values.AppInfo.service_path_tool) || Service.IsInstalled())
                        {
                            if (Network.InternetOk())
                            {
                                if (Directory.Exists(Values.AppInfo.service_path_tool)) { try { Dir.DeleteForever(Values.AppInfo.service_path_tool); } catch { } }
                                Directory.CreateDirectory("service");
                                progress.Value = 35;
                                new Downloader().ShowDialog();
                                progress.Value = 60;
                                SysFile.WriteAllText($"{Values.AppInfo.service_path_tool}WinSW.xml", Resources.WinSW);
                                string t = SysFile.ReadAllText($"{Values.AppInfo.service_path_tool}WinSW.xml");
                                t = t.Replace("<workingdirectory>Path to Shark Remote.exe here", $"<workingdirectory>{Values.AppInfo.startup_path}")
                                    .Replace("<executable>Shark Remote", $"<executable>{Values.AppInfo.startup_path}Shark Remote.exe");
                                SysFile.WriteAllText($"{Values.AppInfo.service_path_tool}WinSW.xml", t);
                                Processes.RunAW($"{Values.AppInfo.service_path_tool}WinSW.exe", $"install", false);
                                progress.Value = 95;
                                using (RegistryKey SharkRemoteDaemon = Registry.LocalMachine.CreateSubKey(@"SYSTEM\ControlSet001\Control\Windows", true))
                                    SharkRemoteDaemon.SetValue("NoInteractiveServices", 0);
                                Processes.RunAW($"{Values.AppInfo.service_path_tool}WinSW.exe", $"start", false);
                                progress.Value = 100;
                                label2.Text = "Включено";
                                VitNX2_MessageBox.Show("Служба установлена, но требуется выход из приложения и\nзапуск службы вручную из Служб Windows (только в первый раз).\nНазвание: Shark Remote (SharkRemoteDaemon)\nНе забудьте сохранить и применить настройки!", "Требуется ручная активация Службы Windows", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                label2.Text = "Сеть отсутствует";
                                VitNX2_MessageBox.Show("Установка Службы Windows в данный момент недоступна, попробуйте позже из настроек приложения", "Установка прервана", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            try
                            {
                                progress.Value = 60;
                                SysFile.WriteAllText($"{Values.AppInfo.service_path_tool}WinSW.xml", Resources.WinSW);
                                string t = SysFile.ReadAllText($"{Values.AppInfo.service_path_tool}WinSW.xml");
                                t = t.Replace("<workingdirectory>Path to Shark Remote.exe here", $"<workingdirectory>{Values.AppInfo.startup_path}");
                                SysFile.WriteAllText($"{Values.AppInfo.service_path_tool}WinSW.xml", t);
                                Processes.RunAW($"{Values.AppInfo.service_path_tool}WinSW.exe", $"install", false);
                                progress.Value = 95;
                                using (RegistryKey SharkRemoteDaemon = Registry.LocalMachine.CreateSubKey(@"SYSTEM\ControlSet001\Control\Windows", true))
                                    SharkRemoteDaemon.SetValue("NoInteractiveServices", 0);
                                Processes.RunAW($"{Values.AppInfo.service_path_tool}WinSW.exe", $"start", false);
                                progress.Value = 100;
                                label2.Text = "Включено";
                                VitNX2_MessageBox.Show("Служба установлена, но требуется выход из приложения и\nзапуск службы вручную из Служб Windows (только в первый раз).\nНазвание: Shark Remote (SharkRemoteDaemon)\nНе забудьте сохранить и применить настройки!", "Требуется ручная активация Службы Windows", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch
                            {
                                label2.Text = "Сеть отсутствует";
                                try { Dir.DeleteForever(Values.AppInfo.service_path_tool); } catch { }
                                VitNX2_MessageBox.Show("Установка Службы Windows в данный момент недоступна, попробуйте позже из настроек приложения", "Установка прервана", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try { Dir.DeleteForever(Values.AppInfo.service_path_tool); } catch { }
                        label2.Text = "Ошибка";
                        VitNX2_MessageBox.Show("Установка Службы Windows в данный момент недоступна, попробуйте позже из настроек приложенияn\nОшибка: " + ex.Message, "Установка прервана", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    Cursor.Current = Cursors.Default;
                }
                else
                {
                    if (Directory.Exists(Values.AppInfo.service_path_tool) && Service.IsInstalled())
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        try
                        {
                            try { Processes.RunAW($"{Values.AppInfo.service_path_tool}WinSW.exe", "uninstall", false); }
                            catch
                            {
                                try { Processes.RunAW($"{Values.AppInfo.service_path_tool}WinSW.exe", "stop", false); } catch { }
                                Thread.Sleep(5000);
                                try { Processes.RunAW($"{Values.AppInfo.service_path_tool}WinSW.exe", "uninstall", false); } catch { }
                            }
                            Thread.Sleep(5000);
                            try { Dir.DeleteForever(Values.AppInfo.service_path_tool); } catch { }
                            label2.Text = "Отключено";
                        }
                        catch
                        {
                            label2.Text = "Ошибка";
                            VitNX2_MessageBox.Show("Удаление Службы Windows в данный момент недоступна, попробуйте позже.", "Удаление прервано", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        Cursor.Current = Cursors.Default;
                    }
                    else
                    {
                        label2.Text = "Ошибка";
                        VitNX2_MessageBox.Show("Удаление Службы Windows в данный момент недоступна, попробуйте позже.", "Удаление прервано", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                progressVisual.Visible = false;
                progress.Visible = false;
            }
            else
            {
                label2.Text = "Включено";
                firstScCheck = false;
            }
            progress.Value = 0;
        }

        private void titleMinimize_MouseEnter(object sender, EventArgs e)
        {
            titleMinimize.ForeColor = Color.FromArgb(0, 144, 242);
            label22.ForeColor = Color.FromArgb(0, 144, 242);
        }

        private void titleMinimize_MouseLeave(object sender, EventArgs e)
        {
            titleMinimize.ForeColor = Color.FromArgb(183, 185, 191);
            label22.ForeColor = Color.FromArgb(183, 185, 191);
        }

        private void vitnX2_Button6_Click(object sender, EventArgs e)
        {
            IsChanged = true;
            new OtherSettings().ShowDialog();
        }
    }
}