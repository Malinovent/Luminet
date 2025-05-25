
namespace Mali.Utils.Timers
{
    using System;
    
    [Serializable]
    public class Timer
    {
        public float RemainingSeconds { get; private set; }
        private float Duration { get; set; }
        public Action OnTimerEnd { get; set; }
        private bool isPaused = false;

        public Timer(float duration)
        {
            Duration = duration;
            RemainingSeconds = duration;
        }
        
        public void Update(float deltaTime)
        {
            if(RemainingSeconds <= 0 || isPaused) return;
            
            RemainingSeconds -= deltaTime;
            //RemainingSeconds = RemainingSeconds >= Duration ? Duration : RemainingSeconds;
            CheckForTimerEnd();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void CheckForTimerEnd()
        {
            if (RemainingSeconds > 0) return;

            RemainingSeconds = 0;
            OnTimerEnd?.Invoke();
        }

        public void Reset()
        {
            RemainingSeconds = Duration;
        }

        public void Pause()
        {
            isPaused = true;
        }
        
        public void Resume()
        {
            isPaused = false;
        }

        public void Stop()
        {
            RemainingSeconds = 0;
        }

        public bool TryReset()
        {
            if (RemainingSeconds <= 0)
            {
                Reset();
                return true;
            }

            return false;
        }
        

        public float GetPercentage()
        {
            return (Duration - RemainingSeconds) / Duration;
        }
        
        public bool IsDone()
        {
            return RemainingSeconds <= 0 /*|| RemainingSeconds >= Duration*/;
        }
        
        public void SetRemainingSeconds(float seconds)
        {
            RemainingSeconds = seconds;
        }
        
        public void SetDuration(float duration)
        {
            Duration = duration;
            
            if (RemainingSeconds > Duration)
            {
                RemainingSeconds = Duration;
            }
        }

        public float GetDuration()
        {
            return Duration;
        }

        public Timer Clone()
        {
            Timer clone = new Timer(Duration)
            {
                RemainingSeconds = RemainingSeconds
            };
            return clone;
        }
    }
}
