namespace FxChat.Domain
{
    public class Message
    {
        public DateTime Time { get; set; }
        public string EndPoint { get; set; }

        public EndPointType EndPointType { get; set; }

        public string? Content { get; set; }

        public static Message BuildMessage(string endPoint, EndPointType endPointType, string content)
        {
            Message message = new Message()
            {
                Time = DateTime.Now,
                EndPoint = endPoint,
                EndPointType = endPointType,
                Content = content
            };
            return message;
        }
    }

    public enum EndPointType
    {
        Local,
        Remote,
    }
}
