﻿namespace Microsoft.ApplicationInsights.Extensibility.Implementation.Tracing
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Implementation;

#if !NET40
    using TaskEx = System.Threading.Tasks.Task;
#endif

    internal class DiagnoisticsEventThrottlingScheduler 
        : IDiagnoisticsEventThrottlingScheduler
    {
        private readonly IList<TaskTimer> timers = new List<TaskTimer>();

        public ICollection<object> Tokens
        {
            get
            {
                return new ReadOnlyCollection<object>(this.timers.Cast<object>().ToList());
            }
        }

        public object ScheduleToRunEveryTimeIntervalInMilliseconds(
            int interval,
            Action actionToExecute)
        {
            if (interval <= 0)
            {
                throw new ArgumentOutOfRangeException("interval");
            }

            if (actionToExecute == null)
            {
                throw new ArgumentNullException("actionToExecute");
            }

            var token = InternalCreateAndStartTimer(interval, actionToExecute);
            this.timers.Add(token);

            CoreEventSource.Log.DiagnoisticsEventThrottlingSchedulerTimerWasCreated(interval.ToString(CultureInfo.InvariantCulture));

            return token;
        }

        public void RemoveScheduledRoutine(object token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            var timer = token as TaskTimer;
            if (timer == null)
            {
                throw new ArgumentException("token");
            }

            if (this.timers.Remove(timer))
            {
                CoreEventSource.Log.DiagnoisticsEventThrottlingSchedulerTimerWasRemoved();
            }
        }


        private static TaskTimer InternalCreateAndStartTimer(
            int intervalInMilliseconds,
            Action action)
        {
            var timer = new TaskTimer
            {
                Delay = TimeSpan.FromMilliseconds(intervalInMilliseconds)
            };

            Func<Task> task = null;

            task = () =>
                {
                    timer.Start(task);
                    action();
                    return TaskEx.FromResult<object>(null);
                };

            timer.Start(task);

            return timer;
        }
    }
}