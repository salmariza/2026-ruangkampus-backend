namespace RuangKampus.API.Models
{
    public class RoomBookingDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string BookerName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public string PurposeOfBooking { get; set; } 
    }
}
