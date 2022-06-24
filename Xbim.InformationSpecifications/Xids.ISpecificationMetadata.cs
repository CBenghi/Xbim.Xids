using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.InformationSpecifications
{
    public partial class Xids : ISpecificationMetadata
    {
        private string? guid;

        /// <inheritdoc />
        public string Guid
        {
            get
            {
                if (string.IsNullOrEmpty(guid))
                {
                    guid = System.Guid.NewGuid().ToString();
                }
                return guid!;
            }

            set
            {
                guid = value;
            }
        }

        /// <inheritdoc />
        public string? Name { get; set; }

        /// <inheritdoc />
        public string? Provider { get; set; }

        /// <inheritdoc />
        public IList<string>? Consumers { get; set; }

        /// <inheritdoc />
        public IList<string>? Stages { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> GetConsumers()
        {
            if (Consumers != null)
                return Consumers;
            return Enumerable.Empty<string>();
        }

        /// <inheritdoc />
        public string? GetProvider()
        {
            if (!string.IsNullOrWhiteSpace(Provider))
                return Provider;
            return Provider;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetStages()
        {
            if (Stages != null && Stages.Any())
                return Stages;
            return Enumerable.Empty<string>();
        }
    }
}
