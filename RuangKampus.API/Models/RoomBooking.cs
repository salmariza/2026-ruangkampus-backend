namespace RuangKampus.API.Models
{
    public class RoomBooking
    {
        public int Id { get; set; }

        public int RoomId { get; set; }
        public Room? Room { get; set; }

        public string BookerName { get; set; } = null!;

        public string PurposeOfBooking { get; set; } = null!;
        
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;
    }
}