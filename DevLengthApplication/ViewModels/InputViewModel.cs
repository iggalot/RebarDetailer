using DevLengthApplication.Models;
using RebarDetailsLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VMDiagrammer.Helpers;

namespace DevLengthApplication.ViewModels
{
    public class InputViewModel : BaseViewModel
    {
        ObservableCollection<int> ocBarSize = new ObservableCollection<int> { 3, 4, 5, 6, 7, 8, 9, 10, 11, 14, 18 };
        ObservableCollection<int> ocSteelYieldStrength = new ObservableCollection<int> { 40000, 60000, 80000, 100000 };
        ObservableCollection<int> ocConcreteCompStrength = new ObservableCollection<int> { 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500, 7000, 7500, 10000 };

        /// <summary>
        /// Our development length model basis
        /// </summary>
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
                if(Model != null)
                    return Model.LD;
                else
                    return -1;
            }
        }

        public bool GetEpoxyBarStatus
        {
            get
            {
                return (Model.DevelopmentLengthObject.EpoxyBarStatus);
            }
        }

        public bool GetTopBarStatus
        {
            get
            {
                switch (Model.DevelopmentLengthObject.DevLengthType)
                {
                    case RebarDetailsLibrary.DevelopmentLengthTypes.DEV_LENGTH_UNDEFINED:
                        break;
                    case RebarDetailsLibrary.DevelopmentLengthTypes.DEV_LENGTH_STRAIGHT:
                        return (((StraightDevelopmentLength)Model.DevelopmentLengthObject).TopBarStatus);
                    case RebarDetailsLibrary.DevelopmentLengthTypes.DEV_LENGTH_HOOKED:
                        break;
                    default:
                        break;
                }
                return true;
            }
        }

        public string GetDisplayFactors
        {
            get
            {
                return (Model.DisplayDevelopmentFactors());
            }
        }

        public double GetSideCover
        {
            get
            {   
                if(Model != null)
                    if(Model.DevelopmentLengthObject != null)
                         return Model.DevelopmentLengthObject.SideCover;
                return -1;
            }
            set
            {
                if (Model != null)
                    if (Model.DevelopmentLengthObject != null)
                    {
                        Model.DevelopmentLengthObject.SideCover = value;
                        OnPropertyChanged("GetSideCover");
                    }

            }
        }

        public double GetBottomCover
        {
            get
            {
                if (Model != null)
                    if (Model.DevelopmentLengthObject != null)
                        return Model.DevelopmentLengthObject.BottomCover;
                return -1;
            }
            set
            {
                if (Model != null)
                    if (Model.DevelopmentLengthObject != null)
                    {
                        Model.DevelopmentLengthObject.BottomCover = value;
                        OnPropertyChanged("GetBottomCover");
                    }
            }
        }

        public double GetClearSpacing
        {
            get
            {
                if(Model != null)
                    if(Model.DevelopmentLengthObject != null)
                       return Model.DevelopmentLengthObject.CC_Spacing;
                return -1;
            }
            set
            {
                if (Model != null)
                    if (Model.DevelopmentLengthObject != null)
                    {
                        Model.DevelopmentLengthObject.CC_Spacing = value;
                        OnPropertyChanged("GetClearSpacing");
                    }
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public InputViewModel(RebarDetailsLibrary.DevelopmentLengthTypes type)
        {
            Model = new InputModel(type);
        }

        public InputViewModel()
        {
            Model = new InputModel(RebarDetailsLibrary.DevelopmentLengthTypes.DEV_LENGTH_UNDEFINED);
        }

        /// <summary>
        /// Setup the view model for this window
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

        /// <summary>
        /// Read the form data, parse it, and create a new model for the data.
        /// </summary>
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

            bool bValidInput = true;
            string parseMessage = "";
            if (!Double.TryParse(MainWin.tbSideCover.Text, out sidecover))
            {
                bValidInput = false;
                parseMessage += "Side Cover must be a valid double\n";
            }

            if (!Double.TryParse(MainWin.tbBottomCover.Text, out bottomcover))
            {
                bValidInput = false;
                parseMessage += "Bottom Cover must be a valid double\n";
            }

            if (!Double.TryParse(MainWin.tbClearSpacing.Text, out clearspacing))
            {
                bValidInput = false;
                parseMessage += "Clear Spacing must be a valid double\n";
            }

            // If our parse info isn't valid, send a message and don't proceed any further.
            if (!bValidInput)
            {
                MessageBox.Show(parseMessage);
                return;
            } else
            {
                GetSideCover = sidecover;
                GetBottomCover = bottomcover;
                GetClearSpacing = clearspacing;
            }

            bool hasmintransstatus = (MainWin.cmbLightweightConcrete.SelectedIndex == 0 ? true : false);

            // Create the model
            Model = new InputModel(RebarDetailsLibrary.DevelopmentLengthTypes.DEV_LENGTH_STRAIGHT, barSize, steelYieldStrength, concreteCompStrength, epoxyStatus, topBarStatus, lightweightStatus, sidecover, bottomcover, clearspacing, hasmintransstatus, 0);

            OnPropertyChanged("GetSelectedBarSizeLabel");
            OnPropertyChanged("GetSelectedSteelYieldStrengthLabel");
            OnPropertyChanged("GetSelectedConcreteCompStrengthLabel");
            OnPropertyChanged("GetDevelopmentLengthStraight");
            OnPropertyChanged("GetEpoxyStatus");
            OnPropertyChanged("GetTopBarStatus");
            OnPropertyChanged("GetLightweightConcreteStatus");
            OnPropertyChanged("GetDisplayFactors");
        }

        public void DrawCanvas(Canvas c)
        {
            c.Children.Clear();
            
            if(Model.DevelopmentLengthObject != null)
            {
                BaseDevelopmentLength model = Model.DevelopmentLengthObject;
                DevelopmentLengthTypes devType = model.DevLengthType;

                double width = c.ActualWidth;
                double height = c.ActualHeight;

                

                switch (devType)
                {
                    case DevelopmentLengthTypes.DEV_LENGTH_UNDEFINED:
                        {
                            DrawingHelpers.DrawText(c, 10, 10, 0, "No development length defined", Brushes.Black, 20);
                            break;
                        }
                    case DevelopmentLengthTypes.DEV_LENGTH_STRAIGHT:
                        {
                            StraightDevelopmentLength straightModel = (StraightDevelopmentLength)model;


                            double length = 100 * model.BarDiameter;
                            double ins_x = 0.1 * width;
                            double ins_y = 0.4 * height;
                            double end_x = 0.9 * width;
                            double end_y = 0.4 * height;

                            double line_thick = model.BarSize * 3;

                            // Draw the title of the sketch
                            DrawingHelpers.DrawText(c, 0.5 * width - 70, height - 50, 0, "Straight Length", Brushes.Black, 20);

                            // Draw the rebar object
                            DrawingHelpers.DrawLine(c, ins_x, ins_y, end_x, end_y, Brushes.Black, line_thick);

                            // Draw dimensions
                            DrawingHelpers.DrawLine(c, ins_x, ins_y-10, ins_x, 0.05*height, Brushes.Green, 1, Linetypes.LINETYPE_PHANTOM);
                            DrawingHelpers.DrawLine(c, end_x, end_y-10, end_x, 0.05*height, Brushes.Green, 1, Linetypes.LINETYPE_PHANTOM);
                            DrawingHelpers.DrawLine(c, ins_x, 0.1*height, end_x, 0.1*height, Brushes.Green, 1, Linetypes.LINETYPE_PHANTOM);
                            DrawingHelpers.DrawText(c, 0.5*(ins_x+end_x)-20, 0.1*height, 0, straightModel.DevLength().ToString() + " in.", Brushes.Black, 25);

                            // Draw arrow and bar size
                            DrawingHelpers.DrawArrowUp(c, 0.5 * (ins_x + end_x), 0.5 * (ins_y + end_y) + 0.5 * line_thick, Brushes.Red, Brushes.Red, 0.1 * line_thick, 1.5 * line_thick, 0.5 * line_thick);
                            DrawingHelpers.DrawText(c, 0.5 * (ins_x + end_x)-15, 0.5 * (ins_y + end_y) + 2.0 * line_thick, 0, "#" + straightModel.BarSize.ToString(), Brushes.Black, 25);

                            // Draw the critical location line
                            DrawingHelpers.DrawLine(c, ins_x-5, ins_y - 0.3 * height, ins_x-5, ins_y + 0.3 * height, Brushes.Blue, 2, Linetypes.LINETYPE_DASHED);
                            DrawingHelpers.DrawText(c, ins_x-18, ins_y + 0.3 * height, 0, "crit. loc.", Brushes.Black, 12);


                            break;
                        }
                    case DevelopmentLengthTypes.DEV_LENGTH_HOOKED:
                        {
                            HookDevelopmentLength hookModel = (HookDevelopmentLength)model;
                            if(hookModel.HookType == HookTypes.HOOK_STANDARD)
                                DrawingHelpers.DrawText(c, 10, 10, 0, "Standard Hook", Brushes.Black, 20);
                            else if (hookModel.HookType == HookTypes.HOOK_STIRRUP_TIE)
                                DrawingHelpers.DrawText(c, 10, 10, 0, "Stirrup / Tie Hook", Brushes.Black, 20);
                            else
                                DrawingHelpers.DrawText(c, 10, 10, 0, "Unknown hook type", Brushes.Black, 20);
                            break;
                        }
                    default:
                        break;
                }
            }
        }
    }
}
