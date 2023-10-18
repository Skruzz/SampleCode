// ***********************************************************************
// Assembly         : 
// Author           : sudarshan
// Created          : 06-18-2023
//
// Last Modified By : sudarshan
// Last Modified On : 06-18-2023
// ***********************************************************************
// <copyright file="IUserAuthentication.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using RunShift.DataAccess.Entity;
using RunShift.Model.RequestModels;
using RunShift.Model.ResponseModels;

namespace RunShift.Service.Services.Interface
{
    /// <summary>
    /// Interface IUserAuthentication
    /// </summary>
    public interface IUserAuthentication
    {
        /// <summary>
        /// Users the sign in.
        /// </summary>
        /// <param name="objSignInRequest">The object sign in request.</param>
        /// <returns>Task&lt;ResponseWrapper&gt;.</returns>
        public Task<ResponseWrapper> UserSignIn(SignInRequest objSignInRequest);

        /// <summary>
        /// Users the sign up.
        /// </summary>
        /// <param name="objSignUpRequest">The object sign up request.</param>
        /// <returns>Task&lt;ResponseWrapper&gt;.</returns>
        public Task<ResponseWrapper> UserSignUp(ApplicationUser objSignUpRequest);
    }
}