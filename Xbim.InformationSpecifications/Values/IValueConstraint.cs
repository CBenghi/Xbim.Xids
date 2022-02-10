using Microsoft.Extensions.Logging;

namespace Xbim.InformationSpecifications
{
	public interface IValueConstraint
	{
		bool IsSatisfiedBy(object candiatateValue, ValueConstraint context, ILogger logger = null);
		string Short();
	}
}