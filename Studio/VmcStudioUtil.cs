using Advent.Common.Interop;
using Advent.Common.IO;
using Advent.MediaCenter;
using Advent.MediaCenter.StartMenu.OEM;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Security.Principal;

namespace Advent.VmcStudio
{
    internal static class VmcStudioUtil
    {
        private static object applicationLock = new object();
        private static List<FontStyle> fontStyles;
        private static List<FontWeight> fontWeights;
        private static VmcStudioApp application;

        public static string ApplicationTitle
        {
            get
            {
                return "Media Center Studio BETA (" + (object)Assembly.GetExecutingAssembly().GetName().Version + ") Administrator: " + VmcStudioUtil.IsUserAdministrator();
            }
        }

        public static bool IsUserAdministrator()
        {
            //bool value to hold our return value
            bool isAdmin;
            try
            {
                //get the currently logged in user
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

        public static string ApplicationName
        {
            get
            {
                return "Media Center Studio";
            }
        }

        public static VmcStudioApp Application
        {
            get
            {
                if (VmcStudioUtil.application == null)
                {
                    lock (VmcStudioUtil.applicationLock)
                    {
                        if (VmcStudioUtil.application == null)
                        {
                            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                                VmcStudioUtil.application = new VmcStudioApp();
                        }
                    }
                }
                return VmcStudioUtil.application;
            }
        }

        public static string ApplicationDataPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), VmcStudioUtil.ApplicationName);
            }
        }

        public static string UserPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), VmcStudioUtil.ApplicationName);
            }
        }

        public static string BackupsPath
        {
            get
            {
                return Path.Combine(VmcStudioUtil.ApplicationDataPath, "Backups");
            }
        }

        public static string LogFilePath
        {
            get
            {
                return Path.Combine(VmcStudioUtil.ApplicationDataPath, "log.txt");
            }
        }

        [Obsolete]
        public static string RunWrapperPath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Environment.CommandLine.Trim('"', ' ')), "MCRunWrapper.exe");
            }
        }

        public static IEnumerable<FontStyle> FontStyles
        {
            get
            {
                if (VmcStudioUtil.fontStyles == null)
                {
                    VmcStudioUtil.fontStyles = new List<FontStyle>();
                    VmcStudioUtil.fontStyles.Add(System.Windows.FontStyles.Normal);
                    VmcStudioUtil.fontStyles.Add(System.Windows.FontStyles.Italic);
                    VmcStudioUtil.fontStyles.Add(System.Windows.FontStyles.Oblique);
                }
                return (IEnumerable<FontStyle>)VmcStudioUtil.fontStyles;
            }
        }

        public static IEnumerable<FontWeight> FontWeights
        {
            get
            {
                if (VmcStudioUtil.fontWeights == null)
                {
                    VmcStudioUtil.fontWeights = new List<FontWeight>();
                    VmcStudioUtil.fontWeights.Add(System.Windows.FontWeights.Normal);
                    VmcStudioUtil.fontWeights.Add(System.Windows.FontWeights.Bold);
                    VmcStudioUtil.fontWeights.Add(System.Windows.FontWeights.SemiBold);
                    VmcStudioUtil.fontWeights.Add(System.Windows.FontWeights.Light);
                }
                return (IEnumerable<FontWeight>)VmcStudioUtil.fontWeights;
            }
        }

        internal static object DragDropObject { get; set; }

        public static ImageSource DefaultApplicationImage
        {
            get
            {
                return Shell.GenerateThumbnail(".exe");
            }
        }

        static VmcStudioUtil()
        {
        }

        public static void LaunchDonate()
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.AppStarting;
                Process.Start(new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=advent%40live%2ecom%2eau&item_name=MC%20Menu%20Mender&no_shipping=1&cn=Comments&tax=0&currency_code=AUD&lc=AU&bn=PP%2dDonationsBF&charset=UTF%2d8"
                });
            }
            catch (Exception)
            {
            }
            finally
            {
                Mouse.OverrideCursor = (System.Windows.Input.Cursor)null;
            }
        }

        public static string GetImageFile()
        {
            string[] imageFiles = VmcStudioUtil.GetImageFiles(false);
            if (imageFiles != null)
                return imageFiles[0];
            else
                return (string)null;
        }

        public static string[] GetImageFiles()
        {
            return VmcStudioUtil.GetImageFiles(true);
        }

        private static string[] GetImageFiles(bool allowMultiple)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.DefaultExt = ".png";
            openFileDialog1.Multiselect = allowMultiple;
            openFileDialog1.Filter = "PNG files (*.png)|*.png|JPG files (*.jpg)|*.jpg|All files|*.*";
            Microsoft.Win32.OpenFileDialog openFileDialog2 = openFileDialog1;
            bool? nullable = openFileDialog2.ShowDialog();
            if ((nullable.HasValue ? (nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
                return openFileDialog2.FileNames;
            else
                return (string[])null;
        }

        public static string GetAudioFile()
        {
            string[] audioFiles = VmcStudioUtil.GetAudioFiles(false);
            if (audioFiles != null)
                return audioFiles[0];
            else
                return (string)null;
        }

        public static string[] GetAudioFiles()
        {
            return VmcStudioUtil.GetAudioFiles(true);
        }

        private static string[] GetAudioFiles(bool allowMultiple)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.DefaultExt = ".wav";
            openFileDialog1.Multiselect = allowMultiple;
            openFileDialog1.Filter = "Wave files (*.wav)|*.wav|WMA files (*.wma)|*.wma|MP3 files (*.mp3)|*.mp3|All files|*.*";
            Microsoft.Win32.OpenFileDialog openFileDialog2 = openFileDialog1;
            bool? nullable = openFileDialog2.ShowDialog();
            if ((nullable.HasValue ? (nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
                return openFileDialog2.FileNames;
            else
                return (string[])null;
        }

        public static string GetVideoFile()
        {
            string[] videoFiles = VmcStudioUtil.GetVideoFiles(false);
            if (videoFiles != null)
                return videoFiles[0];
            else
                return (string)null;
        }

        public static string[] GetVideoFiles()
        {
            return VmcStudioUtil.GetVideoFiles(true);
        }

        private static string[] GetVideoFiles(bool allowMultiple)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.DefaultExt = ".avi";
            openFileDialog1.Multiselect = allowMultiple;
            openFileDialog1.Filter = "WMV files (*.wmv)|*.wmv|AVI files (*.avi)|*.avi|All files|*.*";
            Microsoft.Win32.OpenFileDialog openFileDialog2 = openFileDialog1;
            bool? nullable = openFileDialog2.ShowDialog();
            if ((nullable.HasValue ? (nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
                return openFileDialog2.FileNames;
            else
                return (string[])null;
        }

        [Obsolete]
        public static string GetWrapperArgs(string targetFile, string targetArgs, EntryPointCapabilities caps, IEnumerable<Keys> closeKeys, IEnumerable<Keys> killKeys)
        {
            string str = string.Format("/file \"{0}\" /args {1}", (object)targetFile, (object)targetArgs);
            if ((caps & EntryPointCapabilities.DirectX) == EntryPointCapabilities.DirectX)
                str = "/directx " + str;
            string keys1 = VmcStudioUtil.GetKeys(closeKeys);
            if (!string.IsNullOrEmpty(keys1))
                str = string.Format("/closekeys \"{0}\" {1}", (object)keys1, (object)str);
            string keys2 = VmcStudioUtil.GetKeys(killKeys);
            if (!string.IsNullOrEmpty(keys2))
                str = string.Format("/killkeys \"{0}\" {1}", (object)keys2, (object)str);
            return str;
        }

        public static bool IsShortcut(string file)
        {
            if (!string.IsNullOrEmpty(file))
                return Path.GetExtension(file) == ".lnk";
            else
                return false;
        }

        public static bool PrepareForSave(MediaCenterLibraryCache cache)
        {
            bool flag = true;
            foreach (string path2 in cache.LoadedFiles)
            {
                string str = Path.Combine(cache.SearchPath, path2);
                if (File.Exists(str))
                {
                    VmcStudioUtil.BackupFile(str);
                    if (!VmcStudioUtil.TakeOwnership(str))
                        throw new VmcStudioException(string.Format("Could not take ownership of {0}.", (object)str));
                }
                else
                    Trace.TraceWarning("File not found: {0}", new object[1]
          {
            (object) str
          });
            }
            if (!VmcStudioUtil.EnsureMediaCenterClosed(true))
                flag = false;
            return flag;
        }

        public static bool RestoreModifiedFiles()
        {
            if (!VmcStudioUtil.EnsureMediaCenterClosed(true))
                return false;
            VmcStudioUtil.Application.CommonResources.CloseResources();
            try
            {
                foreach (string str in Directory.GetFiles(MediaCenterUtil.MediaCenterPath))
                {
                    string backupFile = VmcStudioUtil.GetBackupFile(str);
                    if (File.Exists(backupFile))
                        File.Copy(backupFile, str, true);
                }
            }
            finally
            {
                VmcStudioUtil.Application.CommonResources.ResetResources();
            }
            return true;
        }

        public static bool EnsureMediaCenterClosed(bool verifyBeforeClose)
        {
            if (VmcStudioUtil.EnsureNotRunning("ehshell", verifyBeforeClose))
                return VmcStudioUtil.EnsureNotRunning("ehtray", false);
            else
                return false;
        }

        public static string BackupFile(string file)
        {
            bool flag = false;
            string backupFile = VmcStudioUtil.GetBackupFile(file);
            if (!File.Exists(backupFile))
            {
                Trace.TraceInformation("No backup of {0} exists, creating {1}.", (object)file, (object)backupFile);
                flag = true;
            }
            else
            {
                string fileVersion1 = VmcStudioUtil.GetFileVersion(backupFile);
                string fileVersion2 = VmcStudioUtil.GetFileVersion(file);
                if (fileVersion1 != fileVersion2)
                {
                    Trace.TraceInformation("Version of {0} has changed ({1} to {2}), refreshing backup file {3}.", (object)file, (object)fileVersion1, (object)fileVersion2, (object)backupFile);
                    VmcStudioUtil.TakeOwnership(backupFile);
                    flag = true;
                }
            }
            if (flag)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(backupFile));
                File.Copy(file, backupFile, true);
            }
            return backupFile;
        }

        public static string GetFileVersion(string file)
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(file);
            return string.Format("{0}.{1}.{2}.{3}", (object)versionInfo.FileMajorPart, (object)versionInfo.FileMinorPart, (object)versionInfo.FileBuildPart, (object)versionInfo.FilePrivatePart);
        }

        public static bool TakeOwnership(string file)
        {
            if (VmcStudioUtil.Execute("takeown", "/f \"" + file + "\"") && VmcStudioUtil.Execute("icacls", "\"" + file + "\" /grant *S-1-5-32-545:F"))
                return VmcStudioUtil.Execute("icacls", "\"" + file + "\" /grant *S-1-5-32-544:F");
            else
                return false;
        }

        public static string CreateSupportPackage()
        {
            string path2 = Guid.NewGuid().ToString();
            string str1 = Path.Combine(Path.GetTempPath(), path2);
            Trace.TraceInformation("Creating support package...");
            Directory.CreateDirectory(str1);
            try
            {
                File.Copy(VmcStudioUtil.LogFilePath, Path.Combine(str1, Path.GetFileName(VmcStudioUtil.LogFilePath)));
                string path1 = Path.Combine(VmcStudioUtil.ApplicationDataPath, "Logs");
                if (Directory.Exists(path1))
                    FileUtil.CopyTo(new DirectoryInfo(path1), Path.Combine(str1, "Logs"), true);
                using (StreamWriter text = File.CreateText(Path.Combine(str1, "info.txt")))
                {
                    text.WriteLine("Support package ID: {0}", (object)path2);
                    text.WriteLine("{1} Version: {0}", (object)Assembly.GetEntryAssembly().GetName().Version, (object)VmcStudioUtil.ApplicationName);
                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(MediaCenterUtil.MediaCenterPath, "ehshell.exe"));
                    text.WriteLine("Media Center Version: {0}", (object)versionInfo.ProductVersion);
                    text.WriteLine("Date/Time: {0}", (object)DateTime.Now);
                    text.WriteLine("User preferred language: {0}", (object)LanguageUtils.GetUserDefaultUILanguage());
                    text.WriteLine("System default language: {0}", (object)LanguageUtils.GetSystemDefaultUILanguage());
                    text.WriteLine();
                    try
                    {
                        text.WriteLine("Dumping registry...");
                        string str2 = Path.Combine(str1, "Registry");
                        Directory.CreateDirectory(str2);
                        using (RegistryKey key1 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility"))
                        {
                            using (RegistryKey key2 = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility"))
                            {
                                using (RegistryKey key3 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Start Menu"))
                                {
                                    using (RegistryKey key4 = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Start Menu"))
                                    {
                                        VmcStudioUtil.DumpRegistry(key1, Path.Combine(str2, "HKEY_LOCAL_MACHINE Extensibility.txt"));
                                        VmcStudioUtil.DumpRegistry(key2, Path.Combine(str2, "HKEY_CURRENT_USER Extensibility.txt"));
                                        VmcStudioUtil.DumpRegistry(key3, Path.Combine(str2, "HKEY_LOCAL_MACHINE Start Menu.txt"));
                                        VmcStudioUtil.DumpRegistry(key4, Path.Combine(str2, "HKEY_CURRENT_USER Start Menu.txt"));
                                    }
                                }
                            }
                        }
                        text.WriteLine("Success!");
                    }
                    catch (Exception ex)
                    {
                        text.WriteLine(((object)ex).ToString());
                    }
                    text.WriteLine();
                    try
                    {
                        text.WriteLine("Dumping resources...");
                        string path3 = Path.Combine(str1, "Resources");
                        Directory.CreateDirectory(path3);
                        using (MediaCenterLibraryCache centerLibraryCache = new MediaCenterLibraryCache(MediaCenterUtil.MediaCenterPath))
                        {
                            IResourceLibrary lib = centerLibraryCache["ehres.dll"];
                            VmcStudioUtil.DumpHtmlResource(lib, "STARTMENU.XML", path3);
                            VmcStudioUtil.DumpHtmlResource(lib, "SM.ACTIVITIES.XML", path3);
                            VmcStudioUtil.DumpHtmlResource(lib, "SM.MUSIC.XML", path3);
                            VmcStudioUtil.DumpHtmlResource(lib, "SM.PICTURES.XML", path3);
                            VmcStudioUtil.DumpHtmlResource(lib, "SM.SPORTS.XML", path3);
                            VmcStudioUtil.DumpHtmlResource(lib, "SM.TV.XML", path3);
                            UnmanagedLibrary unmanagedLibrary = lib as UnmanagedLibrary;
                            if (unmanagedLibrary != null)
                                text.WriteLine("ehres.dll MUI languages: {0}", (object)VmcStudioUtil.ArrayToString(unmanagedLibrary.GetMUI().Languages));
                            text.WriteLine("STARTMENU.XML languages: {0}", (object)VmcStudioUtil.ArrayToString(lib.GetResource("STARTMENU.XML", (object)23).Languages));
                        }
                        text.WriteLine("Success!");
                    }
                    catch (Exception ex)
                    {
                        text.WriteLine(((object)ex).ToString());
                    }
                }
                string str3 = Path.Combine(VmcStudioUtil.UserPath, "Support Packages");
                if (!Directory.Exists(str3))
                    Directory.CreateDirectory(str3);
                string str4 = Path.Combine(str3, string.Format("Media Center Studio Support {0}.zip", (object)path2));
                new FastZip().CreateZip(str4, str1, true, (string)null);
                Trace.WriteLine("Support package successfully created at {0}", str4);
                return str4;
            }
            finally
            {
                Directory.Delete(str1, true);
            }
        }

        internal static string GetKeys(IEnumerable<Keys> keys)
        {
            string str = (string)null;
            if (keys != null)
            {
                foreach (Keys keys1 in keys)
                    str = str == null ? ((object)keys1).ToString() : str + (object)", " + (string)(object)keys1;
            }
            return str;
        }

        private static bool EnsureNotRunning(string exeName, bool queryUser)
        {
            foreach (Process process in Process.GetProcessesByName(exeName))
            {
                if (queryUser && System.Windows.MessageBox.Show(string.Format("{0} cannot apply changes while {1} is running. Do you want to close {1}?", (object)VmcStudioUtil.ApplicationTitle, (object)process.MainModule.FileVersionInfo.FileDescription), VmcStudioUtil.ApplicationTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes) != MessageBoxResult.Yes)
                    return false;
                if (process.MainWindowHandle != IntPtr.Zero)
                {
                    process.CloseMainWindow();
                    if (!process.WaitForExit(30000))
                    {
                        process.Kill();
                        process.WaitForExit(5000);
                    }
                }
                else
                {
                    process.Kill();
                    process.WaitForExit(5000);
                }
            }
            return true;
        }

        private static string GetBackupFile(string file)
        {
            return Path.Combine(VmcStudioUtil.BackupsPath, Path.GetFileName(file));
        }

        private static bool Execute(string file, string args)
        {
            Process process = Process.Start(new ProcessStartInfo()
            {
                FileName = file,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            process.Start();
            string message1 = process.StandardOutput.ReadToEnd();
            string message2 = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode == 0 || message2 == null || message2.Trim() == string.Empty)
                return true;
            Trace.TraceInformation(message1);
            Trace.TraceError(message2);
            return false;
        }

        private static string ArrayToString(IEnumerable<ushort> arr)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool flag = true;
            if (arr != null)
            {
                foreach (ushort num in arr)
                {
                    if (!flag)
                        stringBuilder.Append(", ");
                    flag = false;
                    stringBuilder.Append(num);
                }
            }
            return ((object)stringBuilder).ToString();
        }

        private static void DumpHtmlResource(IResourceLibrary lib, string resource, string path)
        {
            XmlReader xmlResource = MediaCenterUtil.GetXmlResource(lib, resource, 23);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlResource);
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter)stringWriter)
            {
                Formatting = Formatting.Indented
            };
            xmlDocument.WriteTo((XmlWriter)xmlTextWriter);
            using (StreamWriter text = File.CreateText(Path.Combine(path, resource)))
                text.Write(stringWriter.ToString());
        }

        private static void DumpRegistry(RegistryKey key, string path)
        {
            using (StreamWriter text = File.CreateText(path))
                VmcStudioUtil.DumpRegistry(key, (TextWriter)text);
        }

        private static void DumpRegistry(RegistryKey key, TextWriter file)
        {
            if (key == null)
                return;
            file.WriteLine(key.Name);
            foreach (string name in key.GetValueNames())
                file.WriteLine("{0} = {1}", (object)name, key.GetValue(name) ?? (object)string.Empty);
            file.WriteLine();
            foreach (string name in key.GetSubKeyNames())
            {
                using (RegistryKey key1 = key.OpenSubKey(name))
                    VmcStudioUtil.DumpRegistry(key1, file);
            }
        }
    }
}
