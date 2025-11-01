namespace CSTS.DTOs.CommentDTO
{
    public class CreateCommentDTO
    {

        public int TicketId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
