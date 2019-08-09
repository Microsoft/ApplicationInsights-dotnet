﻿namespace Microsoft.ApplicationInsights.Extensibility.Implementation.Endpoints
{
    using System;
#if !NETSTANDARD1_3
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// This exception is used to notify the user that their Connection String has duplicate keys.
    /// </summary>
#if !NETSTANDARD1_3
    [Serializable]
#endif
    public class ConnectionStringMissingInstrumentationKeyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringMissingInstrumentationKeyException"/> class.
        /// </summary>
        public ConnectionStringMissingInstrumentationKeyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringMissingInstrumentationKeyException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConnectionStringMissingInstrumentationKeyException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringMissingInstrumentationKeyException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name = "innerException" > The exception that is the cause of the current exception, or a null reference(Nothing in Visual Basic) if no inner exception is specified.</param>
        public ConnectionStringMissingInstrumentationKeyException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if !NETSTANDARD1_3
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringMissingInstrumentationKeyException"/> class.
        /// </summary>
        /// <param name="serializationInfo">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="streamingContext">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected ConnectionStringMissingInstrumentationKeyException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
#endif
    }
}