using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PetMinder.Models
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Phone), IsUnique = true)]
    public class User
    {
        [Key]
        public long UserId { get; set; }

        [Required, EmailAddress, StringLength(255)]
        public string Email { get; set; }

        [Required, StringLength(255)]
        public string PasswordHash { get; set; }

        [Required, StringLength(100)]
        public string FirstName { get; set; }

        [Required, StringLength(100)]
        public string LastName { get; set; }

        [Required, Phone, StringLength(9)]
        public string Phone { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [StringLength(512)]
        public string? ProfilePhotoUrl { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsFlagged { get; set; } = false;
        
        public bool IsBanned { get; set; } = false;

        public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();

        public virtual ICollection<BookingRequest> OwnedBookings { get; set; } = new List<BookingRequest>();

        public virtual ICollection<BookingRequest> SittingBookings { get; set; } = new List<BookingRequest>();

        public virtual ICollection<PointsTransaction> TransactionsSent { get; set; } = new List<PointsTransaction>();

        public virtual ICollection<PointsTransaction> TransactionsReceived { get; set; } = new List<PointsTransaction>();

        public virtual SitterSettings SitterSettings { get; set; }

        public virtual ICollection<SitterQualification> SitterQualifications { get; set; } = new List<SitterQualification>();

        public virtual ICollection<SitterRestriction> SitterRestrictions { get; set; } = new List<SitterRestriction>();

        public virtual ICollection<SitterAvailability> Availabilities { get; set; } = new List<SitterAvailability>();

        public virtual ICollection<UserBadge> Badges { get; set; } = new List<UserBadge>();

        public virtual ICollection<UserVerification> Verifications { get; set; } = new List<UserVerification>();

        public virtual ICollection<DeviceFingerprint> DeviceFingerprints { get; set; } = new List<DeviceFingerprint>();

        public virtual ICollection<Message> MessagesSent { get; set; } = new List<Message>();

        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public virtual ICollection<Review> ReviewsWritten { get; set; } = new List<Review>();

        public virtual ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();

        public virtual ICollection<HouseListing> HouseListings { get; set; } = new List<HouseListing>();

        public virtual ICollection<HouseIssueReport> HouseIssueReports { get; set; } = new List<HouseIssueReport>();

        public virtual ICollection<ConversationParticipant> ConversationParticipations { get; set; } = new List<ConversationParticipant>();

        public virtual ICollection<ReviewReport> ReviewReportsMade { get; set; } = new List<ReviewReport>();

        public virtual ICollection<UserVerificationStep> VerificationSteps { get; set; } = new List<UserVerificationStep>();

        public virtual ICollection<EmailVerificationToken> EmailVerificationTokens { get; set; } = new List<EmailVerificationToken>();

        public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
        
        public virtual ICollection<UserReport> ReportsReceived { get; set; } = new List<UserReport>();
        
        public virtual ICollection<UserReport> ReportsMade { get; set; } = new List<UserReport>();

        [StringLength(100)]
        public string? ReferralCode { get; set; }

        public virtual ICollection<Referral> ReferralsMade { get; set; } = new List<Referral>();

        public virtual Referral? ReferralReceived { get; set; }

        public virtual ICollection<NotificationPreference> NotificationPreferences { get; set; } = new List<NotificationPreference>();
    }
}