using System.Collections.Generic;
using System.Linq;

namespace Xbim.InformationSpecifications.Generator.Measures
{
    public class MeasureCollection
    {
        public MeasureCollection(IEnumerable<Measure> initialCollection)
        {
            foreach (var item in initialCollection)
            {
                Add(item);
            }
        }

        List<Measure> measureList;
        public List<Measure> MeasureList
        {
            get
            {
                measureList ??= new List<Measure>();
                return measureList;
            }
        }

        public void Add(Measure meas)
        {
            if (meas == null)
                return;
            MeasureList.Add(meas);
        }

        public Measure GetByUnit(string unit)
        {
            // prioritise by units with dimensional exponent
            var t = MeasureList.FirstOrDefault(m => m.UnitSymbol == unit && m.DimensionalExponents != "");
            if (t != null)
                return t;
            return MeasureList.FirstOrDefault(m => m.UnitSymbol == unit);
        }

    }
}
