namespace PetMinder.Shared.DTO
{
    public class AdminUserListDTO
    {
        public long UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty; 
        public string LastName { get; set; } = string.Empty; 
        public string Phone { get; set; } = string.Empty;
        public string RoleString { get; set; } = string.Empty;
        public int CurrentPoints { get; set; }
        public bool IsFlagged { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    
    public class AdminUserDetailsDTO
    {
        public AdminUserListDTO UserInfo { get; set; }

        public List<AdminTransactionSimpleDTO> RecentTransactions { get; set; } = new();
        public List<AdminBookingSimpleDTO> RecentBookings { get; set; } = new();
        public List<AdminReviewSimpleDTO> RecentReviews { get; set; } = new();
    }
    
    public class AdminTransactionSimpleDTO
    {
        public long Id { get; set; }
        public DateTime OccurredAt { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public bool IsCredit { get; set; } 
    }
    
    public class AdminBookingSimpleDTO
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string OtherUserName { get; set; }
        public string Status { get; set; }
        public int OfferedPoints { get; set; }
    }
    
    public class AdminReviewSimpleDTO
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }
        public string AuthorName { get; set; }
    }

    public class AdminReportedReviewDTO
    {
        public long ReviewId { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; } 
        public DateTime CreatedAt { get; set; }
        
        public string AuthorName { get; set; }
        public string TargetName { get; set; }

        public List<AdminReportDetailDTO> Reports { get; set; } = new();
    }
    
    public class AdminReportDetailDTO
    {
        public long ReportId { get; set; }
        public string ReporterName { get; set; }
        public string Reason { get; set; }
        public DateTime ReportedAt { get; set; }
    }
    
    public class AdminReportedUserDTO
    {
        public long UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsFlagged { get; set; }
        public bool IsBanned { get; set; }
        public int ReportCount { get; set; }
        public DateTime LastReportDate { get; set; }
        public List<AdminReportDetailDTO> Reports { get; set; } = new();
    }
}

