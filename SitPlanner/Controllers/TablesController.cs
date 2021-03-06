﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SitPlanner.Data;
using SitPlanner.Models;
using SitPlanner.Models.Enums;

namespace SitPlanner.Controllers
{
    public class TablesController : Controller
    {
        private readonly SitPlannerContext _context;

        public TablesController(SitPlannerContext context)
        {
            _context = context;
        }

        // GET: Tables
        public async Task<IActionResult> Index(int? id)
        {
            if (MyGlobals.GlobalEventID == 0)
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status405MethodNotAllowed);
            }
            if (id == null)
            {
                var sitPlannerContext = _context.Table.Where(t => t.EventId == MyGlobals.GlobalEventID).Include(t => t.Event);
                ViewData["TotalCapacity"] = _context.Table.Where(c=> c.EventId == MyGlobals.GlobalEventID).Sum(c => c.CapacityOfPeople);
                ViewData["CurrentEvent"] = MyGlobals.GlobalEventName;
                ViewData["SwitchEvent"] = "Switch Event";
                return View(await sitPlannerContext.ToListAsync());
            }
            var seatPlannerContext = _context.Table.Where(t => t.EventId == MyGlobals.GlobalEventID).Include(t => t.Event).Where(i => i.Id == id);
            ViewData["TotalCapacity"] = _context.Table.Where(c => c.EventId == MyGlobals.GlobalEventID).Sum(c => c.CapacityOfPeople);
            ViewData["CurrentEvent"] = MyGlobals.GlobalEventName;
            ViewData["SwitchEvent"] = "Switch Event";
            return View(await seatPlannerContext.ToListAsync());


        }

        // GET: Tables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Table
                .Include(t => t.Event)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (table == null)
            {
                return NotFound();
            }

            return PartialView(table);
        }

        struct TableSizeOption
        {
            int size;
            string name;
        }
        // GET: Tables/Create
        public IActionResult Create()
        {
            //ViewData["EventId"] = new SelectList(_context.Event, "Id", "Name");

            var enumData = from Table.TableTypeEnum e in Enum.GetValues(typeof(Table.TableTypeEnum))
                           select new
                           {
                               TableTypeEnum = e,
                           };
            ViewBag.EnumList = new SelectList(enumData, "TableTypeEnum", "TableTypeEnum");

            var enumData1 = from Table.TableSizesEnum e1 in Enum.GetValues(typeof(Table.TableSizesEnum))
                           select new
                           {
                               TableSizesEnum = e1,
                           };
            ViewBag.EnumListSize = new SelectList(enumData1, "TableSizesEnum", "TableSizesEnum");

            return PartialView();
        }

        // POST: Tables/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CapacityOfPeople,MinCapacityOfPeople,TableType,EventId")] Table table)
        {
            table.EventId = MyGlobals.GlobalEventID;
            if (ModelState.IsValid)
            {
                _context.Add(table);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Event, "Id", "Name", table.EventId);
            return View(table);
        }

        // GET: Tables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Table.FindAsync(id);
            if (table == null)
            {
                return NotFound();
            }
            //ViewData["EventId"] = new SelectList(_context.Event, "Id", "Name", table.EventId);

            var enumData = from Table.TableTypeEnum e in Enum.GetValues(typeof(Table.TableTypeEnum))
                           select new
                           {
                               TableTypeEnum = e,
                           };
            ViewData["TableType"] = new SelectList(enumData, "TableTypeEnum", "TableTypeEnum");

            return PartialView(table);
        }

        // POST: Tables/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CapacityOfPeople,MinCapacityOfPeople,TableType,EventId")] Table table)
        {
            table.EventId = MyGlobals.GlobalEventID;
            if (id != table.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(table);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TableExists(table.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Event.OrderBy(x => x.Name), "Id", "Name", table.EventId);
            return View(table);
        }

        // GET: Tables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Table
                .Include(t => t.Event)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (table == null)
            {
                return NotFound();
            }

            return PartialView(table);
        }

        // POST: Tables/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.InviteeTable.FirstOrDefault(t => t.TableId == id) != null)
            {
                throw new Exception("");
            }
            var table = await _context.Table.FindAsync(id);
            _context.Table.Remove(table);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TableExists(int id)
        {
            return _context.Table.Any(e => e.Id == id);
        }
    }
}
