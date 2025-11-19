namespace Shared.Dtos.Responses
{
    public class StatusResponse
    {
        public int Code { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public string Message
        {
            
            set
            {
                if (Messages == null) Messages = new List<string>();
                Messages.Add(value!);
            }
        }
    }


}
