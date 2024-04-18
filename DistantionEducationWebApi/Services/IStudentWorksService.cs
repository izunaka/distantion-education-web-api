using DistantionEducationWebApi.Models;

namespace DistantionEducationWebApi.Services
{
    public interface IStudentWorksService
    {
        public StudentWork CreateWork(StudentWork work);

        public List<StudentWork> GetAllWorks();

        public StudentWork GetWorkById(Guid id);

        public List<TaskType> GetTaskTypes();

        public void DeleteWork(Guid workId);
    }
}
