﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SitPlanner.Data;
using SitPlanner.Models;

namespace SitPlanner.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly SitPlannerContext _context;
        

        public CategoriesController(SitPlannerContext context)
        {
            _context = context;
            
        }

        public Category GetCategoryByName(string name)
        {
            var item = _context.Category.FirstOrDefault(i => i.Name == name);

            return item;
        }


        // GET: Categories
        public async Task<IActionResult> Index()
        {
            if (MyGlobals.GlobalEventID == 0)
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status405MethodNotAllowed);
            }
            var sitPlannerContext = _context.Category.Where(i => i.EventId == MyGlobals.GlobalEventID).Include(c => c.Event);
            ViewData["CurrentEvent"] = MyGlobals.GlobalEventName;
            ViewData["SwitchEvent"] = "Switch Event";
            return View(await sitPlannerContext.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .Include(c => c.Event)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return PartialView(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            //ViewData["EventId"] = new SelectList(_context.Event.OrderBy(x => x.Name), "Id", "Name");
            return PartialView("_Create");
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,EventId")] Category category)
        {
            category.EventId = MyGlobals.GlobalEventID;
            if (ModelState.IsValid)
            {
                if (!CategoryExists(category.Name))
                {
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                }
                //return RedirectToAction(nameof(Index))
                return RedirectToAction("Index" , "Invitees");
            }
            //ViewData["EventId"] = new SelectList(_context.Event.OrderBy(x => x.Name), "Id", "Name", category.EventId);
            return View(category);
        }

        [Route("Categories/CategoryExists")]
        [HttpGet]
        public async Task<IActionResult> CategoryExistsRequest(string name)
        {
            return (Content(CategoryExists(name) ? "true" : "false"));
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            //ViewData["EventId"] = new SelectList(_context.Event.OrderBy(x => x.Name), "Id", "Name", category.EventId);
            return PartialView(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,EventId")] Category category)
        {
            category.EventId = MyGlobals.GlobalEventID;
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Name))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "Invitees");

            }
            ViewData["EventId"] = new SelectList(_context.Event.OrderBy(x => x.Name), "Id", "Name", category.EventId);
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .Include(c => c.Event)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return PartialView(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task DeleteConfirmed(int id)
        {

            var invitee = _context.Invitee.FirstOrDefault(c => c.CategoryId == id);
            if (invitee != null)
            {
                throw new Exception("");
            }
            var category = await _context.Category.FindAsync(id);
            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
        
        }

        private bool CategoryExists(string name)
        {
            return _context.Category.Where(e => e.EventId == MyGlobals.GlobalEventID).Any(e => e.Name == name);
        }
    }
}
