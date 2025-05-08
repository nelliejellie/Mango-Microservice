using Microsoft.AspNetCore.Mvc;
using Mango.Web.Services.IService;
using Mango.Web.Models.DTO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Text;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto> list = new ();
            var response = await _couponService.GetAllCouponAsync();
            if (response != null && response.Success)
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
            }else
            {
                TempData["Error"] = response?.Message;
                return RedirectToAction("Index", "Home");
            }
            
            return View(list);
        }

        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CouponCreate(CouponDto couponDto)
        {
            if (ModelState.IsValid)
            {
                var response = await _couponService.CreateCouponAsync(couponDto);
                if (response != null && response.Success)
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["Error"] = response?.Message;
                    return RedirectToAction("Index", "Home");
                }   
            }
            return View(couponDto);
        }
        public async Task<IActionResult> CouponEdit(int id)
        {
            var response = await _couponService.GetCouponByIdAsync(id);
            if (response != null && response.Success)
            {
                var couponDto = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
                return View(couponDto);
            }
            else
            {
                TempData["Error"] = response?.Message;
                return RedirectToAction("Index", "Home");
            }
            return NotFound();
        }

        public async Task<IActionResult> CouponDelete(int couponId)
        {
            var response = await _couponService.DeleteCouponAsync(couponId);
            if (response != null && response.Success)
            {
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["Error"] = response?.Message;
                return RedirectToAction("Index", "Home");
            }
            return NotFound();
        }
    }

}