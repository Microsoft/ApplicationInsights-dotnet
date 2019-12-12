﻿namespace Microsoft.ApplicationInsights.Web
{
    using System;
    using System.Web;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Web.Implementation;

    /// <summary>
    /// A telemetry initializer that will set the User properties of Context corresponding to a RequestTelemetry object.
    /// User.AuthenticatedUserId is updated with properties derived from the RequestTelemetry.RequestTelemetry.Context.User.
    /// </summary>
    public class AuthenticatedUserIdTelemetryInitializer : WebTelemetryInitializerBase
    {
        internal static void GetAuthUserContextFromUserCookie(HttpCookie authUserCookie, RequestTelemetry requestTelemetry)
        {
            if (authUserCookie == null)
            {
                // Request does not have authenticated user
                WebEventSource.Log.AuthIdTrackingCookieNotAvailable();
                return;
            }

            if (string.IsNullOrEmpty(authUserCookie.Value))
            {
                // Request does not have authenticated user
                WebEventSource.Log.AuthIdTrackingCookieIsEmpty();
                return;
            }

            var authUserCookieString = HttpUtility.UrlDecode(authUserCookie.Value);

            var cookieParts = authUserCookieString.Split('|');

            if (cookieParts.Length > 0)
            {
                requestTelemetry.Context.User.AuthenticatedUserId = cookieParts[0];
            }
        }

        /// <summary>
        /// Implements initialization logic.
        /// </summary>
        /// <param name="platformContext">Http context.</param>
        /// <param name="requestTelemetry">Request telemetry object associated with the current request.</param>
        /// <param name="telemetry">Telemetry item to initialize.</param>
        protected override void OnInitializeTelemetry(HttpContext platformContext, RequestTelemetry requestTelemetry, ITelemetry telemetry)
        {
            if (telemetry == null)
            {
                throw new ArgumentNullException(nameof(telemetry));
            }

            if (string.IsNullOrEmpty(telemetry.Context.User.AuthenticatedUserId))
            {
                if (string.IsNullOrEmpty(requestTelemetry.Context.User.AuthenticatedUserId))
                {
                    GetAuthUserContextFromUserCookie(platformContext.Request.UnvalidatedGetCookie(RequestTrackingConstants.WebAuthenticatedUserCookieName), requestTelemetry);
                }

                telemetry.Context.User.AuthenticatedUserId = requestTelemetry.Context.User.AuthenticatedUserId;
            }
        }
    }
}
