using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
namespace Advent.VmcStudio
{
    internal class VmcExecuteInstaller : MediaCenterAddInInstaller
    {
        protected override string GacReference
        {
            get
            {
                return VmcStudioUtil.ApplicationName;
            }
        }
        protected override IEnumerable<string> AssemblyPaths
        {
            get
            {
                string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                //Give me a good reason to put any of these into the GAC...
                //yield return Path.Combine(directoryName, "Microsoft.DirectX.dll");
                //yield return Path.Combine(directoryName, "Microsoft.DirectX.DirectDraw.dll");
                //yield return Path.Combine(directoryName, "ICSharpCode.SharpZipLib.dll");
                //yield return Path.Combine(directoryName, "Advent.Common.dll");
                //yield return Path.Combine(directoryName, "Advent.VmcExecute.dll");
                yield break;
            }
        }
        public VmcExecuteInstaller(VmcStudioApp app)
            : base(app.StartMenuManager)
        {
        }
    }
}
