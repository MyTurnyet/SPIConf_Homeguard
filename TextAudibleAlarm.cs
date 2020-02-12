using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeGuard
{
    public class TextAudibleAlarm : IAudibleAlarm
    {
        private bool IsOn = false;

        public void Sound()
        {
			IsOn = true;
            while (IsOn)
            {
                Console.WriteLine("BUZZ BUZZ BUZZ!!!");
                try
                {
                    Thread.Sleep(1000);
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
		}

        public void Silence()
        {
            IsOn = false;
        }
    }
}
