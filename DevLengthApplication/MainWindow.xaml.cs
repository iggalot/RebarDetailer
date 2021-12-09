using ACI318_19Library;
using DevLengthApplication.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevLengthApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static int DEFAULT_BAR_NUM = 4;
        static float DEFAULT_YIELD_STR = 60000;
        static float DEFAULT_CONCRETE_COMP_STR = 3000;
        static bool DEFAULT_DEBUG_MODE = false;
        static DevelopmentLengthTypes DEFAULT_DEV_LENGTH_TYPE = DevelopmentLengthTypes.DEV_LENGTH_UNDEFINED;
       
        private InputViewModel InputVM { get; set; }
        public bool bWindowFinishedLoading = false;

        public MainWindow()
        {
            InputVM = new InputViewModel(DEFAULT_DEV_LENGTH_TYPE, DEFAULT_BAR_NUM, DEFAULT_YIELD_STR, DEFAULT_CONCRETE_COMP_STR, DEFAULT_DEBUG_MODE);

            InitializeComponent();

            OnUserCreate();

            DataContext = InputVM;

            OnUserUpdate();
        }

        /// <summary>
        /// Function that first only once when the application is created.
        /// </summary>
        private void OnUserCreate()
        {
            InputVM.CreateUI(this);
        }

        /// <summary>
        /// Event for when the window finishes loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            bWindowFinishedLoading = true;
            InputVM.Update(); // run the calculations for the current data set
        }

        /// <summary>
        /// Utility function that draws every time an update is needed.
        /// </summary>
        private void OnUserUpdate()
        {
            if (bWindowFinishedLoading)
            {
                InputVM.Update();
                ShowACIDetails();
            }
            InputVM.DrawCanvas(MainCanvas);

        }

        /// <summary>
        /// Event that triggers when a combo box selection has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bWindowFinishedLoading)
            {
                OnUserUpdate();
                return;
            }
        }

        /// <summary>
        /// Event that triggers when the text boxes lose focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (bWindowFinishedLoading)
            {
                OnUserUpdate();
            }
        }

        /// <summary>
        /// Event handler for key presses in the main GUI window.
        /// </summary>
        /// <param name="sener"></param>
        /// <param name="e"></param>
        private void OnKeyDownHandler(object sener, KeyEventArgs e)
        {

            if(e.Key == Key.Return)
            {
                OnUserUpdate();
            }

            if(e.Key == Key.Tab)
            {
                OnUserUpdate();
            }
        }

        /// <summary>
        /// Toggles the ACI details button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ACIDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (svDisplayFactors.Visibility == Visibility.Visible)
            {
                CollapseACIDetails();
            }
            else
            {
                ShowACIDetails();
            }
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


        protected void CollapseACIDetails()
        {
            btnShownACIDetailsExpandedView.Visibility = Visibility.Collapsed;
            btnShownACIDetailsCollapsedView.Visibility = Visibility.Visible;
            svDisplayFactors.Visibility = Visibility.Collapsed;
        }

        protected void ShowACIDetails()
        {
            btnShownACIDetailsExpandedView.Visibility = Visibility.Visible;
            btnShownACIDetailsCollapsedView.Visibility = Visibility.Collapsed;
            svDisplayFactors.Visibility = Visibility.Visible;
        }

        protected void CollapseKtrDetails()
        {
            btnShowKTRInput.Visibility = Visibility.Visible;
            spKTRInput.Visibility = Visibility.Collapsed;
        }

        private void btnCancelKTR_Click(object sender, RoutedEventArgs e)
        {
            CollapseKtrDetails();
        }

        private void btnComputeKTR_Click(object sender, RoutedEventArgs e)
        {
            int numbars;
            double atr, spa;
            bool iscalculated = false;
            bool num_bars_valid = true;
            bool atr_valid = true;
            bool spacing_valid = true;
            bool inputIsValid = true;
           

            if(!int.TryParse(tbNumBars.Text, out numbars))
            {
                inputIsValid = false;
            }

            if (!double.TryParse(tbKTRAreaTransverseSteel.Text, out atr))
            {
                inputIsValid = false;
            }

            if (!double.TryParse(tbKTRSpacing.Text, out spa))
            {
                inputIsValid = false;
            }

            if (inputIsValid)
            {
                if (numbars < 0)
                {
                    num_bars_valid = false;
                }
                lblNumBarStatus.Content = (num_bars_valid ? "" : "cannot be less than zero");

                if (atr < 0)
                {
                    atr_valid = false;
                }
                lblATRStatus.Content = (atr_valid ? "" : "cannot be less than zero");

                if (spa < 0)
                {
                    spacing_valid = false;
                }
                lblKTRSpacingStatus.Content = (spacing_valid ? "" : "cannot be less than zero");

                // Make a new model for the KTR data
                if (InputVM.KTR_VM.Model.IsDefault() || (num_bars_valid && atr_valid && spacing_valid))
                {
                    InputVM.KTR_VM = new KtrViewModel(numbars, atr, spa, iscalculated);
                    InputVM.Update();

                    CollapseKtrDetails();
                }
            }

        }

        private void btnShowKTRInput_Click(object sender, RoutedEventArgs e)
        {
            // Hide the ACI details block
            CollapseACIDetails();

            btnShowKTRInput.Visibility = Visibility.Collapsed;
            spKTRInput.Visibility = Visibility.Visible;

        }
    }
}
