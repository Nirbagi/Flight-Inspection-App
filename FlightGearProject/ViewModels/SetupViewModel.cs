using Caliburn.Micro;
using FlightGearProject.EventModels;
using System;

namespace FlightGearProject.ViewModels
{
    public class SetupViewModel : Screen
    {
        public String Ip { get; set; }
        public int Port { get; set; } = 0;
        public String CSVPath { get; set; }
        private IEventAggregator _events;
        public SetupViewModel(IEventAggregator events, string ip, int p, string csvp)
        {
            _events = events;
            Ip = ip;
            Port = p;
            CSVPath = csvp;
        }
        public void ButtonClick()
        {
            _events.PublishOnUIThread(new SetupEvent(Ip, Port, CSVPath));
            TryClose();
        }
    }
}
