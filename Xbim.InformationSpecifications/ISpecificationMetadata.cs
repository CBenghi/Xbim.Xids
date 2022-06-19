using System.Collections.Generic;

namespace Xbim.InformationSpecifications
{
    public interface ISpecificationMetadata
    {
        string? Provider { get; set; }

        List<string>? Consumers { get; set; }

        List<string>? Stages { get; set; }
    }
}
