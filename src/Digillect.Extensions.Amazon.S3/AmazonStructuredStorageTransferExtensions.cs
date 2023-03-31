using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;
using JetBrains.Annotations;

namespace Digillect.Extensions.Amazon.S3;

[PublicAPI]
public static class AmazonStructuredStorageTransferExtensions
{
	public static Task CopyObjectAsync(
		this IAmazonStructuredStorage storage,
		[NotNull] string sourceKey,
		[NotNull] string targetKey,
		CancellationToken cancellationToken = default)
	{
		Check.NotEmpty(sourceKey, nameof(sourceKey));
		Check.NotEmpty(targetKey, nameof(targetKey));

		storage.Configuration.ThrowIfBucketNameIsNotSpecified();

		return storage.Client.CopyObjectAsync(
			storage.Configuration.BucketName, sourceKey,
			storage.Configuration.BucketName, targetKey,
			cancellationToken);
	}

	public static Task UploadObjectAsync(
		this IAmazonStructuredStorage storage,
		[NotNull] Stream stream,
		[NotNull] string key,
		[NotNull] string contentType,
		CancellationToken cancellationToken = default)
	{
		Check.NotEmpty(contentType, nameof(contentType));

		return storage.UploadObjectAsync(stream, key, request => request.ContentType = contentType, cancellationToken);
	}

	public static Task UploadObjectAsync(
		this IAmazonStructuredStorage storage,
		[NotNull] Stream stream,
		[NotNull] string key,
		[NotNull] string contentType,
		[NotNull] string fileName,
		CancellationToken cancellationToken = default)
	{
		return storage.UploadObjectAsync(stream, key, contentType, fileName, null, null, cancellationToken);
	}

	public static Task UploadObjectAsync(
		this IAmazonStructuredStorage storage,
		[NotNull] Stream stream,
		[NotNull] string key,
		[NotNull] string contentType,
		[NotNull] string fileName,
		[CanBeNull] S3CannedACL cannedAcl,
		[CanBeNull] IEnumerable<KeyValuePair<string, string>> metadata,
		CancellationToken cancellationToken = default)
	{
		Check.NotEmpty(contentType, nameof(contentType));
		Check.NotEmpty(fileName, nameof(fileName));

		return storage.UploadObjectAsync(stream, key, request => ConfigureUploadRequest(
			request,
			contentType,
			Path.GetFileName(fileName),
			cannedAcl,
			metadata), cancellationToken);
	}

	private static Task UploadObjectAsync(
		this IAmazonStructuredStorage storage,
		[NotNull] Stream stream,
		[NotNull] string key,
		[NotNull] Action<TransferUtilityUploadRequest> configureRequest,
		CancellationToken cancellationToken = default)
	{
		Check.NotNull(stream, nameof(stream));
		Check.NotEmpty(key, nameof(key));
		Check.NotNull(configureRequest, nameof(configureRequest));

		storage.Configuration.ThrowIfBucketNameIsNotSpecified();

		var utility = GetTransferUtility(storage);

		var request = new TransferUtilityUploadRequest {
			BucketName = storage.Configuration.BucketName,
			Key = key,
			InputStream = stream
		};

		configureRequest(request);

		return utility.UploadAsync(request, cancellationToken);
	}

	public static Task UploadObjectAsync(
		this IAmazonStructuredStorage storage,
		[NotNull] string filePath,
		[NotNull] string key,
		[NotNull] string contentType,
		CancellationToken cancellationToken = default)
	{
		Check.NotEmpty(contentType, nameof(contentType));

		return storage.UploadObjectAsync(filePath, key, request => request.ContentType = contentType, cancellationToken);
	}

	public static Task UploadObjectAsync(
		this IAmazonStructuredStorage storage,
		[NotNull] string filePath,
		[NotNull] string key,
		[NotNull] string contentType,
		string fileName = null,
		CancellationToken cancellationToken = default)
	{
		return storage.UploadObjectAsync(filePath, key, contentType, fileName, null, null, cancellationToken);
	}

	public static Task UploadObjectAsync(
		this IAmazonStructuredStorage storage,
		[NotNull] string filePath,
		[NotNull] string key,
		[NotNull] string contentType,
		[CanBeNull] string fileName,
		[CanBeNull] S3CannedACL cannedAcl,
		[CanBeNull] IEnumerable<KeyValuePair<string, string>> metadata,
		CancellationToken cancellationToken = default)
	{
		Check.NotEmpty(contentType, nameof(contentType));

		return storage.UploadObjectAsync(filePath, key, request => ConfigureUploadRequest(
			request,
			contentType,
			Path.GetFileName(fileName ?? filePath),
			cannedAcl,
			metadata), cancellationToken);
	}

	private static Task UploadObjectAsync(
		this IAmazonStructuredStorage storage,
		[NotNull] string filePath,
		[NotNull] string key,
		[NotNull] Action<TransferUtilityUploadRequest> configureRequest,
		CancellationToken cancellationToken = default)
	{
		Check.NotEmpty(filePath, nameof(filePath));
		Check.NotEmpty(key, nameof(key));
		Check.NotNull(configureRequest, nameof(configureRequest));

		storage.Configuration.ThrowIfBucketNameIsNotSpecified();

		var utility = GetTransferUtility(storage);

		var request = new TransferUtilityUploadRequest {
			BucketName = storage.Configuration.BucketName,
			Key = key,
			FilePath = filePath
		};

		configureRequest(request);

		return utility.UploadAsync(request, cancellationToken);
	}

	private static void ConfigureUploadRequest(
		[NotNull] TransferUtilityUploadRequest request,
		[NotNull] string contentType,
		[NotNull] string fileName,
		[CanBeNull] S3CannedACL cannedAcl,
		[CanBeNull] IEnumerable<KeyValuePair<string, string>> metadata)
	{
		request.CannedACL = cannedAcl;
		request.ContentType = contentType;
		request.Headers.ContentDisposition = new ContentDisposition { FileName = Uri.EscapeDataString(fileName) }.ToString();

		if (metadata != null)
		{
			foreach (var item in metadata)
			{
				request.Metadata[item.Key] = item.Value;
			}
		}
	}

	public static Task DownloadObjectAsync(
		this IAmazonStructuredStorage storage,
		[NotNull] string filePath,
		[NotNull] string key,
		CancellationToken cancellationToken = default)
	{
		return storage.DownloadObjectAsync(filePath, key, null, cancellationToken);
	}

	public static Task DownloadObjectAsync(
		this IAmazonStructuredStorage storage,
		[NotNull] string filePath,
		[NotNull] string key,
		Action<TransferUtilityDownloadRequest> configureRequest = null,
		CancellationToken cancellationToken = default)
	{
		Check.NotEmpty(filePath, nameof(filePath));
		Check.NotEmpty(key, nameof(key));

		storage.Configuration.ThrowIfBucketNameIsNotSpecified();

		var request = new TransferUtilityDownloadRequest {
			BucketName = storage.Configuration.BucketName,
			Key = key,
			FilePath = filePath
		};

		configureRequest?.Invoke(request);

		var utility = GetTransferUtility(storage);

		return utility.DownloadAsync(request, cancellationToken);
	}

	private static TransferUtility GetTransferUtility(IAmazonStructuredStorage storage)
	{
		return new TransferUtility(storage.Client,
			new TransferUtilityConfig { MinSizeBeforePartUpload = storage.Configuration.MinSizeBeforePartUpload * 1024 * 1024 });
	}
}
