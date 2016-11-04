﻿namespace Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.Implementation.WebAppPerformanceCollector
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    internal class WebAppPerformanceCollector : IPerformanceCollector
    {
        private readonly List<Tuple<PerformanceCounterData, ICounterValue>> performanceCounters = new List<Tuple<PerformanceCounterData, ICounterValue>>();

        private CounterFactory factory = new CounterFactory();

        /// <summary>
        /// Gets a collection of registered performance counters.
        /// </summary>
        public IEnumerable<PerformanceCounterData> PerformanceCounters
        {
            get { return this.performanceCounters.Select(t => t.Item1).ToList(); }
        }

        /// <summary>
        /// Loads instances that are used in performance counter computation.
        /// </summary>
        public void LoadDependentInstances()
        {
            this.factory = new CounterFactory();
        }

        /// <summary>
        /// Performs collection for all registered counters.
        /// </summary>
        /// <param name="onReadingFailure">Invoked when an individual counter fails to be read.</param>
        public IEnumerable<Tuple<PerformanceCounterData, double>> Collect(
            Action<string, Exception> onReadingFailure = null)
        {
            return this.performanceCounters.Where(pc => !pc.Item1.IsInBadState).SelectMany(
                counter =>
                    {
                        double value;

                        try
                        {
                            value = CollectCounter(counter.Item1.OriginalString, counter.Item2);
                        }
                        catch (InvalidOperationException e)
                        {
                            if (onReadingFailure != null)
                            {
                                onReadingFailure(counter.Item1.OriginalString, e);
                            }

                            return new Tuple<PerformanceCounterData, double>[] { };
                        }

                        return new[] { Tuple.Create(counter.Item1, value) };
                    });
        }

        /// <summary>
        /// Refreshes counters.
        /// </summary>
        public void RefreshCounters()
        {
            var countersToRefresh =
                this.PerformanceCounters.Where(pc => pc.IsInBadState)
                    .ToList();

            countersToRefresh.ForEach(pcd => this.RefreshPerformanceCounter(pcd));

            PerformanceCollectorEventSource.Log.CountersRefreshedEvent(countersToRefresh.Count.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Registers a counter using the counter name and reportAs value to the total list of counters.
        /// </summary>
        /// <param name="perfCounter">Name of the performance counter.</param>
        /// <param name="reportAs">Report as name for the performance counter.</param>
        /// <param name="isCustomCounter">Boolean to check if the performance counter is custom defined.</param>
        /// <param name="error">Captures the error logged.</param>
        /// <param name="blockCounterWithInstancePlaceHolder">Boolean that controls the registry of the counter based on the availability of instance place holder.</param>
        public void RegisterCounter(
            string perfCounter,
            string reportAs,
            bool isCustomCounter,
            out string error,
            bool blockCounterWithInstancePlaceHolder)
        {
            error = null;

            try
            {
                bool useInstancePlaceHolder = false;
                string parsingError = null;
                var pc = PerformanceCounterUtility.CreateAndValidateCounter(perfCounter, null, null, out useInstancePlaceHolder, out parsingError);

                if (!string.IsNullOrEmpty(parsingError))
                {
                    error = parsingError;
                }

                if (pc != null)
                {
                    this.RegisterPerformanceCounter(perfCounter, this.GetCounterReportAsName(perfCounter, reportAs), pc.CategoryName, pc.CounterName, pc.InstanceName, useInstancePlaceHolder, false);
                }
                else
                {
                    this.RegisterPerformanceCounter(perfCounter, this.GetCounterReportAsName(perfCounter, reportAs), string.Empty, perfCounter, string.Empty, useInstancePlaceHolder, false);
                }
            }
            catch (Exception e)
            {
                PerformanceCollectorEventSource.Log.WebAppCounterRegistrationFailedEvent(
                    e.Message,
                    perfCounter);
                error = e.Message;
            }
        }

        /// <summary>
        /// Rebinds performance counters to Windows resources.
        /// </summary>
        public void RefreshPerformanceCounter(PerformanceCounterData pcd)
        {
            Tuple<PerformanceCounterData, ICounterValue> tupleToRemove = this.performanceCounters.FirstOrDefault(t => t.Item1 == pcd);
            if (tupleToRemove != null)
            {
                this.performanceCounters.Remove(tupleToRemove);
            }

            this.RegisterPerformanceCounter(
                pcd.OriginalString,
                pcd.ReportAs,
                pcd.CategoryName,
                pcd.CounterName,
                pcd.InstanceName,
                pcd.UsesInstanceNamePlaceholder,
                pcd.IsCustomCounter);
        }

        /// <summary>
        /// Collects a value for a single counter.
        /// </summary>
        private static double CollectCounter(string coutnerOriginalString, ICounterValue counter)
        {
            try
            {
                return counter.GetValueAndReset();
            }
            catch (Exception e)
            {
                 throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.WebAppPerformanceCounterReadFailed,
                        coutnerOriginalString),
                    e);
            }
        }

        /// <summary>
        /// Register a performance counter for collection.
        /// </summary>
        private void RegisterPerformanceCounter(string originalString, string reportAs, string categoryName, string counterName, string instanceName, bool usesInstanceNamePlaceholder, bool isCustomCounter)
        {
            ICounterValue counter = null;

            try
            {
                counter = this.factory.GetCounter(originalString, reportAs);
            }
            catch
            {
                PerformanceCollectorEventSource.Log.CounterNotWebAppSupported(originalString);
                return;
            }

            bool firstReadOk = false;

            try
            {
                // perform the first read. For many counters the first read will always return 0
                // since a single sample is not enough to calculate a value
                var value = counter.GetValueAndReset();
                firstReadOk = true;
            } 
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.WebAppPerformanceCounterFirstReadFailed,
                        counterName),
                    e);
            }
            finally
            {
                PerformanceCounterData perfData = new PerformanceCounterData(
                        originalString,
                        reportAs,
                        usesInstanceNamePlaceholder,
                        isCustomCounter,
                        !firstReadOk,
                        categoryName,
                        counterName,
                        instanceName);

                this.performanceCounters.Add(new Tuple<PerformanceCounterData, ICounterValue>(perfData, counter));
            }
        }

        /// <summary>
        /// Gets metric alias to be the value given by the user.
        /// </summary>
        /// <param name="counterName">Name of the counter to retrieve.</param>
        /// <param name="reportAs">Alias to report the counter.</param>
        /// <returns>Alias that will be used for the counter.</returns>
        private string GetCounterReportAsName(string counterName, string reportAs)
        {
            if (reportAs == null)
            {
                return counterName;
            }
            else
            {
                return reportAs;
            }
        }
    }
}
