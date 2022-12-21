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
               
            };
            if (classProduct.HumanClassificationsId == 0 && classProduct.AgeStagesId == 0 && classProduct.ClothesClassificationsId ==0)
            {
                classificationsOfProducts.Product = await _dbContext.products.Include(p => p.Marka).Include(p => p.ClothesClassification)
                    .AsSplitQuery().AsNoTracking().ToListAsync();
                return View(classificationsOfProducts);
            }
            if(classProduct.ClothesClassificationsId >0 && classProduct.AgeStagesId >0)
            {
                classificationsOfProducts.Product = await _dbContext.products
                .Where(c => c.ClothesClassificationId == classProduct.ClothesClassificationsId && c.AgeStageId == classProduct.AgeStagesId)
               .Include(p => p.Marka).Include(p => p.ClothesClassification)
                   .AsSplitQuery().AsNoTracking().ToListAsync();
                return View(classificationsOfProducts);
            }
            if (classProduct.ClothesClassificationsId > 0)
            {
                classificationsOfProducts.Product = await _dbContext.products
                .Where(c => c.ClothesClassificationId == classProduct.ClothesClassificationsId )
               .Include(p => p.Marka).Include(p => p.ClothesClassification)
                   .AsSplitQuery().AsNoTracking().ToListAsync();
                return View(classificationsOfProducts);
            }
          /*  if (classProduct.AgeStagesId > 0)
            {*/
                classificationsOfProducts.Product = await _dbContext.products
                .Where(c => c.AgeStageId == classProduct.AgeStagesId)
               .Include(p => p.Marka).Include(p => p.ClothesClassification)
                   .AsSplitQuery().AsNoTracking().ToListAsync();
                return View(classificationsOfProducts);
      
        }
      /*  [HttpGet]
        public async Task< IActionResult> ClassProducts(ClassProduct classProduct)
        {
          *//*  List<Product> products = new List<Product>();*/

      
            /*if (classProduct.AgeStagesId == 0 && classProduct.ClothesClassificationsId == 0 && classProduct.HumanClassificationsId == 0)*//*
                var products =await _dbContext.products
                      .AsNoTracking().ToListAsync();
            *//*
                         if (classProduct.AgeStagesId != 0)
                         {
                             products =  _dbContext.products.Where(p => p.AgeStageId == classProduct.AgeStagesId)
                                  .Include(p => p.Marka).Include(p => p.ClothesClassification).Include(c => c.Cards).
                               AsSplitQuery().AsNoTracking().ToList();
                         }*/
            /*        if (classProduct.ClothesClassificationsId != 0 )*/
            /*   products = await _dbContext.products.Where(p => p.ClothesClassificationId == classProduct.ClothesClassificationsId)
                    .Include(p => p.Marka).Include(p => p.ClothesClassification).Include(c => c.Cards).
                 AsSplitQuery().AsNoTracking().ToListAsync();*/

          /*  var product = new List<Product>();
            product = products;
             *//*
          
             return Json(products);
           
        }*/
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
