using Advent.Common.Interop;
using Advent.Common.IO;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Advent.VmcStudio.StartMenu
{
    public class Game
    {
        private static readonly Guid FOLDERIDGameTasks = new Guid("054FAE61-4DD8-4787-80B6-090220C4B700");
        private static readonly Guid FOLDERIDPublicGameTasks = new Guid("DEBF2536-E1A8-4c59-B6A2-414586476AEA");
        private readonly string name;
        private readonly string instanceID;
        private readonly string gameID;
        private readonly string gdfPath;
        private readonly string installPath;
        private readonly string gdfResourceID;
        private readonly Game.GameInstallScope installScope;
        private readonly ImageSource image;
        private readonly string description;
        private List<string> playTasks;
        private List<string> supportTasks;

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public string InstanceID
        {
            get
            {
                return this.instanceID;
            }
        }

        public string GameID
        {
            get
            {
                return this.gameID;
            }
        }

        public string InstallPath
        {
            get
            {
                return this.installPath;
            }
        }

        public Game.GameInstallScope InstallScope
        {
            get
            {
                return this.installScope;
            }
        }

        public ImageSource Image
        {
            get
            {
                return this.image;
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
        }

        public IList<string> PlayTasks
        {
            get
            {
                return (IList<string>)this.playTasks;
            }
        }

        public IList<string> SupportTasks
        {
            get
            {
                return (IList<string>)this.supportTasks;
            }
        }

        static Game()
        {
        }

        internal Game(System.Management.ManagementBaseObject game)
        {
            this.name = game.GetPropertyValue("Name") as string;
            this.instanceID = game.GetPropertyValue("InstanceID") as string;
            this.gameID = game.GetPropertyValue("GameID") as string;
            this.gdfPath = game.GetPropertyValue("GDFBinaryPath") as string;
            string str1 = this.gdfPath;
            this.installPath = game.GetPropertyValue("GameInstallPath") as string;
            this.gdfResourceID = game.GetPropertyValue("ResourceIDForGDFInfo") as string;
            this.installScope = (Game.GameInstallScope)game.GetPropertyValue("InstallScope");
            this.RefreshTasks();
            if (this.gdfPath != null)
            {
                try
                {
                    using (UnmanagedLibrary unmanagedLibrary = new UnmanagedLibrary(this.gdfPath))
                    {
                        try
                        {
                            byte[] bytes = ResourceExtensions.GetBytes(unmanagedLibrary.GetResource("__GDF_THUMBNAIL", (object)"DATA"));
                            if (bytes != null)
                            {
                                try
                                {
                                    this.image = (ImageSource)BitmapDecoder.Create((Stream)new MemoryStream(bytes), BitmapCreateOptions.None, BitmapCacheOption.Default).Frames[0];
                                }
                                catch (Exception ex)
                                {
                                    Trace.TraceWarning(string.Concat(new object[4]
                  {
                    (object) "Image for ",
                    (object) this.name,
                    (object) " could not be loaded: ",
                    (object) ex
                  }));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceWarning(string.Concat(new object[4]
              {
                (object) "Error attempting to get image resource for ",
                (object) this.name,
                (object) ":",
                (object) ex
              }));
                        }
                        using (RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\GameUX\\"))
                        {
                            if (registryKey1 != null)
                            {
                                foreach (object obj in registryKey1.GetSubKeyNames())
                                {
                                    string name = string.Format("{0}\\{1}", obj, (object)this.instanceID);
                                    RegistryKey registryKey2 = registryKey1.OpenSubKey(name);
                                    if (registryKey2 != null)
                                    {
                                        using (registryKey2)
                                        {
                                            if (this.image == null)
                                            {
                                                string uriString = registryKey2.GetValue("BoxArt") as string;
                                                if (uriString != null)
                                                {
                                                    try
                                                    {
                                                        this.image = (ImageSource)BitmapDecoder.Create(new Uri(uriString), BitmapCreateOptions.None, BitmapCacheOption.Default).Frames[0];
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Trace.TraceWarning(string.Concat(new object[4]
                            {
                              (object) "Image for legacy game ",
                              (object) this.name,
                              (object) " could not be loaded: ",
                              (object) ex
                            }));
                                                    }
                                                }
                                            }
                                            this.description = registryKey2.GetValue("Description") as string;
                                            if (this.playTasks.Count == 0)
                                            {
                                                string str2 = registryKey2.GetValue("AppExePath") as string;
                                                if (str2 != null)
                                                {
                                                    this.playTasks.Add(str2);
                                                    str1 = str2;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        string name1 = this.gdfResourceID ?? "__GDF_XML";
                        byte[] bytes1 = ResourceExtensions.GetBytes(unmanagedLibrary.GetResource(name1, (object)"DATA"));
                        if (bytes1 != null)
                        {
                            XmlDocument xmlDocument = new XmlDocument();
                            xmlDocument.Load((XmlReader)new XmlTextReader((Stream)new MemoryStream(bytes1)));
                            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.FirstChild.ChildNodes)
                            {
                                XmlElement xmlElement = xmlNode as XmlElement;
                                if (xmlElement != null && xmlElement.Name == "Description")
                                    this.description = xmlElement.InnerXml;
                            }
                        }
                        else
                            Trace.TraceWarning("GDF resource not found for " + this.Name + ".");
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning(string.Concat(new object[4]
          {
            (object) "Error getting GDF data for ",
            (object) this.Name,
            (object) ": ",
            (object) ex
          }));
                }
            }
            if (this.playTasks.Count == 0)
                throw new ArgumentException("Could not find a play task for " + this.name + ".");
            if (this.image != null)
                return;
            string filename = (string)null;
            if (this.playTasks.Count > 0)
            {
                try
                {
                    WshShell shell = new WshShell();
                    filename = ((IWshShortcut)shell.CreateShortcut(this.playTasks[0])).TargetPath;
                }
                catch
                {
                    throw new Exception("You commented out code, but didn't really care at the time");
                }
            }
            if (string.IsNullOrEmpty(filename))
                filename = str1;
            if (string.IsNullOrEmpty(filename))
                return;
            this.image = Shell.GenerateThumbnail(filename);
        }

        [DllImport("shell32.dll")]
        private static extern bool SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint flags, IntPtr token, out IntPtr pszPath);

        private static string GetKnownFolder(Guid folderID)
        {
            IntPtr pszPath;
            Game.SHGetKnownFolderPath(folderID, 0U, IntPtr.Zero, out pszPath);
            if (!(pszPath != IntPtr.Zero))
                throw new Win32Exception();
            string str = Marshal.PtrToStringUni(pszPath);
            Marshal.FreeCoTaskMem(pszPath);
            return str;
        }

        private void RefreshTasks()
        {
            string knownFolder = Game.GetKnownFolder(this.installScope == Game.GameInstallScope.CurrentUser ? Game.FOLDERIDGameTasks : Game.FOLDERIDPublicGameTasks);
            if (knownFolder == null)
                return;
            string path1 = Path.Combine(knownFolder, this.instanceID);
            this.playTasks = this.GetTasks(Path.Combine(path1, "PlayTasks"));
            this.supportTasks = this.GetTasks(Path.Combine(path1, "SupportTasks"));
        }

        private List<string> GetTasks(string folder)
        {
            List<string> list = new List<string>();
            if (Directory.Exists(folder))
            {
                for (int index = 0; index < 5; ++index)
                {
                    string[] files = Directory.GetFiles(Path.Combine(folder, index.ToString()), "*.lnk");
                    if (files.Length > 0)
                        list.Add(files[0]);
                }
            }
            return list;
        }

        public enum GameInstallScope : uint
        {
            CurrentUser = 2U,
            AllUsers = 3U,
        }
    }
}
