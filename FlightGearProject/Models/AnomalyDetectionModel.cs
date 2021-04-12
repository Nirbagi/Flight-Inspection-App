using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightGearProject.Models
{
    // TO IMPLEMENT
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
    }
}
