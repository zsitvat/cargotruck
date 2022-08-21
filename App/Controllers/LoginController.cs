﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Data;
using App.Models;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;


namespace App.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        public ActionResult Login_page()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Autherize(App.Models.Users userModel)
        {
            using ApplicationDbContext db = _context;
            var userDetails = db.Users.Where(x => x.UserName == userModel.UserName && x.PasswordHash == md5.CreateMD5(userModel.PasswordHash)).FirstOrDefault();
            if (userDetails == null)
            {
                userModel.LoginErrorMessage = "Rossz felhasználónév vagy jelszó."; 
                return View("Login_page", userModel);
            }
            else
            {
                HttpContext.Session.SetInt32("Id", userDetails.Id);
                HttpContext.Session.SetString("UserName", userDetails.UserName);
                if (userDetails.Role!=null) {
                    HttpContext.Session.SetString("Role", userDetails.Role);
                }
                return RedirectToAction("Index", "Home");
            }
        }

    
        public ActionResult LogOut()
        {
            int userId = (int)HttpContext.Session.GetInt32("Id");
            HttpContext.Session.Clear();
            return RedirectToAction("Login_page", "Login");
        }
        [HttpGet]
        public ActionResult Results()
        {
            if (HttpContext.Session.GetString("Id") == null)
            {
                return RedirectToAction("Login_page", "Login");
            }
            else {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}  

