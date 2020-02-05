﻿namespace Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights.Channel;

    internal class TelemetrySerializer
    {
        internal readonly Transmitter Transmitter;
        private Uri endpoindAddress;

        public TelemetrySerializer(Transmitter transmitter) => this.Transmitter = transmitter ?? throw new ArgumentNullException(nameof(transmitter));

        protected TelemetrySerializer()
        {
            // for stubs and mocks
        }

        /// <summary>
        /// Gets or sets the endpoint address.  
        /// </summary>
        public Uri EndpointAddress
        {
            get { return this.endpoindAddress; }
            set { this.endpoindAddress = value ?? throw new ArgumentNullException(nameof(this.EndpointAddress)); }
        }

        public virtual void Serialize(ICollection<ITelemetry> items)
        {
            this.HandleTelemetryException(items);

            var transmission = new Transmission(this.EndpointAddress, items);
            this.Transmitter.Enqueue(transmission);
        }

        public virtual Task<bool> SerializeAsync(ICollection<ITelemetry> items, CancellationToken cancellationToken)
        {
            this.HandleTelemetryException(items);

            var transmission = new Transmission(this.EndpointAddress, items);
            Task<bool> resultTask = transmission.GetFlushTask(cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
            {
                Task.Run(() => this.Transmitter.Enqueue(transmission), cancellationToken);
            }

            return resultTask;
        }

        private void HandleTelemetryException(ICollection<ITelemetry> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (!items.Any())
            {
                throw new ArgumentException("One or more telemetry item is expected", nameof(items));
            }

            if (this.EndpointAddress == null)
            {
                throw new Exception("TelemetrySerializer.EndpointAddress was not set.");
            }
        }
    }
}
