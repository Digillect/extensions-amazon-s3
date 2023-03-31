using Amazon.S3;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace Digillect.Extensions.Amazon.S3;

[UsedImplicitly]
internal class AmazonStructuredStorage : IAmazonStructuredStorage
{
	public AmazonStructuredStorage(IAmazonS3 s3Client, IOptions<AmazonStructuredStorageConfiguration> configuration)
	{
		Configuration = configuration.Value;
		Client = s3Client;
	}

	public AmazonStructuredStorageConfiguration Configuration { get; }
	public IAmazonS3 Client { get; }
}
