using FluentAssertions;
using System.Linq;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.DescriptionTests
{
	public class PropertyFacetDescriptionTests : BaseDescriptionTests
	{
		[InlineData("IsExternal", true, "Pset_DoorCommon", "a property IsExternal in the property set Pset_DoorCommon with value True")]
		[InlineData("Reference", "123.*", "PsetCommon", "a property Reference in the property set PsetCommon with value matching '123.*'", "", ConstraintType.Exact, ConstraintType.Pattern)]
		[InlineData("Reference", null, "PsetCommon", "a property Reference in the property set PsetCommon with value <any>")]
		[InlineData("Reference", null, null, "a property Reference in the property set <any> with value <any>")]
		[InlineData("Length", 5.1, null, "a property Length in the property set <any> with IfcLengthMeasure value 5.1", "IfcLengthMeasure")]
		//[InlineData("Easting", 56.78, "The Easting Attribute shall be 56.78")]
		//[InlineData("Name", null, "The Name Attribute shall be provided")]
		//[InlineData("Status", "NOTSTARTED,STARTED,COMPLETED", "The Status Attribute shall be NOTSTARTED or STARTED or COMPLETED")]
		//[InlineData("Name,Description", null, "The Name or Description Attribute shall be provided")]
		//[InlineData("Name,Description", "P.*", "The Name or Description Attribute shall matching 'P.*'", ConstraintType.Pattern)]
		//[InlineData("Name,Description", "Foo", "The Name or Description Attribute shall be Foo")]
		[Theory]
		public void PropertyFacetsRequirementsDescribed(string propName, object? propValue, string? pset, string expected, string measure = "",
			ConstraintType nameConstraint = ConstraintType.Exact, ConstraintType valueConstraint = ConstraintType.Exact)
		{
			IfcPropertyFacet facet;
			if (propValue is string strValue)
			{
				facet = BuildFacetFromInputs(propName, strValue, psetName: pset, dataType: measure, nameConstraint: nameConstraint, valueConstraint: valueConstraint);
			}
			else
			{
				facet = BuildFacetFromInputs(propName, objInputs: propValue, psetName: pset, dataType: measure, nameConstraint: nameConstraint, valueConstraint: valueConstraint);
			}

			facet.RequirementDescription.Should().Be(expected);
		}

		[InlineData("IsExternal", true, "Pset_DoorCommon", "with property IsExternal in the property set Pset_DoorCommon with value True")]
		[InlineData("Reference", "123.*", "PsetCommon", "with property Reference in the property set PsetCommon with value matching '123.*'", ConstraintType.Exact, ConstraintType.Pattern)]
		//[InlineData("Easting", 56.78, "The Easting Attribute shall be 56.78")]
		//[InlineData("Name", null, "The Name Attribute shall be provided")]
		//[InlineData("Status", "NOTSTARTED,STARTED,COMPLETED", "The Status Attribute shall be NOTSTARTED or STARTED or COMPLETED")]
		//[InlineData("Name,Description", null, "The Name or Description Attribute shall be provided")]
		//[InlineData("Name,Description", "P.*", "The Name or Description Attribute shall matching 'P.*'", ConstraintType.Pattern)]
		//[InlineData("Name,Description", "Foo", "The Name or Description Attribute shall be Foo")]
		[Theory]
		public void PropertyFacetsApplicabilityDescribed(string propName, object propValue, string pset, string expected,
			ConstraintType nameConstraint = ConstraintType.Exact, ConstraintType valueConstraint = ConstraintType.Exact)
		{
			IfcPropertyFacet facet;
			if (propValue is string strValue)
			{

				facet = BuildFacetFromInputs(propName, strValue, psetName: pset, nameConstraint: nameConstraint, valueConstraint: valueConstraint);
			}
			else
			{
				facet = BuildFacetFromInputs(propName, objInputs: propValue, psetName: pset, nameConstraint: nameConstraint, valueConstraint: valueConstraint);
			}

			facet.ApplicabilityDescription.Should().Be(expected);
		}

		private static IfcPropertyFacet BuildFacetFromInputs(string nameInputs, string valueInputs = "", object? objInputs = null, string? psetName = "",
			string? dataType = null,
			ConstraintType nameConstraint = ConstraintType.Exact, ConstraintType valueConstraint = ConstraintType.Exact, ConstraintType psetConstraint = ConstraintType.Exact)
		{
			IfcPropertyFacet facet = new()
			{
				PropertyName = new ValueConstraint(NetTypeName.String),
				PropertySetName = new ValueConstraint(NetTypeName.String),
				PropertyValue = new ValueConstraint(NetTypeName.String),
				DataType = dataType
			};

			var names = nameInputs.Split(',');
			foreach (var name in names)
			{
				AddConstraint(facet.PropertyName, nameConstraint, name);
			}
			if (valueInputs == null && objInputs == null && psetName == null)   // No value
				return facet;

			if (objInputs is not null)   // Non string
			{
				facet.PropertyValue.BaseType = NetTypeName.Double;
				var val = objInputs.ToString();
				if (val is not null)
				{
					AddConstraint(facet.PropertyValue, valueConstraint, val);
				}
			}
			else
			{
				var values = valueInputs is not null
					? valueInputs.Split(',')
					: Enumerable.Empty<string>();
				foreach (var val in values)
				{
					AddConstraint(facet.PropertyValue, valueConstraint, val);
				}
			}

			if (psetName != null)
			{
				var psetvalues = psetName.Split(','); // String or strings
				foreach (var val in psetvalues)
				{
					AddConstraint(facet.PropertySetName, psetConstraint, val);
				}

			}
			return facet;
		}
	}
}
