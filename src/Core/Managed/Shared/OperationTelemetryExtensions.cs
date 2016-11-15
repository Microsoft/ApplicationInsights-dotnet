﻿namespace Microsoft.ApplicationInsights
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using Microsoft.ApplicationInsights.Extensibility.Implementation;

    /// <summary>
    /// Extension functions to operation telemetry that start and stop the timer.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class OperationTelemetryExtensions
    {
        /// <summary>
        /// An extension to telemetry item that starts the timer for the the respective telemetry.
        /// </summary>
        /// <param name="telemetry">Telemetry item object that calls this extension method.</param>
        public static void Start(this OperationTelemetry telemetry)
        {            
            telemetry.Timestamp = DateTimeOffset.UtcNow;

            // Begin time is used internally for calculating duration of operation at the end call,
            // and hence is stored using higher precision Clock.
            telemetry.BeginTime = Clock.Instance.Time;
        }

        /// <summary>
        /// An extension method to telemetry item that stops the timer and computes the duration of the request or dependency.
        /// </summary>
        /// <param name="telemetry">Telemetry item object that calls this extension method.</param>
        public static void Stop(this OperationTelemetry telemetry)
        {
            if (telemetry.Timestamp != DateTimeOffset.MinValue)
            {
                telemetry.Duration = Clock.Instance.Time - telemetry.BeginTime;
            }
            else
            {
                telemetry.Timestamp = DateTimeOffset.UtcNow;
                telemetry.Duration = TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Generate random operation Id and set it to OperationContext.
        /// </summary>
        /// <param name="telemetry">Telemetry to initialize Operation id for.</param>
        public static void GenerateOperationId(this OperationTelemetry telemetry)
        {
            telemetry.Id = Convert.ToBase64String(BitConverter.GetBytes(WeakConcurrentRandom.Instance.Next()));
        }
    }
}
