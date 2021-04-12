using Caliburn.Micro;
using FlightGearProject.EventModels;
using System;

namespace FlightGearProject.ViewModels
{
    public class SetupViewModel : Screen
    {
        // event list to notify when client configuration updated
        private IEventAggregator _events;
        
        /********Client Properties*********/
        public String Ip { get; set; }
        public int Port { get; set; } = 0;
        public String CSVPath { get; set; }
        /**********************************/
        
        public SetupViewModel(IEventAggregator events, string ip, int p, string csvp)
        {
            _events = events;
            Ip = ip;
            Port = p;
            CSVPath = csvp;
        }

        public void SaveClicked()
        {
            _events.PublishOnUIThread(new SetupEvent(Ip, Port, CSVPath));
            TryClose();
        }
    }
}
