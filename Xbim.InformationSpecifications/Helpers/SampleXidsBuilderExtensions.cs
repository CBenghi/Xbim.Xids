using IdsLib.IfcSchema;

namespace Xbim.InformationSpecifications.Helpers;

/// <summary>
/// Fluent extension methods for composing sample <see cref="Xids"/> instances
/// from a <see cref="SampleXidsBuilderContext"/>.
/// </summary>
/// <example>
/// <code>
/// var xids = SampleXidsFactory.CreateXids()
///     .WithDistinctAttributeTypeSpecifications(IfcSchemaVersions.Ifc4)
///     .WithPropertiesOfMeasures(IfcSchemaVersions.Ifc4)
///     .WithPropertiesOfIfcTypes(IfcSchemaVersions.Ifc4)
///     .WithRandomSpecifications(count: 5)
///     .Build();
/// </code>
/// </example>
public static class SampleXidsBuilderExtensions
{
	/// <summary>
	/// Adds attribute-based specifications generated from the IFC schema attributes
	/// for the given <paramref name="schema"/>.
	/// </summary>
	public static SampleXidsBuilderContext WithDistinctAttributeTypeSpecifications(
		this SampleXidsBuilderContext ctx,
		IfcSchemaVersions schema = IfcSchemaVersions.Ifc4)
	{
		SampleXidsFactory.AddDistinctBackingAttributeSpecifications(ctx.Xids, schema); // from the fluent creator
		return ctx;
	}

	/// <summary>
	/// Adds measure-based property specifications for the given <paramref name="schema"/>.
	/// </summary>
	public static SampleXidsBuilderContext WithPropertiesOfMeasures(
		this SampleXidsBuilderContext ctx,
		IfcSchemaVersions schema = IfcSchemaVersions.Ifc4)
	{
		SampleXidsFactory.AddAllMeasurePropertySpecifications(ctx.Xids, schema); // from the fluent creator
		return ctx;
	}

	/// <summary>
	/// Adds IFC data-type value specifications for the given <paramref name="schema"/>.
	/// </summary>
	public static SampleXidsBuilderContext WithPropertiesOfIfcTypes(
		this SampleXidsBuilderContext ctx,
		IfcSchemaVersions schema = IfcSchemaVersions.Ifc4)
	{
		SampleXidsFactory.AddAllIfcTypePropertySpecifications(ctx.Xids, schema); // from the fluent creator
		return ctx;
	}

	/// <summary>
	/// Adds <paramref name="count"/> randomly generated specifications.
	/// </summary>
	/// <param name="ctx">The builder context.</param>
	/// <param name="count">Number of specifications to add. Default is 3.</param>
	/// <param name="retainBuildingSmartOnly">
	/// When <see langword="true"/>, non-buildingSMART facets (relation, document) are excluded.
	/// </param>
	public static SampleXidsBuilderContext WithRandomSpecifications(
		this SampleXidsBuilderContext ctx,
		int count = 3,
		bool retainBuildingSmartOnly = false)
	{
		for (int i = 0; i < count; i++)
			SampleXidsFactory.AddSpecification(ctx.Xids, i, retainBuildingSmartOnly); // from the fluent creator
		return ctx;
	}

	/// <summary>
	/// Returns the composed <see cref="Xids"/> instance.
	/// </summary>
	public static Xids Build(this SampleXidsBuilderContext ctx) => ctx.Xids;
}
