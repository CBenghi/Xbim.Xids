namespace Xbim.InformationSpecifications.Helpers;

/// <summary>
/// Carries a <see cref="Xids"/> instance being composed through the
/// <see cref="SampleXidsBuilderExtensions"/> fluent API.
/// </summary>
/// <remarks>
/// Obtain an instance via <see cref="SampleXidsFactory.CreateXids()"/>.
/// Call <see cref="SampleXidsBuilderExtensions.Build"/> to retrieve the final <see cref="Xids"/>.
/// </remarks>
public sealed class SampleXidsBuilderContext
{
	internal Xids Xids { get; }

	internal SampleXidsBuilderContext(Xids xids)
	{
		Xids = xids;
	}
}
