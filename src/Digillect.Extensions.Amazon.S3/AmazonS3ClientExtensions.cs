using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Util;
using JetBrains.Annotations;

namespace Digillect.Extensions.Amazon.S3;

[PublicAPI]
public static class AmazonS3ClientExtensions
{
	public static Task<bool> BucketExistsAsync(this IAmazonS3 client, [NotNull] string bucketName)
	{
		Check.NotEmpty(bucketName, nameof(bucketName));

		return AmazonS3Util.DoesS3BucketExistV2Async(client, bucketName);
	}

	public static async Task<bool> ObjectExistsAsync(
		this IAmazonS3 client,
		[NotNull] string bucketName,
		[NotNull] string key,
		CancellationToken cancellationToken = default)
	{
		Check.NotEmpty(bucketName, nameof(bucketName));
		Check.NotEmpty(key, nameof(key));

		try
		{
			await client.GetObjectMetadataAsync(bucketName, key, cancellationToken);
		}
		catch (AmazonS3Exception ex)
		{
			if (ex.StatusCode == HttpStatusCode.NotFound)
			{
				return false;
			}

			throw;
		}

		return true;
	}
}
