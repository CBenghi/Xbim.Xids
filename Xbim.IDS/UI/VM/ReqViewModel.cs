using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xbim.Xids.UI.mvvm;

namespace Xbim.Xids.UI.VM
{
	class ReqViewModel : ViewModelBase
	{
		private Requirement requirement;
		
		public ReqViewModel(Requirement item)
		{
			requirement = item;
		}

		public string ModelPart
		{
			get
			{
				return requirement.ModelSubset.Short();
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
				return requirement.Need.Short();
			}
			set
			{
				Change();
			}
		}

		private void Change()
		{
			NotifyPropertyChanged("ModelPart");

			NotifyPropertyChanged("Need");
			NotifyPropertyChanged("Exp");
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
			NotifyPropertyChanged("Need");
			NotifyPropertyChanged("Exp");
		}

		public Expectation Exp
		{
			get
			{
				return requirement.Need;
			}
			set
			{
				requirement.Need = value;
				Change();
			}
		}

	}
}
