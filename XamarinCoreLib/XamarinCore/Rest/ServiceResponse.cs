using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using XamarinCore.Serialization;

namespace XamarinCore.Rest
{
	public class ServiceResponse
	{
		public string RawContent { get; set; }

		public ServiceError Error { get; set; }

		/// <summary>
		/// Prevents a default instance of the <see cref="ServiceResponse"/> class from being created.
		/// </summary>
		public ServiceResponse()
		{
			RawContent = string.Empty;

			Error = ServiceError.None;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceResponse"/> class.
		/// </summary>
		/// <param name="statusCode">The HTTP status code.</param>
		public ServiceResponse(HttpStatusCode statusCode)
			: this()
		{
			RawContent = string.Empty;

			Error = statusCode.ToServiceError();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceResponse"/> class.
		/// </summary>
		/// <param name="error">The error.</param>
		public ServiceResponse(ServiceError error)
			: this()
		{
			Error = error;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceResponse"/> class.
		/// </summary>
		/// <param name="rawContent">The raw content response from service.</param>
		public ServiceResponse(string rawContent)
			: this()
		{
			RawContent = rawContent;
		}
	}

	public sealed class ServiceResponse<TResult> : ServiceResponse
	{
		/// <summary>
		/// The result of service call.
		/// </summary>
		public TResult Result { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceResponse{TResult}"/> class.
		/// </summary>
		/// <param name="statusCodee">The HTTP status codee.</param>
		public ServiceResponse(HttpStatusCode statusCodee)
			: base(statusCodee)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceResponse{TResult}"/> class.
		/// </summary>
		/// <param name="error">The error.</param>
		public ServiceResponse(ServiceError error)
			: base(error)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceResponse{TResult}"/> class.
		/// </summary>
		/// <param name="result">The result of service call.</param>
		public ServiceResponse(TResult result)
		{
			Result = result;
		}

		/// <summary>
		/// Initializes a new instance with HTTP response.
		/// </summary>
		public ServiceResponse(string raw, ServiceResponseFormat format)
			: base(raw)
		{
			if (typeof(TResult) == typeof(string))
			{
				Result = (TResult)Convert.ChangeType(raw, typeof(TResult));
				Error = ServiceError.None;
				return;
			}

			TResult result;
			bool success;

			switch (format)
			{
				case ServiceResponseFormat.Json:
					success = JsonHelper.TryDeserialize(RawContent, out result);
					break;
				case ServiceResponseFormat.Xml:
					success = XmlHelper.TryDeserialize(RawContent, out result);
					break;
				default:
					throw new NotSupportedException($"Service response format {format} is not supported.");
			}

			Result = result;

			Error = success ? ServiceError.None : ServiceError.Undefined;
		}

		public ServiceResponse(string raw, ServiceResponseFormat format, JsonConverter tResultConverter) : base(raw)
		{
			if (typeof(TResult) == typeof(string))
			{
				Result = (TResult)Convert.ChangeType(raw, typeof(TResult));
				Error = ServiceError.None;
				return;
			}

			TResult result;
			bool success;

			switch (format)
			{
				case ServiceResponseFormat.Json:
					success = JsonHelper.TryDeserialize(RawContent, new List<JsonConverter>() { tResultConverter }, out result);
					break;
				case ServiceResponseFormat.Xml:
					success = XmlHelper.TryDeserialize(RawContent, out result);
					break;
				default:
					throw new NotSupportedException($"Service response format {format} is not supported.");
			}

			Result = result;

			Error = success ? ServiceError.None : ServiceError.Undefined;
		}

		public ServiceResponse ToGenericServiceResponse()
		{
			return new ServiceResponse
			{
				Error = Error,
				RawContent = RawContent
			};
		}
	}
}
