using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Xbim.IDS
{
    public partial class Ids
    {
		public Project Project { get; set; } = new Project();

		public ObservableCollection<ModelPart> ModelSetRepository { get; set; } = new ObservableCollection<ModelPart>();

		public ObservableCollection<Expectation> ExpectationsRepository { get; set; } = new ObservableCollection<Expectation>();

		public ObservableCollection<RequirementsCollection> RequirementGroups { get; set; } = new ObservableCollection<RequirementsCollection>();
	}
}
