using System;

namespace FlightGearProject.EventModels
{
    // Anomaly detection setup event
    public class ADSetupEvent
    {
        public String DllPath { get; set; }
        public String TrainCsv { get; set; }
        public String TestFlightCsv { get; set; }
        public bool Saved { get; set; }
        public ADSetupEvent(string dll, string train, string test, bool s)
        {
            DllPath = dll;
            TrainCsv = train;
            TestFlightCsv = test;
            Saved = s;
        }
    }
}
