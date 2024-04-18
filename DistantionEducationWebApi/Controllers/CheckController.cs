using DistantionEducationWebApi.Requests;
using DistantionEducationWebApi.Responses;
using DistantionEducationWebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DistantionEducationWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckController : ControllerBase
    {
        private readonly ICheckWorkService _checkService;

        public CheckController(ICheckWorkService checkService) 
        {
            _checkService = checkService;
        }

        [HttpPost]
        [Route("{id}")]
        public CheckResponse Post(string id, CheckRequest request)
        {
            Guid workId = Guid.Parse(id);
            return _checkService.Check(workId, request);
        }
    }
}
