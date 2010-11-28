namespace CallCenter.SelfManagement.Web.Helpers
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class PropertiesMustMatchAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "'{0}' y '{1}' no coinciden.";
        
        private readonly object typeId = new object();

        public PropertiesMustMatchAttribute(string originalProperty, string confirmProperty)
            : base(DefaultErrorMessage)
        {
            this.OriginalProperty = originalProperty;
            this.ConfirmProperty = confirmProperty;
        }

        public string ConfirmProperty { get; private set; }

        public string OriginalProperty { get; private set; }

        public override object TypeId
        {
            get { return this.typeId; }
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                this.ErrorMessageString,
                this.OriginalProperty,
                this.ConfirmProperty);
        }

        public override bool IsValid(object value)
        {
            var properties = TypeDescriptor.GetProperties(value);
            var originalValue = properties.Find(this.OriginalProperty, true).GetValue(value);
            var confirmValue = properties.Find(this.ConfirmProperty, true).GetValue(value);

            return Object.Equals(originalValue, confirmValue);
        }
    }
}