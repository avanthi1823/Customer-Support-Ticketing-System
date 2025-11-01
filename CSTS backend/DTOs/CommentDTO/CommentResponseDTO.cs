namespace CSTS.DTOs.CommentDTO
{
    public class CommentResponseDTO
    {
        public int CommentId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
