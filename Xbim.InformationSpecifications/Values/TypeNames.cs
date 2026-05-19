using IdsLib.IdsSchema.XsNodes;
using IdsLib.IfcSchema;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using Xbim.InformationSpecifications.Helpers;
using Xbim.InformationSpecifications.Values;

namespace Xbim.InformationSpecifications
{
	// to evaluate more XSD types see 
	// see https://www.w3.org/TR/xmlschema-2/#built-in-primitive-datatypes

	/// <summary>
	/// Type names enumeration for the value types as defined in the .NET framework. 
	/// 
	/// The underlying .NET type can be identified via <see cref="ValueConstraint.GetNetType(NetTypeName)"/> from the enumeration.
	/// 
	/// These can be converted from the underlying XSD types via the 
	/// <see cref="ValueConstraint.GetNamedTypeFromXsd(string?)"/> method.
	/// 
	/// These can also be obtained from the IFC measure name via the 
	/// <see cref="ValueConstraint.TryGetNetType(string?, out NetTypeName)"/> method, 
	/// which attempts to resolve an IFC data type name to the underlying .NET type name.
	/// </summary>
	public enum NetTypeName
	{
		/// <summary>
		/// No type constraint
		/// </summary>
		Undefined,
		/// <summary>
		/// Boolean values as defined in  the .NET framework
		/// </summary>
		Boolean,
		/// <summary>
		/// String values as defined in  the .NET framework
		/// </summary>
		String,
		/// <summary>
		/// Integer values as defined in  the .NET framework
		/// </summary>
		Integer,
		/// <summary>
		/// Floating values as defined in  the .NET framework
		/// </summary>
		Floating,
		/// <summary>
		/// Double values as defined in  the .NET framework
		/// </summary>
		Double,
		/// <summary>
		/// Boolean values as defined in  the .NET framework
		/// </summary>
		Decimal,
		/// <summary>
		/// Date values as defined in  the .NET framework
		/// </summary>
		Date,
		/// <summary>
		/// Time values as defined in  the .NET framework
		/// </summary>
		Time,
		/// <summary>
		/// DateTime values as defined in  the .NET framework
		/// </summary>
		DateTime,
		/// <summary>
		/// Duration values as defined in  the .NET framework
		/// </summary>
		Duration,
		/// <summary>
		/// Uri values as defined in  the .NET framework
		/// </summary>
		Uri,
	}

}
