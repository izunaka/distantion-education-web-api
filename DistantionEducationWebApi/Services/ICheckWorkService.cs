using DistantionEducationWebApi.Models;
using DistantionEducationWebApi.Requests;
using DistantionEducationWebApi.Responses;

namespace DistantionEducationWebApi.Services
{
    public interface ICheckWorkService
    {
        public CheckResponse Check(Guid workId, CheckRequest request);
    }
}
