using System.Collections.Generic;
using System.Diagnostics;

namespace PortedSolver.Utils
{
    public class StopwatchTimer
    {
        private static readonly Dictionary<string, Stopwatch> _stopwatchDictionary = new Dictionary<string, Stopwatch>();
        public static int CornerChecks = 0;

        public static void Start(string id)
        {
            if (_stopwatchDictionary.ContainsKey(id))
            {
                _stopwatchDictionary[id].Start();
            }
            else
            {
                _stopwatchDictionary.Add(id, new Stopwatch());
                _stopwatchDictionary[id].Start();
            }
        }

        public static void Stop(string id)
        {
            if (_stopwatchDictionary.ContainsKey(id))
            {
                _stopwatchDictionary[id].Stop();
            }
            else
            {
                _stopwatchDictionary.Add(id, new Stopwatch());
            }
        }

        public static long GetElapsedMillisecondsAndReset(string id)
        {
            if (_stopwatchDictionary.ContainsKey(id))
            {
                long elapsed = _stopwatchDictionary[id].ElapsedMilliseconds;
                _stopwatchDictionary[id].Reset();
                return elapsed;
            }
            else
            {
                return -1;
            }
        }
    }
}
