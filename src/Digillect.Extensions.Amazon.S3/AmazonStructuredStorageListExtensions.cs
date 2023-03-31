using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3.Model;
using JetBrains.Annotations;

namespace Digillect.Extensions.Amazon.S3;

[PublicAPI]
public static class AmazonStructuredStorageListExtensions
{
	public static Task<IEnumerable<S3Object>> ListObjectsAsync(
		this IAmazonStructuredStorage storage,
		CancellationToken cancellationToken = default)
	{
		return storage.ListObjectsAsync((string) null, null, cancellationToken);
	}

	public static async Task<IEnumerable<S3Object>> ListObjectsAsync(
		this IAmazonStructuredStorage storage,
		string prefix = null,
		string delimiter = null,
		CancellationToken cancellationToken = default)
	{
		var result = new List<S3Object>();

		await storage.ListObjectsAsync(obj => result.Add(obj), prefix, delimiter, cancellationToken);

		return result;
	}

	public static Task ListObjectsAsync(
		this IAmazonStructuredStorage storage,
		Action<S3Object> consumer,
		string prefix = null,
		string delimiter = null,
		CancellationToken cancellationToken = default
	)
	{
		return storage.ListObjectsAsync(consumer, request => {
			request.Prefix = prefix;
			request.Delimiter = delimiter;
		}, cancellationToken);
	}

	public static Task ListObjectsAsync(
		this IAmazonStructuredStorage storage,
		Func<S3Object, bool> consumer,
		string prefix = null,
		string delimiter = null,
		CancellationToken cancellationToken = default
	)
	{
		return storage.ListObjectsAsync(consumer, request => {
			request.Prefix = prefix;
			request.Delimiter = delimiter;
		}, cancellationToken);
	}

	public static Task ListObjectsAsync(
		this IAmazonStructuredStorage storage,
		Func<S3Object, Task<bool>> consumer,
		string prefix = null,
		string delimiter = null,
		CancellationToken cancellationToken = default
	)
	{
		return storage.ListObjectsAsync(consumer, request => {
			request.Prefix = prefix;
			request.Delimiter = delimiter;
		}, cancellationToken);
	}

	public static Task ListObjectsAsync(
		this IAmazonStructuredStorage storage,
		Func<S3Object, Task> consumer,
		string prefix = null,
		string delimiter = null,
		CancellationToken cancellationToken = default
	)
	{
		return storage.ListObjectsAsync(consumer, request => {
			request.Prefix = prefix;
			request.Delimiter = delimiter;
		}, cancellationToken);
	}

	public static async Task ListObjectsAsync(
		this IAmazonStructuredStorage storage,
		Func<S3Object, bool> consumer,
		Action<ListObjectsV2Request> requestConfigurator = null,
		CancellationToken cancellationToken = default)
	{
		Check.NotNull(consumer, nameof(consumer));

		storage.Configuration.ThrowIfBucketNameIsNotSpecified();

		var request = new ListObjectsV2Request {
			BucketName = storage.Configuration.BucketName,
			MaxKeys = 256
		};

		requestConfigurator?.Invoke(request);

		ListObjectsV2Response response;

		do
		{
			response = await storage.Client.ListObjectsV2Async(request, cancellationToken);

			foreach (var entry in response.S3Objects)
			{
				if (!consumer(entry))
				{
					return;
				}
			}

			request.ContinuationToken = response.ContinuationToken;
		} while (response.IsTruncated);
	}

	public static async Task ListObjectsAsync(
		this IAmazonStructuredStorage storage,
		Func<S3Object, Task<bool>> consumer,
		Action<ListObjectsV2Request> requestConfigurator = null,
		CancellationToken cancellationToken = default)
	{
		Check.NotNull(consumer, nameof(consumer));

		storage.Configuration.ThrowIfBucketNameIsNotSpecified();

		var request = new ListObjectsV2Request {
			BucketName = storage.Configuration.BucketName,
			MaxKeys = 256
		};

		requestConfigurator?.Invoke(request);

		ListObjectsV2Response response;

		do
		{
			response = await storage.Client.ListObjectsV2Async(request, cancellationToken);

			foreach (var entry in response.S3Objects)
			{
				if (!await consumer(entry))
				{
					return;
				}
			}

			request.ContinuationToken = response.ContinuationToken;
		} while (response.IsTruncated);
	}

	public static async Task ListObjectsAsync(
		this IAmazonStructuredStorage storage,
		Func<S3Object, Task> consumer,
		Action<ListObjectsV2Request> requestConfigurator = null,
		CancellationToken cancellationToken = default)
	{
		Check.NotNull(consumer, nameof(consumer));

		storage.Configuration.ThrowIfBucketNameIsNotSpecified();

		var request = new ListObjectsV2Request {
			BucketName = storage.Configuration.BucketName,
			MaxKeys = 256
		};

		requestConfigurator?.Invoke(request);

		ListObjectsV2Response response;

		do
		{
			response = await storage.Client.ListObjectsV2Async(request, cancellationToken);

			foreach (var entry in response.S3Objects)
			{
				await consumer(entry);
			}

			request.ContinuationToken = response.ContinuationToken;
		} while (response.IsTruncated);
	}

	public static async Task ListObjectsAsync(
		this IAmazonStructuredStorage storage,
		Action<S3Object> consumer,
		Action<ListObjectsV2Request> requestConfigurator = null,
		CancellationToken cancellationToken = default)
	{
		Check.NotNull(consumer, nameof(consumer));

		storage.Configuration.ThrowIfBucketNameIsNotSpecified();

		var request = new ListObjectsV2Request {
			BucketName = storage.Configuration.BucketName,
			MaxKeys = 256
		};

		requestConfigurator?.Invoke(request);

		ListObjectsV2Response response;

		do
		{
			response = await storage.Client.ListObjectsV2Async(request, cancellationToken);

			foreach (var entry in response.S3Objects)
			{
				consumer(entry);
			}

			request.ContinuationToken = response.ContinuationToken;
		} while (response.IsTruncated);
	}
}
