using System.Net.Mail;

namespace Train.Models
{
    public class EmailRequest
    {
        public string ApiToken { get; set; }
        public List<EmailMessage> Messages { get; set; }
    }
}
