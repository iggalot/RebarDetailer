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
        ObservableCollection<int> ocSteelYieldStrength = new ObservableCollection<int> { 40000, 60000, 80000, 100000 };
        ObservableCollection<int> ocConcreteCompStrength = new ObservableCollection<int> { 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500, 7000, 7500, 10000 };

        public InputModel Model { get; set; } = new InputModel();

        MainWindow MainWin { get; set; }   // reference to this window object

        public string GetSelectedBarSizeLabel
        {
            get
            {
                ComboBoxItem item = (ComboBoxItem)MainWin.cmbBarSize.SelectedItem;
                
                return (string)item.Content;
            }
        }

        public string GetSelectedSteelYieldStrengthLabel
        {
            get
            {
                ComboBoxItem item = (ComboBoxItem)MainWin.cmbSteelYieldStrength.SelectedItem;

                return (string)item.Content;
            }
        }

        public string GetSelectedConcreteCompStrengthLabel
        {
            get
            {
                ComboBoxItem item = (ComboBoxItem)MainWin.cmbConcreteCompStrength.SelectedItem;

                return (string)item.Content;
            }
        }

        public double GetDevelopmentLengthStraight
        {
            get
            {
                return Model.DevelopmentLengthStraight;
            }
        }



        public bool GetEpoxyBarStatus
        {
            get
            {
                return (Model.EpoxyBarStatus);
            }
        }

        public bool GetTopBarStatus
        {
            get
            {
                return (Model.TopBarStatus);
            }
        }

        public string DisplayFactors
        {
            get
            {
                return (Model.DisplayStraightDevelopmentFactors());
            }
        }

        public double GetSideCover
        {
            get => Model.SideCover;
            set
            {
                Model.SideCover = value;
                OnPropertyChanged("GetSideCover");
            }
        }

        public double GetBottomCover
        {
            get => Model.BottomCover;
            set
            {
                Model.BottomCover = value;
                OnPropertyChanged("GetBottomCover");
            }
        }

        public double GetClearSpacing
        {
            get => Model.CC_Spacing;
            set
            {
                Model.CC_Spacing = value;
                OnPropertyChanged("GetClearSpacing");
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public InputViewModel()
        {
            Model = new InputModel();
        }

        /// <summary>
        /// Setus up the view model for this window
        /// </summary>
        /// <param name="window"></param>
        public void Create(MainWindow window)
        {
            MainWin = window;

            // Create the bar size drop down box
            foreach (var item in ocBarSize)
            {
                ComboBoxItem cbi1 = new ComboBoxItem();
                cbi1.Content = item.ToString();
                window.cmbBarSize.Items.Add(cbi1);
            }

            // Set the default to the first item
            window.cmbBarSize.SelectedItem = window.cmbBarSize.Items[1];

            // Create the yield stress drop down
            foreach (var item in ocSteelYieldStrength)
            {
                ComboBoxItem cbi2 = new ComboBoxItem();
                cbi2.Content = item.ToString();
                window.cmbSteelYieldStrength.Items.Add(cbi2);
            }

            // Set the yield strength default to 60000
            window.cmbSteelYieldStrength.SelectedItem = window.cmbSteelYieldStrength.Items[1];

            // Create the concrete compressive stress drop down
            foreach (var item in ocConcreteCompStrength)
            {
                ComboBoxItem cbi3 = new ComboBoxItem();
                cbi3.Content = item.ToString();
                window.cmbConcreteCompStrength.Items.Add(cbi3);
            }

            // Set the concrete compressive strength default to 3000
            window.cmbConcreteCompStrength.SelectedItem = window.cmbConcreteCompStrength.Items[2];

            // Create the epoxy drop down
            ComboBoxItem cbi4 = new ComboBoxItem() { Content = "YES" };
            window.cmbEpoxy.Items.Add(cbi4);
            cbi4 = new ComboBoxItem() { Content = "NO" };
            window.cmbEpoxy.Items.Add(cbi4);
            window.cmbEpoxy.SelectedItem = window.cmbEpoxy.Items[0];

            // Create the top bars drop down
            ComboBoxItem cbi5 = new ComboBoxItem() { Content = "YES" };
            window.cmbTopBars.Items.Add(cbi5);
            cbi5 = new ComboBoxItem() { Content = "NO" };
            window.cmbTopBars.Items.Add(cbi5);
            window.cmbTopBars.SelectedItem = window.cmbTopBars.Items[0];

            // Create the lightweight drop down
            ComboBoxItem cbi6 = new ComboBoxItem() { Content = "YES" };
            window.cmbLightweightConcrete.Items.Add(cbi6);
            cbi6 = new ComboBoxItem() { Content = "NO" };
            window.cmbLightweightConcrete.Items.Add(cbi6);
            window.cmbLightweightConcrete.SelectedItem = window.cmbLightweightConcrete.Items[1];

            // Create the has minimum transverse reinforcement drop down
            ComboBoxItem cbi7 = new ComboBoxItem() { Content = "YES" };
            window.cmbHasMinTransverseReinf.Items.Add(cbi7);
            cbi7 = new ComboBoxItem() { Content = "NO" };
            window.cmbHasMinTransverseReinf.Items.Add(cbi7);
            window.cmbHasMinTransverseReinf.SelectedItem = window.cmbHasMinTransverseReinf.Items[1];
        }

        public void Update()
        {
            bool modelIsValid = true;

            if ((MainWin.cmbBarSize.SelectedIndex == -1) ||
                (MainWin.cmbSteelYieldStrength.SelectedIndex == -1) ||
                (MainWin.cmbConcreteCompStrength.SelectedIndex == -1) ||
                (MainWin.cmbEpoxy.SelectedIndex == -1) ||
                (MainWin.cmbTopBars.SelectedIndex == -1) ||
                (MainWin.cmbHasMinTransverseReinf.SelectedIndex == -1))
            {
                modelIsValid = false;
            }

            if (!modelIsValid)
            {
                Console.WriteLine("Model is not valid at this time!");
                return;
            }

            // Read the input from the UI
            int barSize = ocBarSize[MainWin.cmbBarSize.SelectedIndex];
            double steelYieldStrength = ocSteelYieldStrength[MainWin.cmbSteelYieldStrength.SelectedIndex];
            double concreteCompStrength = ocConcreteCompStrength[MainWin.cmbConcreteCompStrength.SelectedIndex];
            bool epoxyStatus = (MainWin.cmbEpoxy.SelectedIndex == 0 ? true : false);
            bool topBarStatus = (MainWin.cmbTopBars.SelectedIndex == 0 ? true : false);
            bool lightweightStatus = (MainWin.cmbLightweightConcrete.SelectedIndex == 0 ? true : false);

            double sidecover, bottomcover, clearspacing;

            if (!Double.TryParse(MainWin.tbSideCover.Text, out sidecover))
                throw new InvalidOperationException("Side Cover must be a double");

            if (!Double.TryParse(MainWin.tbBottomCover.Text, out bottomcover))
                throw new InvalidOperationException("Bottom Cover must be a double");

            if (!Double.TryParse(MainWin.tbClearSpacing.Text, out clearspacing))
                throw new InvalidOperationException("Clear Spacing must be a double");

            bool hasmintransstatus = (MainWin.cmbLightweightConcrete.SelectedIndex == 0 ? true : false);


            // Create the model
            Model = new InputModel(barSize, steelYieldStrength, concreteCompStrength, epoxyStatus, topBarStatus, lightweightStatus, sidecover, bottomcover, clearspacing, hasmintransstatus, 0);

            OnPropertyChanged("GetSelectedBarSizeLabel");
            OnPropertyChanged("GetSelectedSteelYieldStrengthLabel");
            OnPropertyChanged("GetSelectedConcreteCompStrengthLabel");
            OnPropertyChanged("GetDevelopmentLengthStraight");
            OnPropertyChanged("GetEpoxyStatus");
            OnPropertyChanged("GetTopBarStatus");
            OnPropertyChanged("GetLightweightConcreteStatus");
            OnPropertyChanged("DisplayFactors");


        }
    }
}
