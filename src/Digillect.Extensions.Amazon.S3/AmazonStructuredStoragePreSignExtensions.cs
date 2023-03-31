using System;
using Amazon.S3;
using Amazon.S3.Model;
using JetBrains.Annotations;

namespace Digillect.Extensions.Amazon.S3;

[PublicAPI]
public static class AmazonStructuredStoragePreSignExtensions
{
	public static string PreSignRequest(
		this IAmazonStructuredStorage storage,
		[NotNull] string key,
		HttpVerb verb = HttpVerb.GET,
		Protocol? protocol = null,
		DateTime? expires = null)
	{
		Check.NotEmpty(key, nameof(key));

		storage.Configuration.ThrowIfBucketNameIsNotSpecified();

		var request = new GetPreSignedUrlRequest {
			BucketName = storage.Configuration.BucketName,
			Key = key,
			Verb = verb,
			Protocol = protocol ?? storage.Configuration.GetServiceProtocol(),
			Expires = expires ?? DateTime.Now.AddDays(1)
		};

		return storage.Client.GetPreSignedURL(request);
	}
}
