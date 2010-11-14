﻿namespace CallCenter.SelfManagement.Web.Helpers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Web.Security;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "'{0}' debe tener al menos {1} caracteres de longitud.";
        private readonly int minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public ValidatePasswordLengthAttribute()
            : base(DefaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(
                CultureInfo.CurrentUICulture,
                this.ErrorMessageString,
                name,
                minCharacters);
        }

        public override bool IsValid(object value)
        {
            var valueAsString = value as string;

            return (valueAsString != null && valueAsString.Length >= minCharacters);
        }
    }
}