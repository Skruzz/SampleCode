// ***********************************************************************
// Assembly         : 
// Author           : sudarshan
// Created          : 06-18-2023
//
// Last Modified By : sudarshan
// Last Modified On : 06-18-2023
// ***********************************************************************
// <copyright file="ApplicationUsers.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RunShift.DataAccess.Entity
{
    /// <summary>
    /// Class ApplicationUser.
    /// </summary>
    public class ApplicationUser
    {
        
        [Key]
        public Guid Id { get; set; }

        
        [Required(ErrorMessage = "*")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
       
        public string LastName { get; set; } = string.Empty;


        //[Required(ErrorMessage = "*")]
        //[StringLength(50)]
        //public string UserName { get; set; } = string.Empty;

       
        [Required(ErrorMessage = "*")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(100)]
        public string? Email { get; set; }

        
        [Required(ErrorMessage = "*")]
        [StringLength(13)]
        public string? Mobile { get; set; }

        
        [Required(ErrorMessage = "*")]
        public string Password { get; set; } = string.Empty;

       
        [Required(ErrorMessage = "*")]
        [NotMapped] // Does not effect with your database
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

       
        [Required(ErrorMessage = "*")]
        public Guid RoleId { get; set; }
        
        public virtual Role? Roles { get; set; }
       
        public bool IsActive { get; set; }
       
        public DateTime CreatedOn { get; set; }
       
        public Guid ModifiedBy { get; set; }
      
        public DateTime ModifiedOn { get; set; }
       
        public bool IsDeleted { get; set; }
        
        [NotMapped]
        public bool IsChecktermsConditions { get; set; }
    }
}