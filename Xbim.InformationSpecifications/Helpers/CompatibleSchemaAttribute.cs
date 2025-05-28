using System;
using System.Linq;

namespace Xbim.InformationSpecifications
{
	/// <summary>
	/// Defines the type of Ifc schema compatible with a given element.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class CompatibleSchemaAttribute : Attribute
	{
		private readonly IfcSchemaVersion[] vs;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="validSchemas">Says so on the tin</param>
		public CompatibleSchemaAttribute(IfcSchemaVersion[] validSchemas)
		{
			this.vs = validSchemas;
		}

		/// <summary>
		/// Is the element valid for <paramref name="relevantSchema"/>?
		/// </summary>
		/// <param name="relevantSchema"></param>
		/// <returns>True if it is, false if not</returns>
		public bool IsCompatibleSchema(IfcSchemaVersion relevantSchema)
		{
			return vs.Contains(relevantSchema);
		}
	}

	/// <summary>
	/// schema compatibility helpers functions for specific elements
	/// </summary>
	public static class CompatibleSchemaHelpers
	{
		/// <summary>
		/// Given a Container, is it valid for a required schema?
		/// </summary>
		/// <param name="container">the container</param>
		/// <param name="requiredSchemaVersionString">the required schema as a string to be converted</param>
		/// <returns>True if certainly compatible, false otherwise</returns>
		public static bool IsCompatibleSchema(this PartOfFacet.Container container, string requiredSchemaVersionString)
		{
			if (Enum.TryParse<IfcSchemaVersion>(requiredSchemaVersionString, out var version))
				return IsCompatibleSchema(container, version);
			return false;
		}

		/// <summary>
		/// Given a Container, is it valid for a required schema?
		/// </summary>
		/// <param name="container">the container</param>
		/// <param name="requiredSchemaEnum">the required schema as enum</param>
		/// <returns>True if certainly compatible, false otherwise</returns>
		public static bool IsCompatibleSchema(this PartOfFacet.Container container, IfcSchemaVersion requiredSchemaEnum)
		{
			try
			{
				var enumType = typeof(PartOfFacet.Container);
				var memberInfos = enumType.GetMember(container.ToString());
				var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
				if (enumValueMemberInfo is null)
					return false;
				var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(CompatibleSchemaAttribute), false);
				CompatibleSchemaAttribute foundAttribute = (CompatibleSchemaAttribute)valueAttributes[0];
				return foundAttribute.IsCompatibleSchema(requiredSchemaEnum);
			}
			catch
			{
				return false;
			}
		}

	}
}