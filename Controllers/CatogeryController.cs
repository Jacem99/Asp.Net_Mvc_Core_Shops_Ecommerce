using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shops.Data;
using Shops.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shops.Controllers
{
        [Authorize(Roles = "Admin")]
    public class CatogeryController : Controller
    {
            private readonly ApplicationDbContext _dbContext;

            public CatogeryController(ApplicationDbContext applicationDbContext)
            {
                _dbContext = applicationDbContext;
            }
        public async Task<IActionResult> Index()
        {

            return View(await _dbContext.ClothesClassifications.OrderBy(m => m.Name).ToListAsync());
        }
    
  
        public async Task<IActionResult> Modify(int? id)
        {
            if(id == null)
            {
                ClothesClassification Catogery = new ClothesClassification { };
                return View(Catogery);
            }
            var catogery = await _dbContext.ClothesClassifications.FindAsync(id);
            if (catogery is null)
                return NotFound();
            return View(catogery);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(ClothesClassification Catogery)
        {
            if (!ModelState.IsValid)
                return View(ModelState);

            if (Catogery.Name == null)
            {
                ModelState.AddModelError("Name", "Name is Empty!");
                return View(Catogery);
            }

            if (await _dbContext.ClothesClassifications.AnyAsync(n => n.Name == Catogery.Name))
            {
                ModelState.AddModelError("Name", "Name is Exist !");
                return View(Catogery);
            }


            if(Catogery.Id <= 0)
            {
                ClothesClassification catog = new ClothesClassification
                {
                    Name = Catogery.Name
                };
                await _dbContext.ClothesClassifications.AddAsync(catog);
            }
            else
            {
                var catogery = await _dbContext.ClothesClassifications.FindAsync(Catogery.Id);

                if (catogery is null)
                     return NotFound();

                catogery.Name = Catogery.Name;
            }
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
                return BadRequest();
            var catogery = await _dbContext.ClothesClassifications.FindAsync(Id);
            if (catogery == null)
                return NotFound();
            _dbContext.ClothesClassifications.Remove(catogery);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }

}
