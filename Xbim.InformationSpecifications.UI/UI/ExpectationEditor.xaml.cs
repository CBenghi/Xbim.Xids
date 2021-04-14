using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Xbim.InformationSpecifications.UI
{
	/// <summary>
	/// Interaction logic for ExpectationEditor.xaml
	/// </summary>
	public partial class ExpectationEditor : Window
	{
		private FacetGroup exp;

		public ExpectationEditor()
		{
			InitializeComponent();
		}

		public FacetGroup Exp
		{
			get => exp;
			internal set
			{
				exp = value;
				this.DataContext = exp;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
