﻿using System;

using Microsoft.ApplicationInsights.Metrics.Extensibility;

namespace Microsoft.ApplicationInsights.Metrics
{
    /// <summary>
    /// Configures a metric data series to use aggregators that count the number of distinct values tracked;
    /// and that produce aggregates where Sum = the number of distinct values tracked during the aggregation period,
    /// and Count = total number of tracked values (Man, Max and StdDev are always zero).
    /// 
    /// !! This configuration is not intended for general production systems !!
    /// It creates aggregators that use memory inefficiently by keeping a concurrent dictionary of all unique values
    /// seen during the ongoing aggregation period.
    /// Moreover, aggregates produced via this configuration cannot be combined across multiple application instances.
    /// Therefore, this configuration should only be used in single-instance-applications 
    /// and for metrics where the number of distinct values is relatively small.
    /// 
    /// The primary purpose of this configuration is to validate API usage scenarios where object values are tracked by a
    /// metric series (rather than numeric values).
    /// In unique count / distinct count scenarios, the most common values tracked are strings. Aggregators created by
    /// this configuration  will process any object, but they will convert it to a string (using .ToString()) before
    /// tracking. Numbers are also converted to strings in this manner. Nulls are tracked using the string <c>"null"</c>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1812: Avoid uninstantiated internal classes",
            Justification = "Needed for internal verification.")]
    public class MetricSeriesConfigurationForNaiveDistinctCount : IMetricSeriesConfiguration
    {
        private readonly bool _usePersistentAggregation;
        private readonly bool _caseSensitive;
        private readonly int _hashCode;

        static MetricSeriesConfigurationForNaiveDistinctCount()
        {
            MetricAggregateToTelemetryPipelineConverters.Registry.Add(
                                                                    typeof(ApplicationInsightsTelemetryPipeline),
                                                                    MetricConfigurations.Common.AggregateKinds().NaiveDistinctCount().Moniker,
                                                                    new NaiveDistinctCountAggregateToApplicationInsightsPipelineConverter());
        }

        /// <summary>
        /// </summary>
        /// <param name="usePersistentAggregation"></param>
        public MetricSeriesConfigurationForNaiveDistinctCount(bool usePersistentAggregation)
            : this(usePersistentAggregation, caseSensitiveDistinctions: true)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="usePersistentAggregation"></param>
        /// <param name="caseSensitiveDistinctions"></param>
        public MetricSeriesConfigurationForNaiveDistinctCount(bool usePersistentAggregation, bool caseSensitiveDistinctions)
        {
            _usePersistentAggregation = usePersistentAggregation;
            _caseSensitive = caseSensitiveDistinctions;

            unchecked
            {
                _hashCode = ((17 * 23) + (_usePersistentAggregation.GetHashCode() * 23) + _caseSensitive.GetHashCode());
            }
        }

        /// <summary>
        /// </summary>
        public bool RequiresPersistentAggregation { get { return _usePersistentAggregation; } }

        /// <summary>
        /// </summary>
        public bool IsCaseSensitiveDistinctions { get { return _caseSensitive; } }

        /// <summary>
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="aggregationCycleKind"></param>
        /// <returns></returns>
        public IMetricSeriesAggregator CreateNewAggregator(MetricSeries dataSeries, MetricAggregationCycleKind aggregationCycleKind)
        {
            IMetricSeriesAggregator aggregator = new NaiveDistinctCountMetricSeriesAggregator(this, dataSeries, aggregationCycleKind);
            return aggregator;
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            if (other != null)
            {
                var otherConfig = other as MetricSeriesConfigurationForNaiveDistinctCount;
                if (otherConfig != null)
                {
                    return Equals(otherConfig);
                }
            }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IMetricSeriesConfiguration other)
        {
            return Equals((object) other);
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MetricSeriesConfigurationForNaiveDistinctCount other)
        {
            if (other == null)
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            return (this.RequiresPersistentAggregation == other.RequiresPersistentAggregation);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
