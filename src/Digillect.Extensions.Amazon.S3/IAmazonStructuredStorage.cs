using Amazon.S3;
using JetBrains.Annotations;

namespace Digillect.Extensions.Amazon.S3;

[PublicAPI]
public interface IAmazonStructuredStorage
{
	AmazonStructuredStorageConfiguration Configuration { get; }
	IAmazonS3 Client { get; }
}
