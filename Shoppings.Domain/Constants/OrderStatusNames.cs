namespace Shoppings.Domain.Constants;

public static class OrderStatusNames
{
    public const string Pending = "قيد الانتظار";
    public const string Confirmed = "تم التأكيد";
    public const string InPreparation = "قيد التحضير";
    public const string InShipping = "قيد الشحن";
    public const string Delivered = "تم التوصيل";
    public const string Cancelled = "ملغي";
    public const string Refunded = "مسترد";
} 