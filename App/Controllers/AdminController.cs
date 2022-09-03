﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Data;
using App.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.InkML;
using Microsoft.Data.SqlClient;
using System.Diagnostics.Metrics;
using Users = App.Models.Users;
using DocumentFormat.OpenXml.Office.Word;

namespace App.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Registration
        public async Task<IActionResult> Admin_page(string searchString,string sortOrder)
        {
            var users = from u in _context.Users select u;

            ViewBag.UserNameSortParm = sortOrder == "UserName" ? "UserName_desc" : "UserName";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "Email_desc" : "Email";
            ViewBag.PasswordSortParm = sortOrder == "Password" ? "Password_desc" : "Password";
            ViewBag.RoleSortParm = sortOrder == "Role" ? "Role_desc" : "Role";
            switch (sortOrder)
            {
                case "UserName_desc":
                    users = users.OrderByDescending(s => s.UserName);
                    break;
                case "UserName":
                    users = users.OrderBy(s => s.UserName);
                    break;
                case "Email_desc":
                    users = users.OrderByDescending(s => s.Email);
                    break;
                case "Email":
                    users = users.OrderBy(s => s.Email);
                    break;
                case "Password_desc":
                    users = users.OrderByDescending(s => s.PasswordHash);
                    break;
                case "Password":
                    users = users.OrderBy(s => s.PasswordHash);
                    break;
                case "Role_desc":
                    users = users.OrderByDescending(s => s.Role);
                    break;
                case "Role":
                    users = users.OrderBy(s => s.Role);
                    break;
                default:
                    break;
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                ViewBag.search = searchString;
                users = users.Where(s => s.UserName!.Contains(searchString) || s.Email!.Contains(searchString) || s.Role!.Contains(searchString));
            }
            else { 
                ViewBag.search = "Keresés...";
            }
            return View(await users.ToListAsync());
        }

        // GET: Registration/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Registration/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,Email,PasswordHash,LoginErrorMessage,Role")] Users users)
        {
            if (UserNameExists(users.UserName))
            {
                users.LoginErrorMessage = "Ez a felhasználó már létezik.";
                return View(users);
            }
            else
            {
                users.PasswordHash = md5.CreateMD5(users.PasswordHash);
                _context.Add(users);

                await _context.SaveChangesAsync();
                return RedirectToAction("Admin_page");
            }    
            
        }

        // GET: Registration/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            return View(users);
        }

        // POST: Registration/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,Email,PasswordHash,LoginErrorMessage,Role")] Users users)
        {
            if (id != users.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    users.PasswordHash = md5.CreateMD5(users.PasswordHash);
                    _context.Update(users);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(users.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Admin_page));
            }
            return View(users);
        }

        // GET: Registration/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: Registration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var users = await _context.Users.FindAsync(id);
            _context.Users.Remove(users);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Admin_page));
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        private bool UserNameExists(string UserName)
        {
            return _context.Users.Any(e => e.UserName == UserName);
        }
    }
}
