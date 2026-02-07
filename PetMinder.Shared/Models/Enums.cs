namespace PetMinder.Models
{
    [Flags]
    public enum UserRole
    {
        None = 0,
        BasicUser = 1 << 0, // 1 - Default for all registered users
        Owner = 1 << 1, // 2 - Can create/manage pets, request services
        Sitter = 1 << 2, // 4 - Can create/manage sitter profile, offer services
        Admin = 1 << 3 // 8 - For administrative purposes
    }

    public enum BookingStatus
    {
        Pending,
        Accepted,
        Rejected,
        Cancelled,
        Completed
    }

    public enum ChangeRequestedBy
    {
        Owner,
        Sitter
    }

    public enum CancelledBy
    {
        Owner,
        Sitter
    }

    public enum TransactionType
    {
        Booking,
        Donation,
        Adjustment,
        VerificationReward,
        Expiration,
        ReferralBonus,
        AchievementBonus
    }

    public enum NotificationType
    {
        BookingRequest,
        BookingAccepted,
        BookingRejected,
        BookingCancelled,
        BookingCompleted,
        ChangeRequested,
        ChangeAccepted,
        ChangeRejected,
        SystemAlert
    }

    public enum VerificationStatusName
    {
        Unverified,
        basic,
        Verified,
        HomeVerified
    }

    public enum VerificationStep
    {
        EmailVerification,
        PhoneVerification,
        ProfilePhotoUpload,
        IdentityVerification,
        HomeVerification
    }

    public enum CancellationPolicyName
    {
        Standard,
        Late,
        In_Progress
    }
    public enum PointPolicyServiceType
    {
        Pet,
        House
    }

    public enum AddressType
    {
        Primary,
        Secondary,
        Other
    }

    public enum PetType
    {
        Dog,
        Cat,
        Bird,
        SmallAnimal,
        Reptile,
        Other
    }

    public enum PetBehaviorComplexity
    {
        Low,      // Easy going, low maintenance
        Moderate, // Standard care needed
        High,     // Special needs, energetic, or difficult behavior
        Extreme   // Professional handling required
    }
}
