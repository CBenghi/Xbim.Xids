using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using static Xbim.InformationSpecifications.FacetGroup;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// Wraps this object instance into an IEnumerable of its type;
    /// </summary>
    public static class IEnumerableExt
    {
        /// <summary>
        /// Wraps this object instance into an IEnumerable&lt;T&gt;
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="item"> The instance that will be wrapped. </param>
        /// <returns> An IEnumerable&lt;T&gt; consisting of a single item. </returns>
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Core class for the management of model specifications 
    /// </summary>
    public partial class Xids // core definition file
    {
        /// <summary>
        /// Static helper method to determine whether the XIDS has information worth saving.
        /// </summary>
        /// <param name="xidsToTest">the instance to check</param>
        /// <returns>True if there's any data in the model</returns>
        public static bool HasData(Xids xidsToTest)
        {
            if (xidsToTest == null)
                return false;
            if (xidsToTest.AllSpecifications().Any())
                return true;
            if (xidsToTest.FacetRepository.Collection.Any())
                return true;
            if (xidsToTest.SpecificationsGroups.Any())
                return true;
            return false;
        }

        /// <summary>
        /// Prepares a new specification,
        /// the function takes care of creating a destinationGroup if one suitable for schema is not found in the data.
        /// WARNING: this creates new facetgroups if applicability and requirement are not provided.
        /// </summary>
        /// <param name="ifcVersion">the desired parent collection</param>
        /// <param name="applicability"></param>
        /// <param name="requirement"></param>
        /// <returns>The initialised specification</returns>
        public Specification PrepareSpecification(
            IfcSchemaVersion ifcVersion,
            FacetGroup? applicability = null,
            FacetGroup? requirement = null
            )
        {
            return PrepareSpecification(ifcVersion.Yield(), applicability, requirement);
        }

        /// <summary>
        /// Prepares a new specification,
        /// the function takes care of creating a destinationGroup if one suitable for schema is not found in the data.
        /// WARNING: this creates new facetgroups if applicability and requirement are not provided.
        /// </summary>
        /// <param name="ifcVersion">the desired parent collection</param>
        /// <param name="applicability"></param>
        /// <param name="requirement"></param>
        /// <returns>The initialised specification</returns>
        public Specification PrepareSpecification(
            IEnumerable<IfcSchemaVersion> ifcVersion,
            FacetGroup? applicability = null,
            FacetGroup? requirement = null
        )
        {
            var destinationGroup = SpecificationsGroups.FirstOrDefault();
            if (destinationGroup == null)
            {
                destinationGroup = new SpecificationsGroup(this);
                SpecificationsGroups.Add(destinationGroup);
            }
            return PrepareSpecification(destinationGroup, ifcVersion, applicability, requirement);
        }

        /// <summary>
        /// Prepares a new specification, inside the specified destinationGroup
        /// WARNING: this creates new facetgroups if applicability and requirement are not provided.
        /// </summary>
        /// <param name="destinationGroup">the desired owning collection</param>
        /// <param name="ifcVersion">a required schema version to set</param>
        /// <param name="applicability"></param>
        /// <param name="requirement"></param>
        /// <returns>The initialised specification</returns>
        public Specification PrepareSpecification(
            SpecificationsGroup destinationGroup,
            IfcSchemaVersion ifcVersion,
            FacetGroup? applicability = null,
            FacetGroup? requirement = null
            )
        {
            return PrepareSpecification(destinationGroup, ifcVersion.Yield(), applicability, requirement);
        }

        /// <summary>
        /// Prepares a new specification, inside the specified destinationGroup
        /// WARNING: this creates new facetgroups if applicability and requirement are not provided.
        /// </summary>
        /// <param name="destinationGroup">the desired owning collection</param>
        /// <param name="ifcVersion">an enumerable of model schemas that would be acceptable</param>
        /// <param name="applicability"></param>
        /// <param name="requirement"></param>
        /// <returns>The initialised specification</returns>
        public Specification PrepareSpecification(
            SpecificationsGroup? destinationGroup,
            IEnumerable<IfcSchemaVersion> ifcVersion,
            FacetGroup? applicability = null,
            FacetGroup? requirement = null
            )
        {
            applicability ??= new FacetGroup(FacetRepository);
            requirement ??= new FacetGroup(FacetRepository);

            // checks and/or prepares destination
            destinationGroup ??= SpecificationsGroups.FirstOrDefault();
            if (destinationGroup == null)
            {
                destinationGroup = new SpecificationsGroup(this);
                SpecificationsGroups.Add(destinationGroup);
            }

            // creates new specification
            var t = new Specification(destinationGroup)
            {
                Applicability = applicability,
                Requirement = requirement,
                IfcVersion = ifcVersion.ToList()
            };

            destinationGroup.Specifications.Add(t);
            return t;
        }

        /// <summary>
        /// Obsolete method will be removed soon, use ImportBuildingSmartIDS instead
        /// </summary>
        [Obsolete("This will be removed soon, use ImportBuildingSmartIDS instead.")]
        public static Xids? FromStream(Stream s)
        {
            return Xids.ImportBuildingSmartIDS(s);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Xids()
        {
            FacetRepository = new FacetGroupRepository();
            _readVersion = "not read";
        }

        /// <summary>
        /// enumerate specifications from all the groups
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Specification> AllSpecifications()
        {
            foreach (var rg in SpecificationsGroups)
            {
                foreach (var req in rg.Specifications)
                {
                    yield return req;
                }
            }
        }

        /// <summary>
        /// Enumerates all the <see cref="FacetGroup"/> that match the use enum.
        /// Each instance is evaluated only once.
        /// </summary>
        /// <param name="use">limits the retuned collection to a specific usage of the <see cref="FacetGroup"/>.</param>
        /// <returns>all facet groups matching the use</returns>
        public IEnumerable<FacetGroup> FacetGroups(FacetUse use)
        {
            foreach (var fg in FacetRepository.Collection.ToArray())
            {
                if (fg.IsUsed(this, use))
                    yield return fg;
            }
        }

        /// <summary>
        /// Cleans up the repository removinga all facet groups that are never used.
        /// </summary>
        public void Purge()
        {
            var unusedFG = FacetRepository.Collection.Except(FacetGroups(FacetUse.All)).ToList();
            foreach (var unused in unusedFG)
            {
                FacetRepository.Remove(unused);
            }
        }

        private string _readVersion;

        internal string ReadVersion { get { return _readVersion; } }

        /// <summary>
        /// Sets a recognisable element in the json persistence.
        /// </summary>
#pragma warning disable CA1822 // Mark members as static, because it's useful for persistency in json
        public string ContentType => "XIDS";
#pragma warning restore CA1822 // Mark members as static

        /// <summary>
        /// Version of the DLL, gets stored in the json persistence
        /// </summary>
        public string Version
        {
            get
            {
                // we are getting the version hardcoded, because loading dll information
                // breaks under some scenarios (see blazor in webassembly)
                return AssemblyVersion;
            }
            set
            {
                _readVersion = value;
            }
        }

        /// <summary>
        /// Project metadata 
        /// </summary>
        public Project Project { get; set; } = new Project();

        /// <summary>
        /// the repository of all facet groups that can be reused by GUID
        /// </summary>
        public FacetGroupRepository FacetRepository { get; set; }

        /// <summary>
        /// Specifications can be grouped for any required purpose.
        /// </summary>
        public List<SpecificationsGroup> SpecificationsGroups { get; set; } = new List<SpecificationsGroup>();

        /// <summary>
        /// Retrieve a <see cref="FacetGroup"/> via its GUID as a string
        /// </summary>
        internal FacetGroup? GetFacetGroup(string? guid)
        {
            if (guid is null)
                return null;
            return FacetRepository.FirstOrDefault(x => x?.Guid is not null && x.Guid.ToString() == guid);
        }

        internal FacetGroup? GetFacetGroup(List<IFacet> fs)
        {
            return FacetRepository.FirstOrDefault(x => x.Facets.FilterMatch(fs));
        }
    }
}