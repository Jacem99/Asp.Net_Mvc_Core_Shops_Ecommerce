using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shops.Data;
using Shops.Enumerations;
using Shops.Models;
using Shops.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shops.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly List<string> _arrayOfExtentions = new() { ".png", ".jpg" };

        public ProductController(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }

      
        public async Task< IActionResult> Index()
        {
            var product = await _dbContext.products.Include(m => m.Marka).ToListAsync();

            return View(product);
        }
        private async Task< IActionResult> Extention(ViewProducts viewProducts ,string view)
        {
            ModelState.AddModelError("product.Image", "Must have one of extentions from PNG,JPG ");
            return View(view, await GetSelectItem(viewProducts));
        }
        private async Task<IActionResult> Rate(ViewProducts viewProducts , string view)
        {
            ModelState.AddModelError("All", "Please Choice Rate less than or equal 5");
            return View(view, await GetSelectItem(viewProducts));
        }
        private async Task<IActionResult> Price(ViewProducts viewProducts , string view)
        {
            ModelState.AddModelError("All", "Please Enter price correct for product..!");
            return View(view, await GetSelectItem(viewProducts));
        }
       
        public async Task<IActionResult> Edit(int id)
        {
            ViewProducts viewProducts = new ViewProducts {};
            var vProduct = await GetSelectItem(viewProducts);
            vProduct.product = await _dbContext.products.FindAsync(id);
           return View(vProduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ViewProducts viewProducts)
        {
            var prd = await _dbContext.products.FindAsync(viewProducts.product.Id);
            if (prd == null)
                return BadRequest();

            if (viewProducts == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(await GetSelectItem(viewProducts));
            }
            var file = Request.Form.Files;
           
            if (file.Any())
            {
                using var stream = new MemoryStream();
                file[0].CopyTo(stream);
                viewProducts.product.Image = stream.ToArray();

                var ExtentionPath = Path.GetExtension(file[0].FileName.ToLower());
                if (!_arrayOfExtentions.Contains(ExtentionPath))
                {
                  return await Extention(viewProducts, nameof(Edit));
                }
               
                file[0].CopyTo(stream);
                var fileImage = stream.ToArray();
                prd.Image = stream.ToArray();
                prd.Name = file[0].FileName;
            }

            prd.Size = ((EnumClasses.Sizing)Int16.Parse(viewProducts.product.Size)).ToString();
            prd.SizeNumber = viewProducts.product.SizeNumber;
          
            if(viewProducts.product.Rate > 5)
            {
                return await Rate(viewProducts, nameof(Edit));
            }
            if (viewProducts.product.Price < 0)
            {
                return await Price(viewProducts, nameof(Create));
            }
          
            prd.Title = viewProducts.product.Title;
            prd.Price = viewProducts.product.Price;
            prd.Discount = viewProducts.product.Discount;
            prd.AgeStageId = viewProducts.AgeStageId;
            prd.ClothesClassificationId = viewProducts.ClothesClassificationId;
            prd.HumanClassId = viewProducts.HumanClassId;
            prd.MarkaId = viewProducts.MarkaId;
            prd.Rate = viewProducts.product.Rate;

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Create()
        {
            ViewProducts viewProducts = new ViewProducts { };
            return View(await GetSelectItem(viewProducts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ViewProducts viewProducts)
        {
            var file = Request.Form.Files;

            if (!ModelState.IsValid)
            {
                return View(await GetSelectItem(viewProducts));
            }

            if (viewProducts == null)
                return BadRequest();

            
            if (!file.Any())
            {
                ModelState.AddModelError("product.Image", "Please Select Image Product !");
                return View(await GetSelectItem(viewProducts));
            }
            using var stream = new MemoryStream();
            file[0].CopyTo(stream);
            viewProducts.product.Image = stream.ToArray();
             var ExtentionPath = Path.GetExtension(file[0].FileName.ToLower());
            if (!_arrayOfExtentions.Contains(ExtentionPath))
            {
                return await Extention(viewProducts, nameof(Create));
            }
           /* if (await _dbContext.products.AnyAsync(p => p.Image == stream.ToArray()))
            {
                ModelState.AddModelError("product.Image", "File Exist !!");
                return View(await GetSelectItem(viewProducts));
            }*/
            if (viewProducts.product.Rate > 5)
            {
                return await Rate(viewProducts, nameof(Create));
            }
            if (viewProducts.product.Price < 0)
            {
                return await Price(viewProducts, nameof(Create));
            }
           
            Product product = new Product()
            {
                Name = file[0].FileName,
                Title = viewProducts.product.Title,
                Image = stream.ToArray(),
                Price = viewProducts.product.Price,
                Size = ((EnumClasses.Sizing)Int16.Parse(viewProducts.product.Size)).ToString(),
                SizeNumber = viewProducts.product.SizeNumber,
                Discount = viewProducts.product.Discount,
                AgeStageId = viewProducts.AgeStageId,
                ClothesClassificationId = viewProducts.ClothesClassificationId,
               HumanClassId = viewProducts.HumanClassId,
                MarkaId = viewProducts.MarkaId,
                Rate = viewProducts.product.Rate
            };
            await _dbContext.products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private async Task<ViewProducts> GetSelectItem(ViewProducts viewProducts)
        {
            viewProducts.Markas = await _dbContext.Markas.AsNoTracking().ToListAsync();
            viewProducts.ClothesClassifications = await _dbContext.ClothesClassifications.AsNoTracking().ToListAsync();
            viewProducts.AgeStage = await _dbContext.AgeStages.OrderBy(o => o.Name).AsNoTracking().ToListAsync();
            viewProducts.HumanClasses = await _dbContext.HumanClass.OrderBy(o => o.Name).AsNoTracking().ToListAsync();
            return viewProducts;
        }
        public async Task<IActionResult> Delete(int Id)
        {
            var product = await _dbContext.products.SingleOrDefaultAsync(p=> p.Id == Id);
            if(product is null)
            {
                return NotFound();
            };
            _dbContext.products.Remove(product);
             _dbContext.SaveChanges();
            return Ok();
        }

    }
}
