using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xbim.InformationSpecifications.Helpers
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

		public ClassInfo this[string className]
		{
			get
			{
				if (Classes.TryGetValue(className, out var cl))
				{
					return cl;
				}
				return Classes.Values.FirstOrDefault(x => x.Name.Equals(className, StringComparison.InvariantCultureIgnoreCase));
			}
		}

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
				if (!string.IsNullOrWhiteSpace(parent) && Classes.TryGetValue(parent, out var gotten))
				{
					if (!gotten.SubClasses.Any(x => x.Name == currClass.Name))
					{
						gotten.SubClasses.Add(currClass);
					}
					currClass.Parent = gotten;
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
		public string Name { get; private set; }
		public string ParentName { get; private set; }
		public ClassType Type { get; private set; }
		public ClassInfo Parent { get; internal set; }

		public IEnumerable<string> PredefinedTypeValues { get; private set; }

		public bool Is(string className)
		{
			if (Name.Equals(className, StringComparison.InvariantCultureIgnoreCase))
				return true;
			if (Parent != null)
				return Parent.Is(className);
			return false;
		}

		public List<ClassInfo> SubClasses = new List<ClassInfo>();
		public IEnumerable<ClassInfo> MatchingConcreteClasses
		{
			get
			{
				if (Type == ClassType.Concrete)
					yield return this;
				foreach (var item in SubClasses)
				{
					foreach (var sub in item.MatchingConcreteClasses)
					{
						yield return sub;
					}
				}
			}
		}

		public ClassInfo(string name, string parentName, ClassType type, IEnumerable<string> predefined)
		{
			Name = name;
			ParentName = parentName;
			Type = type;
			PredefinedTypeValues = predefined;
		}
	}
}
