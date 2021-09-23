using DevLengthApplication.ViewModels;
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

namespace DevLengthApplication.Views
{
    /// <summary>
    /// Interaction logic for KtrInput.xaml
    /// </summary>
    public partial class KtrInputView : UserControl
    {
        public KtrInputView()
        {
            InitializeComponent();

            //DataContext = new KtrViewModel();
        }

        private void SelectAll_Textbox(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                tb.SelectAll();
            }
        }

        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    tb.Focus();
                }
            }
        }

        private void ComputeKTR_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
