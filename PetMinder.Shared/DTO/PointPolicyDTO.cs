namespace PetMinder.Shared.DTO;

public class PointPolicyDTO
{
    public long PolicyId { get; set; }
    public string ServiceType { get; set; }
    public int PointsPerStay { get; set; }
    public int MinSpendable { get; set; }
    public int ExpiryDays { get; set; }
}