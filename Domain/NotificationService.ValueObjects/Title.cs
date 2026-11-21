
using NotificationService.ValueObjects.Validators;
using NotificationService.ValueObjects.Base;
namespace NotificationService.ValueObjects;

public class Title(string name) : ValueObject<string>(new TitleValidator(), name);