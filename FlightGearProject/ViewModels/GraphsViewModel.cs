using Caliburn.Micro;
using FlightGearProject.EventModels;
using FlightGearProject.Models;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FlightGearProject.ViewModels
{
    public class GraphsViewModel : Screen, IHandle<GraphEvent>
    {
        AnomalyDetectionModel m;
        private IEventAggregator _events;
        private string _data = null;
        private string _cordata = null;
        private int _anomalyLocation = -1;
        private int _prevLoc = -1;

        List<DataPoint> _Points;
        public List<DataPoint> Points { get { return _Points; } }

        //the Point for the first graph
        public ObservableCollection<DataPoint> _dataPoints = new ObservableCollection<DataPoint> { };

        //the points for the graph of the correlated feature
        private ObservableCollection<DataPoint> _corDataPoints = new ObservableCollection<DataPoint> { };

        //the points for the reg line
        private ObservableCollection<DataPoint> _regLine = new ObservableCollection<DataPoint> { };

        //the last 30 point that were added to the reg line
        private ObservableCollection<DataPoint> _regLine30 = new ObservableCollection<DataPoint> { };

        //the points for the graph of the correlated feature
        private ObservableCollection<ScatterPoint> _realData = new ObservableCollection<ScatterPoint> { };

        //the points for the graph of the correlated feature
        private ObservableCollection<ScatterPoint> _anomalyData = new ObservableCollection<ScatterPoint> { };

        //the points for the graph of the correlated feature
        private ObservableCollection<int> _anomalyDataLocation = new ObservableCollection<int> { 100, 200 };

        // The list that contains all the features
        private List<string> _flightDataNames = new List<string>
        {
             "aileron", "elevator","rudder", "flaps", "slats", "speedbrake", "throttle", "throttle1",
             "engine-pump", "engine-pump1", "electric-pump", "electric-pump1", "external-power",
             "APU-generator", "latitude-deg", "longitude-deg", "altitude-ft", "roll-deg", "pitch-deg",
             "heading-deg", "side-slip-deg", "airspeed-kt", "glideslope", "vertical-speed-fps",
             "airspeed-indicator_indicated-speed-kt", "altimeter_indicated-altitude-ft",
             "altimeter_pressure-alt-ft", "attitude-indicator_indicated-pitch-deg",
             "attitude-indicator_indicated-roll-deg", "attitude-indicator_internal-pitch-deg",
             "attitude-indicator_internal-roll-deg", "encoder_indicated-altitude-ft",
             "encoder_pressure-alt-ft", "gps_indicated-altitude-ft", "gps_indicated-ground-speed-kt",
             "gps_indicated-vertical-speed", "indicated-heading-deg", "magnetic-compass_indicated-heading-deg",
             "slip-skid-ball_indicated-slip-skid", "turn-indicator_indicated-turn-rate",
             "vertical-speed-indicator_indicated-speed-fpm", "engine_rpm"
        };

        // The list of correlated features
        private List<string> _flightDataCorNames = new List<string>
        {
            null, null, null, null, null, null, "engine_rpm",null, null, null, null, null, null,
            null, null, null, null, "attitude-indicator_internal-roll-deg", "attitude-indicator_internal-pitch-deg",
            "indicated-heading-deg",null,"airspeed-indicator_indicated-speed-kt",null, "gps_indicated-vertical-speed",
            "gps_indicated-ground-speed-kt","altimeter_pressure-alt-ft","encoder_pressure-alt-ft",
            null, null, null, null, "encoder_pressure-alt-ft","gps_indicated-altitude-ft","gps_indicated-altitude-ft",
            null, null, null, null, null, null,null, null, null };

        //CTOR
        public GraphsViewModel(IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);
            this.m = new AnomalyDetectionModel();
            this._Points = new List<DataPoint>
            {
                new DataPoint(0, 4),
                new DataPoint(10, 13),
                new DataPoint(20, 15),
                new DataPoint(30, 16),
                new DataPoint(40, 12),
                new DataPoint(50, 12)
            };
        }

        public List<string> FlightDataNames
        {
            get
            { return _flightDataNames; }
            set
            { _flightDataNames = value; }
        }
        public List<string> FlightDataCorNames
        {
            get
            {
                return _flightDataCorNames;
            }
            private set
            {
                _flightDataCorNames = value;
                NotifyOfPropertyChange(() => FlightDataCorNames);
            }
        }
        public ObservableCollection<ScatterPoint> AnomalyData
        {
            get
            { return _anomalyData; }
            set
            {
                _anomalyData = value;
                NotifyOfPropertyChange(() => AnomalyData);
            }
        }
        public int AnomalyLocation
        {
            get { return _anomalyLocation; }
            set
            {
                _anomalyLocation = value;
                NotifyOfPropertyChange(() => AnomalyLocation);
            }
        }
        public ObservableCollection<int> AnomalyDataLocation
        {
            get
            {
                return _anomalyDataLocation;
            }
            set
            {
                _anomalyDataLocation = value;
                NotifyOfPropertyChange(() => AnomalyDataLocation);

            }
        }
        public ObservableCollection<ScatterPoint> RealData
        {
            get
            { return _realData; }
            set
            {
                _realData = value;
                NotifyOfPropertyChange(() => RealData);

            }
        }
        public ObservableCollection<DataPoint> RegLine
        {
            get
            { return _regLine; }
            set
            {
                _regLine = value;
                NotifyOfPropertyChange(() => RegLine);
            }
        }
        public ObservableCollection<DataPoint> RegLine30
        {
            get
            { return _regLine30; }
            set
            {
                _regLine30 = value;
                NotifyOfPropertyChange(() => RegLine30);
            }
        }
        public ObservableCollection<DataPoint> DataPoints
        {
            get
            { return _dataPoints; }
            set
            {
                _dataPoints = value;
                NotifyOfPropertyChange(() => DataPoints);
            }
        }
        public ObservableCollection<DataPoint> CorDataPoints
        {
            get
            { return _corDataPoints; }
            set
            {
                _corDataPoints = value;
                NotifyOfPropertyChange(() => CorDataPoints);
            }
        }
        public String vm_data
        {
            get { return _data; }
            set
            {
                DataPoints.Clear();
                RegLine.Clear();
                RegLine30.Clear();
                RealData.Clear();
                AnomalyData.Clear();
                AnomalyDataLocation.Clear();

                _data = value;

                if (value != null)
                {
                    vm_cordata = FlightDataCorNames[FlightDataNames.IndexOf(vm_data)];
                }
                //Lists of points will be set to a List<DataPoint> of timer and value

                NotifyOfPropertyChange(() => vm_data);
            }
        }
        public String vm_cordata
        {
            get { return _cordata; }
            set
            {
                CorDataPoints.Clear();

                _cordata = value;

                if (value != null)
                {
                    m.DrawList(vm_data, value);
                    RegLine = m.DrawPoints;
                    m.anomaliesList(vm_data, value);
                    AnomalyData = m.AnomaliesPoints;
                    AnomalyDataLocation = m.AnomaliesTime;
                }

                NotifyOfPropertyChange(() => vm_cordata);

            }
        }


        //In each line the Handle method is called and modifies the graphs
        public void Handle(GraphEvent message)
        {
            double data_val = 0;
            double cor_data_val = 0;
            int location = message.CsvLineIndex;
            if (!String.IsNullOrEmpty(vm_data))
            {
                if ((_prevLoc != -1) && ((_prevLoc > location + 10) || (_prevLoc < location - 10)))
                    DataPoints.Clear();

                else
                {
                    data_val = ShellViewModel.SplitToDouble(message.CurLine, FlightDataNames.IndexOf(vm_data));
                    DataPoints.Add(new DataPoint(location, data_val));
                }

            }
            if (!String.IsNullOrEmpty(vm_cordata))
            {
                if ((_prevLoc != -1) && ((_prevLoc > location + 10) || (_prevLoc < location - 10)))
                {
                    CorDataPoints.Clear();
                }
                else
                {
                    cor_data_val = ShellViewModel.SplitToDouble(message.CurLine, FlightDataNames.IndexOf(vm_cordata));
                    CorDataPoints.Add(new DataPoint(location, cor_data_val));
                }
            }
            if (!String.IsNullOrEmpty(vm_data) && !String.IsNullOrEmpty(vm_cordata))
            {
                ScatterPoint p = new ScatterPoint(data_val, cor_data_val);

                if (!AnomalyData.Contains(p))
                {
                    RealData.Add(new ScatterPoint(data_val, cor_data_val));
                }
                for (int i = 0; i < RegLine.Count(); i++)
                {
                    if (RegLine[i].X == data_val)
                    {
                        RegLine30.Add(RegLine[i]);
                        RegLine.RemoveAt(i);
                    }
                }
                if (_regLine30.Count() > 30)
                {
                    RegLine.Add(_regLine30[0]);
                    RegLine.RemoveAt(0);
                }
            }
            _prevLoc = location;
        }
    }
}
