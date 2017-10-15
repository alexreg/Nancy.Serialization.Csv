using CsvHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Nancy.Serialization.Csv.Negotiation
{
	public class CsvSerializer : ISerializer
	{
		public CsvSerializer()
		{
		}

		public bool CanSerialize(string contentType)
		{
			return contentType == MimeTypes.CanonicalMimeType;
		}

		public void Serialize<TModel>(string contentType, TModel model, Stream outputStream)
		{
			var elementType = model.GetType().GetEnumeraleElementTypeFromInterface();
			if (elementType == null)
			{
				throw new ArgumentException("Model is not an enumerable object.", nameof(model));
			}

			using (var textWriter = new StreamWriter(outputStream))
			using (var csvWriter = new CsvWriter(textWriter))
			{
				csvWriter.WriteHeader(elementType);
				csvWriter.WriteRecords(model as IEnumerable);
			}
		}

		public IEnumerable<string> Extensions
		{
			get { yield break; }
		}
	}
}
