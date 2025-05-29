using Mango.Services.ShoppingCart.Models;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class ShoppingCartApiController : ControllerBase
    {
        private ResponseDto _responseDto;
        private readonly ILogger<ShoppingCartApiController> _logger;
        private readonly AppDbContext _context;
        public ShoppingCartApiController(AppDbContext context)
        {
            _context = context;
            _responseDto = new ResponseDto();
        }

        [HttpPost("CartUpsert")]
        public async Task<IActionResult> CartUpsert(CartDto cartDto)
        { 
            try
            {
                var cartHeaderFromDb = await _context.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if(cartHeaderFromDb == null)
                {
                    // Create a new CartHeader if it doesn't exist
                    cartHeaderFromDb = new CartHeader
                    {
                        UserId = cartDto.CartHeader.UserId,
                        CartTotal = 0
                    };
                    await _context.CartHeaders.AddAsync(cartHeaderFromDb);
                }
                else
                {
                    //if card is not null
                    // update the existing CartHeader
                    var cartDetailsFromDb = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u => u.ProductId == cartDto.CartDetails.First().ProductId &&
                        u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if(cartDetailsFromDb != null)
                    {
                        //create header and details
                    }
                    else
                    {

                    }
                }
               
                _responseDto.Message = "Items collected successfully";
                _responseDto.StatusCode = 200;
                _responseDto.Success = true;
            }
            catch (Exception ex)
            {
                _responseDto.Success = false;
                _responseDto.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _responseDto);
            }
            return Ok(_responseDto);
        }
    }
}
