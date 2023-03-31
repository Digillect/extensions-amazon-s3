using System;
using Amazon.S3;
using Digillect.Extensions.Amazon.S3;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DigillectAmazonS3ServiceCollectionExtensions
{
	[PublicAPI]
	public static IServiceCollection AddAmazonStructuredStorage(
		this IServiceCollection services,
		ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
	{
		services.Add(new ServiceDescriptor(typeof(IAmazonS3), S3ClientCreator.CreateS3Client, serviceLifetime));
		services.Add(new ServiceDescriptor(typeof(IAmazonStructuredStorage), typeof(AmazonStructuredStorage), serviceLifetime));

		return services;
	}

	[PublicAPI]
	public static IServiceCollection AddAmazonStructuredStorage(
		this IServiceCollection services,
		IConfiguration configuration,
		ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
	{
		return services
			.Configure<AmazonStructuredStorageConfiguration>(configuration)
			.AddAmazonStructuredStorage(serviceLifetime);
	}

	[PublicAPI]
	public static IServiceCollection AddAmazonStructuredStorage(
		this IServiceCollection services,
		Action<AmazonStructuredStorageConfiguration> configureOptions,
		ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
	{
		return services
			.Configure(configureOptions)
			.AddAmazonStructuredStorage(serviceLifetime);
	}
}
