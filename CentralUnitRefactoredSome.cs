using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeGuard
{
    public class CentralUnitRefactoredSome
    {
		// sensor test status options
		public static string PASS = "PASS";
	    public static string FAIL = "FAIL";
	    public static string PENDING = "pending";
	    public static string READY = "ready";

	    private bool armed = false;
		private string securityCode;
		private readonly List<SensorRefactoredSome> sensors = new List<SensorRefactoredSome>();
		private IHomeGuardView view = new TextView();
		private IAudibleAlarm audibleAlarm = new TextAudibleAlarm();

		// members to help with sensor tests
		private Dictionary<string, string> sensorTestStatusMap;
		private bool runningSensorTest;
		private String sensorTestStatus;

		public bool IsArmed()
		{
			return armed;
		}

		public void Arm()
		{
			armed = true;
		}

		public void SetSecurityCode(String code)
		{
            securityCode = code;
		}

		public bool IsValidCode(String code)
		{
			return securityCode.Equals(code);
		}

		public void EnterCode(String code)
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

		public List<SensorRefactoredSome> GetSensors()
		{
			return sensors;
		}

		public void RegisterSensor(SensorRefactoredSome sensor)
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

		public void ParseRadioBroadcast(String packet)
		{
			//parse the packet
			string[] tokens = packet.Split(',');
			string id = tokens[0];
			string status = tokens[1];

			// find sensor with id
			SensorRefactoredSome sensor = null;
            foreach(SensorRefactoredSome s in sensors)
            {
                if (s.GetId().Equals(id))
				{
					sensor = s;
					break;
				}
			}

			//trip or reset sensor
			if (sensor != null)
			{
				if ("TRIPPED".Equals(status))
					sensor.Trip();
				else
					sensor.Reset();
			}

			//get the message from the sensor and display it
			string message = GetSensorMessage(sensor);
			view.ShowMessage(message);

			// sound the alarm if armed
			if (IsArmed())
				audibleAlarm.Sound();

			// check if a sensor test is running and adjust status
			if (runningSensorTest)
			{
				if ("TRIPPED".Equals(status))
				{
					sensorTestStatusMap.Add(id, PASS);
				}

				// check to see if test is complete
				bool done = true;
                foreach (string testStatus in sensorTestStatusMap.Values)
                {
                    if (PENDING.Equals(testStatus))
					{
						done = false;
						break;
					}
				}

				//terminate test if complete
				if (done)
					TerminateSensorTest();
			}
		}

		public void RunSensorTest()
		{
			runningSensorTest = true;
			sensorTestStatus = PENDING;

			// initialize the status map
			sensorTestStatusMap = new Dictionary<string, string>();
            foreach (SensorRefactoredSome sensor in sensors)
            {
                sensorTestStatusMap.Add(sensor.GetId(), PENDING);
			}
		}

		// used during sensor test
		public void TerminateSensorTest()
		{
			runningSensorTest = false;

			// look at individual sensor status to determine the overall test status
			sensorTestStatus = PASS;
            foreach (string status in sensorTestStatusMap.Values)
            {
                if (status.Equals(PENDING))
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
		public Dictionary<string, string> GetSensorTestStatusMap()
		{
			return sensorTestStatusMap;
		}

		public string GetSensorMessage(SensorRefactoredSome sensor)
		{
            string message = "default";
			if (!sensor.IsTripped())
			{
				if (sensor.MyType().Equals(SensorRefactoredSome.DOOR))
					return sensor.GetLocation() + " is closed";
				else if (sensor.MyType().Equals(SensorRefactoredSome.WINDOW))
					return sensor.GetLocation() + " is sealed";
				else if (sensor.MyType().Equals(SensorRefactoredSome.MOTION))
					return sensor.GetLocation() + " is motionless";
				else if (sensor.MyType().Equals(SensorRefactoredSome.FIRE))
					return sensor.GetLocation() + " temperature is normal";
			}
			else
			{
				if (sensor.MyType().Equals(SensorRefactoredSome.DOOR))
					return sensor.GetLocation() + " is open";
				else if (sensor.MyType().Equals(SensorRefactoredSome.WINDOW))
					return sensor.GetLocation() + " is ajar";
				else if (sensor.MyType().Equals(SensorRefactoredSome.MOTION))
					return "Motion detected in " + sensor.GetLocation();
				else if (sensor.MyType().Equals(SensorRefactoredSome.FIRE))
					return sensor.GetLocation() + " is on FIRE!";
			}
			return message;
		}
	}
}
