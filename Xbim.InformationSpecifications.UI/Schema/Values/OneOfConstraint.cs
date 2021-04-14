using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.IDS
{
	public class OneOfConstraint : IValueConstraint, IEquatable<OneOfConstraint>
	{
		public List<IValueConstraint> List { get; set; }

		public bool Equals(OneOfConstraint other)
		{
			if (other == null)
				return false;
			var comp = new Tools.MultiSetComparer<IValueConstraint>();
			return comp.Equals(this.List, other.List);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as OneOfConstraint);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public bool IsValid(object testObject)
		{
			foreach (var item in List)
			{
				if (item.IsValid(testObject))
					return true;
			}
			return false;
		}

		internal void AddOption(IValueConstraint tVal)
		{
			if (tVal != null)
			{
				if (List == null)
					List = new List<IValueConstraint>();
				List.Add(tVal);
			}
		}

		public override string ToString()
		{
			return string.Join("-", List);
		}
	}
}
