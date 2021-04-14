using FlightGearProject.ViewModels;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace FlightGearProject.Models
{
    public class AnomalyDetectionModel
    {
        private string _dllPath;
        private string _trainCSV;
        private string _testFlightCSV;

        private List<string> _anomalies = new List<string> { };
        private Dictionary<String, List<String>> _draw = new Dictionary<String, List<String>> { };

        private ObservableCollection<ScatterPoint> _anomaliesPoints = new ObservableCollection<ScatterPoint> { };
        private ObservableCollection<int> _anomaliesTime = new ObservableCollection<int> { };
        private ObservableCollection<DataPoint> _drawPoints = new ObservableCollection<DataPoint> { };


        public string DllPath
        {
            get { return _dllPath; }
            set { _dllPath = value; }
        }
        public string TrainCSV
        {
            get { return _trainCSV; }
            set { _trainCSV = value; }
        }
        public string TestFlightCSV
        {
            get { return _testFlightCSV; }
            set { _testFlightCSV = value; }
        }
        public List<string> Anomalies
        {
            get { return _anomalies; }
            set { _anomalies = value; }
        }

        public Dictionary<String, List<String>> Draw
        {
            get { return _draw; }
            set { _draw = value; }
        }

        public ObservableCollection<ScatterPoint> AnomaliesPoints
        {
            get { return _anomaliesPoints; }
            set { _anomaliesPoints = value; }
        }

        public ObservableCollection<int> AnomaliesTime
        {
            get { return _anomaliesTime; }
            set { _anomaliesTime = value; }
        }

        public ObservableCollection<DataPoint> DrawPoints
        {
            get { return _drawPoints; }
            set { _drawPoints = value; }
        }

        // Parse csv line & get specefic value as string
        static public string SplitToString(string line, int column)
        {
            string[] dataOfLine = line.Split(',');
            string data = dataOfLine[column];
            return data;
        }

        // Parse csv line & get specefic value as int
        static public int SplitToInt(string line, int column)
        {
            string[] dataOfLine = line.Split(',');
            int data = Int32.Parse(dataOfLine[column]);
            return data;
        }
        public void anomaliesList(string data, string corData)
        {
            for (int i = 0; i < Anomalies.Count(); i++)
            {
                string value0 = SplitToString(Anomalies[i], 0);
                string value1 = SplitToString(Anomalies[i], 1);
                if (data.Equals(value0) && corData.Equals(value1))
                {
                    AnomaliesPoints.Add(new ScatterPoint(ShellViewModel.SplitToDouble(Anomalies[i], 3), ShellViewModel.SplitToDouble(Anomalies[i], 4)));
                    AnomaliesTime.Add(SplitToInt(Anomalies[i], 2));
                }
            }
        }

        public void DrawList(string data, string corData)
        {
            string name = data + "," + corData;
            for (int i = 0; i < Draw.Count(); i++)
            {
                if (Draw.ContainsKey(name))
                {
                    for (int j = 0; j < Draw[name].Count(); j++)
                    {
                        DrawPoints.Add(new DataPoint(ShellViewModel.SplitToDouble(Draw[name][j], 0), ShellViewModel.SplitToDouble(Draw[name][j], 1)));
                    }
                }
            }
        }

        //public AnomalyDetectionModel {}

        public void ADLoadDLL()
        {
            //given by the user (input)
            var Dll = Assembly.LoadFile(DllPath);

            //classes - UPPERCASE
            //classes - UPPERCASE
            // TimeSeries class
            var TSClass = Dll.GetType("dllLibrary.TimeSeries"); //namespace.class //type
            var ts_train = Activator.CreateInstance(TSClass, new object[] { TrainCSV }); // obj - feature list in features filed
            var ts_test = Activator.CreateInstance(TSClass, new object[] { TestFlightCSV }); // obj

            // linearAlgo class
            var algo = Dll.GetType("dllLibrary.Algo"); //namespace.class // type
            // create Anomaly Dedector 
            var ad = Activator.CreateInstance(algo); // assembly


            // learn Normal train with ad
            var learn_normal_method = algo.GetMethod("learnNormal"); //method name - Anomaly Detector // method
            learn_normal_method.Invoke(ad, new object[] { ts_train }); // result
            /*
            var getCFString = algo.GetMethod("getcFString"); //method name - Anomaly Detector // method
            System.Collections.Generic.List<string>  cfS = (System.Collections.Generic.List<string>)getCFString.Invoke(ad, new object[] { }); // result - create list of correlated features
            */
            // detect test with ad

            var detect_method = algo.GetMethod("detect"); //method name
            var anomalyreport = detect_method.Invoke(ad, new object[] { ts_test });

            // get AR
            var get_ar = algo.GetMethod("getAR"); //method name
            System.Collections.Generic.List<string> ar = (System.Collections.Generic.List<string>)get_ar.Invoke(ad, new object[] { });
            Anomalies = ar;

            // fun for graph
            var getDraw = algo.GetMethod("getDraw"); //method name - Anomaly Detector // method
            System.Collections.Generic.Dictionary<String, List<String>> drawPoints = (System.Collections.Generic.Dictionary<String, List<String>>)getDraw.Invoke(ad, new object[] { }); // result            
            Draw = new Dictionary<String, List<String>>(drawPoints);
        }
    }

}
