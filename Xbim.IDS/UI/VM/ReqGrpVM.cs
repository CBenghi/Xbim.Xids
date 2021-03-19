using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Xbim.Xids.UI.mvvm;

namespace Xbim.Xids.UI.VM
{
	internal class ReqGrpVM : ViewModelBase
	{
		private RequirementsCollection requirementsCollection;

		
		private Xids ids;

		private ICommand addRequirementCommand;
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

		public ReqGrpVM(RequirementsCollection requirementsCollection, Xids ids)
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