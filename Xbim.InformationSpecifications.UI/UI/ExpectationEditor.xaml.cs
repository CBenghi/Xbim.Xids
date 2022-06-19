using System.Windows;

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
