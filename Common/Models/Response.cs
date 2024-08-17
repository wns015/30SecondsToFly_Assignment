using Newtonsoft.Json;

namespace Common.Models
{
    public abstract class ResponseAbstract<T> where T : ResponseAbstract<T>
    {
        public const string SuccessStatus = "0";
        public const string SuccessMessage = "success";

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        public T Success()
        {
            this.Status = SuccessStatus;
            this.Message = SuccessMessage;

            return (T)this;
        }

        public T Is(string status, string message)
        {
            this.Status = status;
            this.Message = message;

            return (T)this;
        }
    }

    public class Response : ResponseAbstract<Response>
    {
        public Response()
        {
        }
    }

    public class Response<T> : ResponseAbstract<Response<T>>
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        public Response(T data)
        {
            this.Data = data;
        }
    }
}
