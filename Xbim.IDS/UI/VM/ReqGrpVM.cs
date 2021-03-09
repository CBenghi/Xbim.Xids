using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Xbim.IDS.UI.mvvm;

namespace Xbim.IDS.UI.VM
{
	internal class ReqGrpVM
	{
		private RequirementsCollection requirementsCollection;

		private ICommand addRequirementCommand;
		private IDS ids;

		public ICommand AddRequirementCommand
		{
			get
			{
				if (addRequirementCommand == null)
				{
					addRequirementCommand = new ActionCommand(i => AddRequirement());
				}
				return addRequirementCommand;
			}
		}

		private void AddRequirement()
		{
			var t = ids.NewRequirement();
			Reqs.Add(new ReqViewModel(t));
		}

		public ReqGrpVM(RequirementsCollection requirementsCollection, IDS ids)
		{
			this.requirementsCollection = requirementsCollection;
			this.ids = ids;

			Reqs = new ObservableCollection<ReqViewModel>();
			foreach (var item in requirementsCollection.Requirements)
			{
				Reqs.Add(new ReqViewModel(item));
			}
		}

		public ObservableCollection<ReqViewModel> Reqs { get; set; }
	}
}