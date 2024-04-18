using DistantionEducationWebApi.Models;
using DistantionEducationWebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DistantionEducationWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskTypesController : ControllerBase
    {
        private readonly ILogger<StudentWorksController> _logger;
        private readonly IStudentWorksService _worksService;

        public TaskTypesController(ILogger<StudentWorksController> logger, IStudentWorksService worksService)
        {
            _logger = logger;
            _worksService = worksService;
        }

        [HttpGet]
        [Route("Get")]
        public List<TaskType> Get()
        {
            return _worksService.GetTaskTypes();
        }
    }
}
