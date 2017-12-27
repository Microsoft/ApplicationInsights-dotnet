﻿using System;

namespace Microsoft.ApplicationInsights.Metrics.ConcurrentDatastructures
{
    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1008: Enums should have zero value",
            Justification = "Crafted these flags to fit into a byte to make the struct container cheaper.")]
    [Flags]
    internal enum MultidimensionalPointResultCodes : byte
    {
        /// <summary>
        /// 
        /// </summary>
        Success_NewPointCreated = 1,

        /// <summary>
        /// 
        /// </summary>
        Success_ExistingPointRetrieved = 2,

        /// <summary>
        /// 
        /// </summary>
        Failure_SubdimensionsCountLimitReached = 8,

        /// <summary>
        /// 
        /// </summary>
        Failure_TotalPointsCountLimitReached = 16,

        /// <summary>
        /// 
        /// </summary>
        Failure_PointDoesNotExistCreationNotRequested = 32,

        /// <summary>
        /// 
        /// </summary>
        Failure_AsyncTimeoutReached = 128,
    }
}
