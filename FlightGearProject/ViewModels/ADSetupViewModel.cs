using Caliburn.Micro;
using FlightGearProject.EventModels;
using System;

namespace FlightGearProject.ViewModels
{
    public class ADSetupViewModel : Screen
    {
        // event list to notify when client configuration updated
        private IEventAggregator _events;

        /********Anomaly Detection Algorithm Setup Properties*********/
        public String DllPath { get; set; }
        public String TrainFlightCSV { get; set; }
        public String TestFlightCSV { get; set; }

        /*************************************************************/

        public ADSetupViewModel(IEventAggregator events, string dll, string train, string test)
        {
            _events = events;
            DllPath = dll;
            TrainFlightCSV = train;
            TestFlightCSV = test;
        }

        public void ADSaveClicked()
        {
            _events.PublishOnUIThread(new ADSetupEvent(DllPath, TrainFlightCSV, TestFlightCSV, true));
            TryClose();
        }
    }
}
