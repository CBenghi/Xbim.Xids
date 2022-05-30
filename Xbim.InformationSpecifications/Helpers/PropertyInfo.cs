using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.InformationSpecifications.Helpers
{
    public interface IPropertyTypeInfo
    {
        public string Name { get; }
        public string Definition { get; set; } 
    }

    public class NamedPropertyType : IPropertyTypeInfo
    {
        public string Name { get; }
        public string Definition { get; set; } = string.Empty;

        public NamedPropertyType(string name)
        {
            Name = name;
        }
    }

    public class EnumerationPropertyType : NamedPropertyType
    {
        public IList<string> EnumerationValues { get; }

        public EnumerationPropertyType(string name, IEnumerable<string> values) : base(name)
        {
            EnumerationValues = values.ToList();
        }
    }

    public class SingleValuePropertyType : NamedPropertyType
    {
        public SingleValuePropertyType(string name, string dataType) : base(name)
        {           
            DataType = dataType;
        }

        public string DataType { get; }
    }
}
