using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.InformationSpecifications.Generator.Measures
{
    internal class MeasureAutomation
    {
        static public IEnumerable<Measure> GetFromWiki()
        {
            var splitter = new string[] { "|" };
            FileInfo f = new FileInfo("Files/Physical-Quantities-and-Units.md");
            var allWiki = File.ReadAllLines(f.FullName);
            var isParsing = false;
            foreach (var oneLine in allWiki)
            {
                if (isParsing)
                {
                    var parts = oneLine.Split(splitter, StringSplitOptions.TrimEntries);
                    if (parts.Length < 7)
                        yield break; // no more measures to find.
                    var retMeasurement = new Measure()
                    {
                         PhysicalQuantity = parts[1],
                         Unit = parts[2],
                         UnitSymbol = parts[3],
                         IfcMeasure = parts[4],
                         DimensionalExponents = parts[5],
                         QUDT = parts[6],
                    };
                    yield return retMeasurement;
                }
                else
                {
                    if (oneLine.Contains("-----"))
                    {
                        isParsing = true;
                        continue;
                    }
                }
            }
        }

        public static string Execute()
        {
            MeasureCollection m = new MeasureCollection(MeasureAutomation.GetFromWiki());

            bool tryImprove = true;
            while (tryImprove)
            {
                tryImprove = false;
                foreach (var missingExp in m.MeasureList.Where(x=>x.DimensionalExponents == ""))
                {
                    var neededSymbols = SymbolBreakDown(missingExp.UnitSymbol);
                    var allSym = true;
                    foreach (var sym in neededSymbols)
                    {
                        var found = m.GetByUnit(sym.UnitSymbol);
                        if (found != null)
                        {
                            if (found.DimensionalExponents != "")
                            {
                                var tde = sym.GetDimensionalExponents(m);
                                Debug.WriteLine($"Found '{found.UnitSymbol}' - {found.DimensionalExponents} - {tde.ToUnitSymbol()}");
                            }
                            else
                            {
                                Debug.WriteLine($"Missing dimensional exponents on '{found.UnitSymbol}'");
                                allSym = false;
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"Missing '{sym.UnitSymbol}' - {missingExp.UnitSymbol}");
                            allSym = false;
                        }
                    }
                    if (allSym)
                    {
                        DimensionalExponents d = null;
                        Debug.WriteLine($"Can do {missingExp.PhysicalQuantity} - {missingExp.UnitSymbol}");
                        foreach (var sym in neededSymbols)
                        {
                            var found = m.GetByUnit(sym.UnitSymbol);
                            if (d == null)
                                d = sym.GetDimensionalExponents(m);
                            else
                                d = d.Multiply(sym.GetDimensionalExponents(m));
                        }
                        if (d != null)
                        {
                            Debug.WriteLine($"Computed: {d} - {d.ToUnitSymbol()}");
                            missingExp.DimensionalExponents = d.ToString();
                            tryImprove = true;
                        }
                        Debug.WriteLine("");

                    }
                    else
                    {
                        Debug.WriteLine($"Cannot do {missingExp.PhysicalQuantity} - {missingExp.UnitSymbol}\r\n");
                    }
                }
            }
            var sb = new StringBuilder();
            foreach (var item in m.MeasureList)
            {
                sb.AppendLine($"{item.PhysicalQuantity}\t{item.DimensionalExponents}");
            }
            Debug.WriteLine(sb.ToString());
            return sb.ToString();
        }



        private static IEnumerable<UnitFactor> SymbolBreakDown(string unitSymbol)
        {
            var fraction = unitSymbol.Split('/');
            var num = fraction[0];
            var den = fraction.Length == 2 ? fraction[1] : "";

            var numUnits = num.Split(' ');
            var denUnits = den.Split(' ');

            foreach (var item in numUnits.Where(x => x != ""))
                yield return  new UnitFactor(item);
            foreach (var item in denUnits.Where(x=>x!=""))
                yield return new UnitFactor(item).Invert();
        }
    }
}
