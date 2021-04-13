using System;
using System.Collections.Generic;
using System.Reflection;

namespace FlightGearProject.Models
{
    public class AnomalyDetectionModel
    {
        private string _dllPath;
        private string _trainCSV;
        private string _testFlightCSV;
        private List<double> _cF;

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
        public List<double> CF
        {
            get { return _cF; }
            set { _cF = value; }
        }

        public void ADLoadDLL()
        {
            //given by the user (input)
            var Dll = Assembly.LoadFile(DllPath);

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
            var getCF = algo.GetMethod("getFC"); //method name - Anomaly Detector // method
            var cf = getCF.Invoke(ad, new object[] { }); // result - list of correlated features

            // detect test with ad
            var detect_method = algo.GetMethod("detect"); //method name
            var anomalyreport = detect_method.Invoke(ad, new object[] { ts_test });
            // get AR
            //var get_ar = algo.GetMethod("get_ar"); //method name
            //var anomalyreport = get_ar.Invoke(ad, new object[] {});
        }
    }
}
