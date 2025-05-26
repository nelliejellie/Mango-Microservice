using Mango.Services.CouponApi.Data;
using Mango.Services.CouponApi.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponApi.Extensions
{
    public static class CouponEndpoints
    {
        public static void MapCouponEndpoints(this WebApplication app)
        {
            var response = new ResponseDto();

            app.MapGet("/coupon", async (AppDbContext _db) =>
            {
                try
                {
                    var coupons = await _db.Coupons.ToListAsync();
                    response.StatusCode = 200;
                    response.Result = coupons;
                    response.Success = true;
                    response.Message = "Coupons fetched successfully.";
                }
                catch (System.Exception)
                {
                    response.StatusCode = 500;
                    response.Message = "An error occurred while fetching coupons.";
                    response.Success = false;
                    throw;
                }

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetCoupons")
            .WithOpenApi();
             
            app.MapGet("/coupon/discount/{code}", async (string code, AppDbContext _db) =>
            {
                var response = new ResponseDto();

                try
                {
                    var coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.CouponCode == code);
                    if (coupon == null)
                    {
                        response.StatusCode = 404;
                        response.Message = "Coupon not found.";
                        response.Success = false;
                        return Results.NotFound(response);
                    }

                    response.StatusCode = 200;
                    response.Result = coupon;
                    response.Success = true;
                    response.Message = "Coupon fetched successfully.";
                }
                catch (System.Exception)
                {
                    response.StatusCode = 500;
                    response.Message = "An error occurred while fetching the coupon.";
                    response.Success = false;
                    throw;
                }

                return Results.Ok(response);
            })
            .WithName("GetCouponbyCode")
            .WithOpenApi();

            app.MapGet("/coupon/{id}", async (int id, AppDbContext _db) =>
            {
                try
                {
                    var coupon = await _db.Coupons.FindAsync(id);
                    if (coupon == null)
                    {
                        response.StatusCode = 404;
                        response.Message = "Coupon not found.";
                        response.Success = false;
                        return Results.NotFound(response);
                    }

                    response.StatusCode = 200;
                    response.Result = coupon;
                    response.Success = true;
                    response.Message = "Coupon fetched successfully.";
                }
                catch (System.Exception)
                {
                    response.StatusCode = 500;
                    response.Message = "An error occurred while fetching the coupon.";
                    response.Success = false;
                    throw;
                }

                return Results.Ok(response);
            })
            .WithName("GetCouponbyId")
            .WithOpenApi();

            app.MapPost("/coupon", async (CouponDto couponDto, AppDbContext _db) =>
            {
                var response = new ResponseDto();

                try
                {
                    var coupon = new Models.Coupon
                    {
                        CouponCode = couponDto.CouponCode,
                        MinAmount = couponDto.MinAmount,
                        DiscountAmount = couponDto.DiscountAmount
                    };

                    await _db.Coupons.AddAsync(coupon);
                    await _db.SaveChangesAsync();

                    response.StatusCode = 201;
                    response.Result = coupon;
                    response.Success = true;
                    response.Message = "Coupon created successfully.";

                    return Results.Created($"/coupon/{coupon.CouponId}", response);
                }
                catch (Exception)
                {
                    response.StatusCode = 500;
                    response.Message = "An error occurred while creating the coupon.";
                    response.Success = false;

                    return Results.Problem(response.Message);
                }
            })
            .WithName("AddCoupon")
            .WithOpenApi();
            
            app.MapDelete("/coupon/{id}", async (int id, AppDbContext _db) =>
            {
                var response = new ResponseDto();

                try
                {
                    var coupon = await _db.Coupons.FindAsync(id);
                    if (coupon == null)
                    {
                        response.StatusCode = 404;
                        response.Message = "Coupon not found.";
                        response.Success = false;
                        return Results.NotFound(response);
                    }

                    _db.Coupons.Remove(coupon);
                    await _db.SaveChangesAsync();

                    response.StatusCode = 200;
                    response.Result = coupon;
                    response.Success = true;
                    response.Message = "Coupon deleted successfully.";
                }
                catch (System.Exception)
                {
                    response.StatusCode = 500;
                    response.Message = "An error occurred while deleting the coupon.";
                    response.Success = false;
                    throw;
                }

                return Results.Ok(response);
            })
            .WithName("DeleteCoupon")
            .WithOpenApi();
            app.MapPut("/coupon/{id}", async (int id, CouponDto couponDto, AppDbContext _db) =>
            {
                var response = new ResponseDto();

                try
                {
                    var coupon = await _db.Coupons.FindAsync(id);
                    if (coupon == null)
                    {
                        response.StatusCode = 404;
                        response.Message = "Coupon not found.";
                        response.Success = false;
                        return Results.NotFound(response);
                    }

                    coupon.CouponCode = couponDto.CouponCode;
                    coupon.MinAmount = couponDto.MinAmount;
                    coupon.DiscountAmount = couponDto.DiscountAmount;

                    _db.Coupons.Update(coupon);
                    await _db.SaveChangesAsync();

                    response.StatusCode = 200;
                    response.Result = coupon;
                    response.Success = true;
                    response.Message = "Coupon updated successfully.";
                }
                catch (System.Exception)
                {
                    response.StatusCode = 500;
                    response.Message = "An error occurred while updating the coupon.";
                    response.Success = false;
                    throw;
                }

                return Results.Ok(response);
            })
            .WithName("UpdateCoupon")
            .WithOpenApi();
        }
    }
}