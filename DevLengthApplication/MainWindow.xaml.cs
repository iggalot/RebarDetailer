using DevLengthApplication.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VMDiagrammer.Helpers;

namespace DevLengthApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private InputViewModel InputVM { get; set; }
        public bool bWindowFinishedLoading = false;

        public MainWindow()
        {
            InputVM = new InputViewModel();

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
            InputVM.Create(this);
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
                DrawingHelpers.DrawLine(MainCanvas, 0, 0, 400, 400, Brushes.Red, 1);
                InputVM.DrawCanvas(MainCanvas);
            }
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

        private void btnComputeKTR_Click(object sender, RoutedEventArgs e)
        {
            if (spKTRInput.Visibility == Visibility.Visible)
            {
                btnKTRCompute.Content = "Close";
                spKTRInput.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnKTRCompute.Content = "Minimize KTR";
                spKTRInput.Visibility = Visibility.Visible;
            }

        }
    }
}
