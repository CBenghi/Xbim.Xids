using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Xbim.InformationSpecifications.Tests
{
	
	public class EnumTests
	{
		[Fact]
		public void CanSetEnum()
		{
			IfcClassificationFacet f = new IfcClassificationFacet();

			var values = Enum.GetValues(typeof(Location)).Cast<Location>();
			foreach (var val in values)
			{
				f.SetLocation(val);
				f.GetLocation().Should().Be(val);				
			}
		}
	}
}
