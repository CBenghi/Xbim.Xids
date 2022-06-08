using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.InformationSpecifications.Generator.Measures;
using Xbim.InformationSpecifications.Helpers;
using Xbim.InformationSpecifications.Helpers.Measures;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.Helpers
{
    public class MeasureHelpers
    {
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
                    Debug.WriteLine($"{meas.ID} {meas.Exponents} - {meas.Exponents.ToUnitSymbol()}");
                }
            }
        }

        //Foot per second squared  to Meter per second squared	 		1 ft² = 0. 3048 m²       
        [Theory]
        [InlineData("ft", IfcMeasures.Length, 0.3048)]
        [InlineData("ft2", IfcMeasures.Area, 0.09290304)]//Square foot to Square meter	 		1 ft² = 0.092903 m²
        [InlineData("ft3", IfcMeasures.Volume, 0.028316846592000004)] //Cubic foot to  Cubic meter	 		1 ft³ = 0.028316 m³
        [InlineData("lb / in3", IfcMeasures.MassDensity, 27680.370321370567)] //Pound per cubic inch to Kilogram per cubic meter	 		1 lb/in³ = 27679.9 047102 kg/m³
        [InlineData("gal / min", IfcMeasures.VolumetricFlowRate, 6.309033333333333E-05)] //Gallon per minute X to m3 / s
        [InlineData("lb / ft2", IfcMeasures.AreaDensity, 4.88250976501953)]
        [InlineData("acre ft / day", IfcMeasures.VolumetricFlowRate, 0.014276467263055554)] //Acre foot per day = Cubic meter per second	 		1 Acre foot per day= 0.01428 m^3/s (cubic meters per second) (wolfram)
        [InlineData("lbf / ft2", IfcMeasures.Pressure, 47.880263121637356)]   //Pound per Square Foot to Pascal	 		1 lbf/ft2 = 47.88025 Pascal
        [InlineData("lbf / in2", IfcMeasures.Pressure, 6894.75788951578)]   //Pound per square inch X 6.894 = Kilopascal	KPa
        [InlineData("°F", IfcMeasures.Temperature, 255.9277777777778)]
        [InlineData("°C", IfcMeasures.Temperature, 274.15)]
        [InlineData("lbf", IfcMeasures.Force, 4.448222)] //Pound force = Newton	 		1 Pound force = 4.448222 Newton
        [InlineData("acre", IfcMeasures.Area, 4046.87261)] //Acre to square meter			1 acre = 4046.856 m²
        [InlineData("ft3/sec", IfcMeasures.VolumetricFlowRate, 0.028316846592000004)]  //Cubic foot per second = Cubic meter per second	 		1 ft³/s = 0.028316847 m³/s
        [InlineData("ft lbf", IfcMeasures.Torque, 1.3558180656)]  //Foot pound torque X 1.356 = Newton meter	N-m
        [InlineData("kip ft", IfcMeasures.Torque, 1355.8180656000002)]  //Kip foot X 1.355 = Kilonewton meter	LN-m
        [InlineData("kip / in2", IfcMeasures.Pressure, 6894757.889515781)]  //Kip per square inch X 6.89 = Megapascal	MPa	
        [InlineData("cm2", IfcMeasures.Area, 0.0001)]  
        //[InlineData("ft / s2", IfcMeasures., 6894757.889515781)]  //Kip per square inch X 6.89 = Megapascal	MPa	
        public void CheckUnit(string complexUnit, IfcMeasures measure, double expected = 1)
        {
            MeasureUnit sourceUnit = new MeasureUnit(complexUnit);
            var t = SchemaInfo.IfcMeasures[measure.ToString()];
            sourceUnit.Exponent.Should().BeEquivalentTo(t.Exponents);
            sourceUnit.TryConvertToSI(1, out var convertedToSI).Should().Be(true);
            convertedToSI.Should().Be(expected, $"source is {complexUnit} (to {t.GetUnit()})");

            sourceUnit.TryConvertFromSI(convertedToSI, out var cnvBack).Should().Be(true); 
            cnvBack.Should().Be(1.0);
        }
    }
}
