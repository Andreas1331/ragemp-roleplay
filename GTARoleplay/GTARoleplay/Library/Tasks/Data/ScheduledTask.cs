using System;

namespace GTARoleplay.Library.Tasks.Data
{
    public class ScheduledTask
    {
        public int Id;
        public readonly Action ScheduledAction;
        public readonly DateTime ScheduledTimestamp;
        public readonly float DelayInMs;
        public readonly bool MakeLoops;
        public readonly bool RunOnMainThread;

        public ScheduledTask(Action scheduledAction, float delayInMs, bool runOnMainThread = true, bool makeLoops = false)
        {
            ScheduledAction = scheduledAction;
            ScheduledTimestamp = DateTime.Now;
            DelayInMs = delayInMs;
            RunOnMainThread = runOnMainThread;
            MakeLoops = makeLoops; 
        }
    }
}
