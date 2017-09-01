using System;
using System.Net;

namespace XamarinCore.Rest
{
	public enum ServiceError
	{
		/// <summary>
		/// No error
		/// </summary>
		None = 0,

		/// <summary>
		/// General error
		/// </summary>
		Undefined = 1,

		/// <summary>
		/// App has no internet when calling service
		/// </summary>
		NoInternet,

		/// <summary>
		/// Service/Client doesn't receive request/response in expected time
		/// </summary>
		Timeout,

		/// <summary>
		/// Client has no permission to call service
		/// </summary>
		Unauthorized,

		/// <summary>
		/// The called service is temporary unavailable
		/// </summary>
		ServiceUnavailable,

		/// <summary>
		/// Error occurs at server side
		/// </summary>
		ServerError,

		/// <summary>
		/// The requested object could not be found
		/// </summary>
		NotFound,

		/// <summary>
		/// The property of device with the passed ID could not be found
		/// </summary>
		PropertyNotKnown,

		/// <summary>
		/// The value is not supported for this property
		/// </summary>
		ValueNotSupported,

		/// <summary>
		/// Some properties cannot be controlled to change their values
		/// </summary>
		InvalidProperties
	}

	public static class ServiceErrorExtension
	{
		public static ServiceError ToServiceError(this HttpStatusCode statusCode)
		{
			switch (statusCode)
			{
				case HttpStatusCode.Accepted:
				case HttpStatusCode.Ambiguous:
				case HttpStatusCode.Continue:
				case HttpStatusCode.Created:
				case HttpStatusCode.Found:
				case HttpStatusCode.Moved:
				case HttpStatusCode.NoContent:
				case HttpStatusCode.OK:
				case HttpStatusCode.RedirectKeepVerb:
				case HttpStatusCode.RedirectMethod:
					return ServiceError.None;

				case HttpStatusCode.BadRequest:
				case HttpStatusCode.ExpectationFailed:
				case HttpStatusCode.Gone:
				case HttpStatusCode.LengthRequired:
				case HttpStatusCode.MethodNotAllowed:
				case HttpStatusCode.NonAuthoritativeInformation:
				case HttpStatusCode.NotAcceptable:
				case HttpStatusCode.NotModified:
				case HttpStatusCode.PartialContent:
				case HttpStatusCode.PaymentRequired:
				case HttpStatusCode.PreconditionFailed:
				case HttpStatusCode.RequestedRangeNotSatisfiable:
				case HttpStatusCode.RequestEntityTooLarge:
				case HttpStatusCode.RequestUriTooLong:
				case HttpStatusCode.ResetContent:
				case HttpStatusCode.SwitchingProtocols:
				case HttpStatusCode.UnsupportedMediaType:
				case HttpStatusCode.Unused:
				case HttpStatusCode.UpgradeRequired:
				case HttpStatusCode.UseProxy:
					return ServiceError.Undefined;

				case HttpStatusCode.RequestTimeout:
					return ServiceError.Timeout;

				case HttpStatusCode.Unauthorized:
				case HttpStatusCode.Forbidden:
				case HttpStatusCode.ProxyAuthenticationRequired:
					return ServiceError.Unauthorized;

				case HttpStatusCode.BadGateway:
				case HttpStatusCode.ServiceUnavailable:
				case HttpStatusCode.GatewayTimeout:
				case HttpStatusCode.NotFound:
					return ServiceError.ServiceUnavailable;

				case HttpStatusCode.Conflict:
				case HttpStatusCode.InternalServerError:
				case HttpStatusCode.NotImplemented:
				case HttpStatusCode.HttpVersionNotSupported:
					return ServiceError.ServerError;

				default:
					throw new NotSupportedException($"Http status code {statusCode} is not supported yet.");
			}
		}
	}
}
