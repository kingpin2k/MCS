using Advent.Common.GAC;
using Advent.MediaCenter.StartMenu;
using Advent.MediaCenter.StartMenu.OEM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Advent.VmcStudio
{
    public abstract class MediaCenterAddInInstaller
    {
        public StartMenuManager StartMenuManager { get; private set; }

        protected abstract string GacReference { get; }

        protected abstract IEnumerable<string> AssemblyPaths { get; }

        protected MediaCenterAddInInstaller(StartMenuManager startMenuManager)
        {
            this.StartMenuManager = startMenuManager;
        }

        public virtual void Install()
        {
            List<AssemblyName> list = new List<AssemblyName>();
            foreach (string str in this.AssemblyPaths)
            {
                AssemblyName assemblyName1 = AssemblyName.GetAssemblyName(str);
                bool flag = true;
                
                foreach (AssemblyName assemblyName2 in Enumerable.Select<string, AssemblyName>(AssemblyCache.Global.SearchAssemblies(assemblyName1.Name), (Func<string, AssemblyName>)(o => new AssemblyName(o))))
                {
                    if (assemblyName2.Version == assemblyName1.Version)
                    {
                        flag = false;
                    }
                    else
                    {
                        Trace.TraceInformation("Version of assembly {0} in GAC ({1}) is different from local assembly ({2}), uninstalling copy from GAC.", (object)assemblyName1.Name, (object)assemblyName2.Version, (object)assemblyName1.Version);
                        AssemblyCache.Global.UninstallAssembly(assemblyName2.FullName, this.GacReference);
                    }
                }
                if (flag)
                {
                    Trace.TraceInformation("Installing assembly {0} (version {1}) into GAC.", (object)str, (object)assemblyName1.Version);
                    AssemblyCache.Global.InstallAssembly(str, this.GacReference, true);
                    list.Add(assemblyName1);
                }
            }
            if (list.Count <= 0)
                return;
            char[] splitter = { ',' };
            foreach (AssemblyName assemblyName in list)
            {
                foreach (EntryPoint entryPoint in Enumerable.Where<EntryPoint>((IEnumerable<EntryPoint>)this.StartMenuManager.OemManager.EntryPoints, (Func<EntryPoint, bool>)(o => o.AddIn != null)))
                {
                    try
                    {
                        string[] strArray = entryPoint.AddIn.Split(splitter);

                        if (strArray.Length > 3)
                        {
                            if (strArray[1].Trim() == assemblyName.Name)
                            {
                                string oldValue = strArray.First(s => s.StartsWith("Version=")).Trim();
                                entryPoint.AddIn = entryPoint.AddIn.Replace(oldValue, "Version=" + assemblyName.Version);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(((object)ex).ToString());
                    }
                }
            }
            this.StartMenuManager.OemManager.Save();
        }

        public virtual void Uninstall()
        {
            foreach (string assemblyFilter in Enumerable.Select<string, string>(this.AssemblyPaths, (Func<string, string>)(o => this.GetAssemblyName(o))))
            {
                foreach (AssemblyName assemblyName in Enumerable.Select<string, AssemblyName>(AssemblyCache.Global.SearchAssemblies(assemblyFilter), (Func<string, AssemblyName>)(o => new AssemblyName(o))))
                    AssemblyCache.Global.UninstallAssembly(assemblyName.FullName, this.GacReference);
            }
        }

        protected virtual string GetAssemblyName(string assemblyPath)
        {
            return Path.GetFileNameWithoutExtension(assemblyPath);
        }
    }
}
