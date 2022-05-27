using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.InformationSpecifications.Helpers
{
    public class DimensionalExponents
    {
        public int Length { get; set; } = 0;
        public int Mass { get; set; } = 0;
        public int Time { get; set; } = 0;
        public int ElectricCurrent { get; set; } = 0;
        public int Temperature { get; set; } = 0;
        public int AmountOfSubstance { get; set; } = 0;
        public int LuminousIntensity { get; set; } = 0;

        public override string ToString()
        {
            var asStringArray = ValuesAsArray().Select(x=>x.ToString()).ToArray();  
            return "(" + string.Join(", ", asStringArray) +")";
        }

        public void Elevate(int exponent)
        {
            Length *= exponent; 
            Mass *= exponent; 
            Time *= exponent; 
            ElectricCurrent *= exponent; 
            Temperature *= exponent; 
            AmountOfSubstance *= exponent; 
            LuminousIntensity *= exponent; 
        }

        public DimensionalExponents Multiply(DimensionalExponents other)
        {
            return new DimensionalExponents
                (
                Length + other.Length,
                Mass + other.Mass, 
                Time + other.Time, 
                ElectricCurrent + other.ElectricCurrent, 
                Temperature + other.Temperature, 
                AmountOfSubstance + other.AmountOfSubstance, 
                LuminousIntensity + other.LuminousIntensity
                );
        }


        public static DimensionalExponents? FromString(string str)
        {
            str = str.Trim();
            str = str.Trim('(');
            str = str.Trim(')');
#if NETSTANDARD2_0 
            var arr = str.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
#else
            var arr = str.Split(new string[] { "," }, System.StringSplitOptions.TrimEntries);
#endif
            if (arr.Length != 7)
                return null;
            var res = new int[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                if (!int.TryParse(arr[i], out res[i]))
                    return null;
            }
            return new DimensionalExponents(
                res[0],
                res[1],
                res[2],
                res[3],
                res[4],
                res[5],
                res[6]
                );
        }

        public DimensionalExponents(
            int length,
            int mass,
            int time,
            int electricCurrent,
            int temperature,
            int amountOfSsubstance,
            int luminousIntensity
            )
        {
            Length = length;
            Mass = mass;
            Time = time;
            ElectricCurrent = electricCurrent;
            Temperature = temperature;
            AmountOfSubstance = amountOfSsubstance;
            LuminousIntensity = luminousIntensity;
        }

        static string[] Units { get; } = new string[]
        {
            "m", "kg", "s", "A", "K", "mol", "cd"
        };

        public string ToUnitSymbol()
        {
            int[] asArray = ValuesAsArray();

            var numerator = GetMultiplier(asArray, true);
            var denominator = GetMultiplier(asArray, false);
            if (denominator == "1")
                return numerator;
            else return numerator + " / " + denominator;
        }

        private int[] ValuesAsArray()
        {
            return new[]
            {
                Length, Mass, Time, ElectricCurrent, Temperature, AmountOfSubstance, LuminousIntensity
            };
        }

        private string GetMultiplier(int[] asArray, bool numerator)
        {
            List<string> vals  = new();
            for (int i = 0; i < asArray.Length; i++)
            {
                if (asArray[i] == 0)
                    continue;
                if (asArray[i] > 0 == numerator)
                {
                    var abs = Math.Abs(asArray[i]);
                    if (abs == 1)
                        vals.Add(Units[i]);
                    else
                        vals.Add(Units[i] + abs);
                }
            }
            if (vals.Any())
                return string.Join(" ", vals.ToArray());
            else
                return "1";
        }
    }

}
