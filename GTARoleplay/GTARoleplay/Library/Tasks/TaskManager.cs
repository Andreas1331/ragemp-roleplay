using GTANetworkAPI;
using GTARoleplay.Library.Tasks.Data;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GTARoleplay.Library.Tasks
{
    public class TaskManager : Script
    {
        private static readonly ConcurrentDictionary<int, ScheduledTask> schduledTasks = new ConcurrentDictionary<int, ScheduledTask>();
        private static int tasksCounter = 0;

        [ServerEvent(Event.Update)]
        public void OnUpdate()
        {
            if(schduledTasks != null && schduledTasks.Count > 0)
            {
                DateTime currentTime = DateTime.Now;

                // Check the tasks if any should run
                Parallel.ForEach(schduledTasks, (_task) =>
                {
                    if (_task.Value != null)
                    {
                        if ((currentTime - _task.Value.ScheduledTimestamp).TotalMilliseconds >= _task.Value.DelayInMs)
                        {
                            if(!_task.Value.RunOnMainThread)
                                _task.Value.ScheduledAction?.Invoke();
                            else
                            {
                                NAPI.Task.Run(() =>
                                {
                                    _task.Value.ScheduledAction?.Invoke();
                                });
                            }

                            // Remove the task if the looping is false
                            if(!_task.Value.MakeLoops)
                                schduledTasks.TryRemove(_task.Value.Id, out ScheduledTask _tmp);
                        }
                    }
                });
            }
        }

        public static void ScheduleTask(ScheduledTask task)
        {
            if (task == null || schduledTasks == null)
                return;

            task.Id = tasksCounter++;
            schduledTasks.TryAdd(task.Id, task);
        }
    }
}
