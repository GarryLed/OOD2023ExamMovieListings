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

namespace MovieListings
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MovieData db = new MovieData();


        public MainWindow()
        {
            InitializeComponent();
        }

        // book seats button 
        private void btnBookSeats_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = dpshowMovie.SelectedDate.Value;

            if (selectedDate != null)
            {
                SearchByDate(selectedDate); // calls the search by date method 
            }
        }


        // searrch database by selected date 
        public void SearchByDate(DateTime selectedDate)
        {
            try
            {
                // LINQ for database query 
                var query = from b in db.Bookings
                            where b.BookingDate == selectedDate
                            select b;
                var results = query.ToList();

                // check if booking is on the selected date, and update 
                if (results.Count > 0)
                {
                    // update list box 
                    lbxMovieListings.ItemsSource = results; // set the list box items to the results from the database query above 

                    // count the total number of tickets sold for movie 
                    int totalTicketsSold = results.Sum(p => p.NumberOfTickets);

                    // get required tickets 
                    int requiredTickets = int.Parse(tbxRequiredSeats.Text);
                    
                    // get current abailable seats 
                    int currentAvailableSeats = (100 - totalTicketsSold);

                    if (currentAvailableSeats >= requiredTickets)
                    {
                        tblkAvailableSeats.Text = currentAvailableSeats.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Sold Out");
                        // return from here? 
                    }
                   
                    

                    // update abailability (total capacity - total participants) 
                    tblkAvailableSeats.Text = (100 - requiredTickets) + "%";

                }
                else
                {
                    // refresh screen if no result is found 
                    //RefreshScreen();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Sorry, error occured when trying to connect to database" + ex.Message);
            }
        }

        private void lbxMovieListings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        
    }
}
