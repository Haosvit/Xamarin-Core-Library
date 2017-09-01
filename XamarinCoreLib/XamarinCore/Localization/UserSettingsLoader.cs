using MvvmCross.Platform.Platform;
using MvvmCross.Plugins.File;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;
using XamarinCore.Serialization;

namespace XamarinCore.Localization
{
	public class UserSettingsLoader<T> where T : UserSetting
	{
		public string CurrentLanguage { get; private set; }

		private readonly IMvxFileStore _fileStore;
		private readonly IMvxResourceLoader _resourceLoader;
		private string _filePath;

		private string _settingFile;
		private string _settingFolder;

		public T UserSetting
		{
			get
			{
				string content;
				var result = default(T);

				if (_fileStore.TryReadTextFile(_filePath, out content))
				{
					if (!string.IsNullOrWhiteSpace(content))
					{
						result = JsonHelper.Deserialize<T>(content);
					}
				}

				return result;
			}
		}

		public UserSettingsLoader(IMvxFileStore fileStore, IMvxResourceLoader resourceLoader)
		{
			_resourceLoader = resourceLoader;
			_fileStore = fileStore;
		}

		public void UpdateLanguage(string locale)
		{
			CurrentLanguage = locale;
			var setting = UserSetting;
			setting.CurrentLanguage = locale;
			WriteFile(setting);
		}

		private void WriteFile(T setting)
		{
			_fileStore.WriteFile(_filePath, JsonConvert.SerializeObject(setting));
		}

		public void Initialize(string settingFolder, string settingFile)
		{
			if (string.IsNullOrEmpty(settingFolder) || string.IsNullOrEmpty(settingFile))
			{
				Debug.WriteLine("Couldn't init empty setting folder and setting file name");
				return;
			}

			_settingFolder = settingFolder;
			_settingFile = settingFile;
			 _filePath = _fileStore.PathCombine(_settingFolder, _settingFile);
			_fileStore.EnsureFolderExists(_settingFolder);

			if (!_fileStore.Exists(_settingFile))
			{
				return;
			}

			if (!_fileStore.TryReadBinaryFile(_settingFile, LoadFrom))
			{
				_resourceLoader.GetResourceStream(_settingFile, inputStream => LoadFrom(inputStream));
			}
		}

		private bool LoadFrom(Stream inputStream)
		{
			var loadedData = XDocument.Load(inputStream);

			if (loadedData.Root == null)
			{
				return false;
			}

			using (var reader = loadedData.Root.CreateReader())
			{
				var setting = (T)new XmlSerializer(typeof(T)).Deserialize(reader);
				CurrentLanguage = setting.CurrentLanguage;
				return true;
			}
		}
	}
}
