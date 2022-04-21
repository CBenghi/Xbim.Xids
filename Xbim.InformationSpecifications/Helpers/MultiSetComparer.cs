using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
///  
/// </summary>
namespace Xbim.InformationSpecifications.Helpers
{
	/// <summary>
	/// Taken from https://stackoverflow.com/questions/50098/comparing-two-collections-for-equality-irrespective-of-the-order-of-items-in-the
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class MultiSetComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        private readonly IEqualityComparer<T> m_comparer;
        public MultiSetComparer(IEqualityComparer<T>? comparer = null)
        {
            m_comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public bool Equals(IEnumerable<T>? first, IEnumerable<T>? second)
        {
            if (first == null)
                return second == null;

            if (second == null)
                return false;

            if (ReferenceEquals(first, second))
                return true;

            if (first is ICollection<T> firstCollection && second is ICollection<T> secondCollection)
            {
                if (firstCollection.Count != secondCollection.Count)
                    return false;

                if (firstCollection.Count == 0)
                    return true;
            }

            var prevMatch = new HashSet<int>();
            var sList = second.ToList();
			foreach (var firstItem in first)
			{
                int start = 0;
                // todo: 2021: deal with objects that match multiple times
                var found = (firstItem is null)
                    ? sList.FindIndex(start, x => x is null)
                    : sList.FindIndex(start, x => firstItem.Equals(x));
                if (found == -1)
                    return false;
                if (!prevMatch.Contains(found))
				{
                    prevMatch.Add(found);
				}
				else
				{
                    return false;
				}
			}
            return true;

            // return !HaveMismatchedElement(first, second);
        }
             
        public int GetHashCode(IEnumerable<T> enumerable)
        {
            if (enumerable == null) throw new
                ArgumentNullException(nameof(enumerable));

            int hash = 17;

            foreach (T val in enumerable)
                hash ^= (val == null ? 42 : m_comparer.GetHashCode(val));

            return hash;
        }
    }
}
