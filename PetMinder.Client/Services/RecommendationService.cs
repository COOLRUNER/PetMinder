using PetMinder.Models;
using PetMinder.Shared.DTO;

namespace PetMinder.Client.Services
{
    public class RecommendationService
    {
        public int CalculateRecommendedPoints(TimeSpan duration, PetType petType, PetBehaviorComplexity complexity, bool isUrgent, double sitterRating, int sitterMinPoints, int policyBaseHourlyRate)
        {
            double baseHourlyRate = (double)policyBaseHourlyRate;            
            double durationHours = duration.TotalHours;
            double basePoints = durationHours * sitterMinPoints;
            if (basePoints < 10) basePoints = 10; 

            double complexityMultiplier = complexity switch
            {
                PetBehaviorComplexity.Low => 1.0,
                PetBehaviorComplexity.Moderate => 1.2,
                PetBehaviorComplexity.High => 1.5,
                PetBehaviorComplexity.Extreme => 2.0,
                _ => 1.0
            };

            double typeMultiplier = petType switch
            {
                PetType.Dog => 1.0,
                PetType.Cat => 0.9,
                PetType.SmallAnimal => 0.7,
                PetType.Bird => 0.8,
                PetType.Reptile => 1.1,
                _ => 1.0
            };

            double urgencyMultiplier = isUrgent ? 1.25 : 1.0;

            double calculated = basePoints * complexityMultiplier * typeMultiplier * urgencyMultiplier;

            if (sitterRating >= 4.8) calculated += 10;
            else if (sitterRating >= 4.5) calculated += 5;

            int recommended = (int)Math.Round(calculated);
            
            int sitterMinimumTotal = (int)Math.Ceiling(durationHours * sitterMinPoints);

            return Math.Max(Math.Max(recommended, sitterMinimumTotal), 0);
        }
    }
}
