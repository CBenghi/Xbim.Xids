using IdsLib.IfcSchema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.InformationSpecifications.Helpers.Measures
{
	/// <summary>
	/// Interface to extract values to be evaluated
	/// </summary>
	public interface IValueProvider
	{
		/// <summary>
		/// The string ID found in the XML persistence
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Dimensional exponents useful for conversion to other units.
		/// </summary>
		public DimensionalExponents Exponents { get; }

		/// <summary>
		/// Retuns the SI preferred unit.
		/// </summary>
		/// <returns>empty string for measures that do not have expected measures</returns>
		public string GetUnit();

		/// <summary>
		/// A textual description, e.g. "Amount of substance"
		/// </summary>
		public string Description { get; }
	}
}
