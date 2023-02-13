using System.Management.Automation;
using System.Text;

using Telegram.Bot;

namespace Shark_Remote.Engine.API
{
    public class Functions
    {
        [Flags]
        public enum ContentType
        {
            File,
            Image,
            Video,
            Audio
        }

        public static async Task DownloadContentManager(ITelegramBotClient botClient,
        Telegram.Bot.Types.Message message,
        string destinationFilePath,
        ContentType contentType)
        {
            if (contentType == ContentType.File)
            {
                var messageDocument = message.Document;
                var documentMessageTypeFileId = messageDocument.FileId;
                await using (FileStream fileStream = File.OpenWrite(destinationFilePath))
                {
                    await botClient.GetInfoAndDownloadFileAsync(documentMessageTypeFileId,
                        destination: fileStream);
                }
            }
            if (contentType == ContentType.Audio)
            {
                var messageDocument = message.Audio;
                var audioMessageTypeFileId = messageDocument.FileId;
                await using (FileStream fileStream = File.OpenWrite(destinationFilePath))
                {
                    await botClient.GetInfoAndDownloadFileAsync(audioMessageTypeFileId,
                        destination: fileStream);
                }
            }
        }

        public static string RunPowerShell(string name,
            string args = "None")
        {
            string script = $@"{Application.StartupPath}\data\plugins\{name}\main.ps1";
            string aargs = $"-NoProfile -ExecutionPolicy unrestricted \"{script}\"";
            if (!args.Contains("None"))
                aargs += $" {args}";
            string PowerShell7 = @"C:\Program Files\PowerShell\7\pwsh.exe";
            string returner = "";
            if (File.Exists(PowerShell7))
            {
                //Process pws = Process.Start(new ProcessStartInfo()
                //{
                //    FileName = PowerShell7,
                //    Arguments = aargs,
                //    UseShellExecute = false,
                //    CreateNoWindow = true,
                //    WindowStyle = ProcessWindowStyle.Hidden,
                //    RedirectStandardOutput = true,
                //    RedirectStandardInput = true,
                //});
                //pws.StandardInput.WriteLine(@"chcp 65001");
                //pws.WaitForExit();
                //returner = pws.StandardOutput.ReadToEnd();
            }
            else
            {
                //Process pws = Process.Start(new ProcessStartInfo()
                //{
                //    FileName = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe",
                //    Arguments = aargs,
                //    UseShellExecute = false,
                //    CreateNoWindow = true,
                //    WindowStyle = ProcessWindowStyle.Hidden,
                //    RedirectStandardOutput = true,
                //    RedirectStandardInput = true,
                //});
                //pws.StandardInput.WriteLine(@"chcp 65001");
                //pws.WaitForExit();
                //returner = pws.StandardOutput.ReadToEnd();
            }
            return returner;
        }

        private static readonly PowerShell _ps = PowerShell.Create();

        public static string Command(string script)
        {
            string errorMsg = string.Empty;
            _ps.AddScript(script);
            _ps.AddCommand("Out-String");
            PSDataCollection<PSObject> outputCollection = new();
            _ps.Streams.Error.DataAdded += (object sender, DataAddedEventArgs e) =>
            {
                errorMsg = ((PSDataCollection<ErrorRecord>)sender)[e.Index].ToString();
            };
            IAsyncResult result = _ps.BeginInvoke<PSObject, PSObject>(null, outputCollection);
            _ps.EndInvoke(result);
            StringBuilder sb = new();
            foreach (var outputItem in outputCollection)
                sb.AppendLine(outputItem.BaseObject.ToString());
            _ps.Commands.Clear();
            if (!string.IsNullOrEmpty(errorMsg))
                return errorMsg;
            return sb.ToString().Trim();
        }
    }
}