﻿namespace Microsoft.ApplicationInsights.DataContracts
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.Extensibility.Implementation;

    /// <summary>
    /// Represents a context for sending telemetry to the Application Insights service.
    /// </summary>
    public sealed class TelemetryContext
    {
        private readonly IDictionary<string, string> properties;
        private readonly Lazy<IDictionary<string, string>> correlationContext;
        private readonly IDictionary<string, string> tags;

        private string instrumentationKey;

        private ComponentContext component;
        private DeviceContext device;
        private CloudContext cloud;
        private SessionContext session;
        private UserContext user;
        private OperationContext operation;
        private LocationContext location;
        private InternalContext internalContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryContext"/> class.
        /// </summary>
        public TelemetryContext()
            : this(new ConcurrentDictionary<string, string>())
        {
        }

        internal TelemetryContext(IDictionary<string, string> properties)
        {
            Debug.Assert(properties != null, "properties");
            this.properties = properties;
            this.tags = new ConcurrentDictionary<string, string>();
            this.correlationContext = new Lazy<IDictionary<string, string>>(() => new Dictionary<string, string>());
        }

        /// <summary>
        /// Gets or sets the default instrumentation key for all <see cref="ITelemetry"/> objects logged in this <see cref="TelemetryContext"/>.
        /// </summary>
        /// <remarks>
        /// By default, this property is initialized with the <see cref="TelemetryConfiguration.InstrumentationKey"/> value
        /// of the <see cref="TelemetryConfiguration.Active"/> instance of <see cref="TelemetryConfiguration"/>. You can specify it 
        /// for all telemetry tracked via a particular <see cref="TelemetryClient"/> or for a specific <see cref="ITelemetry"/> 
        /// instance.
        /// </remarks>
        public string InstrumentationKey
        {
            get { return this.instrumentationKey ?? string.Empty; }
            set { Property.Set(ref this.instrumentationKey, value); }
        }
        
        /// <summary>
        /// Gets the object describing the component tracked by this <see cref="TelemetryContext"/>.
        /// </summary>
        public ComponentContext Component 
        {
            get { return LazyInitializer.EnsureInitialized(ref this.component, () => new ComponentContext(this.Tags)); }
        }

        /// <summary>
        /// Gets the object describing the device tracked by this <see cref="TelemetryContext"/>.
        /// </summary>
        public DeviceContext Device
        {
            get { return LazyInitializer.EnsureInitialized(ref this.device, () => new DeviceContext(this.Tags, this.Properties)); }
        }

        /// <summary>
        /// Gets the object describing the cloud tracked by this <see cref="TelemetryContext"/>.
        /// </summary>
        public CloudContext Cloud
        {
            get { return LazyInitializer.EnsureInitialized(ref this.cloud, () => new CloudContext(this.Tags)); }
        }

        /// <summary>
        /// Gets the object describing a user session tracked by this <see cref="TelemetryContext"/>.
        /// </summary>
        public SessionContext Session
        {
            get { return LazyInitializer.EnsureInitialized(ref this.session, () => new SessionContext(this.Tags)); }
        }

        /// <summary>
        /// Gets the object describing a user tracked by this <see cref="TelemetryContext"/>.
        /// </summary>
        public UserContext User
        {
            get { return LazyInitializer.EnsureInitialized(ref this.user, () => new UserContext(this.Tags)); }
        }

        /// <summary>
        /// Gets the object describing a operation tracked by this <see cref="TelemetryContext"/>.
        /// </summary>
        public OperationContext Operation
        {
            get { return LazyInitializer.EnsureInitialized(ref this.operation, () => new OperationContext(this.Tags)); }
        }

        /// <summary>
        /// Gets the object describing a location tracked by this <see cref="TelemetryContext" />.
        /// </summary>
        public LocationContext Location
        {
            get { return LazyInitializer.EnsureInitialized(ref this.location, () => new LocationContext(this.Tags)); }
        }

        /// <summary>
        /// Gets a dictionary of application-defined property values.
        /// </summary>
        public IDictionary<string, string> Properties
        {
            get { return this.properties; }
        }

        /// <summary>
        /// Gets a Correlation-Context for the operation.
        /// <see href="https://github.com/lmolkova/correlation/blob/master/http_protocol_proposal_v1.md"/> 
        /// </summary>
        internal IDictionary<string, string> CorrelationContext
        {
            get { return this.correlationContext.Value; }
        }

        internal InternalContext Internal
        {
            get { return LazyInitializer.EnsureInitialized(ref this.internalContext, () => new InternalContext(this.Tags)); }
        }

        /// <summary>
        /// Gets a dictionary of context tags.
        /// </summary>
        internal IDictionary<string, string> Tags
        {
            get { return this.tags; }
        }

        internal void Initialize(TelemetryContext source, string instrumentationKey)
        {
            Property.Initialize(ref this.instrumentationKey, instrumentationKey);

            if (source.tags != null && source.tags.Count > 0)
            {
                Utils.CopyDictionary(source.tags, this.Tags);
            }
        }
    }
}