using GXPEngine.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class Timer
    {
        public delegate void TimerEnd();
        public event TimerEnd OnTimerEnd;
        protected bool _enabled;
        public bool enabled { get { return _enabled; } }
        public bool autoReset;
        public float interval;
        public float time;
        public float timeElapsed { get { return interval - time; } }

        private static List<Timer> TimerManager = new List<Timer>();
        public static void Update()
        {
            for (int i = TimerManager.Count() - 1; i>=0; i--)
            {
                Timer timer = TimerManager[i];
                if (timer.time > 0)
                    timer.time -= Time.deltaTimeS;
                else
                {
                    if (timer.autoReset)
                        timer.time = timer.interval;
                    timer.OnTimerEnd?.Invoke();
                }
            }
        }
        public Timer(bool enabled = false) 
        {
            this._enabled = enabled;
        }
        public void Set (float time)
        { 
            interval = time;
            this.time = time; 
        }
        public void Launch()
        {
            time = interval;
            _enabled = true;
            if (!TimerManager.Contains(this))
                TimerManager.Add(this);
        }
        public void Pause()
        {
            _enabled = false;
            TimerManager?.Remove(this);
        }
        public void Resume()
        {
            _enabled = true;
            if (!TimerManager.Contains(this))
                TimerManager.Add(this);
        }
        public void Reset()
        {
            Pause();
            time = 0;
        }
        public void SetLaunch(float time)
        {
            Set(time);
            Launch();
        }
    }
}
