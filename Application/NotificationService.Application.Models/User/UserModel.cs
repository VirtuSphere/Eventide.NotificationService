using NotificationService.Application.Models.Base;

namespace NotificationService.Application.Models.User;

public sealed record UserModel(Guid Id, string Username) : IModel<Guid>;