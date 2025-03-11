using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WishlistService.Data;
using WishlistService.Models;

namespace WishlistService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly WishlistContext _context;

        public WishlistController(WishlistContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromBody] WishlistItem wishlist)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Token is missing.");

            var userEmail = GetEmailFromToken(token);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("Invalid token.");

            //var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            //if (string.IsNullOrEmpty(userEmail))
            //    return Unauthorized("Invalid token.");

            wishlist.UserEmail = userEmail;
            _context.WishlistItems.Add(wishlist);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Event added to wishlist!" });
        }

        [HttpGet]
        public IActionResult GetWishlist()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Token is missing.");

            var userEmail = GetEmailFromToken(token);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("Invalid token.");

            var userWishlists =  _context.WishlistItems.Where(w => w.UserEmail == userEmail).ToList();
            return Ok(userWishlists);
        }

        [HttpDelete("{eventId}")]
        public async Task<IActionResult> RemoveFromWishlist(string eventId)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Token is missing.");

            var userEmail = GetEmailFromToken(token);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("Invalid token.");

            var wishlistItem = await _context.WishlistItems
                .FirstOrDefaultAsync(x => x.EventId == eventId && x.UserEmail == userEmail);

            if (wishlistItem == null)
                return NotFound("Wishlist item not found or unauthorized.");

            _context.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Event removed from wishlist!" });
        }

        private string? GetEmailFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }
    }
}
