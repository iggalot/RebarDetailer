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
        ObservableCollection<int> ocSteelYieldStrength = new ObservableCollection<int> { 40, 60, 80, 100 };
        ObservableCollection<int> ocConcreteCompStrength = new ObservableCollection<int> { 500, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500, 7000, 7500, 10000 };
        ObservableCollection<DevelopmentLengthTypes> ocDevelopmentLengthTypes = new ObservableCollection<DevelopmentLengthTypes> { DevelopmentLengthTypes.DEV_LENGTH_STRAIGHT, DevelopmentLengthTypes.DEV_LENGTH_HOOKED };
        
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
                cbi1.Content = "#"+item.ToString();
                window.cmbBarSize.Items.Add(cbi1);
            }

            // Set the default to the first item
            window.cmbBarSize.SelectedItem = window.cmbBarSize.Items[1];
            window.cmbBarSize.FontSize = 25;
            window.cmbBarSize.FontWeight = FontWeights.Bold;
            window.cmbBarSize.HorizontalAlignment = HorizontalAlignment.Center;
            window.cmbBarSize.VerticalAlignment = VerticalAlignment.Top;


            // Create the yield stress drop down
            foreach (var item in ocSteelYieldStrength)
            {
                ComboBoxItem cbi2 = new ComboBoxItem();
                cbi2.Content = item.ToString();
                window.cmbSteelYieldStrength.Items.Add(cbi2);
            }

            // Set the yield strength default to 60000
            window.cmbSteelYieldStrength.SelectedItem = window.cmbSteelYieldStrength.Items[1];
            window.cmbSteelYieldStrength.FontSize = 15;
            window.cmbSteelYieldStrength.FontWeight = FontWeights.Bold;
            window.cmbSteelYieldStrength.HorizontalAlignment = HorizontalAlignment.Center;
            window.cmbSteelYieldStrength.VerticalAlignment = VerticalAlignment.Top;

            // Create the concrete compressive stress drop down
            foreach (var item in ocConcreteCompStrength)
            {
                ComboBoxItem cbi3 = new ComboBoxItem();
                cbi3.Content = item.ToString();
                window.cmbConcreteCompStrength.Items.Add(cbi3);
            }

            // Set the concrete compressive strength default to 3000
            window.cmbConcreteCompStrength.SelectedItem = window.cmbConcreteCompStrength.Items[5];
            window.cmbConcreteCompStrength.FontSize = 15;
            window.cmbConcreteCompStrength.FontWeight = FontWeights.Bold;
            window.cmbConcreteCompStrength.HorizontalAlignment = HorizontalAlignment.Center;
            window.cmbConcreteCompStrength.VerticalAlignment = VerticalAlignment.Top;

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

            // Create the develope bar type (straight, standard hook, ties, etc)
            foreach (var item in ocDevelopmentLengthTypes)
            {
                ComboBoxItem cbi8 = new ComboBoxItem();
                cbi8.Content = item.ToString();
                window.cmbDevelopmentBarType.Items.Add(cbi8);
            }
            // Set the default bar type to the first item (straight bars)
            //window.cmbDevelopmentBarType.SelectedItem = window.cmbDevelopmentBarType.Items[0];
            window.cmbDevelopmentBarType.FontSize = 15;
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
                (MainWin.cmbHasMinTransverseReinf.SelectedIndex == -1) ||
                (MainWin.cmbDevelopmentBarType.SelectedIndex == -1))
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
            DevelopmentLengthTypes devLengthType = ocDevelopmentLengthTypes[MainWin.cmbDevelopmentBarType.SelectedIndex];
            double steelYieldStrength = ocSteelYieldStrength[MainWin.cmbSteelYieldStrength.SelectedIndex]*1000;
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
            Model = new InputModel(devLengthType, barSize, steelYieldStrength, concreteCompStrength, epoxyStatus, topBarStatus, lightweightStatus, sidecover, bottomcover, clearspacing, hasmintransstatus, 0);

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
                double len = model.DevLength();

                double width = c.ActualWidth;
                double height = c.ActualHeight;

                
                double dim = Math.Min(width, height);

                // bounding box dimensions for our graphic
                double bb_width = (0.9 * dim);
                double bb_height = (0.9 * dim);
                double bb_offset = (0.05 * dim);

                // Bounding box coords
                double bb1_x = 0 + bb_offset;
                double bb1_y = 0 + bb_offset;
                double bb2_x = bb1_x + bb_width;
                double bb2_y = bb1_y;
                double bb3_x = bb2_x;
                double bb3_y = bb2_y + bb_height;
                double bb4_x = bb1_x;
                double bb4_y = bb3_y;
                
                //// BorderBox boundary (for debugging)
                //DrawingHelpers.DrawLine(c, bb1_x, bb1_y, bb2_x, bb2_y, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                //DrawingHelpers.DrawLine(c, bb2_x, bb2_y, bb3_x, bb3_y, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                //DrawingHelpers.DrawLine(c, bb3_x, bb3_y, bb4_x, bb4_y, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);
                //DrawingHelpers.DrawLine(c, bb4_x, bb4_y, bb1_x, bb1_y, Brushes.Red, 1, Linetypes.LINETYPE_DASHED);

                // determine the scale...
                double scale_factor = bb_width / len;

                double line_thick = model.BarSize;

                switch (devType)
                {
                    case DevelopmentLengthTypes.DEV_LENGTH_UNDEFINED:
                        {
                            DrawingHelpers.DrawText(c, 10, 10, 0, "No development length defined", Brushes.Black, 20);
                            break;
                        }
                    case DevelopmentLengthTypes.DEV_LENGTH_STRAIGHT:
                        {
                            double ins_x = bb1_x;
                            double ins_y = 0.5*(bb1_y + bb4_y);
                            double end_x = bb1_x + len * scale_factor;
                            double end_y = ins_y;

                            StraightDevelopmentLength straightModel = (StraightDevelopmentLength)model;

                            // Draw the title of the sketch
                            DrawingHelpers.DrawText(c, 0.5 * (bb3_x + bb4_x) - 0.2 * bb_width, 0.5 * (bb3_y + bb4_y) - 0.10 * bb_height, 0, "Straight Length", Brushes.Black, 20);

                            // Draw the rebar object
                            DrawingHelpers.DrawLine(c, ins_x, ins_y, end_x, end_y, Brushes.Black, line_thick);

                            // Draw dimensions for LD
                            DrawingHelpers.DrawLine(c, ins_x, ins_y - line_thick, ins_x, 0.2 * bb_height, Brushes.Green, 1, Linetypes.LINETYPE_PHANTOM);
                            DrawingHelpers.DrawLine(c, end_x, end_y - line_thick, end_x, 0.2 * bb_height, Brushes.Green, 1, Linetypes.LINETYPE_PHANTOM);
                            DrawingHelpers.DrawLine(c, ins_x, 0.22 * bb_height, end_x, 0.22 * bb_height, Brushes.Green, 1, Linetypes.LINETYPE_PHANTOM);
                            DrawingHelpers.DrawText(c, 0.5 * (ins_x + end_x)-30, 0.20 * height, 0, "Ld: " + straightModel.DevLength().ToString() + "in.", Brushes.Green, 15);

                            // Draw arrow and bar size
                            DrawingHelpers.DrawArrowUp(c, 0.5 * (ins_x + end_x), 0.5 * (ins_y + end_y) + 0.5 * line_thick, Brushes.Red, Brushes.Red, 2, 0.1 * height, 0.035 * height);
                            DrawingHelpers.DrawText(c, 0.5 * (ins_x + end_x)-15, 0.5 * (ins_y + end_y) + 0.1 * height, 0, "#" + straightModel.BarSize.ToString(), Brushes.Black, 25);

                            // Draw the critical location line
                            DrawingHelpers.DrawLine(c, ins_x-5, ins_y - 0.3 * height, ins_x-5, ins_y + 0.3 * height, Brushes.Blue, 2, Linetypes.LINETYPE_DASHED);
                            DrawingHelpers.DrawText(c, ins_x-18, ins_y + 0.3 * height, 0, "crit. loc.", Brushes.Black, 12);

                            break;
                        }
                    case DevelopmentLengthTypes.DEV_LENGTH_HOOKED:
                        {
                            HookDevelopmentLength hookModel = (HookDevelopmentLength)model;

                            if(hookModel.HookType == HookTypes.HOOK_STANDARD)
                                // Draw the title of the sketch
                                DrawingHelpers.DrawText(c, 0.5 * (bb3_x + bb4_x) - 0.2 * bb_width, 0.5 * (bb3_y + bb4_y) - 0.10 * bb_height, 0, "Standard Hook", Brushes.Black, 20);

                            else if (hookModel.HookType == HookTypes.HOOK_STIRRUP_TIE)
                                // Draw the title of the sketch
                                DrawingHelpers.DrawText(c, 0.5 * (bb3_x + bb4_x) - 0.2 * bb_width, 0.5 * (bb3_y + bb4_y) - 0.10 * bb_height, 0, "Stirrup / Tie Hook", Brushes.Black, 20);

                            else
                                // Draw the title of the sketch
                                DrawingHelpers.DrawText(c, 0.5 * (bb3_x + bb4_x) - 0.2 * bb_width, 0.5 * (bb3_y + bb4_y) - 0.10 * bb_height, 0, "Unknown Hook Type", Brushes.Black, 20);

                            // If we dont have a standard hook or a stirrup/tie hook
                            if ((hookModel.HookType != HookTypes.HOOK_STANDARD) && (hookModel.HookType != HookTypes.HOOK_STIRRUP_TIE))
                                // do nothing
                                return;

                            // recompute the scale factors in the eventthat Ldh is shorter than L_EXT + BendDia
                            double val1 = 0.8*bb_width / (hookModel.LDH);
                            double val2 = 0.8*bb_height / (0.5*hookModel.BendDia + hookModel.L_EXT);
                            scale_factor = Math.Min(val1, val2);

                            // reduce the scale factor by an arbitrary amount
                            scale_factor = scale_factor  - 0.05 * hookModel.BarSize;

                            // unscaled dimensions
                            double horiz_unscaled = hookModel.LDH;
                            double vert_unscaled = hookModel.L_EXT + 0.5 * hookModel.BendDia;

                            // LDH_1 (start of horizontal LDH line)
                            double ldh1_x = bb1_x+0.05*bb_width;
                            double ldh1_y = bb1_y+0.10*bb_height;  // shift the line up by half a thickness

                            // LDH_2 (end of horizontal LDH line)
                            double ldh2_x = ldh1_x + (hookModel.LDH - 0.5 * hookModel.BendDia)*scale_factor;
                            double ldh2_y = ldh1_y;

                            // Draw the LDH Line
                            DrawingHelpers.DrawLine(c, ldh1_x, ldh1_y, ldh2_x, ldh2_y, Brushes.Black, line_thick);

                            // Draw the hook and extension now
                            // LEXT_1 (top of hook extension line)
                            double lext1_x = ldh2_x + 0.5 * hookModel.BendDia * scale_factor + 0.5*line_thick;
                            double lext1_y = ldh2_y + 0.5 * hookModel.BendDia * scale_factor + 0.5 * line_thick;

                            // LEXT_1 (bottom of hook extension line)
                            double lext2_x = lext1_x;
                            double lext2_y = lext1_y + hookModel.L_EXT * scale_factor + 0.5 * line_thick;

                            // Draw the LEXT line
                            DrawingHelpers.DrawLine(c, lext1_x, lext1_y, lext2_x, lext2_y, Brushes.Black, line_thick);

                            // Draw the radius between LDH2 and LEXT1 with center at ldh2_x and lext1_y
                            // TODO::  this is kind of hacky.  Find a way to drop arc segments.
                            double center_x = ldh2_x;
                            double center_y = lext1_y;
                            double radius = 0.5 * hookModel.BendDia * scale_factor + 0.5*line_thick;

                            double x_start = lext1_x;
                            double y_start = lext1_y;
                            double x_end;
                            double y_end;

                            for (int i = 0; i <= 90; i=i+5)
                            {
                                x_end = center_x + radius*Math.Cos(i * Math.PI / 180.0);
                                y_end = center_y - radius*Math.Sin(i * Math.PI / 180.0);
                                DrawingHelpers.DrawLine(c, x_start, y_start, x_end, y_end, Brushes.Black, line_thick);
                                x_start = x_end;
                                y_start = y_end;
                            }

                            // Draw the Bend diameter circle (blue dashed) and arrow and label
                            DrawingHelpers.DrawCircle(c, ldh2_x, lext1_y, Brushes.Transparent, Brushes.Blue, hookModel.BendDia * scale_factor, 1, Linetypes.LINETYPE_DASHED); ;
                            // Draw arrow and bar size
                            DrawingHelpers.DrawArrowUp(c, ldh2_x, lext1_y + 0.5 * hookModel.BendDia * scale_factor, Brushes.Blue, Brushes.Blue, 2, 0.13 * height, 0.035 * height);
                            DrawingHelpers.DrawText(c, ldh2_x - 35, lext1_y + 0.5 * hookModel.BendDia * scale_factor + 0.15 * height, 0, "Dia.=" + hookModel.BarSize.ToString() + "in.", Brushes.Blue, 15);

                            // Draw dimensions for LDH
                            DrawingHelpers.DrawLine(c, ldh1_x, ldh1_y - 10, ldh1_x, 0.02 * height, Brushes.Green, 1, Linetypes.LINETYPE_PHANTOM);
                            DrawingHelpers.DrawLine(c, lext1_x + 0.5*line_thick, lext1_y - 30, lext1_x + 0.5*line_thick, 0.02 * height, Brushes.Green, 1, Linetypes.LINETYPE_PHANTOM);
                            DrawingHelpers.DrawLine(c, ldh1_x, 0.03 * height, lext1_x + 0.5*line_thick, 0.03 * height, Brushes.Green, 1, Linetypes.LINETYPE_PHANTOM);
                            DrawingHelpers.DrawText(c, 0.5 * (ldh1_x + ldh2_x), 0.03 * height, 0, "Ldh: " + hookModel.DevLength().ToString() + "in.", Brushes.Green, 15);

                            // Draw dimensions for L_EXT
                            DrawingHelpers.DrawLine(c, lext1_x + line_thick + 10, lext1_y, lext1_x + line_thick + 0.20 * width, lext1_y, Brushes.Green, 1, Linetypes.LINETYPE_PHANTOM);
                            DrawingHelpers.DrawLine(c, lext2_x + line_thick + 10, lext2_y, lext2_x + line_thick + 0.20 * width, lext2_y, Brushes.Green, 1, Linetypes.LINETYPE_PHANTOM);
                            DrawingHelpers.DrawLine(c, lext1_x + line_thick + 0.15 * width, lext1_y, lext2_x + line_thick + 0.15 * width, lext2_y, Brushes.Green, 1, Linetypes.LINETYPE_PHANTOM);
                            DrawingHelpers.DrawText(c, lext1_x + line_thick, 0.5*(lext1_y + lext2_y) - 15, 0, "L_EXT: \n  " + hookModel.L_EXT.ToString() + "in.", Brushes.Green, 15);

                            // Draw arrow and bar size
                            DrawingHelpers.DrawArrowUp(c, 0.25 * (ldh1_x + ldh2_x), 0.5 * (ldh1_y + ldh2_y) + 0.5 * line_thick, Brushes.Red, Brushes.Red, 2, 0.1* height, 0.035 * height);
                            DrawingHelpers.DrawText(c, 0.25 * (ldh1_x + ldh2_x) - 15, 0.5 * (ldh1_y + ldh1_y) + 0.5 * line_thick + 0.1*height, 0, "#" + hookModel.BarSize.ToString(), Brushes.Black, 25);
                           
                            // Draw the critical location line
                            DrawingHelpers.DrawLine(c, ldh1_x-5, bb1_y, ldh1_x-5, bb4_y, Brushes.Blue, 2, Linetypes.LINETYPE_DASHED);
                            DrawingHelpers.DrawText(c, ldh1_x-17, bb4_y, 0, "crit. loc.", Brushes.Black, 12);




                            break;
                        }
                    default:
                        break;
                }
            }
        }
    }
}
