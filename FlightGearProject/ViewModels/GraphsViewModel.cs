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
        FromDll m;
        private IEventAggregator _events;
        private string _data = null;
        private string _cordata = null;
        private int _count = 0;
        private bool _is30 = false;
        private int _prev_loc = 0;
        private String _prev_feat = "";
        private int _dot;

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
        
        // The list that contains all the features
        public List<string> _flightDataNames = new List<string>
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


        public int Dot
        {
            get { return this._dataPoints.Count(); }
            set
            {
                this._dot = value;
                NotifyOfPropertyChange(() => Dot);
            }
        }

        //CTOR
        public GraphsViewModel(IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);
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
        public ObservableCollection<DataPoint> dataPoints
        {
            get
            { return _dataPoints; }
            set
            {
                _dataPoints = value;
                NotifyOfPropertyChange(() => dataPoints);
            }
        }
        public ObservableCollection<DataPoint> corDataPoints
        {
            get
            { return _corDataPoints; }
            set
            {
                _corDataPoints = value;
                NotifyOfPropertyChange(() => corDataPoints);
            }
        }
        public String vm_data
        {
            get { return _data; }
            set
            {
                _data = value;
                //The dataPoints property will be set to a List<DataPoint> of timer and value
                dataPoints.Clear();
                NotifyOfPropertyChange(() => vm_data);
            }
        }
        public String vm_cordata
        {
            get { return _cordata; }
            set
            {
                //_cordata = m.cordata(this.vm_data);
                //_regLine = m.getPointsReg(vm_data, vm_cordata);
                corDataPoints.Clear();
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
                // if playing backwards remove point
                if (!message.FBFlag && dataPoints.Count() != 0)
                    dataPoints.RemoveAt(dataPoints.Count() - 1);
                else
                {
                    data_val = ShellViewModel.SplitToDouble(message.CurLine, FlightDataNames.IndexOf(vm_data));
                    dataPoints.Add(new DataPoint(location, data_val));
                }

            }
            if (!String.IsNullOrEmpty(vm_cordata))
            {
                // if playing backwards remove point
                if (!message.FBFlag && dataPoints.Count() != 0)
                    corDataPoints.RemoveAt(corDataPoints.Count());
                else
                {
                    cor_data_val = ShellViewModel.SplitToDouble(message.CurLine, FlightDataNames.IndexOf(vm_cordata));
                    corDataPoints.Add(new DataPoint(location, cor_data_val));
                }
            }
            if (!String.IsNullOrEmpty(vm_data) && !String.IsNullOrEmpty(vm_cordata))
            {
                ScatterPoint p = new ScatterPoint(data_val, cor_data_val);
                /*
                if (m.getAnomalyData(vm_data, vm_cordata).Contains(p))
                {
                    AnomalyData.Add(p);
                } else
                {
                    RealData.Add(new ScatterPoint(data_val, cor_data_val));
                }
                */
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
        }            
    }
}
