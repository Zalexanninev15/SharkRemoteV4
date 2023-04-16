﻿using Shark_Remote.Helpers;

using System.Diagnostics;
using System.Text;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using File = System.IO.File;
using Lua = NLua.Lua;
using LuaFunction = NLua.LuaFunction;

namespace Shark_Remote.Engine.API
{
    public class Functions
    {
        [Flags]
        public enum ContentType
        {
            File,
            Audio
        }

        public static async Task SendNotification(TelegramBotClient bot,
            ChatId chatId,
            string message)
        {
            string fileName = string.Format("Скриншот_{0}_{1}.png",
            DateTime.Now.ToString("yyyyMMddHHmm"),
            Guid.NewGuid());
            InputOnlineFile inputOnlineFile = new InputOnlineFile(VitNX3.Functions.Information.Monitor.CaptureScreenToMemoryStream(), fileName);
            await bot.SendPhotoAsync(chatId: chatId,
                inputOnlineFile,
                caption: message,
                parseMode: ParseMode.Html);
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

        public static string RunLua(string name,
            string args = "None",
            bool resul_return = true)
        {
            string script = @$"{FileSystem.GetDataPath()}\plugins\{name}\main.lua";
            string script_header = File.ReadAllLines(script, Encoding.UTF8)[0];
            string script_code = File.ReadAllText(script, Encoding.UTF8);
            script_code = script_code.Replace($"{script_header}", "");
            using (Lua state = new Lua())
            {
                state.LoadCLRPackage();
                state.DoString($@"
	        {script_code}
	        ");
                var scriptFunc = state["MainFunc"] as LuaFunction;
                if (resul_return)
                {
                    if (args == "None")
                        return Convert.ToString(scriptFunc.Call("", "", "", "")[0]);
                    else
                    {
                        if (args.Contains("`"))
                        {
                            string[] new_args = args.Split("`");
                            if (new_args.Length == 2)
                                return Convert.ToString(scriptFunc.Call(new_args[0], new_args[1], "", "")[0]);
                            if (new_args.Length == 3)
                                return Convert.ToString(scriptFunc.Call(new_args[0], new_args[1], new_args[2], "")[0]);
                            if (new_args.Length == 4)
                                return Convert.ToString(scriptFunc.Call(new_args[0], new_args[1], new_args[2], new_args[3])[0]);
                        }
                        else
                            return Convert.ToString(scriptFunc.Call(args, "", "", "")[0]);
                    }
                }
                else
                {
                    if (args == "None")
                        scriptFunc.Call("", "", "", "");
                    else
                    {
                        if (args.Contains("`"))
                        {
                            string[] new_args = args.Split("`");
                            if (new_args.Length == 2)
                                scriptFunc.Call(new_args[0], new_args[1], "", "");
                            if (new_args.Length == 3)
                                scriptFunc.Call(new_args[0], new_args[1], new_args[2], "");
                            if (new_args.Length == 4)
                                scriptFunc.Call(new_args[0], new_args[1], new_args[2], new_args[3]);
                        }
                        else
                            scriptFunc.Call(args, "", "", "");
                    }
                }
                return "";
            }
        }

        public static string RunPowerShell(string name,
            string args = "None")
        {
            string script = $@"{FileSystem.GetDataPath()}\plugins\{name}\main.ps1";
            string aargs = $"-NoProfile -ExecutionPolicy unrestricted \"{script}\"";
            if (!args.Contains("None"))
                aargs += $" {args}";
            string PowerShell7 = @"C:\Program Files\PowerShell\7\pwsh.exe";
            string PowerShell = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            string returner;
            try
            {
                if (File.Exists(PowerShell7))
                {
                    Process pws = Process.Start(new ProcessStartInfo()
                    {
                        FileName = PowerShell7,
                        Arguments = aargs,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        RedirectStandardOutput = true,
                    });
                    pws.WaitForExit();
                    returner = pws.StandardOutput.ReadToEnd();
                }
                else
                {
                    if (File.Exists(PowerShell))
                    {
                        Process pws = Process.Start(new ProcessStartInfo()
                        {
                            FileName = PowerShell,
                            Arguments = aargs,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            RedirectStandardOutput = true,
                        });
                        pws.WaitForExit();
                        returner = pws.StandardOutput.ReadToEnd();
                    }
                    else
                        returner = "";
                }
            }
            catch { returner = ""; }
            return returner;
        }
    }
}