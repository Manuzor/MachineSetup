using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    class MainWindowDataContext
    {
        public List<SetupData> DataList { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<SetupData> SetupDataList { get; set; }

        public MainWindow()
        {
            DataContext = new MainWindowDataContext
            {
                DataList = new List<SetupData>
                {
                    new SetupData
                    {
                        DisplayName = "Foo",
                        Description = "The foo description\n\nIt covers several lines.",
                        Links = new string[]{ "first link", "https://google.com" },
                    },

                    new SetupData
                    {
                        DisplayName = "Bar",
                        Links = new string[]{ "https://lab132.com", "whatever." },
                    },
                }
            };

            InitializeComponent();
        }

        private void SetupDataListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0 && e.AddedItems[0] is SetupData data)
            {
                Details.SetupData = data;
            }
        }
    }
}
