using NotificationService.Application.Models.Base;

namespace NotificationService.Application.Models.Administrator;

public sealed record CreateAdministratorModel(string Username) : ICreateModel;
