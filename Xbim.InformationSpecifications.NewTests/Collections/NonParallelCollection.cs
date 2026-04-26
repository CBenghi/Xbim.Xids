using Xunit;

namespace Xbim.InformationSpecifications.Tests.Collections;

/// <summary>
/// Collection definition for tests that cannot run in parallel with other tests.
/// This is used to serialize execution of tests that have side effects or resource conflicts.
/// </summary>
[CollectionDefinition(nameof(NonParallelCollection), DisableParallelization = true)]
public class NonParallelCollection
{
}
