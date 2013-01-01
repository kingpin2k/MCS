using Microsoft.MediaCenter.UI;
using System;
using System.Diagnostics;

namespace Advent.VmcExecute
{
    public class VmcExecutePage : ModelItem
    {
        private Image executionImage;

        public string ExecutionTitle
        {
            get
            {
                if (string.IsNullOrEmpty(VmcExecuteAddIn.ExecutionTitle))
                    return "Unknown";
                else
                    return VmcExecuteAddIn.ExecutionTitle;
            }
        }

        public Image ExecutionImage
        {
            get
            {
                if (this.executionImage == null)
                {
                    string executionImageUrl = VmcExecuteAddIn.ExecutionImageUrl;
                    if (!string.IsNullOrEmpty(executionImageUrl))
                    {
                        try
                        {
                            this.executionImage = new Image("file://" + executionImageUrl);
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.ToString());
                        }
                    }
                }
                return this.executionImage;
            }
        }

        internal ExecutionEngine ExecutionEngine
        {
            get
            {
                return VmcExecuteAddIn.ExecutionEngine;
            }
        }
    }
}
