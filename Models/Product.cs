using Seminar_1.Models;
using System.Data.Common;

namespace Seminar_1.Models
{
    public class Product
    {
        public Product()
        {
            Name = string.Empty;
            Description = string.Empty;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImagePath { get; set; }
        public string? Category { get; set; }
    }
}

