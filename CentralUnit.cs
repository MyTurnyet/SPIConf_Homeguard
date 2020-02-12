using System.Collections;
using System.Collections.Specialized;
using CodeKatas.Workshop.HomeGuard;

namespace HomeGuardOrg {
    public class CentralUnit
    {
        // sensor test status options
        public static readonly string PASS = "PASS";
        public static readonly string FAIL = "FAIL";
        public static readonly string PENDING = "pending";
        public static readonly string READY = "ready";

        private bool armed = false;
        private string securityCode;
        private IList sensors = new ArrayList();
        private IHomeGuardView view = new TextView();
        private IAudibleAlarm audibleAlarm = new TextAudibleAlarm();

        // members to help with sensor tests
        private IDictionary sensorTestStatusMap;
        private bool runningSensorTest;
        private string sensorTestStatus;

        public bool isArmed()
        {
            return armed;
        }

        public void arm()
        {
            armed = true;
        }

        public void SetSecurityCode(string code)
        {
            securityCode = code;
        }

        public bool IsValidCode(string code)
        {
            return securityCode == code;
        }

        public void EnterCode(string code)
        {
            if (IsValidCode(code))
            {
                armed = false;
                audibleAlarm.Silence();
            }
        }

        public bool AudibleAlarmIsOn()
        {
            return false;
        }

        public IList GetSensors()
        {
            return sensors;
        }

        public void RegisterSensor(Sensor sensor)
        {
            sensors.Add(sensor);
        }

        public void SetView(IHomeGuardView view)
        {
            this.view = view;
        }

        public void SetAudibleAlarm(IAudibleAlarm alarm)
        {
            audibleAlarm = alarm;
        }

        public void ParseRadioBroadcast(string packet)
        {
            //parse the packet
            string[] tokens = packet.Split(',');
            string id = tokens[0];
            string status = tokens[1];

            // find sensor with id
            Sensor sensor = null;
            for (IEnumerator enumerator = sensors.GetEnumerator(); enumerator.MoveNext();)
            {
                if (enumerator.Current == null) enumerator.MoveNext();
                Sensor s = (Sensor) enumerator.Current;
                if (s.GetId() == id)
                {
                    sensor = s;
                    break;
                }
            }

            //trip or reset sensor
            if (sensor != null)
            {
                if ("TRIPPED" == status)
                    sensor.Trip();
                else
                    sensor.Reset();
            }

            //get the message from the sensor and display it
            string message = GetSensorMessage(sensor);
            view.ShowMessage(message);

            // sound the alarm if armed
            if (isArmed())
                audibleAlarm.Sound();

            // check if a sensor test is running and adjust status
            if (runningSensorTest)
            {
                if ("TRIPPED" == status)
                {
                    sensorTestStatusMap.Add(id, PASS);
                }

                // check to see if test is complete
                bool done = true;
                for (IEnumerator enumerator = sensorTestStatusMap.Values.GetEnumerator(); enumerator.MoveNext();)
                {
                    if (enumerator.Current == null) enumerator.MoveNext();
                    string testStatus = (string)enumerator.Current;
                    if (PENDING == testStatus)
                    {
                        done = false;
                        break;
                    }
                }

                //terminate test if complete
                if (done)
                    terminateSensorTest();
            }
        }

        public void RunSensorTest()
        {
            runningSensorTest = true;
            sensorTestStatus = PENDING;

            // initialize the status map
            sensorTestStatusMap = new ListDictionary();
            for (IEnumerator enumerator = sensors.GetEnumerator(); enumerator.MoveNext();)
            {
                if (enumerator.Current == null) enumerator.MoveNext();
                Sensor sensor = (Sensor) enumerator.Current;
                sensorTestStatusMap.Add(sensor.GetId(), PENDING);
            }
        }

        // used during sensor test
        public void terminateSensorTest()
        {
            runningSensorTest = false;

            // look at individual sensor status to determine the overall test status
            sensorTestStatus = PASS;
            for (IEnumerator enumerator = sensorTestStatusMap.Values.GetEnumerator(); enumerator.MoveNext();)
            {
                string status = (string)enumerator.Current;
                if (status == PENDING)
                {
                    sensorTestStatus = FAIL;
                    break;
                }
            }
        }

        // used during sensor test
        public string GetSesnsorTestStatus()
        {
            return sensorTestStatus;
        }

        // used during sensor test
        public IDictionary GetSensorTestStatusMap()
        {
            return sensorTestStatusMap;
        }

        public string GetSensorMessage(Sensor sensor)
        {
            string message = "default";
            if (!sensor.IsTripped())
            {
                if (sensor.GetSensorType() == Sensor.DOOR)
                    return sensor.GetLocation() + " is closed";
                else if (sensor.GetSensorType() == Sensor.WINDOW)
                    return sensor.GetLocation() + " is sealed";
                else if (sensor.GetSensorType() == Sensor.MOTION)
                    return sensor.GetLocation() + " is motionless";
                else if (sensor.GetSensorType() == Sensor.FIRE)
                    return sensor.GetLocation() + " temperature is normal";
            }
            else
            {
                if (sensor.GetSensorType() == Sensor.DOOR)
                    return sensor.GetLocation() + " is open";
                else if (sensor.GetSensorType() == Sensor.WINDOW)
                    return sensor.GetLocation() + " is ajar";
                else if (sensor.GetSensorType() == Sensor.MOTION)
                    return "Motion detected in " + sensor.GetLocation();
                else if (sensor.GetSensorType() == Sensor.FIRE)
                    return sensor.GetLocation() + " is on FIRE!";
            }
            return message;
        }
    }
}