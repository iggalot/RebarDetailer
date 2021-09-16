using DevLengthApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace DevLengthApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        InputViewModel InputVM { get; set; }
        bool firstLaunched = true;

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainWindow()
        {
            InitializeComponent();

            OnUserCreate();

            DataContext = InputVM;
        }

        private void OnUserCreate()
        {
            InputVM = new InputViewModel(this);
        }

        private void btnSolve_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmbBarSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(firstLaunched)
            {
                firstLaunched = false;
                return;
            }
            
            InputVM.Update();
        }
    }
}
