using NotificationService.ValueObjects.Base;
using NotificationService.ValueObjects.Validators;

namespace NotificationService.ValueObjects;

public class ExternalUrl(string value) : ValueObject<string>(new ExternalUrlValidator(), value);
