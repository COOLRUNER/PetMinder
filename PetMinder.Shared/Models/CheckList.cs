using Microsoft.EntityFrameworkCore;

namespace PetMinder.Models
{
    [Owned]
    public class CheckList
    {
        public int SizeofHome { get; set; }
        public int NumberofRooms { get; set; }
        public int Floor { get; set; }
        public bool hasElevator { get; set; }
        public bool hasWifi { get; set; }
        public string Description { get; set; }
    }
}
