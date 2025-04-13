using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace MovieListings
{
    public class MovieData : DbContext
    {
        public MovieData() :base("2023OODExam_GarryLedwith") { }

        public DbSet<Movie> Movies { get; set;}
        public DbSet<Booking> Bookings { get; set;}

    }
}
