using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Text.Json;

namespace Wtq.Core.UnitTest;

/// <summary>
/// Provides a set of static methods to verify that JSON objects can meet criteria in tests.
/// </summary>
public static class JsonAssert
{
	private static readonly JsonSerializerOptions SerializerOptions;

	static JsonAssert()
	{
		SerializerOptions = new() { TypeInfoResolver = new DefaultJsonTypeInfoResolver(), WriteIndented = true };

		SerializerOptions.MakeReadOnly();
	}

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	public static void AreEqual(string? expected, string? actual)
		=> AreEqual(expected is null ? null : JsonNode.Parse(expected),
			actual is null ? null : JsonNode.Parse(actual));

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="output">Whether to print diff result.</param>
	public static void AreEqual(string? expected, string? actual, bool output)
		=> AreEqual(expected is null ? null : JsonNode.Parse(expected),
			actual is null ? null : JsonNode.Parse(actual), output);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="diffOptions">The diff options.</param>
	public static void AreEqual(string? expected, string? actual, JsonDiffOptions diffOptions)
		=> AreEqual(expected is null ? null : JsonNode.Parse(expected),
			actual is null ? null : JsonNode.Parse(actual), diffOptions);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="diffOptions">The diff options.</param>
	/// <param name="output">Whether to print diff result.</param>
	public static void AreEqual(string? expected, string? actual, JsonDiffOptions diffOptions, bool output)
		=> AreEqual(expected is null ? null : JsonNode.Parse(expected),
			actual is null ? null : JsonNode.Parse(actual), diffOptions, output);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="outputFormatter">The output formatter.</param>
	public static void AreEqual(string? expected, string? actual, Func<JsonNode, string> outputFormatter)
		=> AreEqual(expected is null ? null : JsonNode.Parse(expected),
			actual is null ? null : JsonNode.Parse(actual), outputFormatter);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="diffOptions">The diff options.</param>
	/// <param name="outputFormatter">The output formatter.</param>
	public static void AreEqual(string? expected, string? actual, JsonDiffOptions diffOptions,
		Func<JsonNode, string> outputFormatter)
		=> AreEqual(expected is null ? null : JsonNode.Parse(expected),
			actual is null ? null : JsonNode.Parse(actual), diffOptions, outputFormatter);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <typeparam name="T">The type of JSON object to be compared.</typeparam>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	public static void AreEqual<T>(T? expected, T? actual)
		where T : JsonNode
		=> HandleAreEqual(expected, actual, null, null);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <typeparam name="T">The type of JSON object to be compared.</typeparam>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="output">Whether to print diff result.</param>
	public static void AreEqual<T>(T? expected, T? actual, bool output)
		where T : JsonNode
		=> HandleAreEqual(expected, actual, null,
			output ? delta => CreateDefaultOutput(expected, actual, delta) : null);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <typeparam name="T">The type of JSON object to be compared.</typeparam>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="diffOptions">The diff options.</param>
	public static void AreEqual<T>(T? expected, T? actual, JsonDiffOptions diffOptions)
		where T : JsonNode
		=> HandleAreEqual(expected, actual,
			diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)), null);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <typeparam name="T">The type of JSON object to be compared.</typeparam>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="diffOptions">The diff options.</param>
	/// <param name="output">Whether to print diff result.</param>
	public static void AreEqual<T>(T? expected, T? actual, JsonDiffOptions diffOptions, bool output)
		where T : JsonNode
		=> HandleAreEqual(expected, actual,
			diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)),
			output ? delta => CreateDefaultOutput(expected, actual, delta) : null);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <typeparam name="T">The type of JSON object to be compared.</typeparam>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="outputFormatter">The output formatter.</param>
	public static void AreEqual<T>(T? expected, T? actual, Func<JsonNode, string> outputFormatter)
		where T : JsonNode
		=> HandleAreEqual(expected, actual, null,
			outputFormatter ?? throw new ArgumentNullException(nameof(outputFormatter)));

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <typeparam name="T">The type of JSON object to be compared.</typeparam>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="diffOptions">The diff options.</param>
	/// <param name="outputFormatter">The output formatter.</param>
	public static void AreEqual<T>(T? expected, T? actual, JsonDiffOptions diffOptions,
		Func<JsonNode, string> outputFormatter)
		where T : JsonNode
		=> HandleAreEqual(expected, actual,
			diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)),
			outputFormatter ?? throw new ArgumentNullException(nameof(outputFormatter)));

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="assert">The assert object.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	public static void JsonAreEqual(this Assert assert, string? expected, string? actual)
		=> AreEqual(expected, actual);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="assert">The assert object.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="output">Whether to print diff result.</param>
	public static void JsonAreEqual(this Assert assert, string? expected, string? actual, bool output)
		=> AreEqual(expected, actual, output);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="assert">The assert object.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="diffOptions">The diff options.</param>
	public static void JsonAreEqual(this Assert assert, string? expected, string? actual,
		JsonDiffOptions diffOptions)
		=> AreEqual(expected, actual, diffOptions);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="assert">The assert object.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="diffOptions">The diff options.</param>
	/// <param name="output">Whether to print diff result.</param>
	public static void JsonAreEqual(this Assert assert, string? expected, string? actual,
		JsonDiffOptions diffOptions, bool output)
		=> AreEqual(expected, actual, diffOptions, output);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="assert">The assert object.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="outputFormatter">The output formatter.</param>
	public static void JsonAreEqual(this Assert assert, string? expected, string? actual,
		Func<JsonNode, string> outputFormatter)
		=> AreEqual(expected, actual, outputFormatter);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="assert">The assert object.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="diffOptions">The diff options.</param>
	/// <param name="outputFormatter">The output formatter.</param>
	public static void JsonAreEqual(this Assert assert, string? expected, string? actual,
		JsonDiffOptions diffOptions,
		Func<JsonNode, string> outputFormatter)
		=> AreEqual(expected, actual, diffOptions, outputFormatter);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <typeparam name="T">The type of JSON object to be compared.</typeparam>
	/// <param name="assert">The assert object.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	public static void JsonAreEqual<T>(this Assert assert, T? expected, T? actual)
		where T : JsonNode
		=> AreEqual(expected, actual);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <typeparam name="T">The type of JSON object to be compared.</typeparam>
	/// <param name="assert">The assert object.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="output">Whether to print diff result.</param>
	public static void JsonAreEqual<T>(this Assert assert, T? expected, T? actual, bool output)
		where T : JsonNode
		=> AreEqual(expected, actual, output);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <typeparam name="T">The type of JSON object to be compared.</typeparam>
	/// <param name="assert">The assert object.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="diffOptions">The diff options.</param>
	public static void JsonAreEqual<T>(this Assert assert, T? expected, T? actual, JsonDiffOptions diffOptions)
		where T : JsonNode
		=> AreEqual(expected, actual, diffOptions);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <typeparam name="T">The type of JSON object to be compared.</typeparam>
	/// <param name="assert">The assert object.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="diffOptions">The diff options.</param>
	/// <param name="output">Whether to print diff result.</param>
	public static void JsonAreEqual<T>(this Assert assert, T? expected, T? actual,
		JsonDiffOptions diffOptions, bool output)
		where T : JsonNode
		=> AreEqual(expected, actual, diffOptions, output);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <typeparam name="T">The type of JSON object to be compared.</typeparam>
	/// <param name="assert">The assert object.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="outputFormatter">The output formatter.</param>
	public static void JsonAreEqual<T>(this Assert assert, T? expected, T? actual,
		Func<JsonNode, string> outputFormatter)
		where T : JsonNode
		=> AreEqual(expected, actual, outputFormatter);

	/// <summary>
	/// Tests whether two JSON objects are equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <typeparam name="T">The type of JSON object to be compared.</typeparam>
	/// <param name="assert">The assert object.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="diffOptions">The diff options.</param>
	/// <param name="outputFormatter">The output formatter.</param>
	public static void JsonAreEqual<T>(this Assert assert, T? expected, T? actual,
		JsonDiffOptions diffOptions,
		Func<JsonNode, string> outputFormatter)
		where T : JsonNode
		=> AreEqual(expected, actual, diffOptions, outputFormatter);

	private static void HandleAreEqual(JsonNode? expected, JsonNode? actual, JsonDiffOptions? diffOptions,
		Func<JsonNode, string>? outputFormatter)
	{
		var diff = expected.Diff(actual, diffOptions);
		if (diff is null)
		{
			return;
		}

		var message = CreateAreEqualFailureMessage(diff, outputFormatter);
		throw new AssertFailedException(message);
	}

	private static string CreateDefaultOutput(JsonNode? expected, JsonNode? actual, JsonNode diff)
	{
		var sb = new StringBuilder();

		sb.Append("Expected:");
		sb.AppendLine();
		sb.Append(expected is null
			? "null"
			: expected.ToJsonString(SerializerOptions));
		sb.AppendLine();

		sb.Append("Actual:");
		sb.AppendLine();
		sb.Append(actual is null
			? "null"
			: actual.ToJsonString(SerializerOptions));
		sb.AppendLine();

		sb.Append("Delta:");
		sb.AppendLine();
		sb.Append(diff.ToJsonString(SerializerOptions));

		return sb.ToString();
	}

	private static string CreateAreEqualFailureMessage(JsonNode diff, Func<JsonNode, string>? outputFormatter)
	{
		var sb = new StringBuilder();
		sb.Append("JsonAssert.AreEqual() failure.");

		if (outputFormatter is not null)
		{
			sb.AppendLine();
			sb.Append(outputFormatter(diff));
		}

		return sb.ToString();
	}

	/// <summary>
	/// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	public static void AreNotEqual(string? expected, string? actual)
		=> AreNotEqual(expected is null ? null : JsonNode.Parse(expected),
			actual is null ? null : JsonNode.Parse(actual));

	/// <summary>
	/// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="diffOptions">The diff options.</param>
	public static void AreNotEqual(string? expected, string? actual, JsonDiffOptions diffOptions)
		=> AreNotEqual(expected is null ? null : JsonNode.Parse(expected),
			actual is null ? null : JsonNode.Parse(actual), diffOptions);

	/// <summary>
	/// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <typeparam name="T">The type of JSON object to be compared.</typeparam>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	public static void AreNotEqual<T>(T? expected, T? actual)
		where T : JsonNode
		=> HandleAreNotEqual(expected, actual, null);

	/// <summary>
	/// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <typeparam name="T">The type of JSON object to be compared.</typeparam>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="diffOptions">The diff options.</param>
	public static void AreNotEqual<T>(T? expected, T? actual, JsonDiffOptions diffOptions)
		where T : JsonNode
		=> HandleAreNotEqual(expected, actual,
			diffOptions ?? throw new ArgumentNullException(nameof(diffOptions)));

	/// <summary>
	/// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="assert">The assert.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	public static void JsonAreNotEqual(this Assert assert, string? expected, string? actual)
		=> AreNotEqual(expected, actual);

	/// <summary>
	/// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="assert">The assert.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="diffOptions">The diff options.</param>
	public static void JsonAreNotEqual(this Assert assert, string? expected, string? actual,
		JsonDiffOptions diffOptions)
		=> AreNotEqual(expected, actual, diffOptions);

	/// <summary>
	/// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="assert">The assert.</param>
	/// <typeparam name="T">The type of JSON object to be compared.</typeparam>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	public static void JsonAreNotEqual<T>(this Assert assert, T? expected, T? actual)
		where T : JsonNode
		=> AreNotEqual(expected, actual);

	/// <summary>
	/// Tests whether two JSON objects are not equal. Note that when comparing the specified objects,
	/// the ordering of members in the objects is not significant.
	/// </summary>
	/// <param name="assert">The assert.</param>
	/// <typeparam name="T">The type of JSON object to be compared.</typeparam>
	/// <param name="expected">The expected value.</param>
	/// <param name="actual">The actual value.</param>
	/// <param name="diffOptions">The diff options.</param>
	public static void JsonAreNotEqual<T>(this Assert assert, T? expected, T? actual, JsonDiffOptions diffOptions)
		where T : JsonNode
		=> AreNotEqual(expected, actual, diffOptions);

	private static void HandleAreNotEqual(JsonNode? expected, JsonNode? actual, JsonDiffOptions? diffOptions)
	{
		var diff = expected.Diff(actual, diffOptions);
		if (diff is not null)
		{
			return;
		}

		throw new AssertFailedException("JsonAssert.AreNotEqual() failure.");
	}
}