using NotificationService.Application.Models.Base;

namespace NotificationService.Application.Models.User;

public sealed record CreateUserModel(string Username) : ICreateModel;