using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3.Model;
using JetBrains.Annotations;

namespace Digillect.Extensions.Amazon.S3;

[PublicAPI]
public static class AmazonStructuredStorageDeleteExtensions
{
	public static Task DeleteObjectAsync(
		this IAmazonStructuredStorage storage,
		[NotNull] string key,
		CancellationToken cancellationToken = default)
	{
		Check.NotEmpty(key, nameof(key));

		storage.Configuration.ThrowIfBucketNameIsNotSpecified();

		return storage.Client.DeleteObjectAsync(storage.Configuration.BucketName, key, cancellationToken);
	}

	public static Task DeleteObjectsAsync(
		this IAmazonStructuredStorage storage,
		[NotNull] IEnumerable<string> keys,
		CancellationToken cancellationToken = default)
	{
		Check.NotNull(keys, nameof(keys));

		storage.Configuration.ThrowIfBucketNameIsNotSpecified();

		var request = new DeleteObjectsRequest {
			BucketName = storage.Configuration.BucketName,
			Objects = keys.Select(key => new KeyVersion { Key = key }).ToList()
		};

		return storage.Client.DeleteObjectsAsync(request, cancellationToken);
	}
}
