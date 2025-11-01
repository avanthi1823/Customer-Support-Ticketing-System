namespace CSTS.DTOs.TicketDTO
{
    public class CreateTicketDTO
    {

       
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public int Priority { get; set; } 
            public int CreatedById { get; set; } 
        }

    
    }

