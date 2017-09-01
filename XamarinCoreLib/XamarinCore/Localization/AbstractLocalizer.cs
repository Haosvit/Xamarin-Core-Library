using MvvmCross.Platform.Platform;
using MvvmCross.Plugins.File;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace XamarinCore.Localization
{

	/// <summary>
	///  The localization static class
	/// </summary>
	public abstract class AbstractLocalizer<T> where T : UserSetting
	{
		// The filenames now contain the same ISO 639-1 two letter code as the CurrentCulture and as the languages
		// codes used in our application, so this can be made smarter without repetition of the full paths
		protected abstract string Assembly { get; }
		protected abstract string LanguageFileNameFormat { get; }
		protected abstract string DefaultLanguageFileName { get; }
		protected abstract string SettingFolder { get; }
		protected abstract string SettingFile { get; }

		private static string _keyPrefix = ""; // if keyPrefix is not empty, add this prefix to each key that is looked up
		private static string _defaultLanguage = "";
		private static string _currentLanguage = "";
		private static Dictionary<string, string> _dictionary = new Dictionary<string, string>();
		public static UserSettingsLoader<T> UserSettingsLoader;

		public static string GetCurrentLanguage()
		{
			return _currentLanguage;
		}

		public void ChangeLanguage(string isoCode)
		{
			if (isoCode != _currentLanguage)
			{
				UserSettingsLoader?.UpdateLanguage(isoCode);
				LoadResources();
			}
		}

		/// <summary>
		///     Setting language files will be loaded from disk or from embedded resource, then load resources
		/// </summary>
		/// <param name="mvxFileStore"></param>
		/// <param name="resourceLoader"></param>
		/// <param name="deviceType"></param>
		/// <param name="defaultLanguage">
		///     When not null and not empty, use this as the default languagecode instead of the current
		///     CultureInfo
		/// </param>
		public void Initialize(IMvxFileStore mvxFileStore, IMvxResourceLoader resourceLoader, DeviceSizeType deviceType = DeviceSizeType.Phone, string defaultLanguage = "")
		{
			_defaultLanguage = defaultLanguage;

			UserSettingsLoader = new UserSettingsLoader<T>(mvxFileStore, resourceLoader);
			UserSettingsLoader.Initialize(SettingFolder, SettingFile);

			// We need type of device because sometime, the text of Phone and Tablet is difference
			switch (deviceType)
			{
				case DeviceSizeType.Phone:
					{
						_keyPrefix = "";
						break;
					}
				case DeviceSizeType.Tablet:
					{
						_keyPrefix = "Tablet#";
						break;
					}
				default:
					{
						throw new ArgumentOutOfRangeException();
					}
			}

			LoadResources();
		}

		/// <summary>
		///     Gets the text. First try with device specific prefix, then without prefix, final fallback is empty string.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public static string GetText(string key)
		{
			// First try to lookup the key including prefix (if prefix configured)
			if (!string.IsNullOrEmpty(_keyPrefix))
			{
				var lookupKey = _keyPrefix + key;
				if (_dictionary.ContainsKey(lookupKey))
				{
					return _dictionary[lookupKey];
				}
			}
			return _dictionary.ContainsKey(key) ? _dictionary[key] : string.Empty;
		}

		#region Load Resources

		/// <summary>
		///     Reads the resource file.
		/// </summary>
		public void LoadResources()
		{
			// Set current language
			_currentLanguage = string.IsNullOrEmpty(UserSettingsLoader.CurrentLanguage)
				? DefaultLanguage
				: UserSettingsLoader.CurrentLanguage;

			// Load all localization keys from corresponding .properties resource file
			var fileName = String.Format(LanguageFileNameFormat, _currentLanguage);
			LoadLocalizationKeys(fileName);
		}

		/// <summary>
		///     Reads the specific resource file.
		/// </summary>
		public string LoadSpecificFileResources(string fileName)
		{
			return ReadResourceFile(fileName);
		}

		private static string DefaultLanguage
		{
			get
			{
				// if available, use the configured default language, otherwise use the current culture
				if (!string.IsNullOrEmpty(_defaultLanguage))
				{
					return _defaultLanguage;
				}
				//https://forums.xamarin.com/discussion/31918/cross-platform-localization-best-practices
				return CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
			}
		}

		private void LoadLocalizationKeys(string fileName)
		{
			_dictionary = new Dictionary<string, string>();

			var localizationResources = ReadResourceFile(fileName);

			using (var reader = new StringReader(localizationResources))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					// # indicates comment line
					if (!(line.StartsWith("#") || string.IsNullOrWhiteSpace(line)))
					{
						// each key value pair: key = value
						var i = line.IndexOf('=');
						if (i >= 0)
						{
							AddKeyAndValue(line, i);
						}
						else
						{
							// In release mode, just ignore this line
							Debug.WriteLine("Line is not a comment or key value pair in file '{0}': '{1}'", fileName, line);
						}
					}
				}
			}
		}

		private static void AddKeyAndValue(string line, int i)
		{
			var key = line.Substring(0, i).Trim();
			if (!string.IsNullOrWhiteSpace(key) && !key.Contains(" "))
			{
				var value = line.Substring(i + 1).Trim().Replace("\\n", "\n");
				_dictionary.Add(key, value);
			}
			else
			{
				Debug.WriteLine("Key '{0}' is not a valid key", key);
			}
		}

		private string ReadResourceFile(string fileName)
		{
			var assembly = typeof(AbstractLocalizer<T>).GetTypeInfo().Assembly;
			var filePath = Assembly + fileName;
			try
			{
				using (var stream = assembly.GetManifestResourceStream(filePath))
				{
					if (stream != null)
					{
						using (var reader = new StreamReader(stream))
						{
							return reader.ReadToEnd();
						}
					}
					return ReadResourceFile(DefaultLanguageFileName);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			return string.Empty;
		}

		#endregion
	}
}
