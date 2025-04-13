using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieListings
{
    public class Movie
    {
        // properties 
        public int MovieId { get; set; }
        public string Title { get; set; }   

        public string ImageName { get; set; }

        public string Description { get; set; }

        public string Cast { get; set; }

        public virtual List<Movie> Movies { get; set; } // one to many relationship between movie and bookings 
    }
}
