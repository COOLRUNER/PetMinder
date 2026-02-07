namespace PetMinder.Shared.DTO
{
    public class BadgeDTO
    {
        public long BadgeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime AwardedAt { get; set; }
    }
}
