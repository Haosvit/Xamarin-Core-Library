using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XamarinCore.Rest
{
	/// <summary>
	/// Client for restful service.
	/// </summary>
	/// <seealso cref="IDisposable" />
	public interface IRestClient : IDisposable
	{
		/// <summary>
		/// Call the GET method of specified URL with optional parameters.
		/// </summary>
		Task<ServiceResponse<TResult>> Get<TResult>(string url, Dictionary<string, string> parameters = null);

		/// <summary>
		/// Call the POST method of specified URL with parameters.
		/// </summary>
		Task<ServiceResponse<TResult>> Post<TResult>(string url, Dictionary<string, string> parameters,
			RestPostRequestBodyType type = RestPostRequestBodyType.FormUrlEncoded);

		/// <summary>
		/// Call the DELETE method of specified URL with optional parameters.
		/// </summary>
		Task<ServiceResponse<TResult>> Delete<TResult>(string url, Dictionary<string, string> parameters = null);
	}
}
