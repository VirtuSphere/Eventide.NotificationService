using AutoMapper;
using NotificationService.Application.Models.Administrator;
using NotificationService.Application.Models.Notification;
using NotificationService.Application.Models.RelatedEntity;
using NotificationService.Application.Models.User;
using NotificationService.Domain;

namespace NotificationService.Application.Services.Mapping;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<Administrator, AdministratorModel>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username.Value));

        CreateMap<User, UserModel>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username.Value));

        CreateMap<RelatedEntity, RelatedEntityModel>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName == null ? null : src.DisplayName.Value))
            .ForMember(dest => dest.ExternalUrl, opt => opt.MapFrom(src => src.ExternalUrl == null ? null : src.ExternalUrl.Value));

        CreateMap<Notification, NotificationModel>()
            .ForMember(dest => dest.RecipientUserId, opt => opt.MapFrom(src => src.RecipientUser.Id))
            .ForMember(dest => dest.AdministratorId, opt => opt.MapFrom(src => src.Administrator == null ? (Guid?)null : src.Administrator.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title.Value))
            .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Body.Value))
            .ForMember(dest => dest.RelatedEntityType, opt => opt.MapFrom(src => src.RelatedEntityType == null ? null : src.RelatedEntityType.Value))
            .ForMember(dest => dest.RelatedEntityId, opt => opt.MapFrom(src => src.RelatedEntity == null ? (Guid?)null : src.RelatedEntity.Id));
    }
}