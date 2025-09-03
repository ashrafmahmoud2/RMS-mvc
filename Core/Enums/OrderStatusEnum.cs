namespace RMS.Web.Core.Enums;
public enum OrderStatusEnum
{
    Received = 0,              // تم استلام الطلب (Pending)
    Preparing = 1,             // جاري التحضير
    Delivering = 2,            // في مرحلة التوصيل
    DriverConfirmedDelivery = 3,             //// Driver confirms the order has arrived( تم التوصيل)
    CustomerConfirmedDelivery = 4, // Customer confirms the order has arrived (driver has no access
    CancelledFromRestaurant = 5,
    CancelledFromCustomer = 6

}


