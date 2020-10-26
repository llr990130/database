using database.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace database.Controllers
{
    [RoutePrefix("api/Homehtml")]
    [RequestAuthorize]
    public class HomehtmlController : ApiController
    {
        [HttpGet]
        public void loadPage()
        {
            return;
        }
    }
}
