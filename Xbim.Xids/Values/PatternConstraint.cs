using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Xbim.Xids
{
	public class PatternConstraint : IValueConstraint, IEquatable<PatternConstraint>
	{
		private Regex compiledRegex;

		private string pattern;

		public string Pattern
		{
			get { return pattern; }
			set {
				compiledRegex = null;
				pattern = value;
			}
		}

		public override int GetHashCode()
		{
			return Pattern.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj as PatternConstraint);
		}

		public bool Equals(PatternConstraint other)
		{
			if (other == null)
				return false;
			// using true to use 
			return (Pattern, true).Equals((other.Pattern, true));
		}

		public bool IsValidPattern
		{
			get
			{
				return EnsureRegex();
			}
		}

		public bool IsSatisfiedBy(object testObject)
		{
			if (!EnsureRegex())
				return false;
			return compiledRegex.IsMatch(testObject.ToString());
		}

		private bool EnsureRegex()
		{
			if (compiledRegex != null)
				return true;			
			try
			{
				compiledRegex = new Regex(Pattern, RegexOptions.Compiled);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public override string ToString()
		{
			return $"Pattern:{Pattern}";
		}
	}
}
