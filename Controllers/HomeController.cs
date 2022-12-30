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
using static Shops.Models.Product;
using static Shops.ViewModels.ViewProducts;

namespace Shops.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }
        
        public async Task< IActionResult> Index(ClassProduct classProduct)
        {
            classificationsOfProducts classificationsOfProducts = new classificationsOfProducts
            {
                ClothesClassifications = await _dbContext.ClothesClassifications.AsNoTracking().ToListAsync(),
                HumanClassifications = await _dbContext.HumanClass.AsNoTracking().ToListAsync(),
                AgeStages = await _dbContext.AgeStages.AsNoTracking().ToListAsync(),
                userProducts = await _dbContext.UserProducts.AsNoTracking().ToListAsync(),
               
            };
            IQueryable<Product> resultProduct = _dbContext.products;
            if(classProduct.ClothesClassificationsId >0 && classProduct.AgeStagesId >0 && classProduct.HumanClassificationsId > 0)
            {
                resultProduct = resultProduct.Where(c => c.ClothesClassificationId == classProduct.ClothesClassificationsId && c.HumanClassId == classProduct.HumanClassificationsId && c.AgeStageId == classProduct.AgeStagesId);
            }
            if (classProduct.ClothesClassificationsId > 0 && classProduct.HumanClassificationsId > 0 && classProduct.AgeStagesId ==0 )
            {
                resultProduct = resultProduct.Where(c => c.ClothesClassificationId == classProduct.ClothesClassificationsId && c.HumanClassId == classProduct.HumanClassificationsId);
            }
            if (classProduct.HumanClassificationsId > 0 && classProduct.AgeStagesId > 0 && classProduct.ClothesClassificationsId == 0)
            {
                resultProduct = resultProduct.Where(c => c.HumanClassId == classProduct.HumanClassificationsId && c.AgeStageId == classProduct.AgeStagesId);
            }
            if (classProduct.ClothesClassificationsId > 0 && classProduct.AgeStagesId > 0 && classProduct.HumanClassificationsId == 0)
            {
                resultProduct = resultProduct.Where(c => c.ClothesClassificationId == classProduct.ClothesClassificationsId && c.AgeStageId == classProduct.AgeStagesId);
            }
            if (classProduct.ClothesClassificationsId > 0 && classProduct.HumanClassificationsId == 0 && classProduct.AgeStagesId == 0)
            {
                resultProduct = resultProduct.Where(c => c.ClothesClassificationId == classProduct.ClothesClassificationsId);
            }
            if (classProduct.HumanClassificationsId> 0 && classProduct.ClothesClassificationsId == 0 && classProduct.AgeStagesId == 0)
            {
                resultProduct = resultProduct.Where(c => c.HumanClassId == classProduct.HumanClassificationsId);
            }
            if (classProduct.AgeStagesId > 0 && classProduct.HumanClassificationsId == 0 && classProduct.ClothesClassificationsId == 0)
            {
                resultProduct = resultProduct.Where(c => c.AgeStageId == classProduct.AgeStagesId);
            }
            resultProduct = resultProduct.Include(p => p.Marka).Include(p => p.ClothesClassification);
            classificationsOfProducts.Product = await resultProduct.AsSplitQuery().AsNoTracking().ToListAsync();
            return View(classificationsOfProducts);
        }
        public async Task< IActionResult> Create()
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
                ModelState.AddModelError("Image", "Please Select Image Product !");
                return View(await GetSelectItem(viewProducts));
            }
            var arrayOfExtentions = new List<string> { ".png", ".jpg" };
            var ExtentionPath = Path.GetExtension(file[0].FileName.ToLower());
            if (!arrayOfExtentions.Contains(ExtentionPath))
            {
                ModelState.AddModelError("Image", "Must have one of extentions from PNG,JPG ");
                return View(await GetSelectItem(viewProducts));
            }
            using var stream = new MemoryStream();
            file[0].CopyTo(stream);
            if (await _dbContext.products.AnyAsync(p => p.Image == stream.ToArray()))
            {
                ModelState.AddModelError("Image", "File Exist !!");
                return View(await GetSelectItem(viewProducts));
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
                ClothesClassificationId =viewProducts.ClothesClassificationId,
                MarkaId = viewProducts.MarkaId
            };
            await _dbContext.products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private async Task<ViewProducts> GetSelectItem(ViewProducts viewProducts)
        {
            viewProducts.Markas = await _dbContext.Markas.AsNoTracking().ToListAsync();
            viewProducts.ClothesClassifications = await _dbContext.ClothesClassifications.AsNoTracking().ToListAsync();
            viewProducts.AgeStage = await _dbContext.AgeStages.AsNoTracking().ToListAsync();
            return viewProducts;
        }

    }



      //Classfication Human
        /*{
       public async Task<IActionResult> Human(string Name)
            return View(await _dbContext.products.Include(p => p.Marka).Where()
                .Include(p => p.ClothesClassification).AsSplitQuery()
                .AsNoTracking().ToListAsync());
        }
      */
        //Classfication AgeStage
        /*{
       public async Task<IActionResult> AgeStage(string Name)
            return View(await _dbContext.products.Include(p => p.Marka).Where()
                .Include(p => p.ClothesClassification).AsSplitQuery()
                .AsNoTracking().ToListAsync());
        }
      */
}
