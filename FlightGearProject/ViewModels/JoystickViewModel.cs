using Caliburn.Micro;
using FlightGearProject.EventModels;

namespace FlightGearProject.ViewModels
{
    public class JoystickViewModel : Screen, IHandle<JoystickDataEvent>
    {
        private IEventAggregator _events;
        private double _aileron;
        private double _elevator;
        private double _rudder;
        private double _altitude;
        private double _airspeed;
        private double _direction;
        private double _yaw;
        private double _roll;
        private double _pitch;
        private double _throttleA;
        private double _throttleB;
        public double Aileron
        {
            get { return _aileron; }
            set 
            { 
                _aileron = value;
                NotifyOfPropertyChange(() => Aileron);
            }
        }
        public double Elevator
        {
            get { return _elevator; }
            set 
            { 
                _elevator = value;
                NotifyOfPropertyChange(() => Elevator);
            }
        }
        public double Rudder
        {
            get { return _rudder; }
            set 
            { 
                _rudder = value;
                NotifyOfPropertyChange(() => Rudder);
            }
        }
        public double Altitude
        {
            get { return _altitude; }
            set 
            { 
                _altitude = value;
                NotifyOfPropertyChange(() => Altitude);
            }
        }
        public double Airspeed
        {
            get { return _airspeed; }
            set 
            { 
                _airspeed = value;
                NotifyOfPropertyChange(() => Airspeed);
            }
        }            
        public double Direction
        {
            get { return _direction; }
            set 
            { 
                _direction = value;
                NotifyOfPropertyChange(() => Direction);
            }
        }    
        public double Yaw
        {
            get { return _yaw; }
            set 
            { 
                _yaw = value;
                NotifyOfPropertyChange(() => Yaw);
            }
        }     
        public double Roll
        {
            get { return _roll; }
            set 
            {
                _roll = value;
                NotifyOfPropertyChange(() => Roll);
            }
        }      
        public double Pitch
        {
            get { return _pitch; }
            set 
            { 
                _pitch = value;
                NotifyOfPropertyChange(() => Pitch);
            }
        }
        public double ThrottleA
        {
            get { return _throttleA; }
            set 
            { 
                _throttleA = value;
                NotifyOfPropertyChange(() => ThrottleA);
            }
        }
        public double ThrottleB
        {
            get { return _throttleB; }
            set 
            { 
                _throttleB = value;
                NotifyOfPropertyChange(() => ThrottleB);
            }
        }
        
        public JoystickViewModel(IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);
        }
        public void Handle(JoystickDataEvent message)
        {
            // Joystick & Flight Properties
            Aileron = (message.Aileron * 60) + 125;
            Elevator = (message.Elevator * 60) + 125;
            Rudder = message.Rudder;
            Altitude = message.Altitude;
            Airspeed = message.Airspeed;
            Direction = message.Direction;
            Yaw = message.Yaw;
            Roll = message.Roll;
            Pitch = message.Pitch;
            ThrottleA = message.ThrottleA;
            ThrottleB = message.ThrottleB;
        }
    }
}
