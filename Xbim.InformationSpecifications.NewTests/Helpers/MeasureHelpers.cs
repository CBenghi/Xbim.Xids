using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Xbim.InformationSpecifications.Helpers;
using Xbim.InformationSpecifications.Helpers.Measures;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.Helpers
{
    public partial class MeasureHelpers
    {
        [Fact]
        public void CharacterDiscovery()
        {
            var t = "°";
            var t2 = "\u00b0";
            t2.Should().Be(t);
            var arr = t.ToCharArray();
            Assert.True(arr.First() == (char)176);
        }

        [Fact]
        public void HasExponents()
        {
            foreach (var item in Enum.GetValues<DimensionType>())
            {
                Debug.WriteLine($"=== {item}");
                var t = SchemaInfo.IfcMeasures.Values.Where(x => x.Exponents is not null && x.Exponents.GetExponent(item) != 0).ToList();
                t.Should().NotBeEmpty();
            }

            foreach (var item in Enum.GetValues<DimensionType>())
            {
                Debug.WriteLine($"=== {item}");
                var t = SchemaInfo.IfcMeasures.Values.Where(x => x.Exponents is not null && x.Exponents.Equals(DimensionalExponents.GetUnit(item))).ToList();
                // t.Count().Should().Be(1, $"{item} is expected");

                foreach (var meas in t)
                {
                    Debug.WriteLine($"{meas.Id} {meas.Exponents} - {meas.Exponents!.ToUnitSymbol()}");
                }
            }
        }

        [Fact]
        public void Can_discriminate_valid_and_invalid_units()
        {
            var unit1 = new MeasureUnit("lb/m2");
            unit1.IsValid.Should().BeTrue();

            var unit2 = new MeasureUnit("lb/pizza2");
            unit2.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Unit()
        {
            var sourceString = "°F";
            Regex r = UnitChars();
            var t = r.IsMatch(sourceString);
            t.Should().BeTrue();

            var _ = new MeasureUnit(sourceString);

            IfcMeasureInfo t2 = SchemaInfo.IfcMeasures[IfcValue.IfcThermodynamicTemperatureMeasure.ToString()];
            t2.Should().NotBeNull("library should be complete.");
        }

		[GeneratedRegex("[°'a-zA-Z]+")]
		private static partial Regex UnitChars();


		//Foot per second squared to Meter per second squared 1 ft² = 0. 3048 m² - acceleration is not defined
		[Theory]
        [InlineData(1.0, "ft", IfcValue.IfcLengthMeasure, 0.3048)]
        [InlineData(2.0, "ft", IfcValue.IfcLengthMeasure, 0.6096)]
        [InlineData(1.0, "ft2", IfcValue.IfcAreaMeasure, 0.09290304)]//Square foot to Square meter	 		1 ft² = 0.092903 m²
        [InlineData(1.0, "ft3", IfcValue.IfcVolumeMeasure, 0.028316846592000004)] //Cubic foot to  Cubic meter	 		1 ft³ = 0.028316 m³
        [InlineData(1.0, "lb / in3", IfcValue.IfcMassDensityMeasure, 27680.370321370567)] //Pound per cubic inch to Kilogram per cubic meter	 		1 lb/in³ = 27679.9 047102 kg/m³
        [InlineData(1.0, "gal / min", IfcValue.IfcVolumetricFlowRateMeasure, 6.309033333333333E-05)] //Gallon per minute X to m3 / s
        [InlineData(1.0, "lb / ft2", IfcValue.IfcAreaDensityMeasure, 4.88250976501953)]
        [InlineData(1.0, "acre ft / day", IfcValue.IfcVolumetricFlowRateMeasure, 0.014276467263055554)] //Acre foot per day = Cubic meter per second	 		1 Acre foot per day= 0.01428 m^3/s (cubic meters per second) (wolfram)
        [InlineData(1.0, "lbf / ft2", IfcValue.IfcPressureMeasure, 47.880263121637356)]   //Pound per Square Foot to Pascal	 		1 lbf/ft2 = 47.88025 Pascal
        [InlineData(1.0, "lbf / in2", IfcValue.IfcPressureMeasure, 6894.75788951578)]   //Pound per square inch X 6.894 = Kilopascal	KPa
        [InlineData(1.0, "lbf", IfcValue.IfcForceMeasure, 4.448222)] //Pound force = Newton	 		1 Pound force = 4.448222 Newton
        [InlineData(1.0, "acre", IfcValue.IfcAreaMeasure, 4046.87261)] //Acre to square meter			1 acre = 4046.856 m²
        [InlineData(1.0, "ft3/sec", IfcValue.IfcVolumetricFlowRateMeasure, 0.028316846592000004)]  //Cubic foot per second = Cubic meter per second	 		1 ft³/s = 0.028316847 m³/s
        [InlineData(1.0, "ft lbf", IfcValue.IfcTorqueMeasure, 1.3558180656)]  //Foot pound torque X 1.356 = Newton meter	N-m
        [InlineData(1.0, "kip ft", IfcValue.IfcTorqueMeasure, 1355.8180656000002)]  //Kip foot X 1.355 = Kilonewton meter	LN-m
        [InlineData(1.0, "kip / in2", IfcValue.IfcPressureMeasure, 6894757.889515781)]  //Kip per square inch X 6.89 = Megapascal	MPa	
        [InlineData(1.0, "cm2", IfcValue.IfcAreaMeasure, 0.0001)]
        [InlineData(1.0, "mol", IfcValue.IfcAmountOfSubstanceMeasure, 1)]
        [InlineData(1.0, "kg", IfcValue.IfcMassMeasure, 1)]
#if true
        [InlineData(1.0, "°C", IfcValue.IfcThermodynamicTemperatureMeasure, 274.15)]
        [InlineData(1.0, "°F", IfcValue.IfcThermodynamicTemperatureMeasure, 255.9277777777778)]
        [InlineData(12.0, "°F/s", IfcValue.IfcTemperatureRateOfChangeMeasure, 6.666666666666667)]
        [InlineData(5.0, "m2 / s2 °F", IfcValue.IfcSpecificHeatCapacityMeasure, 9)]
        [InlineData(5.0, "J / kg °F", IfcValue.IfcSpecificHeatCapacityMeasure, 9)]
#endif
        public void CheckUnit(double originalUnit, string complexUnitString, IfcValue expectedMeasure, double expected)
        {
            MeasureUnit sourceUnit = new (complexUnitString);
            IfcMeasureInfo t = SchemaInfo.IfcMeasures[expectedMeasure.ToString()];
            t.Should().NotBeNull("library should be complete.");

            var sourceUnitExponent = sourceUnit.Exponent;
            var measureExponent = t.Exponents;
            sourceUnitExponent.Should().Be(measureExponent);
            sourceUnit.TryConvertToSI(originalUnit, out var convertedToSI).Should().Be(true);
            convertedToSI.Should().Be(expected, $"source is {originalUnit} {complexUnitString} (to {t.GetUnit()})");

            sourceUnit.TryConvertFromSI(convertedToSI, out var cnvBack).Should().Be(true);
            cnvBack.Should().BeApproximately(originalUnit, 1.0E-07, "converting back with tolerance should be possible.");
        }


        public static IEnumerable<object[]> GetMeasures => Enum.GetValues<IfcValue>().Select(x => new object[] { x }).ToArray();

        [Theory]
        [MemberData(nameof(GetMeasures))]
        public void VerifyMeasureUnit(IfcValue item)
        {
            var measUnit = SchemaInfo.GetMeasure(item);
            if (measUnit is DirectValue)
                return; // todo: do we need to extend this test?
            if (measUnit.Exponents is null)
                return;
            var standard = measUnit.GetUnit();
            var works = new MeasureUnit(standard);
            works.IsValid.Should().BeTrue($"'{standard}' is expected to be equivalent to '{measUnit.Exponents.ToUnitSymbol()}'");
        }
	}
}
