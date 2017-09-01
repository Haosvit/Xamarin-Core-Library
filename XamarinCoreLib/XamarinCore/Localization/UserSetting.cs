using System.Xml.Serialization;

namespace XamarinCore.Localization
{
	public class UserSetting
	{
		[XmlAttribute("language")]
		public string CurrentLanguage { get; set; }
	}
}
