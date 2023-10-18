// ***********************************************************************
// Assembly         : 
// Author           : sudarshan
// Created          : 06-18-2023
//
// Last Modified By : sudarshan
// Last Modified On : 06-18-2023
// ***********************************************************************
// <copyright file="UserAuthentication.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RunShift.DataAccess.CommonFunction;
using RunShift.DataAccess.Entity;
using RunShift.Model.RequestModels;
using RunShift.Model.ResponseModels;
using RunShift.Service.Services.Interface;
using System.Net;

namespace RunShift.Service.Services
{
    /// <summary>
    /// Class UserAuthentication.
    /// </summary>
    public class UserAuthentication : IUserAuthentication
    {
       
        private readonly RunShiftDbContext _context;
       
        private readonly IConfiguration _configuration;
       
        private ResponseWrapper? objResponseWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAuthentication"/> class.
        /// </summary>
        /// <param name="configuration">The IConfiguration configuration.</param>
        /// <param name="context">The RunShiftDbContext context.</param>
        public UserAuthentication(IConfiguration configuration, RunShiftDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Users the sign in.
        /// </summary>
        /// <param name="objSignInRequst">The object sign in requst.</param>
        /// <returns>ResponseWrapper.</returns>
        public async Task<ResponseWrapper> UserSignIn(SignInRequest objSignInRequst)
        {
            try
            {
                string encodingString = GlobalFunction.MD5Hash(objSignInRequst.Password).ToUpper();
                var appUsersDetails = await _context.ApplicationUsers.Where(X => (X.Email == objSignInRequst.EmailId)
                                    && X.Password == encodingString
                                    && X.IsActive == true
                                    && X.IsDeleted == false).FirstOrDefaultAsync();
                if (appUsersDetails != null)
                {
                    objResponseWrapper = new ResponseWrapper
                    {
                        Data = appUsersDetails,
                        Message = "Successfully",
                        StatusCode = (int)HttpStatusCode.OK,
                        IsSuccess = true
                    };
                }
                else
                {
                    objResponseWrapper = new ResponseWrapper
                    {
                        Data = appUsersDetails,
                        Message = "Unauthorized",
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        IsSuccess = false
                    };
                }
            }
            catch (Exception ex)
            {
                string errorMessge = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                objResponseWrapper = new ResponseWrapper
                {
                    Data = 0,
                    Message = "Exception : " + errorMessge,
                    StatusCode = (int)HttpStatusCode.BadRequest, //////400
                    IsSuccess = false
                };
            }

            return objResponseWrapper;
        }

        /// <summary>
        /// Users the sign up.
        /// </summary>
        /// <param name="objSignUpRequest">The object sign up request.</param>
        /// <returns>ResponseWrapper.</returns>
        public async Task<ResponseWrapper> UserSignUp(ApplicationUser objSignUpRequest)
        {
            try
            {
                var roleList = _context.Roles.Where(p => p.IsDeleted == false).ToList();
                var isExist = await _context.ApplicationUsers.AnyAsync(o => o.Email == objSignUpRequest.Email && o.IsDeleted == false);
                if (!isExist)
                {
                    ApplicationUser objApplicationUsers = new()
                    {
                        FirstName = objSignUpRequest.FirstName,
                        LastName = objSignUpRequest.LastName,
                        Email = objSignUpRequest.Email,
                        Mobile = objSignUpRequest.Mobile,
                        Password = GlobalFunction.MD5Hash(objSignUpRequest.Password).ToUpper(),
                        RoleId = roleList.Where(o => o.RoleName == "User").Select(o => o.Id).FirstOrDefault(),
                        CreatedOn = DateTime.UtcNow,
                        ModifiedOn = DateTime.UtcNow,
                        ModifiedBy = roleList.Where(o => o.RoleName == "User").Select(o => o.Id).FirstOrDefault(),
                        IsDeleted = false,
                        IsActive = true,
                    };
                    _context.ApplicationUsers.Add(objApplicationUsers);
                    _context.SaveChanges();
                    objResponseWrapper = new ResponseWrapper
                    {
                        Data = objApplicationUsers.Id,
                        Message = "Added",
                        StatusCode = (int)HttpStatusCode.OK, /////200
                        IsSuccess = true
                    };
                }
                else
                {
                    objResponseWrapper = new ResponseWrapper
                    {
                        Data = null,
                        Message = "Already",
                        StatusCode = (int)HttpStatusCode.Found, /////200
                        IsSuccess = false
                    };
                }
            }
            catch (Exception ex)
            {
                string errorMessge = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                objResponseWrapper = new ResponseWrapper
                {
                    Data = 0,
                    Message = "Exception : " + errorMessge,
                    StatusCode = (int)HttpStatusCode.BadRequest, //////400
                    IsSuccess = false
                };
            }

            return objResponseWrapper;
        }
    }
}