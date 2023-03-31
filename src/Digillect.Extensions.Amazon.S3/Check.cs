using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace Digillect.Extensions;

[DebuggerStepThrough]
internal static class Check
{
	[ContractAnnotation("value:null => halt")]
	public static T NotNull<T>([NoEnumeration] T value, [NotNull] [InvokerParameterName] string parameterName)
		where T : class
	{
		if (value == null)
		{
			NotEmpty(parameterName, nameof(parameterName));

			throw new ArgumentNullException(parameterName);
		}

		return value;
	}

	[ContractAnnotation("value:null => halt")]
	public static IReadOnlyList<T> NotEmpty<T>(
		IReadOnlyList<T> value,
		[InvokerParameterName] [NotNull] string parameterName)
	{
		NotNull(value, parameterName);

		if (value.Count == 0)
		{
			NotEmpty(parameterName, nameof(parameterName));

			throw new ArgumentException("Argument can't be an empty list", parameterName);
		}

		return value;
	}

	[ContractAnnotation("value:null => halt")]
	public static string NotEmpty(
		string value,
		[InvokerParameterName] [NotNull] string parameterName)
	{
		Exception exception = null;

		if (value == null)
		{
			exception = new ArgumentNullException(parameterName);
		}
		else if (value.Trim().Length == 0)
		{
			exception = new ArgumentException("Argument is empty", parameterName);
		}

		if (exception != null)
		{
			NotEmpty(parameterName, nameof(parameterName));

			throw exception;
		}

		return value;
	}

	public static string NullButNotEmpty(
		string value,
		[InvokerParameterName] [NotNull] string parameterName)
	{
		if (value?.Length == 0)
		{
			NotEmpty(parameterName, nameof(parameterName));

			throw new ArgumentException("Argument can't be an empty string", parameterName);
		}

		return value;
	}

	public static IReadOnlyList<T> HasNoNulls<T>(
		IReadOnlyList<T> value,
		[InvokerParameterName] [NotNull] string parameterName)
		where T : class
	{
		NotNull(value, parameterName);

		if (value.Any(i => i == null))
		{
			NotEmpty(parameterName, nameof(parameterName));

			throw new ArgumentException(parameterName);
		}

		return value;
	}

	[ContractAnnotation("halt <= condition: false")]
	public static void OutOfRange(
		bool condition,
		[InvokerParameterName] [NotNull] string parameterName)
	{
		if (!condition)
		{
			NotEmpty(parameterName, nameof(parameterName));

			throw new ArgumentOutOfRangeException(parameterName);
		}
	}
}
