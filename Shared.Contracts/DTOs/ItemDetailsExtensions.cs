using System.Linq;

namespace Shared.Contracts.DTOs
{
    public static class ItemDetailsExtensions
    {
        public static string GetName(this ItemDetailsDTO itemDetails)
        {
            return itemDetails switch
            {
                ProductDetailsDTO product => product.Name,
                ServiceDetailsDTO service => service.Name,
                _ => "عنصر غير معروف"
            };
        }

        public static string GetImageUrl(this ItemDetailsDTO itemDetails)
        {
            if (itemDetails is ProductDetailsDTO product && product.Media?.Any() == true)
            {
                return product.Media.FirstOrDefault()?.Url;
            }
            else if (itemDetails is ServiceDetailsDTO service && service.Media?.Any() == true)
            {
                return service.Media.FirstOrDefault()?.Url;
            }
            return null;
        }
    }
} 