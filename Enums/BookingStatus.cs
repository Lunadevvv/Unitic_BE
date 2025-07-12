public enum BookingStatus
{
    Paid = 1,           //Đã mua
    Confirmed = 2,      // Đã check in
    Failed = 3,         // Có lỗi khi mua chưa biết nên thêm ở đâu?
    Refunded = 4,       // Hoàn tiền
    Expired = 5         // Không check in
}