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
using System.Data.Entity;

namespace MovieListings
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
         MovieData db = new MovieData();
        private readonly int TotalCapacity = 100; 

        public MainWindow()
        {
            InitializeComponent();
        }

        // display movie data on initial window load 
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshScreen();
            DisplayInitialMovieTitles();
        }

        // book seats button 
        private void btnBookSeats_Click(object sender, RoutedEventArgs e)
        {
            // store selected movie 
            var selectedMovie = lbxMovieListings.SelectedItem as Movie;

            // store selected date 
            var selectedDate = dpshowMovie.SelectedDate.Value;

            // check existing moves 
            var existingMovies = db.Bookings
                .Where(b => b.MovieId == selectedMovie.MovieId && b.BookingDate == selectedDate).ToList();

            // store number of required seats
            int requiredSeats = int.Parse(tbxRequiredSeats.Text); // add validaton here 

            // count total tickets booked for movie 
            int totalTicketsBooked = existingMovies.Sum(b => b.NumberOfTickets);

            // count available seats 
            int availableSeats = TotalCapacity - totalTicketsBooked;

            // create and saving booking to database 
            var newBooking = new Booking
            {
                MovieId = selectedMovie.MovieId,
                BookingDate = selectedDate,
                NumberOfTickets = requiredSeats
            };

            db.Bookings.Add(newBooking);
            db.SaveChanges();

            // display success message 
            MessageBox.Show("Booking was successful");

            tblkAvailableSeats.Text = (availableSeats - requiredSeats).ToString();
            SearchByDate(selectedDate);
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

                    // get total required tickets 
                    int requiredTickets = int.Parse(tbxRequiredSeats.Text);
                    
                    // get current abailable seats 
                    int availableSeats = (TotalCapacity - totalTicketsSold);

                    if (availableSeats >= requiredTickets)
                    {
                        tblkAvailableSeats.Text = availableSeats.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Sold Out");
                        tblkAvailableSeats.Text = "0";
                        return; 
                    }
                }
                else
                {
                    lbxMovieListings.ItemsSource = null; 
                    // refresh screen if no result is found 
                    RefreshScreen();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Sorry, error occured when trying to connect to database" + ex.Message);
            }
        }

        private void lbxMovieListings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // when movie title  is selected, a synopsis will be displayed in the text block on the right of the screen 

            if (lbxMovieListings.SelectedItem is Movie selectedMovie)
            {
                tblkMovieSynopsis.Text = selectedMovie.Description;
            }
        }

        // update ui with refreshing screen 
        public void RefreshScreen()
        {
            
            tblkAvailableSeats.Text =  TotalCapacity.ToString();
            tblkMovieSynopsis.Text = "";

          
        }

        // load initial movies 
        private void DisplayInitialMovieTitles()
        {
            using (db)
            {
                // LINQ query to display movie titles in list box 
                var movies = db.Movies
                    .Where(m => m.Title != null)
                    .OrderBy(m => m.Title)
                    .ToList();
                             
                lbxMovieListings.ItemsSource = movies;
            }
        }

       
    }
}
