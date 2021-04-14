using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Xbim.InformationSpecifications.UI.VM;

namespace Xbim.InformationSpecifications
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			DirectoryInfo d = new DirectoryInfo(".");
			Debug.WriteLine(d.FullName);
			var ids = Xids.ImportBuildingSmartIDS(@"..\..\..\Xbim.IDS.Tests\Files\bS\Example01.xml");
			DataContext = new ReqGrpVM(ids.SpecificationsGroups.FirstOrDefault(), ids);
		}

		
	}
}
