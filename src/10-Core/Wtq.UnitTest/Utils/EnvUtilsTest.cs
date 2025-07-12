namespace Wtq.Core.UnitTest.Utils;

[TestClass]
public class EnvUtilsTest
{
	[TestInitialize]
	public void Setup()
	{
		Environment.SetEnvironmentVariable("A_Regular_Value", "Value_1");
		Environment.SetEnvironmentVariable("An_Empty_String", "");
		Environment.SetEnvironmentVariable("A_Space", " ");
		Environment.SetEnvironmentVariable("A_Padded_String", " Padded_Value_1 ");
	}

	[TestMethod]

	// @formatter:off
#pragma warning disable SA1027

	[DataRow("PRE_ %A_Regular_Value% _POST",	"PRE_ Value_1 _POST")] // Exact match
	// [DataRow("PRE_ %my_var_1% _POST",		"PRE_ Value_1 _POST")] // casing TODO: Fix case sensitivity.
	// [DataRow("PRE_ %MY_VAR_1% _POST",		"PRE_ Value_1 _POST")] // CASING TODO: Fix case sensitivity.
	[DataRow("PRE_ %An_Empty_String% _POST",	"PRE_  _POST")] // Empty string ""
	[DataRow("PRE_ %A_Space% _POST",			"PRE_   _POST")] // Space " "

	// @formatter:on
#pragma warning restore SA1027
	public void ExpandEnvVars(string input, string expectedOutput)
	{
		Assert.AreEqual(expectedOutput, input.ExpandEnvVars());
	}

	[TestMethod]

	// @formatter:off
#pragma warning disable SA1027

	[DataRow("A_Regular_Value",		"Value_1")] // Exact match
	[DataRow("a_regular_value",		"Value_1")] // casing
	[DataRow("A_REGULAR_VALUE",		"Value_1")] // CASING
	[DataRow("A_Padded_String",		"Padded_Value_1")] // " Trim "
	[DataRow("An_Empty_String",		null)] // Empty string ""
	[DataRow("A_Space",				null)] // Space " "

	// @formatter:on
#pragma warning restore SA1027
	public void GetEnvVar(string varName, string expectedOutput)
	{
		Assert.AreEqual(expectedOutput, EnvUtils.GetEnvVar(varName));
	}

	[TestMethod]

	// @formatter:off
#pragma warning disable SA1027

	[DataRow("A_Regular_Value",		"The_Default_Value",	"Value_1")] // Exact match
	[DataRow("a_regular_value",		"The_Default_Value",	"Value_1")] // casing
	[DataRow("A_REGULAR_VALUE",		"The_Default_Value",	"Value_1")] // CASING
	[DataRow("An_Empty_String",		"The_Default_Value",	"The_Default_Value")] // Empty string ""
	[DataRow("A_Space",				"The_Default_Value",	"The_Default_Value")] // Space " "

	// @formatter:on
#pragma warning restore SA1027
	public void GetEnvVarOrDefault(string varName, string defaultValue, string expectedOutput)
	{
		Assert.AreEqual(expectedOutput, EnvUtils.GetEnvVarOrDefault(varName, defaultValue));
	}

	[TestMethod]

	// @formatter:off
#pragma warning disable SA1027

	[DataRow("A_Regular_Value",		"Value_1",				true)] // Exact match
	[DataRow("a_regular_value",		"value_1",				true)] // casing
	[DataRow("A_REGULAR_VALUE",		"VALUE_1",				true)] // CASING
	[DataRow("A_Padded_String",		"Padded_Value_1",		true)] // Padding
	[DataRow("A_Regular_Value",		"Different_Value_1",	false)] // No match

	// @formatter:on
#pragma warning restore SA1027
	public void HasEnvVarWithValue(string varName, string value, bool expectedResult)
	{
		Assert.AreEqual(expectedResult, EnvUtils.HasEnvVarWithValue(varName, value));
	}
}