using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Util;
using JetBrains.Annotations;

namespace Digillect.Extensions.Amazon.S3;

[PublicAPI]
public static class AmazonStructuredStorageExistenceExtensions
{
	public static Task<bool> BucketExistsAsync(this IAmazonStructuredStorage storage)
	{
		storage.Configuration.ThrowIfBucketNameIsNotSpecified();

		return AmazonS3Util.DoesS3BucketExistV2Async(storage.Client, storage.Configuration.BucketName);
	}

	public static async Task<bool> ObjectExistsAsync(
		this IAmazonStructuredStorage storage,
		[NotNull] string key,
		CancellationToken cancellationToken = default)
	{
		Check.NotEmpty(key, nameof(key));

		storage.Configuration.ThrowIfBucketNameIsNotSpecified();

		try
		{
			await storage.Client.GetObjectMetadataAsync(storage.Configuration.BucketName, key, cancellationToken);
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
