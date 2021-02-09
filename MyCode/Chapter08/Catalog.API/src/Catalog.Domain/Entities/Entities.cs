using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Domain.Entities
{
    //Artist.cs file
    public class Artist
    {
        public Guid ArtistId { get; set; }
        public string ArtistName { get; set; }
    }
    //Genre.cs file
    public class Genre
    {
        public Guid GenreId { get; set; }
        public string GenreDescription { get; set; }
    }
    //Price.cs file
    public class Price
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
