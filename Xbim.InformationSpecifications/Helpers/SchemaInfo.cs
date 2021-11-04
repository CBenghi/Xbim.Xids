using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xbim.InformationSpecifications.Helpers
{
	/// <summary>
	/// Provides static methods to get the collection of classe in the pubblished schemas.
	/// </summary>
	public partial class SchemaInfo: IEnumerable<ClassInfo>
	{
		/// <summary>
		/// Ensures that the information up to date.
		/// </summary>
		bool linked = false;
		private Dictionary<string, ClassInfo> Classes;

		/// <summary>
		/// from the attribute name to the names of the classes that have the attribute.
		/// </summary>
		private Dictionary<string, string[]> Attributes { get; set; }

		/// <summary>
		/// Get the classinfo by name string.
		/// </summary>
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

		/// <summary>
		/// Add a new classInfo to the collection
		/// </summary>
		public void Add(ClassInfo classToAdd)
		{
			linked = false;
			if (Classes == null)
				Classes = new Dictionary<string, ClassInfo>();
			Classes.Add(classToAdd.Name, classToAdd);
		}

		/// <summary>
		/// Ensure relationships between classes are correct.
		/// </summary>
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

		private static SchemaInfo schemaIFC4;
		/// <summary>
		/// Static property for the Ifc4 schema
		/// </summary>
		public static SchemaInfo SchemaIfc4
		{
			get
			{
				if (schemaIFC4 == null)
				{
					GetClassesIFC4();
					GetAttributesIFC4();
				}
				return schemaIFC4;
			}
		}

		public string[] GetAttributeClasses(string attributeName)
		{
			if (Attributes == null)
				return null;
			if (Attributes.TryGetValue(attributeName, out var ret))
				return ret;
			return null;
		}

		private static SchemaInfo schemaIFC2x3;
		/// <summary>
		/// Static property for the Ifc2x3 schema
		/// </summary>
		public static SchemaInfo SchemaIfc2x3
		{
			get
			{
				if (schemaIFC2x3 == null)
				{
					GetClassesIFC2x3();
					GetAttributesIFC2x3();
				}
				return schemaIFC2x3;
			}
		}

		static partial void GetClassesIFC2x3();

		public IEnumerable<string> GetAttributeNames()
		{
			return Attributes?.Keys;
		}

		static partial void GetClassesIFC4();

		static partial void GetAttributesIFC2x3();
		static partial void GetAttributesIFC4();

		private void AddAttribute(string attributeName, string[] classNames)
		{
			if (Attributes == null)
				Attributes = new Dictionary<string, string[]>();
			Attributes.Add(attributeName, classNames);
		}

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
}
