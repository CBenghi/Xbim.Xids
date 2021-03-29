using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Xbim.Xids.Helpers
{
	public partial class SchemaInfo
	{
		static partial void GetClassesIFC2x3()
		{
			schemaIFC2x3 = new SchemaInfo();
			schemaIFC2x3.Add(new ClassInfo("IfcProduct", "NotFound", ClassType.Abstract));
			schemaIFC2x3.Add(new ClassInfo("IfcElement", "IfcProduct", ClassType.Concrete));	
		}
		static partial void GetClassesIFC4()
		{
			schemaIFC4 = new SchemaInfo();
			schemaIFC4.Add(new ClassInfo("IfcProduct", "NotFound", ClassType.Abstract));
			schemaIFC4.Add(new ClassInfo("IfcElement", "IfcProduct", ClassType.Concrete));
		}
	}
}
