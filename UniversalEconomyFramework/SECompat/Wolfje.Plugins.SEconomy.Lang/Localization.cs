using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Wolfje.Plugins.SEconomy.Lang
{
	public class Localization
	{
		public string Locale
		{
			get;
			protected set;
		}

		public string[] StringTable
		{
			get;
			protected set;
		}

		public Localization(string Locale)
		{
			this.Locale = Locale;
			Load();
			_ = 0;
		}

		public static void PrepareLanguages()
		{
			Regex regex = new Regex("\\$(.*)$");
			string text = string.Format("{1}{0}Lang{0}", Path.DirectorySeparatorChar, Config.BaseDirectory);
			Directory.CreateDirectory(text);
			string[] manifestResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
			foreach (string text2 in manifestResourceNames)
			{
				string text3 = null;
				string text4 = null;
				Match match = null;
				if (!text2.EndsWith(".xml") || !regex.IsMatch(text2) || (match = regex.Match(text2)) == null || (text4 = match.Groups[1].Value) == null)
				{
					continue;
				}
				text3 = string.Format("{0}{2}", text, Path.DirectorySeparatorChar, text4);
				try
				{
					using (StreamReader streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(text2)))
					{
						File.WriteAllText(text3, streamReader.ReadToEnd());
					}
				}
				catch (Exception)
				{
				}
			}
		}

		public string StringOrDefault(int index, string defaultText = "")
		{
			string text = null;
			if (StringTable == null || (text = StringTable.ElementAtOrDefault(index)) == null)
			{
				text = defaultText;
			}
			return text;
		}

		public int Load()
		{
			XDocument xDocument = null;
			IEnumerable<XElement> enumerable = null;
			int num = 0;
			string text = string.Format("{1}{0}Lang{0}{2}.xml", Path.DirectorySeparatorChar, Config.BaseDirectory, Locale);
			if (string.IsNullOrEmpty(Locale))
			{
				return -1;
			}
			if (!File.Exists(text))
			{
				return -1;
			}
			try
			{
				xDocument = XDocument.Load(text);
			}
			catch
			{
				return -1;
			}
			num = xDocument.Root.Elements().Count();
			StringTable = new string[num];
			enumerable = xDocument.Root.Elements("s");
			for (int i = 0; i < num; i++)
			{
				XElement xElement = null;
				if ((xElement = enumerable.ElementAtOrDefault(i)) == null)
				{
					return -1;
				}
				if ((StringTable[i] = xElement.Value) == null)
				{
					return -1;
				}
			}
			return 0;
		}
	}
}
