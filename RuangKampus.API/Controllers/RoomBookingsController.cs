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
public async Task<ActionResult<IEnumerable<RoomBookingDto>>> GetBookings()
{
    var bookings = await _context.RoomBookings
        .Include(b => b.Room)
        .ToListAsync();

    var result = bookings.Select(b => new RoomBookingDto
    {
        Id = b.Id,
        RoomId = b.RoomId,
        RoomName = b.Room != null ? b.Room.Name : "",
        BookerName = b.BookerName,
        PurposeOfBooking = b.PurposeOfBooking,
        StartTime = b.StartTime,
        EndTime = b.EndTime,
        Status = b.Status.ToString()
    }).ToList();

    return Ok(result);
}


        // POST: api/RoomBookings
        [HttpPost]
        public async Task<ActionResult<RoomBookingDto>> CreateBooking(RoomBookingDto bookingDto)
        {
            // Validasi StartTime dan EndTime
            if (bookingDto.StartTime >= bookingDto.EndTime)
            {
                return BadRequest("StartTime harus lebih kecil dari EndTime.");
            }

            // Validasi RoomId
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == bookingDto.RoomId);
            if (room == null)
            {
                return BadRequest("RoomId tidak valid.");
            }

            // Validasi bentrok jadwal
            var isConflict = await _context.RoomBookings.AnyAsync(b =>
                b.RoomId == bookingDto.RoomId &&
                b.Status == BookingStatus.Approved &&
                (
                    (bookingDto.StartTime >= b.StartTime && bookingDto.StartTime < b.EndTime) ||
                    (bookingDto.EndTime > b.StartTime && bookingDto.EndTime <= b.EndTime) ||
                    (bookingDto.StartTime <= b.StartTime && bookingDto.EndTime >= b.EndTime)
                )
            );

            if (isConflict)
            {
                return BadRequest("Ruangan sudah dibooking di jam tersebut.");
            }

            // Simpan ke database
            var booking = new RoomBooking
            {
                RoomId = bookingDto.RoomId,
                BookerName = bookingDto.BookerName,
                StartTime = bookingDto.StartTime,
                EndTime = bookingDto.EndTime,
                Status = BookingStatus.Pending, // Default status
                PurposeOfBooking = bookingDto.PurposeOfBooking
            };

            try
            {
                _context.RoomBookings.Add(booking);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            // Mengonversi ke DTO untuk response
            var bookingResponse = new RoomBookingDto
            {
                Id = booking.Id,
                RoomId = booking.RoomId,
                BookerName = booking.BookerName,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Status = booking.Status.ToString(),
                PurposeOfBooking = booking.PurposeOfBooking
            };

            return CreatedAtAction(nameof(GetBookings), new { id = booking.Id }, bookingResponse);
        }

        // PATCH: api/RoomBookings/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, RoomBookingDto bookingDto)
        {
            var booking = await _context.RoomBookings.FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            // Validasi StartTime dan EndTime
            if (bookingDto.StartTime >= bookingDto.EndTime)
            {
                return BadRequest("StartTime harus lebih kecil dari EndTime.");
            }

            // Update fields
            booking.BookerName = bookingDto.BookerName;
            booking.StartTime = bookingDto.StartTime;
            booking.EndTime = bookingDto.EndTime;
            booking.Status = (BookingStatus)Enum.Parse(typeof(BookingStatus), bookingDto.Status);
            booking.PurposeOfBooking = bookingDto.PurposeOfBooking;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return NoContent(); // Successful update without returning data
        }

        // GET: api/RoomBookings/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomBookingDto>> GetBookingById(int id)
        {
            var booking = await _context.RoomBookings
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            var bookingDto = new RoomBookingDto
            {
                Id = booking.Id,
                RoomId = booking.RoomId,
                BookerName = booking.BookerName,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Status = booking.Status.ToString(),
                PurposeOfBooking = booking.PurposeOfBooking
            };

            return Ok(bookingDto);
        }

        // DELETE: api/RoomBookings/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.RoomBookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.RoomBookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/RoomBookings/by-status?status=Approved
        [HttpGet("by-status")]
        public async Task<IActionResult> GetByStatus([FromQuery] BookingStatus status)
        {
            var bookings = await _context.RoomBookings
                .Where(b => b.Status == status)
                .ToListAsync();

            var bookingDtos = bookings.Select(b => new RoomBookingDto
            {
                Id = b.Id,
                RoomId = b.RoomId,
                BookerName = b.BookerName,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status.ToString(),
                PurposeOfBooking = b.PurposeOfBooking
            }).ToList();

            return Ok(bookingDtos);
        }

        // GET: api/RoomBookings/by-room?roomId=1
        [HttpGet("by-room")]
        public async Task<IActionResult> GetByRoom([FromQuery] int roomId)
        {
            var bookings = await _context.RoomBookings
                .Where(b => b.RoomId == roomId)
                .ToListAsync();

            var bookingDtos = bookings.Select(b => new RoomBookingDto
            {
                Id = b.Id,
                RoomId = b.RoomId,
                BookerName = b.BookerName,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status.ToString(),
                PurposeOfBooking = b.PurposeOfBooking
            }).ToList();

            return Ok(bookingDtos);
        }

        // GET: api/RoomBookings/by-date?date=2026-02-12
        [HttpGet("by-date")]
        public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
        {
            var bookings = await _context.RoomBookings
                .Where(b => b.StartTime.Date == date.Date)
                .ToListAsync();

            var bookingDtos = bookings.Select(b => new RoomBookingDto
            {
                Id = b.Id,
                RoomId = b.RoomId,
                BookerName = b.BookerName,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status.ToString(),
                PurposeOfBooking = b.PurposeOfBooking
            }).ToList();

            return Ok(bookingDtos);
        }
    }
}
