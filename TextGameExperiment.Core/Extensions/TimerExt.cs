using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TextGameExperiment.Core.Extensions
{
    public class TimerExt
    {
        private const int MinimumTimerInterval = 15; // In milliseconds. Any lower and we start to choke the platform for no actual speed gain.

        private Timer _backingTimer;
        private TimerCallback _currentCallback;
        private bool _isRunning;      
        private int _currentInterval;
        private object _lock = new object();

        public TimerExt(TimerCallback callback, object state, int dueTime, int period)
        {
            _currentInterval = period;
            _currentCallback = callback;
            _backingTimer = new Timer(OnTick, state, dueTime, period);                        
        }

        public TimerExt(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period) 
            : this(callback, state, (int)dueTime.TotalMilliseconds, (int)period.TotalMilliseconds)
        {

        }

        public void Start()
        {
            bool wasRunningWhenCalled = false;
            lock (_lock)
            {
                wasRunningWhenCalled = _isRunning;
                _isRunning = true;
            }
            if (!wasRunningWhenCalled)
            {
                this.Change(0, _currentInterval);
            }
        }

        public void Stop()
        {
            bool wasRunningWhenCalled = false;
            lock (_lock)
            {
                wasRunningWhenCalled = _isRunning;
                _isRunning = false;
            }
            if (wasRunningWhenCalled)
            {
                _backingTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }        

        public void ChangeInterval(int milliseconds)
        {            
            this.Change(0, milliseconds);
        }

        public void Change(int delay, int interval)
        {
            int adjustedInterval = interval == -1 ? interval : Math.Max(MinimumTimerInterval, interval); //-1 is a valid value, but nothing else lower than the minimum should be.
            _currentInterval = adjustedInterval;
            _backingTimer.Change(delay, adjustedInterval);
        }

        public void Delay(int delay)
        {
            this.Change(delay, _currentInterval);
        }

        private void OnTick(object state)
        {
            // do stuff we need to do locally
            lock (_lock)
            {
                _isRunning = true;
            }
            
            _currentCallback(state);                        
        }
    }
}
