using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TextGameExperiment.Core.Extensions
{
    public class TimerExt
    {
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
                _backingTimer.Change(0, _currentInterval);
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
            _currentInterval = milliseconds;
            _backingTimer.Change(0, milliseconds);
        }

        public void Change(int delay, int interval)
        {
            _currentInterval = interval;
            _backingTimer.Change(delay, interval);
        }

        public void Delay(int delay)
        {
            _backingTimer.Change(delay, _currentInterval);
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
