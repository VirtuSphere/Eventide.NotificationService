using NotificationService.ValueObjects.Base;
using NotificationService.ValueObjects.Validators;

namespace NotificationService.ValueObjects;

public class RelatedEntityTypeName(string value) : ValueObject<string>(new RelatedEntityTypeNameValidator(), value);