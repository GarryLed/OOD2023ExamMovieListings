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
            var selectedMovie = lbxMovieListings.SelectedItem as Movie;
            if (selectedMovie == null)
            {
                MessageBox.Show("Please select a movie.");
                return;
            }


            if (!dpshowMovie.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a valid date.");
                return;
            }

            DateTime selectedDate = dpshowMovie.SelectedDate.Value;


            if (!int.TryParse(tbxRequiredSeats.Text, out int requiredSeats) || requiredSeats <= 0)
            {
                MessageBox.Show("Please enter a valid number of seats.");
                return;
            }

            var existingBookings = db.Bookings
                .Where(b => b.MovieId == selectedMovie.MovieId && b.BookingDate == selectedDate)
                .ToList();

            int totalBooked = existingBookings.Sum(b => b.NumberOfTickets);
            int availableSeats = TotalCapacity - totalBooked;

            if (availableSeats < requiredSeats)
            {
                MessageBox.Show($"Only {availableSeats} seats are available. Please choose a smaller number.");
                return;
            }

            var newBooking = new Booking
            {
                MovieId = selectedMovie.MovieId,
                BookingDate = selectedDate,
                NumberOfTickets = requiredSeats
            };

            db.Bookings.Add(newBooking);
            db.SaveChanges();

            MessageBox.Show("Booking successful!");
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
                // Get all bookings for the selected date
                var bookingsOnDate = db.Bookings
                    .Include(b => b.Movie)
                    .Where(b => DbFunctions.TruncateTime(b.BookingDate) == selectedDate.Date)
                    .ToList();

                // Get unique movies booked on that date
                var moviesOnDate = bookingsOnDate
                    .Select(b => b.Movie)
                    .Distinct()
                    .ToList();

                if (moviesOnDate.Any())
                {
                    lbxMovieListings.ItemsSource = moviesOnDate;

                   
                    var firstMovie = moviesOnDate.FirstOrDefault();
                    if (firstMovie != null)
                    {
                        int totalTicketsSold = bookingsOnDate
                            .Where(b => b.MovieId == firstMovie.MovieId)
                            .Sum(b => b.NumberOfTickets);

                        int availableSeats = TotalCapacity - totalTicketsSold;
                        tblkAvailableSeats.Text = availableSeats.ToString();
                    }
                }
                else
                {
                    lbxMovieListings.ItemsSource = null;
                    RefreshScreen();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading bookings: " + ex.Message);
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
            try
            {
                var movies = db.Movies
                    .Where(m => !string.IsNullOrEmpty(m.Title))
                    .OrderBy(m => m.Title)
                    .ToList();

                lbxMovieListings.ItemsSource = movies;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load movies: " + ex.Message);
            }
        }


    }
}
