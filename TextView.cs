using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeGuard
{
    public class TextView : IHomeGuardView
    {
        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
