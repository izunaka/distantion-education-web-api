using DistantionEducationWebApi.Models;
using DistantionEducationWebApi.Repositories;
using DistantionEducationWebApi.Services;

namespace DistantionEducationWebApi.Implementations
{
    public class StudentWorksService : IStudentWorksService
    {
        private readonly StudentWorksRepository _repository;

        public StudentWorksService(StudentWorksRepository repository)
        {
            _repository = repository;
        }

        public StudentWork CreateWork(StudentWork work)
        {
            EnrichWorkWithIds(work);

            _repository.CreateWork(work);

            return _repository.GetWorkById(work.Id);
        }

        public List<StudentWork> GetAllWorks()
        {
            return _repository.GetAllWorks();
        }

        public StudentWork GetWorkById(Guid id)
        {
            return _repository.GetWorkById(id);
        }

        public List<TaskType> GetTaskTypes()
        {
            return _repository.GetTaskTypes();
        }

        public void DeleteWork(Guid id)
        {
            _repository.DeleteWork(id);
        }

        private void EnrichWorkWithIds(StudentWork work)
        {
            work.Id = Guid.NewGuid();
            foreach (var task in work.Tasks)
            {
                task.Id = Guid.NewGuid();
                foreach (var answer in task.Answers)
                {
                    answer.Id = Guid.NewGuid();
                }
            }
        }
    }
}
