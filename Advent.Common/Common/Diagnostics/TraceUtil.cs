
using System.Diagnostics;
using System.IO;

namespace Advent.Common.Diagnostics
{
    public static class TraceUtil
    {
        public static void SetupFileTrace(string filePath)
        {
            TraceUtil.SetupFileTrace(filePath, TraceOptions.DateTime);
        }

        public static void SetupFileTrace(string filePath, TraceOptions options)
        {
            Trace.AutoFlush = true;
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
            fileStream.Seek(0L, SeekOrigin.End);
            TextWriterTraceListener writerTraceListener = new TextWriterTraceListener((Stream)fileStream);
            writerTraceListener.TraceOutputOptions = options;
            Trace.Listeners.Add((TraceListener)writerTraceListener);
        }
    }
}
