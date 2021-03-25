using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;

namespace Travel_Agency
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Label adultsNum;
        private Label childrenNum;

        private List<Car> cars = new List<Car>() { new Car("RR Wraith", 120),
                                                   new Car("RR Phantom", 150) };

        private List<City> cities = new List<City>() { new City("Rome", 40),
                                                       new City("Milan", 40),
                                                       new City("Venice", 40) };

        private List<Booking> bookings =  new List<Booking>();

        private Receipt receipt = new Receipt();

        public MainWindow()
        {
            InitializeComponent();
            LoadCars();
            LoadCities();

            adultsNum = (Label)numSelectAdults.Children[0];
            childrenNum = (Label)numSelectChildren.Children[0];

            addSelectionChangedEvents();
        }

        private void LoadCars()
        {
            comboCar.Items.Add("None");

            for (int i = 0; i < cars.Count; i++)
            {
                comboCar.Items.Add(cars[i].Name);
            }
        }

        private void LoadCities()
        {
            for (int i = 0; i < cities.Count; i++)
            {
                comboCity.Items.Add(cities[i].Name);
            }
        }

        private void addSelectionChangedEvents()
        {
            comboCity.SelectionChanged += comboBox_SelectionChanged;
            comboCar.SelectionChanged += comboBox_SelectionChanged;
            comboTour.SelectionChanged += comboBox_SelectionChanged;
        }

        private void updateReceipt()
        {
            int adults = int.Parse(adultsNum.Content.ToString());
            int children = int.Parse(childrenNum.Content.ToString());
            int nights = int.Parse(((Label)numSelectNights.Children[0]).Content.ToString());
            Car car = null;
            bool tour = false;
            string city = comboCity.SelectedItem.ToString();

            if (comboCar.SelectedItem.ToString() != "None") car = cars.ToList().Find(c => c.Name == comboCar.SelectedItem.ToString());
            if (comboTour.SelectedIndex == 1) tour = true;

            receipt.Update(adults, children, nights, car, tour, city);

            List<string> res = receipt.Info();
            lstReceipt.Items.Clear();
            for (int i = 0; i < res.Count; i++)
            {
                if (i == res.Count - 1 && bookings.Count == 1)
                {
                    if (receipt.Total <= bookings[0].receipt.Total) // '<=' incase both are same price, at least one will be discounted
                    {
                        lstReceipt.Items.Add("Discount: 20%");
                        lstReceipt.Items.Add($"Total: £{receipt.Total * 0.8}");
                    }
                    else lstReceipt.Items.Add(res[i]);
                } else lstReceipt.Items.Add(res[i]);
            }
        }

        private void resetForm()
        {
            adultsNum.Content = 0;
            childrenNum.Content = 0;
            ((Label)numSelectNights.Children[0]).Content = 1;
            comboCity.SelectedIndex = 0;
            comboCar.SelectedIndex = 0;
            comboTour.SelectedIndex = 0;
            lstReceipt.Items.Clear();
        }

        private void refreshBookings()
        {
            lstBookings.Items.Clear();

            if (bookings.Count == 2)
            {
                if (bookings[0].receipt.Total >= bookings[1].receipt.Total) bookings[1].Discount = true; // >= incase both are same price, so at least one is discounted
                else bookings[0].Discount = true;
            }
            else if (bookings.Count == 1) bookings[0].Discount = false;

            for (int i = 0; i < bookings.Count; i++)
            {
                lstBookings.Items.Add($"{i + 1}\n" + bookings[i].Info());
            }
        }

        // Num selector buttons

        private void numSubtract(Object sender, MouseButtonEventArgs e)
        {
            Grid numGrid = (Grid)((Label)sender).Parent;
            Label numLabel = (Label)numGrid.Children[0];
            int num = int.Parse(numLabel.Content.ToString());
            if (numGrid.Name == "numSelectNights")
            {
                if (num > 1) numLabel.Content = num - 1;
            }
            else if (num > 0) numLabel.Content = num - 1;

            updateReceipt();
        }

        private void numAdd(Object sender, MouseButtonEventArgs e)
        {
            Grid numGrid = (Grid)((Label)sender).Parent;
            Label numLabel = (Label)numGrid.Children[0];
            int num = int.Parse(numLabel.Content.ToString());
            int totalNum = int.Parse(adultsNum.Content.ToString()) + int.Parse(childrenNum.Content.ToString());

            if (numGrid.Name == "numSelectNights")
            {
                if (num < 14) numLabel.Content = num + 1;
                else showError("Max amount of nights is 14");
            }
            else if (totalNum < 6) numLabel.Content = num + 1;
            else showError("Max amount of people is 6");

            updateReceipt();
        }

        // Other events

        private void btnBook_Click(object sender, RoutedEventArgs e)
        {
            int totalPeople = int.Parse(adultsNum.Content.ToString()) + int.Parse(childrenNum.Content.ToString());
            
            if (lstBookings.Items.Count >= 2)
            {
                showError("Max amount of bookings per user is 2");
                return;
            }

            if (cities.Find(c => c.Name == comboCity.SelectedItem.ToString()).Vacancies < totalPeople)
            {
                showError($"Not enough vacancies at {comboCity.SelectedItem}");
                return;
            }

            if (totalPeople == 0)
            {
                showError($"Amount of residents must be between 1-6");
                return;
            }

            bookings.Add(new Booking(receipt.Clone()));

            refreshBookings();

            cities.Find(c => c.Name == comboCity.SelectedItem.ToString()).Vacancies -= totalPeople;

            resetForm();
            showNotification("Sucessfully booked room");
        }

        private void btnRemoveBooking_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((ContentPresenter)((Grid)((Button)sender).Parent).Children[0]).Content.ToString().Split('\n')[0]) - 1;

            cities.Find(c => c.Name == bookings[index].receipt.CurrentCity).Vacancies += bookings[index].receipt.NumAdults + bookings[index].receipt.NumChildren;

            bookings.RemoveAt(index);

            refreshBookings();
            showNotification("Successfully removed booking");
        }

        private void btnClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void btnMinimise_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void comboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            updateReceipt();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            resetForm();
            showNotification("Successfully reset form");
        }

        private void obj_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Control)sender).Opacity = .8;
        }

        private void obj_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Control)sender).Opacity = 1;
        }

        // Notifications

        private void showError(string msg)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                Dispatcher.Invoke(() =>
                {
                    NotificationPopup.Visibility = Visibility.Hidden;
                    ErrorMsg.Content = msg;
                    ErrorPopup.Visibility = Visibility.Visible;
                });

                Thread.Sleep(2000);

                Dispatcher.Invoke(() =>
                {
                    ErrorPopup.Visibility = Visibility.Hidden;
                });
            }).Start();
        }

        private void showNotification(string msg)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                Dispatcher.Invoke(() =>
                {
                    ErrorPopup.Visibility = Visibility.Hidden;
                    NotificationMsg.Content = msg;
                    NotificationPopup.Visibility = Visibility.Visible;
                });

                Thread.Sleep(2000);

                Dispatcher.Invoke(() =>
                {
                    NotificationPopup.Visibility = Visibility.Hidden;
                });
            }).Start();
        }
    }
}
