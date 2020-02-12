using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeGuard
{
    public class MockAudibleAlarm : IAudibleAlarm
    {
        public bool IsOn = false;

        public void Sound()
        {
            IsOn = true;
        }

        public void Silence()
        {
            IsOn = false;
        }
    }
}
