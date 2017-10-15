using Nancy.Responses.Negotiation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nancy.Serialization.Csv.Negotiation
{
	public class CsvProcessor : IResponseProcessor
	{
		private static readonly Tuple<string, MediaRange>[] extensionMappings;

		static CsvProcessor()
		{
			extensionMappings = new[] {
				Tuple.Create("csv", new MediaRange(MimeTypes.CanonicalMimeType)),
			};
		}

		private readonly ISerializer serializer;

		public CsvProcessor(IEnumerable<ISerializer> serializers)
		{
			this.serializer = serializers.FirstOrDefault(s => s.CanSerialize(MimeTypes.CanonicalMimeType));
		}

		public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
		{
			return new ProcessorMatch
			{
				ModelResult = MatchResult.DontCare,
				RequestedContentTypeResult = requestedMediaRange.Matches(MimeTypes.CanonicalMimeType) || requestedMediaRange.Matches("application/csv") ? MatchResult.ExactMatch : MatchResult.NoMatch,
			};
		}

		public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
		{
			return new CsvResponse(model, this.serializer);
		}

		public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
		{
			get { return extensionMappings; }
		}
	}
}
