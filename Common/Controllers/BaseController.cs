using Common.Models;
using System.Web.Http;

namespace Common.Controllers
{
    public abstract class BaseController : ApiController
    {
        protected Response<T> Response<T>(T data)
        {
            return new Response<T>(data);
        }

        protected Response Response()
        {
            return new Response();
        }
    }
}
