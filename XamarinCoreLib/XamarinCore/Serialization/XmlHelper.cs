using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace XamarinCore.Serialization
{
	public static class XmlHelper
	{
		public static string Serialize(object obj)
		{
			using (var write = new StringWriter())
			{
				var serializer = new XmlSerializer(obj.GetType());

				serializer.Serialize(write, obj);

				return write.ToString();
			}
		}

		public static bool TryDeserialize<T>(string value, out T result)
		{
			result = default(T);

			try
			{
				using (var reader = new StringReader(value))
				{
					var serializer = new XmlSerializer(typeof(T));

					result = (T)serializer.Deserialize(reader);
				}

				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Deserializing fail with data: {value}. Reason: {ex.Message}");

				return false;
			}
		}

		public static T Deserialize<T>(string value)
		{
			try
			{
				using (var reader = new StringReader(value))
				{
					var serializer = new XmlSerializer(typeof(T));

					return (T)serializer.Deserialize(reader);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Deserializing fail with data: {value}. Reason: {ex.Message}");

				return default(T);
			}
		}
	}
}
