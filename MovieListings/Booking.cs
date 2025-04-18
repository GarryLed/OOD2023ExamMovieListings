using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieListings
{
    public class Booking
    {

        // Properties 
        public  int BookingId { get; set; }

        public DateTime BookingDate { get; set; }
        public int NumberOfTickets { get; set; }

        // foreign key and relationship for Movie class 
        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }


       
    }
}
