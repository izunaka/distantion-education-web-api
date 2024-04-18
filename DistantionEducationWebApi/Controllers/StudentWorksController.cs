using DistantionEducationWebApi.Models;
using DistantionEducationWebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DistantionEducationWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentWorksController : ControllerBase
    {
        private readonly ILogger<StudentWorksController> _logger;
        private readonly IStudentWorksService _worksService;

        public StudentWorksController(ILogger<StudentWorksController> logger, IStudentWorksService worksService)
        {
            _logger = logger;
            _worksService = worksService;
        }

        [HttpPost]
        [Route("Create")]
        public StudentWork Post(StudentWork request)
        {
            return _worksService.CreateWork(request);
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public void Post(string id)
        {
            _worksService.DeleteWork(Guid.Parse(id));
        }

        [HttpGet]
        [Route("Get")]
        public List<StudentWork> Get()
        {
            return _worksService.GetAllWorks();
        }

        [HttpGet]
        [Route("Get/{id}")]
        public StudentWork Get(string id)
        {
            return _worksService.GetWorkById(Guid.Parse(id));
        }
    }
}
