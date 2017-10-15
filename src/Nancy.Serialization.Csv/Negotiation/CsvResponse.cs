using System;
using System.IO;

namespace Nancy.Serialization.Csv.Negotiation
{
	public class CsvResponse : CsvResponse<object>
	{
		public CsvResponse(object model, ISerializer serializer)
			: base(model, serializer)
		{
		}
	}

	public class CsvResponse<TModel> : Response
	{
		public CsvResponse()
		{
		}

		public CsvResponse(TModel model, ISerializer serializer)
		{
			if (serializer == null)
			{
				throw new ArgumentNullException(nameof(serializer));
			}

			this.Contents = GetContents(model, serializer);
			this.ContentType = MimeTypes.CanonicalMimeType;
			this.StatusCode = HttpStatusCode.OK;
		}

		private static Action<Stream> GetContents(TModel model, ISerializer serializer)
		{
			return stream => serializer.Serialize(MimeTypes.CanonicalMimeType, model, stream);
		}
	}
}
