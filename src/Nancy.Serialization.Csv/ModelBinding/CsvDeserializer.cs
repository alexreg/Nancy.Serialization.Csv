using CsvHelper;
using Nancy.ModelBinding;
using System;
using System.IO;
using System.Linq;
using System.Net.Mime;

namespace Nancy.Serialization.Csv.ModelBinding
{
	public class CsvDeserializer : IBodyDeserializer
	{
		public CsvDeserializer()
		{
		}

		public bool CanDeserialize(string contentType, BindingContext context)
		{
			if (string.IsNullOrEmpty(contentType))
			{
				return false;
			}

			var mimeType = new ContentType(contentType);
			return
				mimeType.MediaType.Equals(MimeTypes.CanonicalMimeType, StringComparison.InvariantCultureIgnoreCase) ||
				mimeType.MediaType.Equals("application/csv", StringComparison.InvariantCultureIgnoreCase) ||
				(mimeType.MediaType.StartsWith("application/vnd", StringComparison.InvariantCultureIgnoreCase) && mimeType.MediaType.EndsWith("+csv", StringComparison.InvariantCultureIgnoreCase));
		}

		public object Deserialize(string contentType, Stream bodyStream, BindingContext context)
		{
			var destinationType = context.DestinationType;
			Type elementType = null;
			var hasConstructor = false;
			var implementsCollection = false;
			var singleElement = false;
			if (destinationType.IsArray)
			{
				elementType = destinationType.BaseType;
			}
			else if ((elementType = destinationType.GetCollectionElementTypeFromConstructor()) != null)
			{
				hasConstructor = true;
			}
			else if ((elementType = destinationType.GetCollectionElementTypeFromInterface()) != null)
			{
				implementsCollection = true;
			}
			else
			{
				elementType = destinationType;
				singleElement = true;
			}

			using (var bodyReader = new StreamReader(bodyStream))
			using (var csvReader = new CsvReader(bodyReader))
			{
				object model = null;
				if (singleElement)
				{
					model = csvReader.GetRecord(elementType);
				}
				else
				{
					var records = csvReader.GetRecords(elementType);
					if (destinationType.IsArray)
					{
						model = records.ToArray();
					}
					else if (hasConstructor)
					{
						model = Activator.CreateInstance(destinationType, records);
					}
					else if (implementsCollection)
					{
						model = Activator.CreateInstance(destinationType);
						var addMethod = destinationType.GetMethod("Add", new[] { elementType });
						foreach (var curRecord in records)
						{
							addMethod.Invoke(model, new[] { curRecord });
						}
					}

					if (model != null)
					{
						context.Configuration.BodyOnly = true;
					}
					return model;
				}
			}

			return null;
		}
	}
}
