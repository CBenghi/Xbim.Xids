using Microsoft.Extensions.Logging;

namespace Xbim.InformationSpecifications
{
	public interface IValueConstraint
	{
		bool IsSatisfiedBy(object candiatateValue, ValueConstraint context, bool ignoreCase, ILogger logger = null);
		string Short();
	}
}