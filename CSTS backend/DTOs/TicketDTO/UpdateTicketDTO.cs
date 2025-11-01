namespace CSTS.DTOs
{
    public class UpdateTicketDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Priority { get; set; }
        public int? AssignedToId { get; set; }
        public int? Status { get; set; } 
    }
}
