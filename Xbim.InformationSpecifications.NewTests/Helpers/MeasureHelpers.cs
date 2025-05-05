using FluentAssertions;
using IdsLib.IfcSchema;
using Microsoft.Extensions.Logging;
using NSubstitute.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Xbim.InformationSpecifications.Generator.Measures;
using Xbim.InformationSpecifications.Helpers;
using Xbim.InformationSpecifications.Helpers.Measures;
using Xunit;
using Xunit.Abstractions;
using SchemaInfo = IdsLib.IfcSchema.SchemaInfo;

namespace Xbim.InformationSpecifications.Tests.Helpers
{
    public partial class MeasureHelpers
    {
		public MeasureHelpers(ITestOutputHelper outputHelper)
		{
			OutputHelper = outputHelper;
			log = LoggingTestHelper.GetXunitLogger<MeasureHelpers>(OutputHelper);
		}
		private ITestOutputHelper OutputHelper { get; }
		ILogger<MeasureHelpers> log;

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
                var t = SchemaInfo.AllMeasureInformation.Where(x => x.Exponents is not null && x.Exponents.GetExponent(item) != 0).ToList();
                t.Should().NotBeEmpty();
            }

            foreach (var item in Enum.GetValues<DimensionType>())
            {
                Debug.WriteLine($"=== {item}");
                var t = SchemaInfo.AllMeasureInformation.Where(x => x.Exponents is not null && x.Exponents.Equals(DimensionalExponents.GetUnit(item))).ToList();
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

		[Theory]
		[InlineData("kg2", "kg2")]
		[InlineData("ft", "")]
		[InlineData("ft2", "")]
		[InlineData("ft2 ", "ft2")]
		[InlineData("ft²", "ft2")]
		[InlineData("ft1", "ft")]
		[InlineData("ft-1", "ft-1")]
		public void CorrectlyFormatUnitFactor(string startingString, string expectedRebuilt)
		{
			if (expectedRebuilt == "")
				expectedRebuilt = startingString;
			foreach (var factor in UnitFactor.SymbolBreakDown(startingString))
			{
				var rebuilt = factor.ToString();
				rebuilt.Should().BeEquivalentTo(expectedRebuilt);
			}
		}


        [Fact]
        public void Unit()
        {
            var sourceString = "°F";
            Regex r = UnitChars();
            var t = r.IsMatch(sourceString);
            t.Should().BeTrue();

            var _ = new MeasureUnit(sourceString);

            var t2 = SchemaInfo.AllMeasureInformation.Where(x=>x.IfcMeasure == "IfcThermodynamicTemperatureMeasure".ToUpperInvariant());
            t2.Should().NotBeNull("library should be complete.");
        }

		[GeneratedRegex("[°'a-zA-Z]+")]
		private static partial Regex UnitChars();


		//Foot per second squared to Meter per second squared 1 ft² = 0. 3048 m² - acceleration is not defined
		[Theory]
		[InlineData("IfcAmountOfSubstanceMeasure", "mol", 1.0, 1)]
		[InlineData("IfcAreaDensityMeasure", "lb / ft2", 1.0, 4.882427636383051)]
		[InlineData("IfcAreaMeasure", "acre", 1.0, 4046.8564224)] //Acre to square meter1 acre = 4046.856 m²
		[InlineData("IfcAreaMeasure", "cm2", 1.0, 0.0001)]
		[InlineData("IfcAreaMeasure", "ft2", 1.0, 0.09290304)]//Square foot to Square meter 1 ft² = 0.092903 m²
		[InlineData("IfcAreaMeasure", "ha", 1.0, 10000)] // Hectare to square meter: 1 ha = 10,000 m²
		[InlineData("IfcForceMeasure", "dyn", 1.0, 1E-5)] // Dyne to Newton: 1 dyn = 1E-5 N  dyne is a derived unit of force specified in the centimetre–gram–second (CGS) system of units, a predecessor of the modern SI
		[InlineData("IfcForceMeasure", "lbf", 1.0, 4.4482216152605)] //Pound force = Newton 1 Pound force = 4.448222 Newton
		[InlineData("IfcForceMeasure", "N", 1.0, 1.0)] // Newton to Newton: 1 N = 1 N
		[InlineData("IfcLengthMeasure", "''", 1.0, 0.0254)] // Inch to meter: 1 in = 0.0254 m
		[InlineData("IfcLengthMeasure", "'", 2.0, 0.6096)]
		[InlineData("IfcLengthMeasure", "\"", 1.0, 0.0254)] // Inch to meter: 1 in = 0.0254 m
		[InlineData("IfcLengthMeasure", "ft", 1.0, 0.3048)]
		[InlineData("IfcLengthMeasure", "ft", 2.0, 0.6096)]
		[InlineData("IfcLengthMeasure", "in", 1.0, 0.0254)] // Inch to meter: 1 in = 0.0254 m
		[InlineData("IfcLengthMeasure", "yd", 1.0, 0.9144)] // Yard to meter: 1 yd = 0.9144 m
		[InlineData("IfcMassDensityMeasure", "lb / in3", 1.0, 27679.904710203125)] //Pound per cubic inch to Kilogram per cubic meter 1 lb/in³ = 27679.9 047102 kg/m³
		[InlineData("IfcMassMeasure", "g", 1.0, 0.001)] // Gram to kilogram: 1 g = 0.001 kg
		[InlineData("IfcMassMeasure", "kg", 1.0, 1)]
		[InlineData("IfcMassMeasure", "ton", 1.0, 907.18474)] // US ton to kilogram: 1 ton = 907.18474 kg
		[InlineData("IfcPressureMeasure", "bar", 1.0, 100000)] // Bar to Pascal: 1 bar = 100,000 Pa
		[InlineData("IfcPressureMeasure", "kip / in2", 1.0, 6894757.293168361)]  //Kip per square inch X 6.89 = MegapascalMPa
		[InlineData("IfcPressureMeasure", "lbf / ft2", 1.0, 47.88025898033584)]   //Pound per Square Foot to Pascal 1 lbf/ft2 = 47.88025 Pascal
		[InlineData("IfcPressureMeasure", "lbf / in2", 1.0, 6894.757293168362)]   //Pound per square inch X 6.894 = KilopascalKPa
		[InlineData("IfcPressureMeasure", "Pa", 1.0, 1.0)] // Pascal to Pascal: 1 Pa = 1 Pa
		[InlineData("IfcSpecificHeatCapacityMeasure", "J / kg °F", 5.0, 9)]
		[InlineData("IfcSpecificHeatCapacityMeasure", "m2 / s2 °F", 5.0, 9)]
		[InlineData("IfcTemperatureRateOfChangeMeasure", "°F/s", 12.0, 6.666666666666667)]
		[InlineData("IfcThermodynamicTemperatureMeasure", "°C", 1.0, 274.15)]
		[InlineData("IfcThermodynamicTemperatureMeasure", "°F", 1.0, 255.9277777777778)]
		[InlineData("IfcThermodynamicTemperatureMeasure", "°R", 1.0, 0.5555555555555556)] // Rankine to Kelvin: 1 °R = 5/9 K
		[InlineData("IfcThermodynamicTemperatureMeasure", "K", 1.0, 1.0)] // Kelvin to Kelvin: 1 K = 1 K
		[InlineData("IfcTorqueMeasure", "ft lbf", 1.0, 1.3558179483314003)]  //Foot pound torque X 1.356 = Newton meterN-m
		[InlineData("IfcTorqueMeasure", "kgf·m", 1.0, 9.80665)] // Kilogram-force meter to Newton meter: 1 kgf·m = 9.80665 Nm
		[InlineData("IfcTorqueMeasure", "kip ft", 1.0, 1355.8179483314004)]  //Kip foot X 1.355 = Kilonewton meterLN-m
		[InlineData("IfcTorqueMeasure", "N m", 1.0, 1.0)] // Newton meter to Newton meter: 1 Nm = 1 Nm
		[InlineData("IfcVolumeMeasure", "ft3", 1.0, 0.028316846592000004)] //Cubic foot to  Cubic meter 1 ft³ = 0.028316 m³
		[InlineData("IfcVolumeMeasure", "in3", 1.0, 1.6387064E-5)] // Cubic inch to cubic meter: 1 in³ = 1.6387064E-5 m³
		[InlineData("IfcVolumetricFlowRateMeasure", "acre ft / day", 1.0, 0.014276410156800002)] //Acre foot per day = Cubic meter per second 1 Acre foot per day= 0.01428 m^3/s (cubic meters per second) (wolfram)
		[InlineData("IfcVolumetricFlowRateMeasure", "ft3/sec", 1.0, 0.028316846592000004)]  //Cubic foot per second = Cubic meter per second 1 ft³/s = 0.028316847 m³/s
		[InlineData("IfcVolumetricFlowRateMeasure", "gal / min", 1.0, 6.30901964E-05)] //Gallon per minute X to m3 / s
		[InlineData("IfcVolumetricFlowRateMeasure", "L/min", 1.0, 1.6666666666666667E-5)] // Liter per minute to cubic meter per second: 1 L/min = 1.6666666666666667E-5 m³/s
		[InlineData("IfcVolumetricFlowRateMeasure", "m3/h", 1.0, 0.0002777777777777778)] // Cubic meter per hour to cubic meter per second: 1 m³/h = 0.0002777777777777778 m³/s
		public void CheckUnit(string expectedMeasure, string complexUnitString, double originalUnit, double expected)
		{
			var computedSourceUnit = new MeasureUnit(complexUnitString, log);
			computedSourceUnit.IsValid.Should().BeTrue($"units need to be hardcoded for `{complexUnitString}`");

			var systemMeasureInfo = GetMeasureInfo(expectedMeasure.ToString());
			systemMeasureInfo.Should().NotBeNull("library should be complete.");

			var computedSourceUnitExponent = computedSourceUnit.Exponent;
			var systemMeasureExponent = systemMeasureInfo.Exponents;
			computedSourceUnitExponent.Should().Be(systemMeasureExponent);
			computedSourceUnit.TryConvertToSI(originalUnit, out var convertedToSI).Should().Be(true);
			convertedToSI.Should().Be(expected, $"source is {originalUnit} {complexUnitString} (to {systemMeasureInfo.GetUnit()})");

			computedSourceUnit.TryConvertFromSI(convertedToSI, out var cnvBack).Should().Be(true);
			cnvBack.Should().BeApproximately(originalUnit, 1.0E-07, "converting back with tolerance should be possible.");
		}

		private static IfcMeasureInformation? GetMeasureInfo(string expectedMeasure)
		{
			return SchemaInfo.AllMeasureInformation.FirstOrDefault(x=> x.IfcMeasure.Equals(expectedMeasure, StringComparison.OrdinalIgnoreCase));
		}
	}
}
