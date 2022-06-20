using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// Provides methods for reusing facetgroups
    /// </summary>
    public class FacetGroupRepository
    {
        private readonly Xids ids;
        /// <summary>
        /// Use only for persistence and testing, use <see cref="FacetGroupRepository(Xids)"/> instead
        /// </summary>
        [Obsolete("Use only for persistence and testing, otherwise prefer other constructors")]
        [JsonConstructor]
        public FacetGroupRepository()
        {
            ids = new Xids();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="ids">a valid repository</param>
        public FacetGroupRepository(Xids ids)
        {
            this.ids = ids;
        }

        /// <summary>
        /// count of the <see cref="FacetGroup"/> instances in the collection
        /// </summary>
        [JsonIgnore]
        public int Count => collection.Count;

        private List<FacetGroup> collection = new();

        /// <summary>
        /// readonly collection, use other methods in this class to add/modify
        /// </summary>
        public IEnumerable<FacetGroup> Collection
        {
            get { return collection; }
            set
            {
                collection = new List<FacetGroup>();
                foreach (var item in value)
                {
                    Add(item);
                }
            }
        }
        
        /// <summary>
        /// Adds an item to the collection
        /// </summary>
        /// <param name="group"></param>
        public void Add(FacetGroup group)
        {
            // just to be on the safe side, let's only add it once.
            if (collection.Contains(group))
                return;
            collection.Add(group);
        }

        /// <summary>
        /// Just like the LINQ method
        /// </summary>
        public FacetGroup? FirstOrDefault(Func<FacetGroup, bool> p)
        {
            return collection.FirstOrDefault(p);
        }

        /// <summary>
        /// Creates a new FacetGroup, and associates it with the collection.
        /// </summary>
        /// <returns>A facetgroup that does not need to be added to the collection</returns>
        public FacetGroup CreateNew()
        {
            var ret = new FacetGroup(this);
            return ret;
        }


        /// <summary>
        /// Removes an item from the collection
        /// </summary>
        /// <param name="toRemove">item to remove</param>
        public void Remove(FacetGroup toRemove)
        {
            collection.Remove(toRemove);
        }
    }
}
