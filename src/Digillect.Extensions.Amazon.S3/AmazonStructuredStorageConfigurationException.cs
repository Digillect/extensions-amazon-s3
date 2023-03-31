using System;
using System.Runtime.Serialization;

namespace Digillect.Extensions.Amazon.S3;

public class AmazonStructuredStorageConfigurationException : Exception
{
	public AmazonStructuredStorageConfigurationException()
	{
	}

	public AmazonStructuredStorageConfigurationException(string message)
		: base(message)
	{
	}

	public AmazonStructuredStorageConfigurationException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected AmazonStructuredStorageConfigurationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
