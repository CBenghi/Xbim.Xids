namespace Xbim.Xids
{
	public interface IValueConstraint
	{
		bool IsSatisfiedBy(object candiatateValue, ValueConstraint context);
	}
}