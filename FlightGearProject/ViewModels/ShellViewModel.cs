using Caliburn.Micro;
using FlightGearProject.EventModels;
using FlightGearProject.Models;
using System.Threading;
using System.Threading.Tasks;

namespace FlightGearProject.ViewModels
{
    public class ShellViewModel : Conductor<Screen>.Collection.AllActive, IHandle<SetupEvent>
    {        
        // private members 
        public enum FlightData {
            // Columns of various flight properties in the given CSV file
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
        private IEventAggregator _events = new EventAggregator();
        private SetupViewModel _clientSetup;
        private JoystickViewModel _joystick;
        private GraphsViewModel _graphs;
        private BindableCollection<float> _videoSpeeds = new BindableCollection<float>();
        private float _videoSpeed = 1;
        private FGClient _simClient = new FGClient();
        private float _progressElapsed;                
        private bool _updateTimeRunning = false;

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
        public FGClient SimClient
        {
            get { return _simClient; }
            set { _simClient = value; }
        }
        public float ProgressElapsed
        {
            get { return _progressElapsed; }
            set
            {
                _progressElapsed = value;
                SimClient.Location = (int)(value * SimClient.VideoSize) / 100;
                NotifyOfPropertyChange(() => ProgressElapsed);
            }
        }    
        public bool UpdateTimeRunning
        {
            get { return _updateTimeRunning; }
            set { _updateTimeRunning = value; }
        }

        // public members
        public bool SetupAlreadyOpen { get; set; } = false;
        public bool JoystickAlreadyOpen { get; set; } = false;
        public bool GraphsAlreadyOpen { get; set; } = false;
        public int SimTotalSeconds { get; set; }
        public int SimTotalMins { get; set; }
        public int SimTotalHours { get; set; }
        public int ElapsedTotalSeconds { get; set; } = 0;
        public int ElapsedTotalMins { get; set; }
        public int ElapsedTotalHours { get; set;  }
        public BindableCollection<float> VideoSpeeds
        {
            get { return _videoSpeeds; }
            set { _videoSpeeds = value; }
        }
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
        // Helper Method - parse csv line & get specefic value as double
        public double SplitToDouble(string line, int column)
        {
            string[] dataOfLine = line.Split(',');
            double data = double.Parse(dataOfLine[column]);
            return data;
        }
        public void UpdateTime()
        {
            while (true) {
                if (SimClient.PauseFlag)
                    continue;
                ProgressElapsed = 100 * (float)SimClient.Location/SimClient.VideoSize;
                ElapsedTotalSeconds = SimClient.Location / 10;
                ElapsedTotalMins = SimClient.Location / 600;
                ElapsedTotalHours = SimClient.Location / 36000;
                ElapsedTotalSeconds -= (ElapsedTotalMins * 60 + ElapsedTotalHours * 3600);
                NotifyOfPropertyChange(() => ElapsedTotalSeconds);
                NotifyOfPropertyChange(() => ElapsedTotalMins);
                NotifyOfPropertyChange(() => ElapsedTotalHours);
                Thread.Sleep((int)(1000/VideoSpeed));
            }
        }

        public void UpdateJoystick()
        {
            string curLine;
            double ail, ele, rud, alt, air, dir, yaw, rol, pit, tA, tB;
            while (JoystickAlreadyOpen)
            {
                if (SimClient.PauseFlag)
                    continue;
                curLine = SimClient.FileLines[SimClient.Location];
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

        public void LoadSetup()
        {                      
            if (SetupAlreadyOpen == false) {
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
            else if(SetupAlreadyOpen == true)
            {
                // Disable Setup UC
                SetupAlreadyOpen = false;
                DeactivateItem(ClientSetup, true);
                ClientSetup = null;                
            }               
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
                Joystick= new JoystickViewModel(_events);
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

        public void LoadGraphs()
        {
            if (GraphsAlreadyOpen == false)
            {
                // Disable Setup UC
                SetupAlreadyOpen = false;
                DeactivateItem(ClientSetup, true);
                ClientSetup = null;

                // Enable Graphs UC
                GraphsAlreadyOpen = true;
                Graphs = new GraphsViewModel();
                ActivateItem(Graphs);
            }
            else if (GraphsAlreadyOpen == true)
            {
                // Disable Graphs UC
                GraphsAlreadyOpen = false;
                DeactivateItem(Graphs, true);
                Graphs = null;
            }
        }
        public async Task StartSimClient()
        {
            await Task.Run(() => SimClient.InitFGClient());
            SimTotalSeconds = SimClient.VideoSize / 10;
            SimTotalHours = SimTotalSeconds / 3600;
            SimTotalMins = SimTotalSeconds / 60;
            SimTotalSeconds -= (SimTotalMins * 60 + SimTotalHours * 3600);
            NotifyOfPropertyChange(() => SimTotalHours);
            NotifyOfPropertyChange(() => SimTotalMins);
            NotifyOfPropertyChange(() => SimTotalSeconds);
            await Task.Run(() => UpdateTime());
        }

        public async Task PlaySim()
        {            
            SimClient.PauseFlag = false;
            SimClient.FBFlag = true;
            if (UpdateTimeRunning == false)
            {
                UpdateTimeRunning = true;
                await Task.Run(() => SimClient.StartPlayCSV());
            }            
        }
        public void PauseSim()
        {
            SimClient.PauseFlag = true;
        }
        public void PlayForward()
        {
            SimClient.PauseFlag = false;
            SimClient.FBFlag = true;
        }
        public void PlayBackwards()
        {
            SimClient.PauseFlag = false;
            SimClient.FBFlag = false;
        }
        public void GoToBeggining()
        {
            SimClient.PauseFlag = true;
            SimClient.Location = 0;
        }

        public void Handle(SetupEvent message)
        {
            SimClient.FGIp = message.Ip;
            SimClient.FGPort = message.Port;
            SimClient.CsvPath = message.CSVPath;
            DeactivateItem(ClientSetup, true);
            ClientSetup = null;
            SetupAlreadyOpen = false;
        }
    }
}
