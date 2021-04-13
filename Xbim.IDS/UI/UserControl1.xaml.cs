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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Xbim.InformationSpecifications.UI
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class UserControl1 : UserControl
	{
		public UserControl1()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty MyOwnDPIDeclaredInMyUcProperty =
			DependencyProperty.Register("MyOwnDPIDeclaredInMyUc",
			typeof(string), typeof(UserControl1));

		public string MyOwnDPIDeclaredInMyUc
		{
			get
			{
				return (string)GetValue(MyOwnDPIDeclaredInMyUcProperty);
			}
			set
			{
				SetValue(MyOwnDPIDeclaredInMyUcProperty, value);
			}
		}

		public static readonly DependencyProperty ExpProperty =
			DependencyProperty.Register("Exp",
			typeof(FacetGroup), typeof(UserControl1));

		public FacetGroup Exp
		{
			get
			{
				return (FacetGroup)GetValue(ExpProperty);
			}
			set
			{
				SetValue(ExpProperty, value);
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var t = new ExpectationEditor
			{
				DataContext = Exp
			};
			t.ShowDialog();
			Exp = (FacetGroup)t.DataContext;
		}
	}
}
