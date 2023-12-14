using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ApprenticeAccounts.Web.Attributes
{
    public class RequiredWithDynamicMessageAttribute : RequiredAttribute
    {
        private readonly string _staticMessage;
        private readonly string _propertyToAppend;
        private readonly bool _lowerCasePropertyValue;

        public RequiredWithDynamicMessageAttribute(string staticMessage, string propertyToAppend, bool lowerPropertyValueCase = true)
        {
            _staticMessage = staticMessage;
            _propertyToAppend = propertyToAppend;
            _lowerCasePropertyValue = lowerPropertyValueCase;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (base.IsValid(value, validationContext) != ValidationResult.Success)
            {
                var instance = validationContext.ObjectInstance;
                var type = instance.GetType();
                var property = type.GetProperty(_propertyToAppend);

                if (property == null)
                {
                    throw new ArgumentException($"Property with name {_propertyToAppend} not found");
                }

                var propertyValue = property.GetValue(instance, null).ToString();
                var errorMessage = $"{_staticMessage} {(_lowerCasePropertyValue ? propertyValue.ToLower() : propertyValue)}";

                return new ValidationResult(errorMessage);
            }

            return ValidationResult.Success;
        }
    }

}
