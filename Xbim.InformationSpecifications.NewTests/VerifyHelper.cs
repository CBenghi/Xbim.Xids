using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VerifyTests;

namespace Xbim.InformationSpecifications.Tests
{
	public static class VerifyExtensions
	{
		private static readonly Regex DecimalRegex =
			new(@"(\d+)\.(\d*?)(0+)$", RegexOptions.Compiled | RegexOptions.Multiline);

		public static SettingsTask IgnoreTrailingZeros(this SettingsTask task) =>
			task.AddScrubber(ScrubTrailingZeros);

		public static void IgnoreTrailingZeros(this VerifySettings settings) =>
			settings.AddScrubber(ScrubTrailingZeros);

		private static void ScrubTrailingZeros(StringBuilder builder)
		{
			var input = builder.ToString();
			var output = DecimalRegex.Replace(
				input,
				m => m.Groups[3].Value.Length == 0
					? m.Groups[0].Value
					: m.Groups[2].Value.Length > 0
						? $"{m.Groups[1].Value}.{m.Groups[2].Value}"
						: $"{m.Groups[1].Value}");
			builder.Clear();
			builder.Append(output);
		}
	}
}
