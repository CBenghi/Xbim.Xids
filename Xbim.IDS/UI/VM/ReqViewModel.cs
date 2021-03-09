using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.IDS.UI.VM
{
	class ReqViewModel : INotifyPropertyChanged
	{
		private Requirement requirement;
		
		public ReqViewModel(Requirement item)
		{
			requirement = item;
		}

		public DateTime LaData { get; set; } = DateTime.Now;

		public string ModelPart
		{
			get
			{
				return requirement.ModelSubset.Short();
			}
			set {
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
			NotifyPropertyChanged("Need");
			NotifyPropertyChanged("ModelPart");
			LaData = DateTime.Now;
			NotifyPropertyChanged("LaData");
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

		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(String info)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
		}
	}
}
