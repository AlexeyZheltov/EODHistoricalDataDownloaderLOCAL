using System;
using System.Windows;
using System.Windows.Controls;

namespace EODHistoricalDataDownloader.View
{
    /// <summary>
    /// Логика взаимодействия для IntradayHistoricalData.xaml
    /// </summary>
    public partial class IntradayPage : Page
    {
        public IntradayPage()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (cbxAllAvailable.IsChecked != null)
            {
                if (cbxAllAvailable.IsChecked.Value)
                {
                    lFrom.IsEnabled = false;
                    switch (cbxInterval.SelectedIndex)
                    {
                        case 0:
                            {
                                dtFrom.SelectedDate = DateTime.Today.AddDays(-120);
                                break;
                            }
                        case 1:
                            {
                                dtFrom.SelectedDate = DateTime.Today.AddDays(-600);
                                break;
                            }
                        case 2:
                            {
                                dtFrom.SelectedDate = DateTime.Today.AddDays(-7200);
                                break;
                            }
                    }
                    dtFrom.IsEnabled = false;
                    lTo.IsEnabled = false;
                    dtTo.SelectedDate = DateTime.Today;
                    dtTo.IsEnabled = false;
                }
                else
                {
                    lFrom.IsEnabled = true;
                    dtFrom.IsEnabled = true;
                    lTo.IsEnabled = true;
                    dtTo.IsEnabled = true;
                }
            }
        }
    }
}
