using DevLengthApplication.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DevLengthApplication.ViewModels
{

    public class InputViewModel : BaseViewModel
    {
        ObservableCollection<int> ocBarSize = new ObservableCollection<int> { 3, 4, 5, 6, 7, 8, 9, 10, 11, 14, 18 };

        InputModel Model = new InputModel();

        MainWindow MainWin { get; set; }

        public string GetSelectedBarSizeLabel
        {
            get
            {
                ComboBoxItem item = (ComboBoxItem)MainWin.cmbBarSize.SelectedItem;
                
                return (string)item.Content;
            }
        }

        public InputViewModel(MainWindow window)
        {
            MainWin = window;

            // Create the bar size drop down box
            foreach (var item in ocBarSize)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = item.ToString();
                window.cmbBarSize.Items.Add(cbi);
            }

            // Set the default to the first item
            window.cmbBarSize.SelectedItem = window.cmbBarSize.Items[0];
        }

        public void Update()
        {
            OnPropertyChanged("GetSelectedBarSizeLabel");
        }
    }
}
