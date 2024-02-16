using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SleepTimer
{
    public partial class MainWindow : Window
    {
        private double dResult;
        private double hResult;
        private double mResult;
        private double sResult;
        private double ovrseconds;
        private DispatcherTimer timer;

        // Flags to track initial input for each TextBox
        private bool isInitialInputDays = true;
        private bool isInitialInputHours = true;
        private bool isInitialInputMinutes = true;
        private bool isInitialInputSeconds = true;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize and configure the timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            // Set initial input flags to true
            isInitialInputDays = true;
            isInitialInputHours = true;
            isInitialInputMinutes = true;
            isInitialInputSeconds = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Timer tick event handler
            if (ovrseconds > 0)
            {
                ovrseconds--;
                UpdateTimerLabel();
            }
            else
            {
                timer.Stop();
                ShutdownComputer();
            }
        }

        private void HandleInitialInput(TextBox textBox, ref bool isInitialInput, TextCompositionEventArgs e)
        {
            // Handle initial input logic for TextBox
            if (isInitialInput && textBox.Text == "0")
            {
                textBox.Text = e.Text;
                textBox.SelectAll();
                isInitialInput = false;
                e.Handled = true;
            }
            else
            {
                isInitialInput = false;
            }
        }

        private void days_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // PreviewTextInput event handler for days TextBox
            HandleInitialInput(days, ref isInitialInputDays, e);
        }

        private void hours_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // PreviewTextInput event handler for hours TextBox
            HandleInitialInput(hours, ref isInitialInputHours, e);
        }

        private void minutes_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // PreviewTextInput event handler for minutes TextBox
            HandleInitialInput(minutes, ref isInitialInputMinutes, e);
        }

        private void seconds_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // PreviewTextInput event handler for seconds TextBox
            HandleInitialInput(seconds, ref isInitialInputSeconds, e);
        }

        private void days_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TextChanged event handler for days TextBox
            UpdateTotalSeconds();
            StandardZero();
        }

        private void hours_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TextChanged event handler for hours TextBox
            UpdateTotalSeconds();
            StandardZero();
        }

        private void minutes_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TextChanged event handler for minutes TextBox
            UpdateTotalSeconds();
            StandardZero();
        }

        private void seconds_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TextChanged event handler for seconds TextBox
            UpdateTotalSeconds();
            StandardZero();
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            // Execute button click event handler
            isInitialInputDays = true;
            isInitialInputHours = true;
            isInitialInputMinutes = true;
            isInitialInputSeconds = true;

            // Ensure TextBoxes are not empty, set to "0" if necessary
            if (string.IsNullOrWhiteSpace(days.Text))
            {
                days.Text = "0";
            }

            if (string.IsNullOrWhiteSpace(hours.Text))
            {
                hours.Text = "0";
            }

            if (string.IsNullOrWhiteSpace(minutes.Text))
            {
                minutes.Text = "0";
            }

            if (string.IsNullOrWhiteSpace(seconds.Text))
            {
                seconds.Text = "0";
            }

            // Parse input values and calculate total seconds
            if (double.TryParse(days.Text, out double d))
            {
                dResult = d * 86400;
            }
            else
            {
                MessageBox.Show("Not a valid value for Days");
                return;
            }

            if (double.TryParse(hours.Text, out double h))
            {
                hResult = h * 3600;
            }
            else
            {
                MessageBox.Show("Not a valid value for Hours");
                return;
            }

            if (double.TryParse(minutes.Text, out double m))
            {
                mResult = m * 60;
            }
            else
            {
                MessageBox.Show("Not a valid value for Minutes");
                return;
            }

            if (double.TryParse(seconds.Text, out double s))
            {
                sResult = s;
            }
            else
            {
                MessageBox.Show("Not a valid value for Seconds");
                return;
            }

            // Calculate total seconds and start the timer
            ovrseconds = sResult + mResult + hResult + dResult;

            MessageBoxResult result = MessageBox.Show($"The timer will be started. Do you want to continue?", "Confirm", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                UpdateTimerLabel();
                timer.Start();
            }
            else
            {
                return;
            }
        }

        private void UpdateTotalSeconds()
        {
            // Update total seconds based on TextBox values
            if (double.TryParse(days.Text, out double d) &&
                double.TryParse(hours.Text, out double h) &&
                double.TryParse(minutes.Text, out double m) &&
                double.TryParse(seconds.Text, out double s))
            {
                ovrseconds = d * 86400 + h * 3600 + m * 60 + s;
                UpdateTimerLabel();
            }
        }

        private void StandardZero()
        {
            // Set TextBoxes to "0" if empty
            if (string.IsNullOrWhiteSpace(days.Text))
            {
                days.Text = "0";
            }

            if (string.IsNullOrWhiteSpace(hours.Text))
            {
                hours.Text = "0";
            }

            if (string.IsNullOrWhiteSpace(minutes.Text))
            {
                minutes.Text = "0";
            }

            if (string.IsNullOrWhiteSpace(seconds.Text))
            {
                seconds.Text = "0";
            }
        }

        private void ClearFields()
        {
            // Clear TextBox values and reset flags
            days.Text = string.Empty;
            hours.Text = string.Empty;
            minutes.Text = string.Empty;
            seconds.Text = string.Empty;

            isInitialInputDays = true;
            isInitialInputHours = true;
            isInitialInputMinutes = true;
            isInitialInputSeconds = true;
        }

        private void UpdateTimerLabel()
        {
            // Update timer label with the formatted time
            timerLabel.Content = $"{TimeSpan.FromSeconds(ovrseconds).ToString(@"dd\:hh\:mm\:ss")}";
        }

        private void ShutdownComputer()
        {
            // Shutdown the computer
            string shutdownCommand = $"/s /t 0";
            Process.Start("shutdown", shutdownCommand);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Cancel button click event handler
            timer.Stop();
            timerLabel.Content = "00:00:00:00";
            ClearFields();

            if (timer.IsEnabled)
            {
                MessageBox.Show("No timer active!");
            }
            else
            {
                Process.Start("shutdown", "-a");
                MessageBox.Show("Timer canceled!");
            }

            // Reset flags
            isInitialInputDays = true;
            isInitialInputHours = true;
            isInitialInputMinutes = true;
            isInitialInputSeconds = true;
        }

        private void ClearFields_Click(object sender, RoutedEventArgs e)
        {
            // ClearFields button click event handler
            ClearFields();
        }
    }
}
