using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Xbim.InformationSpecifications.Helpers
{
	/// <summary>
	/// part of the serialization solution at for <see cref="HeterogenousListConverter{TItem, TList}"/>
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	public class ReversibleLookup<T1, T2> : ReadOnlyDictionary<T1, T2>
		where T1 : notnull
		where T2 : notnull
	{

		public ReversibleLookup(params (T1, T2)[] mappings)
		: base(new Dictionary<T1, T2>())
		{
			ReverseLookup = new ReadOnlyDictionary<T2, T1>(reverseLookup);

			foreach (var mapping in mappings)
				Add(mapping.Item1, mapping.Item2);
		}

		private readonly Dictionary<T2, T1> reverseLookup = new();
		public ReadOnlyDictionary<T2, T1> ReverseLookup { get; }

		[DebuggerHidden]
		public void Add(T1 value1, T2 value2)
		{
			if (ContainsKey(value1))
				throw new InvalidOperationException($"{nameof(value1)} is not unique");

			if (ReverseLookup.ContainsKey(value2))
				throw new InvalidOperationException($"{nameof(value2)} is not unique");

			Dictionary.Add(value1, value2);
			reverseLookup.Add(value2, value1);
		}

		public void Clear()
		{
			Dictionary.Clear();
			reverseLookup.Clear();
		}
	}
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member