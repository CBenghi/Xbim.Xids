using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xbim.InformationSpecifications.UI.mvvm;

namespace Xbim.InformationSpecifications.UI.VM
{
	class ReqViewModel : ViewModelBase
	{
		private readonly Specification requirement;
		
		public ReqViewModel(Specification item)
		{
			requirement = item;
		}

		public string ModelPart
		{
			get
			{
				return requirement.Applicability.Short();
			}
			set
			{
				Change();
			}
		}

		public string Need
		{
			get
			{
				return requirement.Requirement.Short();
			}
			set
			{
				Change();
			}
		}

		private void Change()
		{
			NotifyPropertyChanged(nameof(ModelPart));
			NotifyPropertyChanged(nameof(Need));
			NotifyPropertyChanged(nameof(Exp));
		}

		private ICommand editExpectationCommand;
		public ICommand EditExpectationCommand
		{
			get
			{
				if (editExpectationCommand == null)
				{
					editExpectationCommand = new ActionCommand(parameter => EditExpectation());
				}
				return editExpectationCommand;
			}
		}

		private void EditExpectation()
		{
			var t = new ExpectationEditor
			{
				Exp = Exp
			};
			t.ShowDialog();
			NotifyPropertyChanged(nameof(Need));
			NotifyPropertyChanged(nameof(Exp));
		}

		public FacetGroup Exp
		{
			get
			{
				return requirement.Requirement;
			}
			set
			{
				requirement.Requirement = value;
				Change();
			}
		}

	}
}
