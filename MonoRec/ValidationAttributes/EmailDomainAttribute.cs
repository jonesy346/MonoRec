using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MonoRec.ValidationAttributes;

public class EmailDomainAttribute : ValidationAttribute, IClientModelValidator
{
    public EmailDomainAttribute()
    {
        ErrorMessage = "Email must be in format: username@domain.extension (e.g., user@example.com)";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success;
        }

        string email = value.ToString()!;

        // Check if the email contains @ and has text after it
        if (!email.Contains('@'))
        {
            return new ValidationResult(ErrorMessage);
        }

        // Split by @ and check if there's a domain part
        var parts = email.Split('@');
        if (parts.Length != 2)
        {
            return new ValidationResult(ErrorMessage);
        }

        // Check if there's something after the @
        if (string.IsNullOrWhiteSpace(parts[1]))
        {
            return new ValidationResult(ErrorMessage);
        }

        // Check if the domain has a dot (e.g., .com, .org)
        if (!parts[1].Contains('.'))
        {
            return new ValidationResult(ErrorMessage);
        }

        // Check if there's text after the last dot
        var lastDotIndex = parts[1].LastIndexOf('.');
        if (lastDotIndex == parts[1].Length - 1 || lastDotIndex == 0)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.Attributes.Add("data-val", "true");
        context.Attributes.Add("data-val-emaildomain", ErrorMessage);
    }
}
