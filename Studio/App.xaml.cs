using Advent.Common.Diagnostics;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Advent.VmcStudio
{
    public partial class App : Application
    {

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Exception exception = e.Exception;
            if (exception is XamlParseException)
                exception = exception.GetBaseException();
            Trace.TraceError(((object)exception).ToString());
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            TraceUtil.SetupFileTrace(Path.Combine(VmcStudioUtil.ApplicationDataPath, "log.txt"));
        }


    }
}
