using System;
using Amazon;
using Amazon.S3;
using JetBrains.Annotations;

namespace Digillect.Extensions.Amazon.S3;

[PublicAPI]
public class AmazonStructuredStorageConfiguration
{
	public string BucketName { get; set; }

	public string Region { get; set; } = "us-east-1";
	public string ServiceUrl { get; set; }
	public bool ForcePathStyle { get; set; }

	public string Profile { get; set; }
	public string ProfilesLocation { get; set; }

	public string AccessKeyId { get; set; }
	public string SecretAccessKey { get; set; }

	public int MinSizeBeforePartUpload { get; set; } = 5;

	public ResponseLoggingOption LogResponses { get; set; } = ResponseLoggingOption.Never;

	public Uri GetObjectUri([NotNull] string path)
	{
		Check.NotEmpty(path, nameof(path));

		var uri = GetServiceUri();

		return new Uri(uri, path);
	}

	public string GetObjectPath([NotNull] string path)
	{
		return GetObjectUri(path).AbsoluteUri;
	}

	public Protocol GetServiceProtocol()
	{
		var uri = GetServiceUri();

		return uri.Scheme == Uri.UriSchemeHttp ? Protocol.HTTP : Protocol.HTTPS;
	}

	public Uri GetServiceUri()
	{
		ThrowIfBucketNameIsNotSpecified();

		string serviceUrl;
		string host, scheme;

		if (!string.IsNullOrEmpty(ServiceUrl))
		{
			var uri = new Uri(ServiceUrl);

			host = uri.IsDefaultPort ? uri.Host : $"{uri.Host}:{uri.Port}";
			scheme = uri.Scheme;
		}
		else
		{
			host = "s3." + (Region is null or "us-east-1" ? "" : Region + ".") + "amazonaws.com";
			scheme = Uri.UriSchemeHttp;
		}

		serviceUrl = ForcePathStyle
			? $"{scheme}://{host}/{BucketName}/"
			: $"{scheme}://{BucketName}.{host}/";

		return new Uri(serviceUrl);
	}

	public void ThrowIfBucketNameIsNotSpecified()
	{
		if (string.IsNullOrEmpty(BucketName))
		{
			throw new AmazonStructuredStorageConfigurationException("Bucket name is not specified");
		}
	}
}
