namespace FlightGearProject.EventModels
{
    public class JoystickDataEvent
    {
        public double Aileron { get; set; }
        public double Elevator { get; set; }
        public double Rudder { get; set; }
        public double Altitude { get; set; }
        public double Airspeed { get; set; }
        public double Direction { get; set; }
        public double Yaw { get; set; }
        public double Roll { get; set; }
        public double Pitch { get; set; }
        public double ThrottleA { get; set; }
        public double ThrottleB { get; set; }

        public JoystickDataEvent(double ail, double ele, double rud, double alt, 
            double air, double dir, double yaw, double rol, double pit, double tA, double tB)
        {
            Aileron = ail;
            Elevator = ele;
            Rudder = rud;
            Altitude = alt;
            Airspeed = air;
            Direction = dir;
            Yaw = yaw;
            Roll = rol;
            Pitch = pit;
            ThrottleA = tA;
            ThrottleB = tB;
        }
    }
}
