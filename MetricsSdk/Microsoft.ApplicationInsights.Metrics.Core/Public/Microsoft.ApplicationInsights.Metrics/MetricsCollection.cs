﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.ApplicationInsights.Metrics
{
    /// <summary>
    /// </summary>
    public class MetricsCollection : ICollection<Metric>
    {
        private readonly MetricManager _metricManager;
        private readonly ConcurrentDictionary<string, Metric> _metrics = new ConcurrentDictionary<string, Metric>();

        /// <summary>
        /// </summary>
        public int Count
        {
            get { return _metrics.Count; }
        }

        /// <summary>
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// </summary>
        /// <param name="metricManager"></param>
        internal MetricsCollection(MetricManager metricManager)
        {
            Util.ValidateNotNull(metricManager, nameof(metricManager));
            _metricManager = metricManager;
        }

        /// <summary>
        /// </summary>
        /// <param name="metricId"></param>
        /// <param name="dimension1Name"></param>
        /// <param name="dimension2Name"></param>
        /// <param name="metricConfiguration"></param>
        /// <returns></returns>
        public Metric GetOrCreate(
                                string metricId,
                                string dimension1Name,
                                string dimension2Name,
                                IMetricConfiguration metricConfiguration)
        {
            metricId = metricId?.Trim();
            Util.ValidateNotNullOrWhitespace(metricId, nameof(metricId));
            
            string metricObjectId = Metric.GetObjectId(metricId, dimension1Name, dimension2Name);
            Metric metric = _metrics.GetOrAdd(
                                            metricObjectId,
                                            (key) => new Metric(
                                                                _metricManager,
                                                                metricId,
                                                                dimension1Name,
                                                                dimension2Name,
                                                                metricConfiguration ?? MetricConfigurations.Common.Default())
                                             );

            if (metricConfiguration != null && ! metric._configuration.Equals(metricConfiguration))
            {
                throw new ArgumentException("A Metric with the specified Id and dimension names already exists, but it has a configuration"
                                          + " that is different from the specified configuration. You may not change configurations once a"
                                          + " metric was created for the first time. Either specify the same configuration every time, or"
                                          + " specify 'null' during every invocation except the first one. 'Null' will match against any"
                                          + " previously specified configuration when retrieving existing metrics, or fall back to"
                                          +$" the default when creating new metrics. (Metric Object Id: \"{metricObjectId}\".)");
            }

            return metric;
        }

        /// <summary>
        /// </summary>
        public void Clear()
        {
            _metrics.Clear();
        }

        /// <summary>
        /// </summary>
        /// <param name="metric"></param>
        /// <returns></returns>
        public bool Contains(Metric metric)
        {
            if (metric == null)
            {
                return false;
            }

            return _metrics.ContainsKey(metric._objectId);
        }

        /// <summary>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(Metric[] array, int arrayIndex)
        {
            Util.ValidateNotNull(array, nameof(array));

            if (arrayIndex < 0 || arrayIndex >= array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            _metrics.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// </summary>
        /// <param name="metric"></param>
        /// <returns></returns>
        public bool Remove(Metric metric)
        {
            if (metric == null)
            {
                return false;
            }

            Metric removedMetric;
            return _metrics.TryRemove(metric._objectId, out removedMetric);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Metric> GetEnumerator()
        {
            return _metrics.Values.GetEnumerator();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// The Add(..) method is not supported. To add a new metric, use the GetOrCreate(..) method.
        /// </summary>
        /// <param name="unsupported"></param>
        void ICollection<Metric>.Add(Metric unsupported)
        {
            throw new NotSupportedException($"The Add(..) method is not supported by this {nameof(MetricsCollection)}."
                                           + " To add a new metric, use the GetOrCreate(..) method.");
        }
    }
}