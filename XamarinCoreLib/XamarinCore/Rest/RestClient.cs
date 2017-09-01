using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using XamarinCore.Serialization;

namespace XamarinCore.Rest
{
	/// <summary>
	/// Client for restful service.
	/// </summary>
	/// <seealso cref="System.Net.Http.HttpClient" />
	/// <seealso cref="IRestClient" />
	public class RestClient : HttpClient, IRestClient
	{
		private const string Scheme = "Basic";

		private const string AuthorizationFormat = "{0}:{1}";

		private RestClient()
		{
		}

		public static IRestClient Create(string username, string password, string applicationToken)
		{
			var client = new RestClient();

			var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format(AuthorizationFormat, username, password)));

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Scheme, auth);
			client.DefaultRequestHeaders.Add("Application", applicationToken);

			client.Timeout = TimeSpan.FromSeconds(10);

			return client;
		}
		
		/// <summary>
		/// Call the GET method of specified URL with optional parameters.
		/// </summary>
		public async Task<ServiceResponse<TResult>> Get<TResult>(string url, Dictionary<string, string> parameters = null)
		{
			try
			{
				BaseAddress = new Uri(url);

				var request = parameters != null
					? $"?{string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"))}"
					: string.Empty;

				using (var response = await GetAsync(request).ConfigureAwait(false))
				{
					var raw = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

					if (!response.IsSuccessStatusCode)
					{
						return new ServiceResponse<TResult>(response.StatusCode);
					}

					return new ServiceResponse<TResult>(raw, ServiceResponseFormat.Json);
				}
			}
			catch (OperationCanceledException)
			{
				return new ServiceResponse<TResult>(ServiceError.Timeout);
			}
			catch (UnauthorizedAccessException)
			{
				return new ServiceResponse<TResult>(ServiceError.Unauthorized);
			}
			catch (Exception)
			{
				return new ServiceResponse<TResult>(ServiceError.Undefined);
			}
		}

		/// <summary>
		/// Call the POST method of specified URL with optional parameters.
		/// </summary>
		public async Task<ServiceResponse<TResult>> Post<TResult>(string url, Dictionary<string, string> parameters, RestPostRequestBodyType type = RestPostRequestBodyType.FormUrlEncoded)
		{
			try
			{
				BaseAddress = new Uri(url);

				HttpContent content;

				switch (type)
				{
					case RestPostRequestBodyType.FormUrlEncoded:
						content = new FormUrlEncodedContent(parameters);
						break;
					case RestPostRequestBodyType.Json:
						content = new StringContent(JsonHelper.Serialize(parameters), Encoding.UTF8, "application/json");
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(type), type, null);
				}
				
				using (var response = await PostAsync(url, content).ConfigureAwait(false))
				{
					var raw = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

					if (!response.IsSuccessStatusCode)
					{
						return new ServiceResponse<TResult>(response.StatusCode);
					}

					return new ServiceResponse<TResult>(raw, ServiceResponseFormat.Json);
				}
			}
			catch (OperationCanceledException)
			{
				return new ServiceResponse<TResult>(ServiceError.Timeout);
			}
			catch (UnauthorizedAccessException)
			{
				return new ServiceResponse<TResult>(ServiceError.Unauthorized);
			}
			catch (Exception)
			{
				return new ServiceResponse<TResult>(ServiceError.Undefined);
			}
		}

		/// <summary>
		/// Call the DELETE method of specified URL with optional parameters.
		/// </summary>
		public async Task<ServiceResponse<TResult>> Delete<TResult>(string url, Dictionary<string, string> parameters = null)
		{
			try
			{
				BaseAddress = new Uri(url);

				var request = parameters != null
					? $"?{string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}"))}"
					: string.Empty;

				using (var response = await DeleteAsync(request).ConfigureAwait(false))
				{
					var raw = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

					if (!response.IsSuccessStatusCode)
					{
						return new ServiceResponse<TResult>(response.StatusCode);
					}

					return new ServiceResponse<TResult>(raw, ServiceResponseFormat.Json);
				}
			}
			catch (OperationCanceledException)
			{
				return new ServiceResponse<TResult>(ServiceError.Timeout);
			}
			catch (UnauthorizedAccessException)
			{
				return new ServiceResponse<TResult>(ServiceError.Unauthorized);
			}
			catch (Exception)
			{
				return new ServiceResponse<TResult>(ServiceError.Undefined);
			}
		}
	}
}
