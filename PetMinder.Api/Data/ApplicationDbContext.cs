using Microsoft.EntityFrameworkCore;
using PetMinder.Models;

namespace PetMinder.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserReport> UserReports { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<BookingRequest> BookingRequests { get; set; }
        public DbSet<BookingChange> BookingChanges { get; set; }
        public DbSet<BookingCancellation> BookingCancellations { get; set; }
        public DbSet<CancellationPolicy> CancellationPolicies { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<HouseListing> HouseListings { get; set; }
        public DbSet<HousePhoto> HousePhotos { get; set; }
        public DbSet<HouseIssueReport> HouseIssueReports { get; set; }
        public DbSet<Shelter> Shelters { get; set; }
        public DbSet<PointPolicy> PointPolicies { get; set; }
        public DbSet<PointsTransaction> PointsTransactions { get; set; }
        public DbSet<PointDonation> PointDonations { get; set; }
        public DbSet<PointsLot> PointsLots { get; set; }

        public DbSet<VerificationStatus> VerificationStatuses { get; set; }
        public DbSet<UserVerification> UserVerifications { get; set; }
        public DbSet<UserVerificationStep> UserVerificationSteps { get; set; }

        public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }

        public DbSet<DeviceFingerprint> DeviceFingerprints { get; set; }
        public DbSet<QualificationType> QualificationTypes { get; set; }
        public DbSet<SitterQualification> SitterQualifications { get; set; }
        public DbSet<RestrictionType> RestrictionTypes { get; set; }
        public DbSet<SitterRestriction> SitterRestrictions { get; set; }
        public DbSet<SitterAvailability> SitterAvailabilities { get; set; }
        public DbSet<SitterSettings> SitterSettings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewReport> ReviewReports { get; set; }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<Referral> Referrals { get; set; }
        public DbSet<NotificationPreference> NotificationPreferences { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //ENUM types
            modelBuilder.HasPostgresEnum<UserRole>();
            modelBuilder.HasPostgresEnum<BookingStatus>();
            modelBuilder.HasPostgresEnum<ChangeRequestedBy>();
            modelBuilder.HasPostgresEnum<CancelledBy>();
            modelBuilder.HasPostgresEnum<TransactionType>();
            modelBuilder.HasPostgresEnum<NotificationType>();
            modelBuilder.HasPostgresEnum<VerificationStatusName>();
            modelBuilder.HasPostgresEnum<CancellationPolicyName>();
            modelBuilder.HasPostgresEnum<PointPolicyServiceType>();
            modelBuilder.HasPostgresEnum<VerificationStep>();
            modelBuilder.HasPostgresEnum<AddressType>();

            //CheckList
            modelBuilder.Entity<HouseListing>()
                .OwnsOne(h => h.CheckList);

            //Composite Unique Indexes

            // A sitter cannot have the same qualification twice
            modelBuilder.Entity<SitterQualification>()
                .HasIndex(sq => new { sq.SitterId, sq.QualificationTypeId })
                .IsUnique();

            // A sitter cannot have the same restriction twice
            modelBuilder.Entity<SitterRestriction>()
                .HasIndex(sr => new { sr.SitterId, sr.RestrictionTypeId })
                .IsUnique();

            // A user cannot join the same conversation twice
            modelBuilder.Entity<ConversationParticipant>()
                .HasIndex(cp => new { cp.ConversationId, cp.UserId })
                .IsUnique();

            // A user cannot earn the same badge twice
            modelBuilder.Entity<UserBadge>()
                .HasIndex(ub => new { ub.UserId, ub.BadgeId })
                .IsUnique();

            // enum conversion
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<int>();

            // block duplicate step entries for the same user
            modelBuilder.Entity<UserVerificationStep>()
                .HasIndex(uvs => new { uvs.UserId, uvs.Step })
                .IsUnique();

            // a user can't have the same address linked twice
            modelBuilder.Entity<UserAddress>()
                .HasIndex(ua => new { ua.UserId, ua.AddressId })
                .IsUnique();

            // Additional Indexes for Performance and Integrity

            // User Table
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Phone)
                .IsUnique();

            // SitterAvailability Table
            modelBuilder.Entity<SitterAvailability>()
                .HasIndex(sa => new { sa.SitterId, sa.StartTime, sa.EndTime });

            // BookingRequest Table
            modelBuilder.Entity<BookingRequest>()
                .HasIndex(br => new { br.OwnerId, br.CreatedAt })
                .IsDescending(false, true);

            modelBuilder.Entity<BookingRequest>()
                .HasIndex(br => new { br.SitterId, br.CreatedAt })
                .IsDescending(false, true);

            modelBuilder.Entity<BookingRequest>()
                .HasIndex(br => new { br.SitterId, br.Status, br.StartTime, br.EndTime });

            // PointsLot Table
            modelBuilder.Entity<PointsLot>()
                .HasIndex(pl => new { pl.UserId, pl.IsExpired, pl.PointsRemaining, pl.ExpiresAt });

            // Review Table
            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.RevieweeId, r.CreatedAt })
                .IsDescending(false, true);

            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.ReviewerId, r.BookingId })
                .IsUnique();

            // ReviewReport Table
            modelBuilder.Entity<ReviewReport>()
                .HasIndex(rr => new { rr.ReviewId, rr.ReporterId })
                .IsUnique();

            // PointsTransaction Table
            modelBuilder.Entity<PointsTransaction>()
                .HasIndex(pt => new { pt.SenderId, pt.OccurredAt })
                .IsDescending(false, true);

            modelBuilder.Entity<PointsTransaction>()
                .HasIndex(pt => new { pt.ReceiverId, pt.OccurredAt })
                .IsDescending(false, true);

            // Message Table
            modelBuilder.Entity<Message>()
                .HasIndex(m => new { m.ConversationId, m.SentAt })
                .IsDescending(false, true);

            // Notification Table
            modelBuilder.Entity<Notification>()
                .HasIndex(n => new { n.UserId, n.CreatedAt })
                .IsDescending(false, true);

            // enum conversion
            modelBuilder.Entity<UserAddress>()
                .Property(ua => ua.Type)
                .HasConversion<int>();

            //Relationships & Cascades

            // User → Pet (1:N)
            modelBuilder.Entity<Pet>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.Pets)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User → BookingRequest (Owner) (1:N)
            modelBuilder.Entity<BookingRequest>()
                .HasOne(br => br.Owner)
                .WithMany(u => u.OwnedBookings)
                .HasForeignKey(br => br.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → BookingRequest (Sitter) (1:N)
            modelBuilder.Entity<BookingRequest>()
                .HasOne(br => br.Sitter)
                .WithMany(u => u.SittingBookings)
                .HasForeignKey(br => br.SitterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Pet → BookingRequest (1:N)
            modelBuilder.Entity<BookingRequest>()
                .HasOne(br => br.Pet)
                .WithMany(p => p.BookingRequests)
                .HasForeignKey(br => br.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            // BookingRequest → BookingChange (1:N)
            modelBuilder.Entity<BookingChange>()
                .HasOne(bc => bc.BookingRequest)
                .WithMany(br => br.BookingChanges)
                .HasForeignKey(bc => bc.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // BookingRequest → BookingCancellation (1:0..1)
            modelBuilder.Entity<BookingCancellation>()
                .HasOne(bc => bc.BookingRequest)
                .WithOne(br => br.Cancellation)
                .HasForeignKey<BookingCancellation>(bc => bc.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // BookingCancellation → CancellationPolicy (N:1)
            modelBuilder.Entity<BookingCancellation>()
                .HasOne(bc => bc.CancellationPolicy)
                .WithMany(cp => cp.Cancellations)
                .HasForeignKey(bc => bc.PolicyId)
                .OnDelete(DeleteBehavior.Restrict);

            // BookingRequest → Review (1:N)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Booking)
                .WithMany(br => br.Reviews)
                .HasForeignKey(r => r.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Pet → Review (1:N)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Pet)
                .WithMany(p => p.PetReviews)
                .HasForeignKey("PetId")
                .OnDelete(DeleteBehavior.Restrict);

            // User → Review (Reviewer) (1:N)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany(u => u.ReviewsWritten)
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → Review (Reviewee) (1:N)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewee)
                .WithMany(u => u.ReviewsReceived)
                .HasForeignKey(r => r.RevieweeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review → ReviewReport (1:N)
            modelBuilder.Entity<ReviewReport>()
                .HasOne(rr => rr.Review)
                .WithMany(r => r.ReviewReports)
                .HasForeignKey(rr => rr.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            // ReviewReport → User (Reporter) (N:1)
            modelBuilder.Entity<ReviewReport>()
                .HasOne(rr => rr.Reporter)
                .WithMany(u => u.ReviewReportsMade)
                .HasForeignKey(rr => rr.ReporterId)
                .OnDelete(DeleteBehavior.Cascade);

            // User → Notification (1:N)
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User → NotificationPreference (1:N)
            modelBuilder.Entity<NotificationPreference>()
                .HasOne(np => np.User)
                .WithMany(u => u.NotificationPreferences)
                .HasForeignKey(np => np.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // NotificationPreference uniqueness (User + Type)
            modelBuilder.Entity<NotificationPreference>()
                .HasIndex(np => new { np.UserId, np.NotificationType })
                .IsUnique();

            // BookingRequest → Notification (1:N, optional)
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.BookingRequest)
                .WithMany(br => br.Notifications)
                .HasForeignKey(n => n.BookingId)
                .OnDelete(DeleteBehavior.SetNull);

            // Message → Notification (1:N, optional)
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Message)
                .WithMany(m => m.Notifications)
                .HasForeignKey(n => n.MessageId)
                .OnDelete(DeleteBehavior.SetNull);

            // BookingRequest → Conversation (1:0..1)
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.BookingRequest)
                .WithOne(br => br.Conversation)
                .HasForeignKey<Conversation>(c => c.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Conversation → ConversationParticipant (1:N)
            modelBuilder.Entity<ConversationParticipant>()
                .HasOne(cp => cp.Conversation)
                .WithMany(c => c.Participants)
                .HasForeignKey(cp => cp.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            // User → ConversationParticipant (1:N)
            modelBuilder.Entity<ConversationParticipant>()
                .HasOne(cp => cp.User)
                .WithMany(u => u.ConversationParticipations)
                .HasForeignKey(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Conversation → Message (1:N)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            // User → Message (Sender) (1:N)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.MessagesSent)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → DeviceFingerprint (1:N)
            modelBuilder.Entity<DeviceFingerprint>()
                .HasOne(df => df.User)
                .WithMany(u => u.DeviceFingerprints)
                .HasForeignKey(df => df.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User → UserBadge (1:N)
            modelBuilder.Entity<UserBadge>()
                .HasOne(ub => ub.User)
                .WithMany(u => u.Badges)
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Badge → UserBadge (1:N)
            modelBuilder.Entity<UserBadge>()
                .HasOne(ub => ub.Badge)
                .WithMany(b => b.UserBadges)
                .HasForeignKey(ub => ub.BadgeId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → UserVerification (1:N)
            modelBuilder.Entity<UserVerification>()
                .HasOne(uv => uv.User)
                .WithMany(u => u.Verifications)
                .HasForeignKey(uv => uv.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User → UserVerificationStep (1:N)
            modelBuilder.Entity<UserVerificationStep>()
                .HasOne(uvs => uvs.User)
                .WithMany(u => u.VerificationSteps)
                .HasForeignKey(uvs => uvs.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // User → EmailVerificationToken
            modelBuilder.Entity<EmailVerificationToken>()
                .HasOne(evt => evt.User)
                .WithMany(u => u.EmailVerificationTokens)
                .HasForeignKey(evt => evt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // VerificationStatus → UserVerification (1:N)
            modelBuilder.Entity<UserVerification>()
                .HasOne(uv => uv.VerificationStatus)
                .WithMany(vs => vs.UserVerifications)
                .HasForeignKey(uv => uv.VerificationStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → SitterQualification (1:N)
            modelBuilder.Entity<SitterQualification>()
                .HasOne(sq => sq.Sitter)
                .WithMany(u => u.SitterQualifications)
                .HasForeignKey(sq => sq.SitterId)
                .OnDelete(DeleteBehavior.Cascade);

            // QualificationType → SitterQualification (1:N)
            modelBuilder.Entity<SitterQualification>()
                .HasOne(sq => sq.QualificationType)
                .WithMany(qt => qt.SitterQualifications)
                .HasForeignKey(sq => sq.QualificationTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → SitterRestriction (1:N)
            modelBuilder.Entity<SitterRestriction>()
                .HasOne(sr => sr.Sitter)
                .WithMany(u => u.SitterRestrictions)
                .HasForeignKey(sr => sr.SitterId)
                .OnDelete(DeleteBehavior.Cascade);

            // RestrictionType → SitterRestriction (1:N)
            modelBuilder.Entity<SitterRestriction>()
                .HasOne(sr => sr.RestrictionType)
                .WithMany(rt => rt.SitterRestrictions)
                .HasForeignKey(sr => sr.RestrictionTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → SitterAvailability (1:N)
            modelBuilder.Entity<SitterAvailability>()
                .HasOne(sa => sa.Sitter)
                .WithMany(u => u.Availabilities)
                .HasForeignKey(sa => sa.SitterId)
                .OnDelete(DeleteBehavior.Cascade);

            // SitterSettings: 1:1 with User
            modelBuilder.Entity<SitterSettings>()
                .HasOne(ss => ss.User)
                .WithOne(u => u.SitterSettings)
                .HasForeignKey<SitterSettings>(ss => ss.SitterId)
                .OnDelete(DeleteBehavior.Cascade);

            // PointPolicy → PointsTransaction (1:N)
            modelBuilder.Entity<PointsTransaction>()
                .HasOne(pt => pt.PointPolicy)
                .WithMany(pp => pp.Transactions)
                .HasForeignKey(pt => pt.PolicyId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → PointsTransaction (Sender) (1:N)
            modelBuilder.Entity<PointsTransaction>()
                .HasOne(pt => pt.Sender)
                .WithMany(u => u.TransactionsSent)
                .HasForeignKey(pt => pt.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → PointsTransaction (Receiver) (1:N)
            modelBuilder.Entity<PointsTransaction>()
                .HasOne(pt => pt.Receiver)
                .WithMany(u => u.TransactionsReceived)
                .HasForeignKey(pt => pt.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // PointsTransaction → PointDonation (1:0..1)
            modelBuilder.Entity<PointDonation>()
                .HasOne(pd => pd.PointsTransaction)
                .WithOne(pt => pt.PointDonation)
                .HasForeignKey<PointDonation>(pd => pd.PointsTransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Shelter → PointDonation (1:N)
            modelBuilder.Entity<PointDonation>()
                .HasOne(pd => pd.Shelter)
                .WithMany(s => s.PointDonations)
                .HasForeignKey(pd => pd.ShelterId)
                .OnDelete(DeleteBehavior.Restrict);

            // HouseListing → HousePhoto (1:N)
            modelBuilder.Entity<HousePhoto>()
                .HasOne(hp => hp.Listing)
                .WithMany(hl => hl.Photos)
                .HasForeignKey(hp => hp.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            // HouseListing → HouseIssueReport (1:N)
            modelBuilder.Entity<HouseIssueReport>()
                .HasOne(hir => hir.BookingRequest)
                .WithMany(br => br.HouseIssues)
                .HasForeignKey(hir => hir.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // HouseIssueReport → User (Reporter) (N:1)
            modelBuilder.Entity<HouseIssueReport>()
                .HasOne(hir => hir.Reporter)
                .WithMany(u => u.HouseIssueReports)
                .HasForeignKey(hir => hir.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserAddress -> User (N:1)
            modelBuilder.Entity<UserAddress>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAddresses)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserAddress -> Address (N:1)
            modelBuilder.Entity<UserAddress>()
                .HasOne(ua => ua.Address)
                .WithMany(a => a.UserAddresses)
                .HasForeignKey(ua => ua.AddressId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserReport -> User (Reporter) (N:1)
            modelBuilder.Entity<UserReport>()
                .HasOne(ur => ur.Reporter)
                .WithMany(u => u.ReportsMade)
                .HasForeignKey(ur => ur.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserReport -> User (Reported User) (N:1)
            modelBuilder.Entity<UserReport>()
                .HasOne(ur => ur.ReportedUser)
                .WithMany(u => u.ReportsReceived)
                .HasForeignKey(ur => ur.ReportedUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> Referral (Referrer) (1:N)
            modelBuilder.Entity<Referral>()
                .HasOne(r => r.Referrer)
                .WithMany(u => u.ReferralsMade)
                .HasForeignKey(r => r.ReferrerId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> Referral (Referred) (1:0..1)
            modelBuilder.Entity<Referral>()
                .HasOne(r => r.ReferredUser)
                .WithOne(u => u.ReferralReceived)
                .HasForeignKey<Referral>(r => r.ReferredUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // SEED DATA
            modelBuilder.Entity<QualificationType>().HasData(
                new QualificationType { QualificationTypeId = 1, Code = "VET_EXP", Description = "Professional experience in veterinary care." },
                new QualificationType { QualificationTypeId = 2, Code = "FIRST_AID", Description = "Certified in pet first aid and CPR." },
                new QualificationType { QualificationTypeId = 3, Code = "DOG_TRAINER", Description = "Experience with dog training techniques." },
                new QualificationType { QualificationTypeId = 4, Code = "CAT_SPECIALIST", Description = "Specialized knowledge in cat behavior." }
            );

            // Seed data for Restriction Types
            modelBuilder.Entity<RestrictionType>().HasData(
                new RestrictionType { RestrictionTypeId = 1, Code = "NO_LARGE_DOGS", Description = "Cannot care for dogs weighing over 50 lbs." },
                new RestrictionType { RestrictionTypeId = 2, Code = "NO_CATS", Description = "Allergic to or cannot care for cats." },
                new RestrictionType { RestrictionTypeId = 3, Code = "NO_UNVACCINATED", Description = "Only cares for pets with up-to-date vaccinations." },
                new RestrictionType { RestrictionTypeId = 4, Code = "NO_OVERNIGHT", Description = "Only offers daytime services, no overnight care." }
            );

            // Seed Badges
            modelBuilder.Entity<Badge>().HasData(
                new Badge { BadgeId = 1, Name = "Reliable Sitter Streak", Description = "Completed 5+ bookings in a row without cancellations.", IsSpendable = false },
                new Badge { BadgeId = 2, Name = "Peak-Season Helper", Description = "Provided pet sitting services during high-demand holidays.", IsSpendable = false },
                new Badge { BadgeId = 3, Name = "Top Rated", Description = "Maintains an average rating of 4.8+ with at least 10 reviews.", IsSpendable = false },
                new Badge { BadgeId = 4, Name = "Frequent Sitter", Description = "Completed 10 or more bookings.", IsSpendable = false },
                new Badge { BadgeId = 5, Name = "Reviewer", Description = "Contributed 5 or more reviews to the community.", IsSpendable = false }
            );

            // Seed "fake" system user for awarding points:
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = -1,
                    Email = "system@petminder.com", // fake email
                    PasswordHash = "NO_LOGIN_SYSTEM_ACC",
                    FirstName = "System",
                    LastName = "Account",
                    Phone = "000000000",
                    Role = UserRole.None,
                    ProfilePhotoUrl = null,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });
        }
    }
}
