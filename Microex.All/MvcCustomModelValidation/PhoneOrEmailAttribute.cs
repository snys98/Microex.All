using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Microex.All.MvcCustomModelValidation
{
    public class PhoneOrEmailAttribute : ValidationAttribute
    {
        private static Regex _emailRegex = new Regex("^[A-Za-z0-9\\u4e00-\\u9fa5]+@[a-zA-Z0-9_-]+(\\.[a-zA-Z0-9_-]+)+$");
        private static Regex _phoneRegex = new Regex("^((13[0-9])|(14[5,7,9])|(15[^4])|(18[0-9])|(17[0,1,3,5,6,7,8]))\\d{8}$");
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var realValue = value.ToString();
            var isValid = realValue.Contains("@") ? _emailRegex.IsMatch(realValue) : _phoneRegex.IsMatch(realValue);
            if (isValid)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("邮箱或手机号格式有误");
        }
    }
}
