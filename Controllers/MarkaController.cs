using Microsoft.AspNetCore.Authorization;
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
    public class MarkaController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public MarkaController(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }
        public async Task<IActionResult> Index()
        {

            return View(await _dbContext.Markas.ToListAsync());
        }
        public IActionResult Create()
        {
            Marka Catogery = new Marka { };
            return View(Catogery);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Marka marka)
        {
            if (!ModelState.IsValid)
                return View(ModelState);

            if (marka.Name == null)
            {
                ModelState.AddModelError("Name", "Name is Empty !");
                return View(marka);
            }
            if (await _dbContext.Markas.AnyAsync(n => n.Name == marka.Name))
            {
                ModelState.AddModelError("Name", "Name is Exist !");
                return View(marka);
            }

            Marka _marka = new Marka
            {
                Name = marka.Name
            };
            await _dbContext.Markas.AddAsync(_marka);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var _marka = await _dbContext.Markas.FindAsync(id);
            if (_marka is null)
                return NotFound();
            return View(_marka);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Marka marka )
        {
            if (!ModelState.IsValid)
                return View(ModelState);
            var _marka = await _dbContext.Markas.FindAsync(marka.Id);

            if (_marka is null)
                return NotFound();

            if (marka.Name == null)
            {
                ModelState.AddModelError("Name", "Name is Empty!");
                return View(marka);
            }
            if (await _dbContext.ClothesClassifications.AnyAsync(n => n.Name == marka.Name))
            {
                ModelState.AddModelError("Name", "Name is Exist !");
                return View(marka);
            }

            _marka.Name = marka.Name;
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
                return BadRequest();
            var _marka = await _dbContext.Markas.FindAsync(Id);
            if (_marka == null)
                return NotFound();
            _dbContext.Markas.Remove(_marka);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
