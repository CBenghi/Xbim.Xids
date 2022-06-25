using Microsoft.Extensions.Logging;
using System;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// A constraint component based on a regex-like pattern
    /// </summary>
    public class PatternConstraint : IValueConstraintComponent, IEquatable<PatternConstraint>
    {
        private Regex? compiledCaseSensitiveRegex;
        private Regex? compiledCaseInsensitiveRegex;

        private string pattern = string.Empty;
        
        /// <summary>
        /// Default empty constructor, pattern is set to empty string
        /// </summary>
        public PatternConstraint()
        {

        }

        /// <summary>
        /// Fully qualified constructor 
        /// </summary>
        /// <param name="pattern">defines the <see cref="Pattern"/> of valid values</param>
        public PatternConstraint(string pattern)
        {
            Pattern = pattern;
        }

        /// <summary>
        /// Defines the patter for the evaluator
        /// </summary>
        public string Pattern
        {
            get { return pattern; }
            set
            {
                compiledCaseSensitiveRegex = null;
                pattern = value;
            }
        }
        
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (Pattern, true).GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return base.Equals(obj as PatternConstraint);
        }

        /// <inheritdoc />
        public bool Equals(PatternConstraint? other)
        {
            if (other == null)
                return false;
            // using true to exploit feature of tuples
            return (Pattern, true).Equals((other.Pattern, true));
        }

        /// <summary>
        /// Helper method to provide feedback to see if the pattern is valid
        /// </summary>
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

        /// <summary>
        /// Helper providing an error string in case of pattern is not valid
        /// </summary>
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override string ToString()
        {
            if (pattern == null)
                return $"Pattern: <null>";
            return $"Pattern: '{Pattern}'";
        }

        /// <inheritdoc />
        public string Short()
        {
            return $"matches the pattern: '{Pattern}'";
        }

        /// <inheritdoc />
        public bool IsValid(ValueConstraint context)
        {
            if (pattern == null)
                return false;
            return IsValidPattern;
        }
    }
}
