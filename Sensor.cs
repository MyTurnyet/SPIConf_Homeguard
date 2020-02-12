namespace HomeGuard {
    public class Sensor
    {
        public static readonly string DOOR = "door";
        public static readonly string MOTION = "motion";
        public static readonly string WINDOW = "window";
        public static readonly string FIRE = "fire";

        private string id;
        private string location;
        private string sensorType;
        private bool tripped = false;

        public Sensor(string id, string location, string sensorType)
        {
            this.id = id;
            this.location = location;
            this.sensorType = sensorType;
        }

        public string GetId()
        {
            return id;
        }

        public string GetSensorType()
        {
            return sensorType;
        }

        public string GetLocation()
        {
            return location;
        }

        public void Trip()
        {
            tripped = true;
        }

        public void Reset()
        {
            tripped = false;
        }

        public bool IsTripped()
        {
            return tripped;
        }
    }
}