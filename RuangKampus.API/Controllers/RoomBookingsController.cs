using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuangKampus.API.Data;
using RuangKampus.API.Models;

namespace RuangKampus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomBookingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomBookingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/RoomBookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomBooking>>> GetBookings()
        {
            return await _context.RoomBookings
                .Include(b => b.Room)
                .ToListAsync();
        }

        // POST: api/RoomBookings
        [HttpPost]
        public async Task<ActionResult<RoomBooking>> CreateBooking(RoomBooking booking)
        {
            // 1. Cek bentrok jadwal
            var isConflict = await _context.RoomBookings.AnyAsync(b =>
                b.RoomId == booking.RoomId &&
                b.Status == BookingStatus.Approved &&
                (
                    (booking.StartTime >= b.StartTime && booking.StartTime < b.EndTime) ||
                    (booking.EndTime > b.StartTime && booking.EndTime <= b.EndTime) ||
                    (booking.StartTime <= b.StartTime && booking.EndTime >= b.EndTime)
                )
            );

            // 2. Kalau bentrok, tolak
            if (isConflict)
            {
                return BadRequest("Ruangan sudah dibooking di jam tersebut.");
            }

            // 3. Kalau aman, simpan
            _context.RoomBookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookings), new { id = booking.Id }, booking);
        }

        // PATCH: api/RoomBookings/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] BookingStatus status)
        {
            var booking = await _context.RoomBookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            booking.Status = status;
            await _context.SaveChangesAsync();

            return Ok(booking); 
        }

        // GET: api/RoomBookings/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomBooking>> GetBookingById(int id)
        {
            var booking = await _context.RoomBookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();

            return Ok(booking);
        }

        // GET: api/RoomBookings/by-status?status=Approved
        [HttpGet("by-status")]
        public async Task<IActionResult> GetByStatus([FromQuery] BookingStatus status)
        {
            var bookings = await _context.RoomBookings
                .Include(b => b.Room)
                .Where(b => b.Status == status)
                .ToListAsync();

            return Ok(bookings);
        }

        // GET: api/RoomBookings/by-room?roomId=1
        [HttpGet("by-room")]
        public async Task<IActionResult> GetByRoom([FromQuery] int roomId)
        {
            var bookings = await _context.RoomBookings
                .Include(b => b.Room)
                .Where(b => b.RoomId == roomId)
                .ToListAsync();

            return Ok(bookings);
        }

        // GET: api/RoomBookings/by-date?date=2026-02-12
        [HttpGet("by-date")]
        public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
        {
            var bookings = await _context.RoomBookings
                .Include(b => b.Room)
                .Where(b =>
                    b.StartTime.Date == date.Date
                )
                .ToListAsync();

            return Ok(bookings);
        }

        // DELETE: api/RoomBookings/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.RoomBookings.FindAsync(id);

            if (booking == null)
                return NotFound();

            _context.RoomBookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}