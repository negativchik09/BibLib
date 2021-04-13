﻿using System.ComponentModel.DataAnnotations;

namespace BibLib.Models.ViewModels
{
    public class RegistrationViewModel
    {
        public string Email { get; set; }
        public string ConfirmEmail { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}