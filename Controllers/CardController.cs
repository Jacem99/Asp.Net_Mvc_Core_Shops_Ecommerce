using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shops.Data;
using Shops.Models;
using Shops.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shops.Controllers
{
   [Authorize]
    public class CardController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public CardController(ApplicationDbContext applicationDbContext,
            UserManager<ApplicationUser> userManager)
        {
            _dbContext = applicationDbContext;
            _userManager = userManager;
        }
        public async Task< IActionResult> Index()
        {
          
            string IdUser = _userManager.GetUserId(User);

                var Cards = await _dbContext.Cards.Where(u => u.IdentityUserId == IdUser && u.Favourite == true)
            .Include(p => p.Products).Include(m => m.Products.Marka)
            .AsNoTracking().AsSplitQuery().ToListAsync();

             return View(Cards);
            
    
        }
        public async Task<ActionResult> Favourite()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var favourite = await _dbContext.Cards.Where(c => c.IdentityUserId == userEmail && c.Favourite==true )
                                  .AsNoTracking().ToListAsync();
            return View(favourite);
        }
        public async Task<ActionResult> Buy()
        {
            string idUser = _userManager.GetUserId(User);

            var buyedCards = await _dbContext.Cards.Where(u => u.IdentityUserId == idUser && u.Buyed == true)
           .Include(p => p.Products).Include(m => m.Products.Marka)
           .AsNoTracking().AsSplitQuery().ToListAsync();

            return View(buyedCards);
        }
        public async Task< ActionResult> addFavourite(string Id)
        {
            var productId = Convert.ToInt32(Id);
         
           var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

           var checkFavourite = await _dbContext.Cards.SingleOrDefaultAsync(f => f.IdentityUserId == userId && f.ProductId == productId );
            if (checkFavourite is null)
            {
                Card card = new Card
                {
                    Favourite = true,
                    Buyed = false,
                    IdentityUserId = userId,
                    ProductId = productId,
                    mount = 1
                };
                await _dbContext.Cards.AddAsync(card);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            else if (checkFavourite != null)
            {
                if (checkFavourite.Buyed == true && checkFavourite.Favourite == false)
                {
                    checkFavourite.Favourite = true;
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
             };
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult> addBuyed(addBuyed addBuyed)
        {
            string idUser = _userManager.GetUserId(User);

            var checkBuyed = await _dbContext.Cards.SingleOrDefaultAsync(f => f.IdentityUserId == idUser &&  f.ProductId == addBuyed.ProductId );
            if (checkBuyed is not null)
            {
                if (checkBuyed.Favourite==true && checkBuyed.Buyed==false)
                {
                    checkBuyed.Buyed = true;
                    checkBuyed.mount = addBuyed.ValueMount;
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
                else if(checkBuyed.Favourite == true && checkBuyed.Buyed == true)
                {
                    checkBuyed.mount = addBuyed.ValueMount;
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                 }
                }
                return Ok();
        }

        public async Task<IActionResult> Delete(string id)
        {
            var Id = Convert.ToInt32(id);
            var card = await _dbContext.Cards.FindAsync(Id);
            if (card is null)
            {
                return Ok();
            }
            _dbContext.Cards.Remove(card);
            _dbContext.SaveChanges();
            return Ok();
        }

        public async Task<IActionResult> DeleteFavourite(string id)
        {
            var Id = Convert.ToInt32(id);
            var card = await _dbContext.Cards.FindAsync(Id);
            if (card is null)
            {
                return Ok();
            }
            if(card.Favourite ==true && card.Buyed == false)
            {
                _dbContext.Cards.Remove(card);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            else if(card.Favourite == true && card.Buyed == true)
            {
                card.Favourite = false;
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            return Ok();
        }
        public async Task<IActionResult> DeleteBuyed(string id)
        {
            var Id = Convert.ToInt32(id);
            var card = await _dbContext.Cards.FindAsync(Id);
            if (card is null)
            {
                return Ok();
            }
            card.Buyed = false;
            _dbContext.SaveChanges();
            return Ok();
        }

    }
}

