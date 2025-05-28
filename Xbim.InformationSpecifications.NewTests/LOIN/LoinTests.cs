using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.LOIN
{
	public class LoinTests
	{
		[Fact]
		public void ProviderIsPropagated()
		{
			var x = new Xids();
			var g = new SpecificationsGroup(x);
			x.SpecificationsGroups.Add(g);
			var s = x.PrepareSpecification(g, IfcSchemaVersion.IFC4);

			s.GetProvider().Should().BeEmpty();

			var value = "Some";
			x.Provider = value;
			s.GetProvider().Should().Be(value);

			var otherValue = "SomeOther";
			g.Provider = otherValue;
			s.GetProvider().Should().Be(otherValue);

			var locValue = "SomeOther";
			s.Provider = locValue;
			s.GetProvider().Should().Be(locValue);
		}

		[Fact]
		public void ConsumersArePropagated()
		{
			var x = new Xids();
			var g = new SpecificationsGroup(x);
			x.SpecificationsGroups.Add(g);
			var s = x.PrepareSpecification(g, IfcSchemaVersion.IFC4);

			s.GetConsumers().Should().BeEmpty();
			s.GetConsumers().Should().NotBeNull();

			var value = "Some";
			x.Consumers = new List<string> { value, value };
			s.GetConsumers().Should().AllBe(value);
			s.GetConsumers().Should().HaveCount(2);

			var otherValue = "SomeOther";
			g.Consumers = new List<string> { otherValue, otherValue };
			s.GetConsumers().Should().AllBe(otherValue);
			s.GetConsumers().Should().HaveCount(2);

			var locValue = "SomeOther";
			s.Consumers = new List<string> { locValue, locValue };
			s.GetConsumers().Should().AllBe(locValue);
			s.GetConsumers().Should().HaveCount(2);
		}


		[Fact]
		public void StagesArePropagated()
		{
			var x = new Xids();
			var g = new SpecificationsGroup(x);
			x.SpecificationsGroups.Add(g);
			var s = x.PrepareSpecification(g, IfcSchemaVersion.IFC4);

			s.GetStages().Should().BeEmpty();
			s.GetStages().Should().NotBeNull();

			var value = "Some";
			x.Stages = new List<string> { value, value };
			s.GetStages().Should().AllBe(value);
			s.GetStages().Should().HaveCount(2);

			var otherValue = "SomeOther";
			g.Stages = new List<string> { otherValue, otherValue };
			s.GetStages().Should().AllBe(otherValue);
			s.GetStages().Should().HaveCount(2);

			var locValue = "SomeOther";
			s.Stages = new List<string> { locValue, locValue };
			s.GetStages().Should().AllBe(locValue);
			s.GetStages().Should().HaveCount(2);
		}
	}
}
