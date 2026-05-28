using System;
using NotificationService.ValueObjects.Base;
using NotificationService.ValueObjects.Exceptions;

namespace NotificationService.ValueObjects.Validators;

public class ExternalUrlValidator : IValidator<string>
{
    public static int MAX_LENGTH => 2000;

    public void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullOrWhiteSpaceException(nameof(value));

        if (value.Length > MAX_LENGTH)
            throw new ArgumentLongValueException(nameof(value), value, MAX_LENGTH);

        if (!Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute))
            throw new ArgumentException($"Value is not a valid URI: {value}", nameof(value));
    }
}
