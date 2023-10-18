// ***********************************************************************
// Assembly         : 
// Author           : sudarshan
// Created          : 06-18-2023
//
// Last Modified By : sudarshan
// Last Modified On : 06-18-2023
// ***********************************************************************
// <copyright file="ApplicationUserController.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RunShift.Dashboard.Models;
using RunShift.DataAccess.CommonFunction;
using RunShift.DataAccess.Entity;
using RunShift.Model.ResponseModels;
using RunShift.Service.Services.Interface;
using System.Net;

namespace RunShift.Dashboard.Controllers
{
    /// <summary>
    /// Class ApplicationUserController.
    /// </summary>
    public class ApplicationUserController : Controller
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly RunShiftDbContext _context;
        /// <summary>
        /// The i user authentication service
        /// </summary>
        public IUserAuthentication _iUserAuthService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationUserController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="iUserAuthService">The i user authentication service.</param>
        public ApplicationUserController(RunShiftDbContext context, IUserAuthentication iUserAuthService)
        {
            _context = context;
            _iUserAuthService = iUserAuthService;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        public IActionResult Index()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                GetApplicationUser();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "SignIn");
            }

        }

        /// <summary>
        /// Gets the application user.
        /// </summary>
        private void GetApplicationUser()
        {
            var result = _context.ApplicationUsers
                .Include(o => o.Roles)
                .Where(o => o.IsDeleted == false)
                .Select(s => new
                {
                    s.Id,
                    //s.UserName,
                    s.RoleId,
                    s.Roles.RoleName,
                    s.Email,
                    s.FirstName,
                    s.LastName,
                    s.IsDeleted,
                    s.CreatedOn,
                    s.IsActive,
                    s.Mobile,
                    s.ModifiedOn
                }).OrderByDescending(o => o.Id).ToList();
            ViewBag.Details = result;
        }

        /// <summary>
        /// Users the profile by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="isProfile">The is profile.</param>
        /// <returns>IActionResult.</returns>
        public IActionResult UserProfileById(string id, string isProfile)
        {
            Guid useGuId = new(id);

            UserDetailsWrapper objUserDetailsWrapper = new();
            if (!string.IsNullOrEmpty(id))
            {
                GetRoleList();
                objUserDetailsWrapper.IsProfile = isProfile;
                objUserDetailsWrapper.AppUsers = (from oappUsers in _context.ApplicationUsers.Include(o => o.Roles)
                                                  where oappUsers.IsDeleted == false && oappUsers.IsActive == true && oappUsers.Id == useGuId
                                                  select new AppUserViewModel
                                                  {
                                                      Id = oappUsers.Id,
                                                      Email = oappUsers.Email,
                                                      RoleId = oappUsers.RoleId,
                                                      RoleName = oappUsers.Roles.RoleName,
                                                      IsActive = oappUsers.IsActive,
                                                      FirstName = oappUsers.FirstName,
                                                      LastName = oappUsers.LastName,
                                                      Mobile = oappUsers.Mobile,
                                                      IsDeleted = oappUsers.IsDeleted,
                                                      ModifiedOn = oappUsers.ModifiedOn,
                                                      // UserName = oappUsers.UserName,
                                                      CreatedOn = oappUsers.CreatedOn,
                                                  }).OrderByDescending(o => o.Id).FirstOrDefault();
                if (objUserDetailsWrapper.AppUsers != null)
                {
                    objUserDetailsWrapper.Companies = (from oCompanies in _context.Companies.Include(o => o.ApplicationUsers)
                                                       where oCompanies.IsDeleted == false && oCompanies.IsActive == true && oCompanies.ApplicationUserId == objUserDetailsWrapper.AppUsers.Id
                                                       select new CompanyViewModel
                                                       {
                                                           Id = oCompanies.Id,
                                                           AddressLine2 = string.IsNullOrEmpty(oCompanies.AddressLine2) ? "-" : oCompanies.AddressLine2,
                                                           ApplicationUserId = oCompanies.ApplicationUserId,
                                                           IsActive = oCompanies.IsActive,
                                                           DateCreated = oCompanies.DateCreated,
                                                           AddressPostalCode = oCompanies.AddressPostalCode,
                                                           IsDeleted = oCompanies.IsDeleted,
                                                           ModifiedOn = oCompanies.ModifiedOn,
                                                           AddressTownCity = oCompanies.AddressTownCity,
                                                           AddressLine1 = oCompanies.AddressLine1,
                                                           BusinessPhoneNumber = oCompanies.BusinessPhoneNumber,
                                                           ModifiedBy = oCompanies.ModifiedBy,
                                                           Name = oCompanies.Name,
                                                           Url = oCompanies.Url,
                                                           CreatedOn = oCompanies.CreatedOn,
                                                       }).OrderByDescending(o => o.Id).FirstOrDefault();
                }
            }
            return View(objUserDetailsWrapper);
        }

        /// <summary>
        /// Gets the role list.
        /// </summary>
        private void GetRoleList()
        {
            var roleList = _context.Roles.Where(o => o.IsDeleted == false).Select(s => new
            {
                Name = s.RoleName,
                s.Id
            }).OrderByDescending(o => o.Id).ToList();

            var applicationUsersList = _context.ApplicationUsers.Where(o => o.IsDeleted == false).Select(s => new
            {
                Name = s.FirstName + " - " + s.LastName,
                s.Id
            }).OrderByDescending(o => o.Id).ToList();
            ViewBag.RoleList = new SelectList(roleList, "Id", "Name");
            ViewBag.ApplicationUser = new SelectList(applicationUsersList, "Id", "Name");
        }

        /// <summary>
        /// Edits the application user.
        /// </summary>
        /// <param name="objUserDetails">The object user details.</param>
        /// <returns>IActionResult.</returns>
        public IActionResult EditApplicationUser(UserDetailsWrapper objUserDetails)
        {
            ApplicationUser? objDetails = new ApplicationUser();
            objDetails = _context.ApplicationUsers.Where(o => o.Id == objUserDetails.AppUsers.Id && o.IsDeleted == false).FirstOrDefault();
            if (objDetails != null)
            {
                objDetails.FirstName = objUserDetails.AppUsers.FirstName;
                objDetails.LastName = objUserDetails.AppUsers.LastName;
                objDetails.Email = objUserDetails.AppUsers.Email;
                objDetails.Mobile = objUserDetails.AppUsers.Mobile;
                objDetails.RoleId = objUserDetails.AppUsers.RoleId;
                objDetails.ModifiedBy = new Guid(HttpContext.Session.GetString("UserId").ToString());
                objDetails.ModifiedOn = DateTime.UtcNow;
                _context.SaveChanges();
                objUserDetails.Status = (int)HttpStatusCode.OK;
            }
            else
            {
                objUserDetails.Status = (int)HttpStatusCode.NotFound;
            }
            return Json(objUserDetails);
        }
        /// <summary>
        /// Edits the companies.
        /// </summary>
        /// <param name="objUserDetails">The object user details.</param>
        /// <returns>IActionResult.</returns>
        public IActionResult EditCompanies(UserDetailsWrapper objUserDetails)
        {
            Company? objDetails = _context.Companies.FirstOrDefault(o => o.Id == objUserDetails.Companies.Id && !o.IsDeleted);

            if (objDetails == null)
            {
                objDetails = new Company
                {
                    ApplicationUserId = new Guid(HttpContext.Session.GetString("UserId").ToString())
                };
                _context.Add(objDetails);
            }

            objDetails.Name = objUserDetails.Companies.Name;
            objDetails.BusinessPhoneNumber = objUserDetails.Companies.BusinessPhoneNumber;
            objDetails.AddressLine1 = objUserDetails.Companies.AddressLine1;
            objDetails.AddressLine2 = objUserDetails.Companies.AddressLine2;
            objDetails.Url = objUserDetails.Companies.Url;
            objDetails.AddressTownCity = objUserDetails.Companies.AddressTownCity;
            objDetails.AddressPostalCode = objUserDetails.Companies.AddressPostalCode;
            objDetails.ModifiedBy = new Guid(HttpContext.Session.GetString("UserId").ToString());
            objDetails.ModifiedOn = DateTime.UtcNow;

            _context.SaveChanges();
            objUserDetails.Status = (int)HttpStatusCode.OK;

            return Json(objUserDetails);

        }
    }
}