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

namespace MachineSetup.GUI
{
    using static Global;

    /// <summary>
    /// Interaction logic for SetupControl.xaml
    /// </summary>
    public partial class SetupControl : UserControl
    {
        public SetupControl()
        {
            InitializeComponent();
        }

        private void EnabledCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            SetupData data = DataContext as SetupData;
            data.IsEnabled = EnabledCheckBox.IsChecked ?? false;
        }
    }
}
