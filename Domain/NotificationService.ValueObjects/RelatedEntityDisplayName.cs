using NotificationService.ValueObjects.Base;
using NotificationService.ValueObjects.Validators;

namespace NotificationService.ValueObjects;

public class RelatedEntityDisplayName(string value) : ValueObject<string>(new RelatedEntityDisplayNameValidator(), value);
