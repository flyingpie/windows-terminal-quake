using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Wtq.Core.UnitTest.Utils;

[TestClass]
public class AttrUtilsTest
{
	private class TestClass
	{
		[Display(Name = "The Enum Display Name", Prompt = "The Enum Prompt")]
		[DefaultValue(HideOnFocusLost.True)]
		public HideOnFocusLost EnumPropertyWithAttr { get; set; }

		public HideOnFocusLost EnumPropertyWithoutAttr { get; set; }

		[Display(Name = "The Enum Display Name", Prompt = "The Enum Prompt")]
		[DefaultValue(HideOnFocusLost.True)]
		public HideOnFocusLost? EnumPropertyNullableWithAttr { get; set; }

		public HideOnFocusLost? EnumPropertyNullableWithoutAttr { get; set; }


		[Display(Name = "The Reference Display Name", Prompt = "The Reference Prompt")]
		[DefaultValue("The Default Reference Value")]
		public string RefPropertyWithAttr { get; set; } = null!;

		public string RefPropertyWithoutAttr { get; set; } = null!;

		[Display(Name = "The Reference Display Name", Prompt = "The Reference Prompt")]
		[DefaultValue("The Default Reference Value")]
		public string? RefPropertyNullableWithAttr { get; set; }

		public string? RefPropertyNullableWithoutAttr { get; set; }


		[Display(Name = "The Value Display Name", Prompt = "The Value Prompt")]
		[DefaultValue(1234)]
		public int ValPropertyWithAttr { get; set; }

		public int ValPropertyWithoutAttr { get; set; }

		[Display(Name = "The Value Display Name", Prompt = "The Value Prompt")]
		[DefaultValue(1234)]
		public int ValPropertyNullableWithAttr { get; set; }

		public int ValPropertyNullableWithoutAttr { get; set; }
	}

	private readonly TestClass _tc = null!;

	[TestMethod]
	public void GetDefaultValueFor()
	{
		// Reference type.
		Assert.AreEqual("The Default Reference Value", AttrUtils.GetDefaultValueFor<string>(() => _tc.RefPropertyWithAttr));
		Assert.IsNull(AttrUtils.GetDefaultValueFor<string>(() => _tc.RefPropertyWithoutAttr));

		Assert.AreEqual("The Default Reference Value", AttrUtils.GetDefaultValueFor<string>(() => _tc.RefPropertyNullableWithAttr));
		Assert.IsNull(AttrUtils.GetDefaultValueFor<string>(() => _tc.RefPropertyNullableWithoutAttr));

		// Enum type (seem to behave a little differently sometimes compare to ints, so we're testing them separately, even though these are just value types).
		Assert.AreEqual(HideOnFocusLost.True, AttrUtils.GetDefaultValueFor<HideOnFocusLost>(() => _tc.EnumPropertyWithAttr));
		Assert.AreEqual(HideOnFocusLost.None, AttrUtils.GetDefaultValueFor<HideOnFocusLost>(() => _tc.EnumPropertyWithoutAttr));

		Assert.AreEqual(HideOnFocusLost.True, AttrUtils.GetDefaultValueFor<HideOnFocusLost>(() => _tc.EnumPropertyNullableWithAttr));
		Assert.AreEqual(HideOnFocusLost.None, AttrUtils.GetDefaultValueFor<HideOnFocusLost>(() => _tc.EnumPropertyNullableWithoutAttr));

		// Value type.
		Assert.AreEqual(1234, AttrUtils.GetDefaultValueFor<int>(() => _tc.ValPropertyWithAttr));
		Assert.AreEqual(0, AttrUtils.GetDefaultValueFor<int>(() => _tc.ValPropertyWithoutAttr));

		Assert.AreEqual(1234, AttrUtils.GetDefaultValueFor<int>(() => _tc.ValPropertyNullableWithAttr));
		Assert.AreEqual(0, AttrUtils.GetDefaultValueFor<int>(() => _tc.ValPropertyNullableWithoutAttr));
	}

	[TestMethod]
	public void GetDisplayAttr()
	{
		var attr1 = AttrUtils.GetDisplayAttr(() => _tc.EnumPropertyWithAttr);
		Assert.IsNotNull(attr1);
		Assert.AreEqual("The Enum Display Name", attr1.Name);

		var attr2 = AttrUtils.GetDisplayAttr(() => _tc.EnumPropertyWithoutAttr);
		Assert.IsNull(attr2);
	}

	[TestMethod]
	public void GetDisplayName()
	{
		Assert.AreEqual("The Reference Display Name", AttrUtils.GetDisplayName(() => _tc.RefPropertyWithAttr));
		Assert.AreEqual(nameof(_tc.RefPropertyWithoutAttr), AttrUtils.GetDisplayName(() => _tc.RefPropertyWithoutAttr));
	}

	[TestMethod]
	public void GetDisplayPrompt()
	{
		Assert.AreEqual("The Reference Prompt", AttrUtils.GetPrompt(() => _tc.RefPropertyWithAttr));
		Assert.IsNull(AttrUtils.GetPrompt(() => _tc.RefPropertyWithoutAttr));
	}
}