using System;

namespace FlightGearProject.EventModels
{
    // Event to update FlightGear client configuration
    public class SetupEvent
    {
        /********Client Properties*********/
        public String Ip { get; set; }
        public int Port { get; set; }
        public String CSVPath { get; set; }
        /**********************************/

        public SetupEvent(String ip, int port, String csvp)
        {
            Ip = ip;
            Port = port;
            CSVPath = csvp;           
        }
    }
}
