using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace XamarinCore.Serialization
{
	public static class JsonHelper
	{
		public static string Serialize(object obj)
		{
			return JsonConvert.SerializeObject(obj);
		}

		public static string Serialize(Dictionary<string, string> parameters)
		{
			return JsonConvert.SerializeObject(parameters);
		}

		public static bool TryDeserialize<T>(string value, out T result)
		{
			try
			{
				result = JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings
				{
					DateFormatHandling = DateFormatHandling.IsoDateFormat
				});

				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Deserializing fail with data: {value}. Reason: {ex.Message}");

				result = default(T);
				return false;
			}
		}
		public static bool TryDeserialize<T>(string value, IList<JsonConverter> jsonConverters, out T result)
		{
			try
			{
				result = JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings
				{
					DateFormatHandling = DateFormatHandling.IsoDateFormat,
					Converters = jsonConverters
				});

				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Deserializing fail with data: {value}. Reason: {ex.Message}");

				result = default(T);
				return false;
			}
		}

		/// <summary>
		/// Deserializes the specified value.
		/// </summary>
		public static T Deserialize<T>(string value)
		{
			var result = default(T);

			try
			{
				result = JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings
				{
					DateFormatHandling = DateFormatHandling.IsoDateFormat
				});
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Deserializing fail with data: {value}. Reason: {ex.Message}");
			}

			return result;
		}
	}

	public class MergedDictConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var outputJsonArray = new JArray();

			foreach (var kp in ((Dictionary<string, string>)value).ToList())
			{
				var tempJson = JObject.FromObject(new Dictionary<string, string> { [kp.Key] = kp.Value });
				outputJsonArray.Add(tempJson);
			}
			outputJsonArray.WriteTo(writer);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
			JsonSerializer serializer)
		{
			var objArray = JArray.Load(reader);
			return objArray.Children()
				.Select(item => item.Children<JProperty>().First())
				.ToDictionary(prop => prop.Name, prop => (string)prop.Value);
		}

		public override bool CanConvert(Type objectType)
		{
			return true;
		}
	}
}
