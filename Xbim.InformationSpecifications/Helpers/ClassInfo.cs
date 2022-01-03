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

	/// <summary>
	/// Contains information on relevant standard properties of IFC classes
	/// </summary>
	public partial class ClassInfo
	{	
		/// <summary>
		/// Name string
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// Parent name as string
		/// </summary>
		public string ParentName { get; private set; }
		public ClassType Type { get; private set; }
		/// <summary>
		/// Resolved parent Classinfo
		/// </summary>
		public ClassInfo Parent { get; internal set; }

		/// <summary>
		/// List of predefined type strings from the schema
		/// </summary>
		public IEnumerable<string> PredefinedTypeValues { get; private set; }

		/// <summary>
		/// Similar to the c# Is clause
		/// </summary>
		/// <param name="className">the class we are comparing against</param>
		public bool Is(string className)
		{
			if (Name.Equals(className, StringComparison.InvariantCultureIgnoreCase))
				return true;
			if (Parent != null)
				return Parent.Is(className);
			return false;
		}

		/// <summary>
		/// The namespace of the class
		/// </summary>
		public string NameSpace { get; internal set; }

		/// <summary>
		/// List of all subclasses.
		/// </summary>
		public List<ClassInfo> SubClasses = new List<ClassInfo>();
		
		
		/// <summary>
		/// All matching concrete classes, including self and entire subclass tree
		/// </summary>
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

		public ClassInfo(string name, string parentName, ClassType type, IEnumerable<string> predefined, string nameSpace)
		{
			Name = name;
			ParentName = parentName;
			Type = type;
			PredefinedTypeValues = predefined;
			NameSpace = nameSpace;
		}
	}
}
