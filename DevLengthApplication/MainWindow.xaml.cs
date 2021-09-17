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
        private InputViewModel InputVM { get; set; }

        public bool firstLaunched = true;

        public MainWindow()
        {
            InputVM = new InputViewModel();

            InitializeComponent();

            OnUserCreate();

            DataContext = InputVM;

            firstLaunched = false; // tells us our initial setup is correct
        }

        private void OnUserCreate()
        {
            InputVM.Create(this);
        }


        private void cmbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!firstLaunched)
            {
                InputVM.Update();
                return;
            }


        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (!firstLaunched)
            //{
            //    {
            //        InputVM.Update();
            //        return;
            //    }
            //}
            //if(!firstLaunched)
            //{
            //    var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
            //    binding.UpdateSource();
            //    //InputVM.Update();
            //}

        }
    }
}
