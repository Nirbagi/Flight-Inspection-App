using System;

namespace FlightGearProject.EventModels
{
    public class SetupEvent
    {
        public String Ip { get; set; }
        public int Port { get; set; }
        public String CSVPath { get; set; }


        public SetupEvent(String ip, int port, String csvp)
        {
            Ip = ip;
            Port = port;
            CSVPath = csvp;           
        }
    }
}
