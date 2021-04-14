namespace Xbim.InformationSpecifications
{
	public interface IValueConstraint
	{
		bool IsSatisfiedBy(object candiatateValue, ValueConstraint context);
	}
}