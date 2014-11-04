using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

// EXPECTS XML IN THIS FORMAT
//
//<?xml version="1.0"?>
//<TokenValues xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
//  <Sections>
//    <Section Name="system">
//      <Tokens>
//        <Token Key="productName" Value="Epsilon" />
//      </Tokens>
//    </Section>
//  </Sections>
//</TokenValues>

namespace KeithLink.Svc.Impl.Component
{
	class TokenValuesCollection
	{
		Section rootSection = null;

		public string this[string token]
		{
			get
			{
				if (this.rootSection == null)
					throw new Exception("Token values collection root section is not initialized");

				if (string.IsNullOrEmpty(token))
					throw new ArgumentException("The token cannot be null", "token");

				string[] path = token.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

				Section currentSection = this.rootSection;

				for (int i = 0; i < path.Length; i++)
				{
					if (i == path.Length - 1) // find token
					{
						Token targetToken = currentSection.Tokens.FirstOrDefault(t => t.Key.Equals(path[i], StringComparison.InvariantCultureIgnoreCase));
						if (targetToken == null)
							return null;
						else
							return targetToken.Value;
					}
					else // find section
					{
						currentSection = currentSection.Sections.FirstOrDefault(s => s.Name.Equals(path[i], StringComparison.InvariantCultureIgnoreCase));
					}

					if (currentSection == null) // section in path did not exist
						break;
				}

				return null;
			}
		}

		public TokenValuesCollection(string tokenValuesDataPath)
		{
			if (string.IsNullOrEmpty(tokenValuesDataPath))
				throw new ArgumentException("The path of the token values data file can not be null", "tokenValuesDataPath");

			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(Section));
				using (Stream dataStream = File.OpenRead(tokenValuesDataPath))
					this.rootSection = (Section)serializer.Deserialize(dataStream);
			}
			catch (Exception ex)
			{
				throw new Exception("Could not load token value data", ex);
			}
		}
	}

	// models

	[XmlRoot("TokenValues")]
	public class Section
	{
		[XmlAttribute("Name")]
		public string Name = string.Empty;

		[XmlArray("Sections")]
		public List<Section> Sections;

		[XmlArray("Tokens")]
		public List<Token> Tokens;
	}

	public class Token
	{
		[XmlAttribute("Key")]
		public string Key = string.Empty;

		[XmlAttribute("Value")]
		public string Value;
	}
}

