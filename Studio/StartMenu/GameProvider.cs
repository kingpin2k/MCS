using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

namespace Advent.VmcStudio.StartMenu
{
    public class GameProvider
    {
        private List<Game> games;

        public IList<Game> Games
        {
            get
            {
                return (IList<Game>)this.games;
            }
        }

        public GameProvider()
        {
            this.games = new List<Game>();
            foreach (ManagementObject managementObject in new ManagementObjectSearcher(new ManagementScope("\\\\.\\root\\CIMV2\\Applications\\Games"), new ObjectQuery("select * from Game")).Get())
            {
                try
                {
                    this.games.Add(new Game((ManagementBaseObject)managementObject));
                }
                catch (ArgumentException ex)
                {
                    Trace.TraceWarning(((object)ex).ToString());
                }
            }
        }
    }
}
