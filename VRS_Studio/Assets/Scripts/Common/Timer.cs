using UnityEngine;

namespace VRSStudio.Common
{
    /// <summary>
    /// Used to check timeout.  Time.unscaledTime is used on calculation.
    /// Usage:
    ///   Timer timer = new timer(2); // timeout for two second
    ///   void Update() {
    ///       if (timer.Check()) {
    ///           timer.Set();  // run again
    ///       }
    ///   }
    /// </summary>
    class Timer
    {
        //private const string TAG = "Timer";

        /// <summary>
        /// Used for Timer.ToString()
        /// </summary>
        public string name;

        /// <summary>
        /// This value will set to the time when <see cref="Set"/>.
        /// </summary>
        public float time;

        /// <summary>
        /// The timer's timeout setting.
        /// </summary>
        public float period;

        /// <summary>
        /// Check if the timer is on going.
        /// </summary>
        public bool IsSet { get; private set; }

        /// <summary>
        /// See if the timer is timout and checked.
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Create a timeout timer with timeout setting.
        /// </summary>
        /// <param name="sec">timeout</param>
        public Timer(float sec)
        {
            if (sec < Mathf.Epsilon * 2)
            {
                period = 0;
                IsPaused = true;
                return;
            }
            period = sec;
        }

        /// <summary>
        /// Means: "Start/Go/Begin".  Will update the check time.
        /// </summary>
        /// <param name="sec">set the timeout in second</param>
        public void Set(float sec)
        {
            //Log.d(TAG, "Timer Set", true);
            IsPaused = false;
            IsSet = true;
            time = Time.unscaledTime + sec;
            if (sec < Mathf.Epsilon * 2)
            {
                period = 0;
                return;
            }
            period = sec;
        }

        /// <summary>
        /// Means: "Start/Go/Begin".  Keep last timeout setting.
        /// </summary>
        public void Set()
        {
            //Log.d(TAG, "Timer Set", true);
            IsPaused = false;
            IsSet = true;
            time = Time.unscaledTime + period;
            if (period < Mathf.Epsilon * 2)
            {
                period = 0;
                return;
            }
        }

        /// <summary>
        /// Reset timer, but not start. Keep last timeout setting.
        /// </summary>
        public void Reset()
        {
            //Log.d(TAG, "Timer Reset", true);
            IsSet = false;
            IsPaused = false;
        }

        /// <summary>
        /// Let timer timerout immediately.  But not change the period.
        /// </summary>
        public void Timeout()
        {
            time = -1;
        }

        /// <summary>
        /// Check if timeout.  It will always return true if timeout.
        /// Call <see cref="Set"/> or <see cref="Reset"/> to start again.
        /// Use <see cref="Pause"/> to check if timer is already checked once.
        /// </summary>
        /// <returns>true if timeout.</returns>
        public bool Check()
        {
            //Log.d(TAG, "Timer Check = " + (time < Time.unscaledTime));
            if (!IsSet) return false;
            var c = Time.unscaledTime > time;
            if (c)
                IsPaused = true;
            return c;
        }

        /// <summary>
        /// progress from 0 to 1
        /// </summary>
        /// <returns>progress</returns>
        public float Progress()
        {
            if (period < Mathf.Epsilon * 2)
                return 1;
            return Mathf.Clamp01(1 - ((time - Time.unscaledTime) / period));
        }

        public override string ToString()
        {
            string state = IsPaused ? "Paused" : (IsSet ? "Set" : "NoSet");
            if (string.IsNullOrEmpty(name))
                return $"{{ s={state}, p={Progress()} }}";
            else
                return $"{{ n={name}, s={state}, p={Progress()} }}";
        }
    }
}