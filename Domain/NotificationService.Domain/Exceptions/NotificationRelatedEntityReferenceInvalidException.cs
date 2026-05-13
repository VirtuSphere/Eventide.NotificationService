namespace NotificationService.Domain.Exceptions;

public class NotificationRelatedEntityReferenceInvalidException()
    : InvalidOperationException("RelatedEntityType and RelatedEntityId must be either both specified or both null.");