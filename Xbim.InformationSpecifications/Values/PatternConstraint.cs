using Microsoft.Extensions.Logging;
using System;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Xbim.InformationSpecifications
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
			return (Pattern, true).GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj as PatternConstraint);
		}

		public bool Equals(PatternConstraint other)
		{
			if (other == null)
				return false;
			// using true to exploit feature of tuples
			return (Pattern, true).Equals((other.Pattern, true));
		}

		[JsonIgnore]
		public bool IsValidPattern
		{
			get
			{
				return EnsureRegex();
			}
		}

		public bool IsSatisfiedBy(object candiatateValue, ValueConstraint context, ILogger logger = null)
		{
			if (!EnsureRegex(logger))
				return false;
			return compiledRegex.IsMatch(candiatateValue.ToString());
		}

		private bool EnsureRegex(ILogger logger = null)
		{
			if (compiledRegex != null)
				return true;			
			try
			{
				var preProcess = XmlRegex.Preprocess(Pattern);
				compiledRegex = new Regex(preProcess, RegexOptions.Compiled | RegexOptions.Singleline);
				return true;
			}
			catch (Exception)
			{
				logger?.LogError("Invalid pattern constraint: {pattern}", Pattern);
				return false;
			}
		}

		public override string ToString()
		{
			return $"Pattern:{Pattern}";
		}

		public string Short()
		{
			return $"matches the regular pattern: '{Pattern}'";
		}
	}
}
