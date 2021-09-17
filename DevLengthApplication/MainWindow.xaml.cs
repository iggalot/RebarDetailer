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
    public partial class MainWindow : Window
    {
        private InputViewModel m_inputViewModel;
        private InputViewModel InputVM 
        {
            get
            {
                return m_inputViewModel;
            }
            set
            {
                m_inputViewModel = value;
            } 
        }

        public bool firstLaunched = true;

        public MainWindow()
        {
            InputVM = new InputViewModel();

            InitializeComponent();

            OnUserCreate();

            DataContext = InputVM;
        }

        private void OnUserCreate()
        {
            InputVM.Create(this);
        }

        private void cmbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (firstLaunched)
            {
                firstLaunched = false;
                return;
            }

            InputVM.Update();
        }

        private void btnSolve_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
