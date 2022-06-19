using Microsoft.Extensions.Logging;
using System;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Xbim.InformationSpecifications
{
    public class PatternConstraint : IValueConstraint, IEquatable<PatternConstraint>
    {
        private Regex? compiledCaseSensitiveRegex;
        private Regex? compiledCaseInsensitiveRegex;

        private string pattern = String.Empty;

        public string Pattern
        {
            get { return pattern; }
            set
            {
                compiledCaseSensitiveRegex = null;
                pattern = value;
            }
        }

        public override int GetHashCode()
        {
            return (Pattern, true).GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj as PatternConstraint);
        }

        public bool Equals(PatternConstraint? other)
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
                try
                {
                    // we are running the test omitting boundaries that 
                    // might alter the nature of the result
                    // e.g. repetition on the initial boundary
                    var mod = XmlRegex.Preprocess(pattern, true);
                    var r = new Regex(mod);
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }

        [JsonIgnore]
        public string PatternError
        {
            get
            {
                try
                {
                    var mod = XmlRegex.Preprocess(pattern, true);
                    var r = new Regex(mod);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                return "";
            }
        }

        public bool IsSatisfiedBy(object candiatateValue, ValueConstraint context, bool ignoreCase, ILogger? logger = null)
        {
            if (ignoreCase)
            {
                if (!EnsureRegex(out var _, ignoreCase, logger))
                    return false;
                if (compiledCaseInsensitiveRegex is null) // this should never be the case
                {
                    logger?.LogError("CaseInsensitiveRegex was unexpectedly null for pattern {pattern}.", pattern);
                    return false;
                }
                var str = candiatateValue.ToString();
                if (str is null)
                    return false;
                return compiledCaseInsensitiveRegex.IsMatch(str);
            }
            else
            {
                if (!EnsureRegex(out var _, ignoreCase, logger))
                    return false;
                if (compiledCaseSensitiveRegex is null) // this should never be the case
                {
                    logger?.LogError("CaseSensitiveRegex was unexpectedly null for pattern {pattern}.", pattern);
                    return false;
                }
                var str = candiatateValue.ToString();
                if (str is null)
                    return false;
                return compiledCaseSensitiveRegex.IsMatch(str);
            }
        }

        private bool EnsureRegex(out string errorMessage, bool ignoreCase, ILogger? logger = null)
        {
            errorMessage = "";
            if (
                (ignoreCase && compiledCaseInsensitiveRegex != null)
                ||
                (!ignoreCase && compiledCaseSensitiveRegex != null)
                )
                return true;

            if (string.IsNullOrEmpty(Pattern))
            {
                errorMessage = "Invalid null pattern constraint";
                logger?.LogError(errorMessage);
                return false;
            }

            try
            {
                var preProcess = XmlRegex.Preprocess(Pattern);
                if (!ignoreCase)
                    compiledCaseSensitiveRegex = new Regex(preProcess, RegexOptions.Compiled | RegexOptions.Singleline);
                else
                    compiledCaseInsensitiveRegex = new Regex(preProcess, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                logger?.LogError("Invalid pattern constraint: {pattern}", Pattern);
                return false;
            }
        }

        public override string ToString()
        {
            if (pattern == null)
                return $"Pattern: <null>";
            return $"Pattern: '{Pattern}'";
        }

        public string Short()
        {
            return $"matches the pattern: '{Pattern}'";
        }
    }
}
