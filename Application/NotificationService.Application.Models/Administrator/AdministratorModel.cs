using NotificationService.Application.Models.Base;

namespace NotificationService.Application.Models.Administrator;

public sealed record AdministratorModel(Guid Id, string Username) : IModel<Guid>;
