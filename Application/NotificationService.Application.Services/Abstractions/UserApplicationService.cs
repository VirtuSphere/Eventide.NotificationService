using NotificationService.Application.Models.User;
using NotificationService.Application.Services.Abstractions.Base;

namespace NotificationService.Application.Services.Abstractions;

public interface IUserApplicationService : IApplicationService<UserModel, CreateUserModel, Guid>;
