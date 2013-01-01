


using System;

namespace Advent.MediaCenter.Theme
{
    public class ThemeApplicationException : Exception
    {
        public ThemeApplicationException(string message)
            : base(message)
        {
        }

        public ThemeApplicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
