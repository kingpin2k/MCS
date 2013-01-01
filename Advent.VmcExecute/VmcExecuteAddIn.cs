
using Advent.Common.Diagnostics;
using Advent.Common.UI;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace Advent.VmcExecute
{
    public class VmcExecuteAddIn : IAddInModule, IAddInEntryPoint
    {
        private const int ERROR_FILE_NOT_FOUND = 2;
        private static HistoryOrientedPageSession session;
        private static Thread messagePump;
        private static GlobalHook globalHook;
        private ExecutionInfo executionInfo;
        private static bool isLoggingInitialised;
        private static Dictionary<string, object> entryPointInfo;

        public static string ExecutionTitle { get; private set; }

        public static string ExecutionImageUrl { get; private set; }

        internal static ExecutionEngine ExecutionEngine { get; set; }

        public void Initialize(Dictionary<string, object> appInfo, Dictionary<string, object> entryPointInformation)
        {
            try
            {
                VmcExecuteAddIn.entryPointInfo = entryPointInformation;
                object obj1;
                VmcExecuteAddIn.entryPointInfo.TryGetValue("Title", out obj1);
                VmcExecuteAddIn.ExecutionTitle = obj1 as string;
                object obj2;
                if (!VmcExecuteAddIn.entryPointInfo.TryGetValue("ImageUrl", out obj2))
                    VmcExecuteAddIn.entryPointInfo.TryGetValue("ThumbnailUrl", out obj2);
                VmcExecuteAddIn.ExecutionImageUrl = obj2 as string;
                object obj3;
                if (!VmcExecuteAddIn.entryPointInfo.TryGetValue("Context", out obj3))
                    throw new InvalidOperationException("Context not found.");
                this.executionInfo = (ExecutionInfo)new XmlSerializer(typeof(ExecutionInfo)).Deserialize((TextReader)new StringReader(((string)obj3).Trim()));
                if (this.executionInfo == null)
                    throw new FormatException("Could not interpret context.");
                bool flag1 = !string.IsNullOrEmpty(this.executionInfo.FileName);
                bool flag2 = this.executionInfo.Media != null && this.executionInfo.Media.Count > 0;
                if (!flag1 && !flag2 && !this.executionInfo.Page.HasValue)
                    throw new InvalidOperationException("No file name, page or media specified.");
                if (!flag1)
                    return;
                this.executionInfo.FileName = System.Environment.ExpandEnvironmentVariables(this.executionInfo.FileName);
                object obj4;
                if (!this.executionInfo.RequiresDirectX && VmcExecuteAddIn.entryPointInfo.TryGetValue("CapabilitiesRequired", out obj4))
                    this.executionInfo.RequiresDirectX = ((string)obj4).IndexOf("directx", StringComparison.InvariantCultureIgnoreCase) != -1;
                VmcExecuteAddIn.globalHook = new GlobalHook();
                VmcExecuteAddIn.ExecutionEngine = new ExecutionEngine(this.executionInfo, VmcExecuteAddIn.globalHook);
                VmcExecuteAddIn.ExecutionEngine.ExecutionFinished += (EventHandler)delegate
                {
                    Application.DeferredInvoke(new DeferredHandler(this.ExecutionFinishedHandler));
                };
                VmcExecuteAddIn.ExecutionEngine.ExecutionError += (EventHandler<Advent.VmcExecute.ErrorEventArgs>)((sender, args) => Application.DeferredInvoke(new DeferredHandler(this.ExecutionErrorHandler), (object)args));
                if (string.IsNullOrEmpty(VmcExecuteAddIn.ExecutionTitle))
                    VmcExecuteAddIn.ExecutionTitle = Path.GetFileNameWithoutExtension(this.executionInfo.FileName);
                VmcExecuteAddIn.messagePump = new Thread((ThreadStart)(() =>
                {
                    try
                    {
                        if (VmcExecuteAddIn.ExecutionEngine.RequiresKeyboardHook)
                            VmcExecuteAddIn.globalHook.Start(false, true);
                        System.Windows.Threading.Dispatcher.Run();
                    }
                    catch (ThreadAbortException)
                    {
                    }
                }));
                VmcExecuteAddIn.messagePump.SetApartmentState(ApartmentState.STA);
                VmcExecuteAddIn.messagePump.IsBackground = true;
                VmcExecuteAddIn.messagePump.Start();
            }
            catch (Exception ex)
            {
                VmcExecuteAddIn.LogError(ex.ToString());
                throw;
            }
        }

        public void Uninitialize()
        {
        }

        public void Launch(AddInHost host)
        {
            try
            {
                if (this.executionInfo.Media != null && this.executionInfo.Media.Count > 0)
                {
                    bool flag = true;
                    foreach (MediaInfo mediaInfo in this.executionInfo.Media)
                    {
                        MediaCenterEnvironment centerEnvironment = AddInHost.Current.MediaCenterEnvironment;
                        MediaType? mediaType = mediaInfo.MediaType;
                        int num1 = mediaType.HasValue ? (int)mediaType.GetValueOrDefault() : -1;
                        string url = mediaInfo.Url;
                        int num2 = !flag ? 1 : 0;
                        centerEnvironment.PlayMedia((MediaType)num1, (object)url, num2 != 0);
                        flag = false;
                    }
                    MediaExperience mediaExperience = AddInHost.Current.MediaCenterEnvironment.MediaExperience;
                    if (mediaExperience != null)
                        mediaExperience.GoToFullScreen();
                }
                if (VmcExecuteAddIn.ExecutionEngine != null)
                {
                    VmcExecuteAddIn.session = new HistoryOrientedPageSession();
                    ((PageSession)VmcExecuteAddIn.session).GoToPage("resx://Advent.VmcExecute/Advent.VmcExecute.Resources/MainPage");
                    VmcExecuteAddIn.ExecutionEngine.BeginExecute();
                }
                else
                {
                    if (!this.executionInfo.Page.HasValue)
                        return;
                    AddInHost.Current.MediaCenterEnvironment.NavigateToPage(this.executionInfo.Page.Value, (object)null);
                }
            }
            catch (Exception ex)
            {
                VmcExecuteAddIn.LogError(ex.ToString());
                throw;
            }
        }

        internal static void LogInfo(string text)
        {
            VmcExecuteAddIn.InitialiseLogging();
            Trace.TraceInformation(text);
        }

        internal static void LogError(string text)
        {
            VmcExecuteAddIn.InitialiseLogging();
            Trace.TraceError(text);
        }

        private static void InitialiseLogging()
        {
            if (VmcExecuteAddIn.isLoggingInitialised || VmcExecuteAddIn.entryPointInfo == null)
                return;
            object obj;
            if (VmcExecuteAddIn.entryPointInfo.TryGetValue("Id", out obj))
                TraceUtil.SetupFileTrace(Path.Combine(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "Media Center Studio"), string.Format("Logs\\EntryPoints\\{0}.txt", obj)));
            VmcExecuteAddIn.isLoggingInitialised = true;
        }

        private void ExecutionFinishedHandler(object state)
        {
            if (AddInHost.Current == null)
                return;
            if (this.executionInfo.Page.HasValue)
                AddInHost.Current.MediaCenterEnvironment.NavigateToPage(this.executionInfo.Page.Value, (object)null);
            AddInHost.Current.ApplicationContext.CloseApplication();
        }

        private void ExecutionErrorHandler(object state)
        {
            Advent.VmcExecute.ErrorEventArgs errorEventArgs = (Advent.VmcExecute.ErrorEventArgs)state;
            VmcExecuteAddIn.LogError(errorEventArgs.Exception.ToString());
            if (AddInHost.Current == null)
                return;
            Win32Exception win32Exception = errorEventArgs.Exception as Win32Exception;
            int num = (int)AddInHost.Current.MediaCenterEnvironment.Dialog(win32Exception == null || win32Exception.NativeErrorCode != 2 ? errorEventArgs.Exception.Message : win32Exception.Message + ":\n" + this.executionInfo.FileName, "Media Center Studio", DialogButtons.Ok, 10000, false);
        }
    }
}
