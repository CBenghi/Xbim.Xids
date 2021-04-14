using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.IDS
{
	public interface IValueConstraint
	{
		bool IsValid(object testObject);
	}
}
