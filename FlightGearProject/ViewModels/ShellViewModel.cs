using Caliburn.Micro;
using FlightGearProject.EventModels;
using FlightGearProject.Models;
using System.Threading;
using System.Threading.Tasks;

namespace FlightGearProject.ViewModels
{
    public class ShellViewModel : Conductor<Screen>.Collection.AllActive, IHandle<SetupEvent>, IHandle<ADSetupEvent>
    {
        /****Columns of various flight properties in the given CSV file****/
        public enum FlightData
        {
            aileron = 0,
            elevator = 1,
            rudder = 2,
            altitude = 16,
            airspeed = 21,
            direction = 19,
            yaw = 20,
            roll = 17,
            pitch = 18,
            throttleA = 6,
            throttleB = 7
        }
        /******************************************************************/

        /********************Private Members & Public Setters/Getters*********************/
        // private members 
        private IEventAggregator _events = new EventAggregator();
        private IWindowManager _manager = new WindowManager();
        private SetupViewModel _clientSetup;
        private JoystickViewModel _joystick;
        private GraphsViewModel _graphs;
        private ADSetupViewModel _aDSetup;
        private BindableCollection<float> _videoSpeeds = new BindableCollection<float>();
        private float _videoSpeed = 1;
        private FGClientModel _simClient = new FGClientModel();
        private AnomalyDetectionModel _aDAlgo = new AnomalyDetectionModel();
        private bool _isClientConnected = true;
        private float _progressElapsed = 0;
        private bool _updateTimeRunning = false;
        private int _remainingSiminSecs = 0;
        private int _simTotalSeconds = 0;
        private int _simTotalMins = 0;
        private int _simTotalHours = 0;
        private int _elapsedTotalSeconds = 0;
        private int _elapsedTotalMins = 0;
        private int _elapsedTotalHours = 0;

        // public setters/getters to private members
        public SetupViewModel ClientSetup
        {
            get { return _clientSetup; }
            set
            {
                _clientSetup = value;
                NotifyOfPropertyChange(() => ClientSetup);
            }
        }
        public JoystickViewModel Joystick
        {
            get { return _joystick; }
            set
            {
                _joystick = value;
                NotifyOfPropertyChange(() => Joystick);
            }
        }
        public GraphsViewModel Graphs
        {
            get { return _graphs; }
            set
            {
                _graphs = value;
                NotifyOfPropertyChange(() => Graphs);
            }
        }
        public ADSetupViewModel ADSetup
        {
            get { return _aDSetup; }
            set
            {
                _aDSetup = value;
                NotifyOfPropertyChange(() => ADSetup);
            }
        }
        public BindableCollection<float> VideoSpeeds
        {
            get { return _videoSpeeds; }
            set
            {
                _videoSpeeds = value;
                NotifyOfPropertyChange(() => VideoSpeeds);
            }
        }
        public float VideoSpeed
        {
            get { return _videoSpeed; }
            set
            {
                _videoSpeed = value;
                NotifyOfPropertyChange(() => VideoSpeed);
                SimClient.TranSpeed = (int)(100 / VideoSpeed);
            }
        }
        public FGClientModel SimClient
        {
            get { return _simClient; }
            set { _simClient = value; }
        }
        public AnomalyDetectionModel ADAlgo
        {
            get { return _aDAlgo; }
            set { _aDAlgo = value; }
        }
        public bool IsClientConnected
        {
            get { return _isClientConnected; }
            set
            {
                _isClientConnected = value;
                NotifyOfPropertyChange(() => IsClientConnected);
            }
        }
        public float ProgressElapsed
        {
            get { return _progressElapsed; }
            set
            {
                float cur = _progressElapsed;
                _progressElapsed = value;
                SimClient.CsvLineNum = (int)(value * SimClient.VideoSize) / 100;
                NotifyOfPropertyChange(() => ProgressElapsed);
                // update Graphs Data
                _events.PublishOnUIThread(new GraphEvent(SimClient.FileLines[SimClient.CsvLineNum], SimClient.CsvLineNum));
            }
        }
        public bool AlreadyPlaying
        {
            get { return _updateTimeRunning; }
            set { _updateTimeRunning = value; }
        }
        public bool StopUpdateGraph { get; set; } = false;
        public int RemainingSiminSecs
        {
            get { return _remainingSiminSecs; }
            set { _remainingSiminSecs = value; }
        }
        public int SimTotalSeconds
        {
            get { return _simTotalSeconds; }
            set
            {
                _simTotalSeconds = value;
                NotifyOfPropertyChange(() => SimTotalSeconds);
            }
        }
        public int SimTotalMins
        {
            get { return _simTotalMins; }
            set
            {
                _simTotalMins = value;
                NotifyOfPropertyChange(() => SimTotalMins);
            }
        }
        public int SimTotalHours
        {
            get { return _simTotalHours; }
            set
            {
                _simTotalHours = value;
                NotifyOfPropertyChange(() => SimTotalHours);
            }
        }
        public int ElapsedTotalSeconds
        {
            get { return _elapsedTotalSeconds; }
            set
            {
                _elapsedTotalSeconds = value;
                NotifyOfPropertyChange(() => ElapsedTotalSeconds);
            }
        }
        public int ElapsedTotalMins
        {
            get { return _elapsedTotalMins; }
            set
            {
                _elapsedTotalMins = value;
                NotifyOfPropertyChange(() => ElapsedTotalMins);
            }
        }
        public int ElapsedTotalHours
        {
            get { return _elapsedTotalHours; }
            set
            {
                _elapsedTotalHours = value;
                NotifyOfPropertyChange(() => ElapsedTotalHours);
            }
        }
        /**********************************************************************************/

        /*********************public members***********************/
        public bool SetupAlreadyOpen { get; set; } = false;
        public bool JoystickAlreadyOpen { get; set; } = false;
        public bool GraphsAlreadyOpen { get; set; } = false;
        /**********************************************************/

        /**********************************Can Click Buttons Properties******************************/
        private bool _canLoadSetup = true;
        private bool _canStartSimClient = false;
        private bool _canLoadJoystick = false;
        private bool _canLoadGraphs = false;
        public bool CanLoadSetup
        {
            get { return _canLoadSetup; }
            set
            {
                _canLoadSetup = value;
                NotifyOfPropertyChange(() => CanLoadSetup);
            }
        }
        public bool CanStartSimClient
        {
            get { return _canStartSimClient; }
            set
            {
                _canStartSimClient = value;
                NotifyOfPropertyChange(() => CanStartSimClient);
            }
        }
        public bool CanLoadJoystick
        {
            get { return _canLoadJoystick; }
            set
            {
                _canLoadJoystick = value;
                NotifyOfPropertyChange(() => CanLoadJoystick);
            }
        }
        public bool CanLoadGraphs
        {
            get { return _canLoadGraphs; }
            set
            {
                _canLoadGraphs = value;
                NotifyOfPropertyChange(() => CanLoadGraphs);
            }
        }
        /********************************************************************************************/

        /******************CTOR*******************/
        public ShellViewModel()
        {
            VideoSpeeds.Add((float)0.25);
            VideoSpeeds.Add((float)0.5);
            VideoSpeeds.Add((float)0.75);
            VideoSpeeds.Add(1);
            VideoSpeeds.Add(2);
            VideoSpeeds.Add(3);
            VideoSpeeds.Add(4);
            _events.Subscribe(this);
        }
        /*****************************************/

        /********************************Helper Methods*******************************/
        // Parse csv line & get specefic value as double
        static public double SplitToDouble(string line, int column)
        {
            string[] dataOfLine = line.Split(',');
            double data = double.Parse(dataOfLine[column]);
            return data;
        }
        /*****************************************************************************/

        /**********************************Buttons Methods******************************/
        public async Task StartSimClient()
        {
            // continue only if was able to init client (parameters were correct)
            if (!SimClient.InitFGClient())
                return;
            // Disable 'Setup' button & Enable 'Load Joystick', 'Load Graphs' buttons 
            CanStartSimClient = false;
            CanLoadSetup = false;
            CanLoadJoystick = true;
            CanLoadGraphs = true;
            RemainingSiminSecs = SimClient.VideoSize / 10;
            SimTotalMins = SimClient.VideoSize / 600;
            SimTotalHours = SimClient.VideoSize / 36000;
            SimTotalSeconds = RemainingSiminSecs - (SimTotalMins * 60 + SimTotalHours * 3600);
            await Task.Run(() => UpdateTime());
        }

        public async Task PlaySim()
        {
            if (AlreadyPlaying == false)
            {
                AlreadyPlaying = true;
                SimClient.PauseFlag = false;
                SimClient.ForwardBackwardFlag = true;
                await Task.Run(() => SimClient.StartPlayCSV());
                AlreadyPlaying = false;
            }
        }

        public void PauseSim()
        {
            SimClient.PauseFlag = true;
        }

        public void PlayForward()
        {
            SimClient.PauseFlag = false;
            SimClient.ForwardBackwardFlag = true;
        }

        public void PlayBackwards()
        {
            SimClient.PauseFlag = false;
            SimClient.ForwardBackwardFlag = false;
        }

        public void JumpBackwards()
        {
            SimClient.CsvLineNum -= 50;
            StopUpdateGraph = false;
            // Update GraphsVM            
            _events.PublishOnUIThread(new GraphEvent(SimClient.FileLines[SimClient.CsvLineNum],
                SimClient.CsvLineNum));
            StopUpdateGraph = true;
        }

        public void SkipForward()
        {
            SimClient.CsvLineNum += 50;
            // Update GraphsVM
            _events.PublishOnUIThread(new GraphEvent(SimClient.FileLines[SimClient.CsvLineNum],
             SimClient.CsvLineNum));
        }

        // Stops the playback and return to the beggining of the simulation
        public void StopSimulation()
        {
            SimClient.PauseFlag = true;
            SimClient.CsvLineNum = 0;
            ProgressElapsed = 0;
            ElapsedTotalSeconds = 0;
            ElapsedTotalMins = 0;
            ElapsedTotalHours = 0;
        }
        /*******************************************************************************/

        /**********************************Load UC Methods******************************/
        public void LoadSetup()
        {
            if (SetupAlreadyOpen == false)
            {
                // Disable Joystick UC
                JoystickAlreadyOpen = false;
                DeactivateItem(Joystick, true);
                Joystick = null;

                // Disable Graphs UC
                GraphsAlreadyOpen = false;
                DeactivateItem(Graphs, true);
                Graphs = null;

                // Enable Setup UC
                SetupAlreadyOpen = true;
                ClientSetup = new SetupViewModel(_events, SimClient.FGIp, SimClient.FGPort, SimClient.CsvPath);
                ActivateItem(ClientSetup);
            }
            else if (SetupAlreadyOpen == true)
            {
                // Disable Setup UC
                SetupAlreadyOpen = false;
                DeactivateItem(ClientSetup, true);
                ClientSetup = null;
            }
        }

        public async Task LoadADSetup()
        {
            ADSetup = new ADSetupViewModel(_events, ADAlgo.DllPath, ADAlgo.TrainCSV, ADAlgo.TestFlightCSV);
            _manager.ShowWindow(ADSetup);
            /*
                      await Task.Run(() =>
                      {
                          while (!StopUpdateGraph)
                              continue;
                      });
                      ADAlgo.ADLoadDLL();*/
        }

        public async Task LoadJoystick()
        {
            if (JoystickAlreadyOpen == false)
            {
                // Disable Setup UC
                SetupAlreadyOpen = false;
                DeactivateItem(ClientSetup, true);
                ClientSetup = null;

                // Enable Joystick UC
                JoystickAlreadyOpen = true;
                Joystick = new JoystickViewModel(_events);
                ActivateItem(Joystick);
                await Task.Run(() => UpdateJoystick());

            }
            else if (JoystickAlreadyOpen == true)
            {
                // Disable Joystick UC
                JoystickAlreadyOpen = false;
                DeactivateItem(Joystick, true);
                Joystick = null;
            }
        }

        public async Task LoadGraphs()
        {
            if (GraphsAlreadyOpen == false)
            {
                // Disable Setup UC
                SetupAlreadyOpen = false;
                DeactivateItem(ClientSetup, true);
                ClientSetup = null;

                // Enable Graphs UC
                GraphsAlreadyOpen = true;
                Graphs = new GraphsViewModel(_events);
                ActivateItem(Graphs);
                await Task.Run(() => UpdateGraphs());
            }
            else if (GraphsAlreadyOpen == true)
            {
                // Disable Graphs UC
                GraphsAlreadyOpen = false;
                DeactivateItem(Graphs, true);
                Graphs = null;
            }
        }
        /*******************************************************************************/

        /****************************Background Methods - Update Info******************************/
        // this function update elapsed time & slider progress
        public void UpdateTime()
        {
            while (true)
            {
                if (SimClient.PauseFlag)
                    continue;
                ProgressElapsed = 100 * (float)SimClient.CsvLineNum / SimClient.VideoSize;
                RemainingSiminSecs = SimClient.CsvLineNum / 10;
                ElapsedTotalMins = SimClient.CsvLineNum / 600;
                ElapsedTotalHours = SimClient.CsvLineNum / 36000;
                ElapsedTotalSeconds = RemainingSiminSecs - (ElapsedTotalMins * 60 + ElapsedTotalHours * 3600);
                Thread.Sleep((int)(1000 / VideoSpeed));
            }
        }

        // this function update JoystickVM for the correct state
        public void UpdateJoystick()
        {
            string curLine;
            double ail, ele, rud, alt, air, dir, yaw, rol, pit, tA, tB;
            while (JoystickAlreadyOpen)
            {
                if (SimClient.PauseFlag)
                    continue;
                curLine = SimClient.FileLines[SimClient.CsvLineNum];
                ail = SplitToDouble(curLine, (int)FlightData.aileron);
                ele = SplitToDouble(curLine, (int)FlightData.elevator);
                rud = SplitToDouble(curLine, (int)FlightData.rudder);
                alt = SplitToDouble(curLine, (int)FlightData.altitude);
                air = SplitToDouble(curLine, (int)FlightData.airspeed);
                dir = SplitToDouble(curLine, (int)FlightData.direction);
                yaw = SplitToDouble(curLine, (int)FlightData.yaw);
                rol = SplitToDouble(curLine, (int)FlightData.roll);
                pit = SplitToDouble(curLine, (int)FlightData.pitch);
                tA = SplitToDouble(curLine, (int)FlightData.throttleA);
                tB = SplitToDouble(curLine, (int)FlightData.throttleB);
                _events.PublishOnUIThread(new JoystickDataEvent(ail, ele, rud, alt, air, dir, yaw, rol, pit, tA, tB));
                Thread.Sleep((int)(1000 / VideoSpeed));
            }
        }

        // this function update GraphsVM for the correct state
        public void UpdateGraphs()
        {
            while (GraphsAlreadyOpen)
            {
                if (SimClient.PauseFlag || StopUpdateGraph)
                    continue;
                _events.PublishOnUIThread(new GraphEvent(SimClient.FileLines[SimClient.CsvLineNum], SimClient.CsvLineNum));
                if (Graphs.AnomalyLocation != -1)
                {
                    SimClient.CsvLineNum = Graphs.AnomalyLocation;
                    Graphs.AnomalyLocation = -1;
                }
                Thread.Sleep((int)(1000 / VideoSpeed));
            }
        }
        /****************************************************************************************/

        /****************************Event Handlers Methods*****************************/
        // SetupEvent Handler - Update Client Configuration
        public void Handle(SetupEvent message)
        {
            SimClient.FGIp = message.Ip;
            SimClient.FGPort = message.Port;
            SimClient.CsvPath = message.CSVPath;
            CanStartSimClient = true;
            DeactivateItem(ClientSetup, true);
            ClientSetup = null;
            SetupAlreadyOpen = false;
        }

        // Anomaly Detection Setup Event:
        // Update Alogrithm dll , Train/Test flights files paths
        public void Handle(ADSetupEvent message)
        {
            ADAlgo.DllPath = message.DllPath;
            ADAlgo.TrainCSV = message.TrainCsv;
            ADAlgo.TestFlightCSV = message.TestFlightCsv;
            DeactivateItem(ADSetup, true);
            ADSetup = null;
            StopUpdateGraph = true;
            ADAlgo.ADLoadDLL();
        }
        /*******************************************************************************/
    }
}