using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeGuard
{
    public interface IAudibleAlarm
    {
        void Sound();
        void Silence();
    }
}
