using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Domain.Helpers
{
    public class OrderIdGenerator
    {
        private static int _counter = 0;
        private static readonly object _lock = new object();

        public static string GenerateOrderId()
        {
            lock (_lock)
            {
                _counter++;
                return $"MEK{_counter:D6}";
            }
        }
    }
}
