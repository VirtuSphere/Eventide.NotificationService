using NotificationService.ValueObjects.Base;
using NotificationService.ValueObjects.Validators;

namespace NotificationService.ValueObjects;

public class Body(string value) : ValueObject<string>(new BodyValidator(), value);