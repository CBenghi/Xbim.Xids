using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.InformationSpecifications.Tests.DescriptionTests
{
    public abstract class BaseDescriptionTests
    {
        protected static void AddConstraint(ValueConstraint constraint, ConstraintType constraintType, string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            switch (constraintType)
            {
                case ConstraintType.Pattern:
                    constraint.AddAccepted(new PatternConstraint(value));
                    break;
                case ConstraintType.Exact:
                    constraint.AddAccepted(new ExactConstraint(value));
                    break;
                default:
                    throw new NotImplementedException(constraintType.ToString());
            }
        }

        public enum ConstraintType
        {
            Exact,
            Pattern,
            Range,
            Structure
        }
    }
}
