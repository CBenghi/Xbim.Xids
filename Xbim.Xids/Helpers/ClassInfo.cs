using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Xids.Helpers
{

	public enum ClassType
	{
		Abstract,
		Concrete,
		Enumeration
	}

	public partial class SchemaInfo: IEnumerable<ClassInfo>
	{
		Dictionary<string, ClassInfo> Classes;
		bool linked = false;

		public void Add(ClassInfo classToAdd)
		{
			linked = false;
			if (Classes == null)
				Classes = new Dictionary<string, ClassInfo>();
			Classes.Add(classToAdd.Name, classToAdd);
		}

		private void LinkTree()
		{
			foreach (var currClass in Classes.Values)
			{
				var parent = currClass.ParentName;
				if (!string.IsNullOrWhiteSpace(parent))
				{
					if (Classes.TryGetValue(parent, out var gotten))
					{
						if (!gotten.SubClasses.Where(x=>x.Name == currClass.Name).Any())
						{
							gotten.SubClasses.Add(currClass);
						}
					}
				}
			}
			linked = true;
		}

		public static SchemaInfo schemaIFC4;
		public static SchemaInfo SchemaIfc4
		{
			get
			{
				if (schemaIFC4 == null)
					GetClassesIFC4();
				return schemaIFC4;
			}
		}

		public static SchemaInfo schemaIFC2x3;
		public static SchemaInfo SchemaIfc2x3
		{
			get
			{
				if (schemaIFC2x3 == null)
					GetClassesIFC2x3();
				return schemaIFC2x3;
			}
		}

		static partial void GetClassesIFC2x3();
		static partial void GetClassesIFC4();

		public IEnumerator<ClassInfo> GetEnumerator()
		{
			if (!linked)
				LinkTree();
			return Classes.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (!linked)
				LinkTree();
			return Classes.Values.GetEnumerator();
		}
	}

	public partial class ClassInfo
	{	
		public string Name { get; set; }
		public string ParentName { get; set; }
		public ClassType Type { get; set; }

		public List<ClassInfo> SubClasses = new List<ClassInfo>();

		public ClassInfo(string name, string parentName, ClassType type)
		{
			Name = name;
			ParentName = parentName;
			Type = type;
		}
	}
}
