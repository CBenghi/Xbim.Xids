using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.InformationSpecifications
{
	public interface ISpecificationMetadata
	{
		string Provider { get; set; }

		List<string> Consumers { get; set; }

		List<string> Stages { get; set; }
	}
}
