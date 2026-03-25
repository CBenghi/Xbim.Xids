using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Xbim.InformationSpecifications.Helpers
{
	/// <summary>
	/// Provides utility methods for converting between enum values and their XML string representations using the
	/// XmlEnumAttribute.
	/// </summary>
	/// <remarks>This class is intended to simplify working with enums that use the XmlEnumAttribute for custom XML
	/// serialization names. It supports parsing XML strings to enum values and retrieving the XML string representation
	/// for a given enum value. All methods are static and thread-safe.</remarks>
	public static class EnumHelper
	{
		/// <summary>
		/// Parses a string value to its corresponding enumeration value, matching either the value of the XmlEnumAttribute or
		/// the enum field name.
		/// </summary>
		/// <remarks>This method is useful when working with XML-serialized enums that use the XmlEnumAttribute to
		/// specify custom string values. If no XmlEnumAttribute is present or matched, the method falls back to matching the
		/// enum field name.</remarks>
		/// <typeparam name="T">The enumeration type to parse the value into. Must be a struct and an Enum.</typeparam>
		/// <param name="value">The string representation of the enumeration value. This can be either the value specified in the XmlEnumAttribute
		/// or the name of the enum field.</param>
		/// <returns>The enumeration value of type T that corresponds to the specified string.</returns>
		/// <exception cref="ArgumentException">Thrown if the specified value does not match any XmlEnumAttribute value or enum field name in the target
		/// enumeration type.</exception>
		public static T ParseFromXmlEnum<T>(string value) where T : struct, Enum
		{
			var type = typeof(T);

			foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
			{
				var attr = field.GetCustomAttribute<XmlEnumAttribute>();

				if (attr != null && attr.Name == value)
					return (T)field.GetValue(null)!;

				// Optional fallback: match enum name
				if (field.Name == value)
					return (T)field.GetValue(null)!;
			}

			throw new ArgumentException($"Unknown value '{value}' for enum {type.Name}");
		}

		/// <summary>
		/// Attempts to parse the specified string as a value of the specified enumeration type, matching either the field
		/// name or the value of the XmlEnumAttribute, if present.
		/// </summary>
		/// <remarks>This method supports parsing both the standard enum field names and custom values defined by the
		/// XmlEnumAttribute. It is case-sensitive and does not throw exceptions for invalid input.</remarks>
		/// <typeparam name="T">The enumeration type to parse the value into. Must be a struct and an Enum.</typeparam>
		/// <param name="value">The string representation of the enumeration value to parse. This can be either the name of the enum field or the
		/// value specified in the XmlEnumAttribute.</param>
		/// <param name="result">When this method returns, contains the parsed enumeration value if the parse operation succeeds; otherwise, the
		/// default value for the type.</param>
		/// <returns>true if the value was successfully parsed into the specified enumeration type; otherwise, false.</returns>
		public static bool TryParseFromXmlEnum<T>(string value, out T result) where T : struct, Enum
		{
			var type = typeof(T);

			foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
			{
				var attr = field.GetCustomAttribute<XmlEnumAttribute>();

				if ((attr != null && attr.Name == value) || field.Name == value)
				{
					result = (T)field.GetValue(null)!;
					return true;
				}
			}

			result = default;
			return false;
		}

		/// <summary>
		/// Returns the XML representation of the specified enumeration value, using the value defined by the XmlEnumAttribute
		/// if present.
		/// </summary>
		/// <remarks>This method is useful when serializing enumeration values to XML, ensuring that any custom names
		/// specified by the XmlEnumAttribute are respected. If the enumeration value does not have an XmlEnumAttribute, the
		/// method returns the standard name of the value.</remarks>
		/// <typeparam name="T">The enumeration type whose value is to be converted.</typeparam>
		/// <param name="value">The enumeration value to convert to its XML string representation.</param>
		/// <returns>A string containing the XML name for the enumeration value if an XmlEnumAttribute is applied; otherwise, the
		/// default name of the enumeration value.</returns>
		public static string ToXmlEnumString<T>(this T value) where T : Enum
		{
			var type = typeof(T);
			var name = value.ToString();

			var field = type.GetField(name);
			var attr = field?.GetCustomAttribute<XmlEnumAttribute>();

			return attr?.Name ?? name;
		}
	}
}
