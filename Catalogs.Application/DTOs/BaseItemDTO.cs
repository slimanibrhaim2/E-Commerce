using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogs.Application.DTOs
{
    public class BaseItemDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public bool IsAvailable { get; set; }
        public Guid CategoryId { get; set; }
        public Guid UserId { get; set; }
        // Optionally add collections for Media, Favorites, etc. if needed
    }
}
