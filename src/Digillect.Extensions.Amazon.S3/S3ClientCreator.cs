using System;
using System.IO;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Digillect.Extensions.Amazon.S3;

internal static class S3ClientCreator
{
	internal static object CreateS3Client(IServiceProvider serviceProvider)
	{
		var options = serviceProvider.GetRequiredService<IOptions<AmazonStructuredStorageConfiguration>>();
		var configuration = options.Value;

		var config = new AmazonS3Config {
			RegionEndpoint = RegionEndpoint.GetBySystemName(configuration.Region),
			ServiceURL = configuration.ServiceUrl,
			ForcePathStyle = configuration.ForcePathStyle,
			LogResponse = configuration.LogResponses != ResponseLoggingOption.Never
		};

		if (configuration.LogResponses != ResponseLoggingOption.Never)
		{
			AWSConfigs.LoggingConfig.LogResponses = configuration.LogResponses;
			AWSConfigs.LoggingConfig.LogTo = LoggingOptions.Console;
		}

		if (!string.IsNullOrEmpty(configuration.AccessKeyId))
		{
			return new AmazonS3Client(configuration.AccessKeyId, configuration.SecretAccessKey, config);
		}

		var credentials = GetCredentials(configuration);

		return new AmazonS3Client(credentials, config);
	}

	private static AWSCredentials GetCredentials(AmazonStructuredStorageConfiguration configuration)
	{
		if (string.IsNullOrEmpty(configuration.ProfilesLocation))
		{
			throw new AmazonStructuredStorageConfigurationException("AWS Shared Credentials file is not specified");
		}

		if (!File.Exists(configuration.ProfilesLocation))
		{
			throw new AmazonStructuredStorageConfigurationException($"AWS Shared Credentials file {configuration.ProfilesLocation} doesn't exists");
		}

		var sharedFile = new SharedCredentialsFile(configuration.ProfilesLocation);

		if (!sharedFile.TryGetProfile(configuration.Profile, out var profile))
		{
			throw new AmazonStructuredStorageConfigurationException($"Unable to get profile {configuration.Profile} from AWS Shared Credentials file");
		}

		if (!AWSCredentialsFactory.TryGetAWSCredentials(profile, sharedFile, out var credentials))
		{
			throw new AmazonStructuredStorageConfigurationException("Unable to get credentials from AWS Shared Credentials file");
		}

		return credentials;
	}
}
