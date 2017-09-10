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
    /// Interaction logic for SetupDetailsControl.xaml
    /// </summary>
    public partial class SetupDetailsControl : UserControl
    {
        SetupData _setupData = EmptySetupData;
        public SetupData SetupData
        {
            get => DataContext as SetupData;
            set
            {
                DataContext = value;
            }
        }

        public SetupDetailsControl()
        {
            SetupData = _setupData;
            InitializeComponent();
        }
    }
}
