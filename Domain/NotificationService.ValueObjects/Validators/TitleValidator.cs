using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationService.ValueObjects.Base;
using NotificationService.ValueObjects.Exceptions;
namespace NotificationService.ValueObjects.Validators;

public class TitleValidator : IValidator<string>
{
    public static int MAX_LENGTH => 30;

    public static int MIN_LENGTH => 3;
    public void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullOrWhiteSpaceException(nameof(value));

        if (value.Length > MAX_LENGTH)
            throw new ArgumentLongValueException(nameof(value), value, MAX_LENGTH);

        if (value.Length < MIN_LENGTH)
            throw new ArgumentShortValueException(nameof(value), value, MIN_LENGTH);
    }
}
