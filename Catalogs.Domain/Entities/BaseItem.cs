using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalogs.Domain.Entities;

namespace Catalogs.Domain.Entities
{
    public class BaseItem 
    {
        public Guid Id { get; set; }
        public string Name { get;  set; }
        public string Description { get; set; }
        public decimal Price { get;  set; }
        public bool IsAvailable { get;  set; }
        public Guid CategoryId { get;  set; }
        public Category Category { get;  set; }
        public Guid UserId { get;  set; }
        public ICollection<Favorite> Favorites { get;  set; }
        public ICollection<Media> Media { get;  set; }

        public BaseItem()
        {
            Favorites = new List<Favorite>();
            Media = new List<Media>();
        }

        public BaseItem(
            string name,
            string description,
            decimal price,
            Guid categoryId,
            Guid userId) : this()
        {
            SetName(name);
            SetDescription(description);
            SetPrice(price);
            SetCategory(categoryId);
            SetUser(userId);
            IsAvailable = true;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            Name = name;
        }

        public void SetDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));

            Description = description;
        }

        public void SetPrice(decimal price)
        {
            if (price < 0)
                throw new ArgumentException("Price cannot be negative", nameof(price));

            Price = price;
        }

        public void SetAvailability(bool isAvailable)
        {
            IsAvailable = isAvailable;
        }

        public void SetCategory(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
                throw new ArgumentException("Category ID cannot be empty", nameof(categoryId));

            CategoryId = categoryId;
        }

        public void SetUser(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            UserId = userId;
        }

        public void AddFavorite(Favorite favorite)
        {
            if (favorite == null)
                throw new ArgumentNullException(nameof(favorite));

            Favorites.Add(favorite);
        }

        public void AddMedia(Media media)
        {
            if (media == null)
                throw new ArgumentNullException(nameof(media));

            Media.Add(media);
        }
    }
}
