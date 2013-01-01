using System;

namespace Advent.VmcStudio
{
    internal class VmcStudioException : Exception
    {
        public VmcStudioException(string message)
            : base(message)
        {
        }

        public VmcStudioException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
