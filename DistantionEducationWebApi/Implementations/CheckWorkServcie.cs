using DistantionEducationWebApi.Models;
using DistantionEducationWebApi.Repositories;
using DistantionEducationWebApi.Requests;
using DistantionEducationWebApi.Responses;
using DistantionEducationWebApi.Services;

namespace DistantionEducationWebApi.Implementations
{
    public class CheckWorkServcie : ICheckWorkService
    {
        private readonly StudentWorksRepository _repository;
        private readonly ITextAnalisysService _analisysService;

        public CheckWorkServcie(StudentWorksRepository repository, ITextAnalisysService analisysService)
        {
            _repository = repository;
            _analisysService = analisysService;
        }

        public CheckResponse Check(Guid workId, CheckRequest request)
        {
            var work = _repository.GetWorkById(workId);

            var result = new List<TaskResult>();

            foreach (var task in work.Tasks)
            {
                var answer = request.Answers.Find(ans => ans.TaskId.Equals(task.Id));

                switch (task.Type.Rule.Rule)
                {
                    case Rule.Match:
                        result.Add(CheckMatch(answer, task));
                        break;
                    case Rule.String:
                        result.Add(CheckString(answer, task));
                        break;
                    case Rule.Text:
                        result.Add(CheckText(answer, task, request));
                        break;
                    default:
                        throw new NotImplementedException("Checking for this rule is not implemented");
                }
            }

            return new CheckResponse()
            {
                Tasks = result,
                TotalScore = result.Aggregate(0.0, (acc, task) => acc + task.Score)
            };
        }

        private TaskResult CheckMatch(CheckAnswer answer, WorkTask task)
        {
            double score = 0.0;
            switch (task.Type.Rule.Answer)
            {
                case RuleAnswer.Single:
                    score = answer.Values?.First()?.ValueId == task.Answers.First(ans => ans.IsRight).Id ? 1 : 0;
                    break;
                case RuleAnswer.Multi:
                    var totalRightAnswers = task.Answers.Where(ans => ans.IsRight);
                    double totalRightsAnswersCount = totalRightAnswers.Count();
                    double rightAnswersCount = answer.Values.Where(v => totalRightAnswers.Any(ans => ans.Id == v.ValueId)).Count();
                    double incorrectAnswersCount = answer.Values.Where(v => !totalRightAnswers.Any(ans => ans.Id == v.ValueId)).Count();
                    double result = (rightAnswersCount - incorrectAnswersCount) / totalRightsAnswersCount;
                    score = result > 0 ? result : 0;
                    break;
                default:
                    score = 0;
                    break;
            }

            return new TaskResult()
            {
                TaskId = task.Id,
                Score = score,
                RightAnswers = task.Answers.Where((ans) => ans.IsRight)
            };
        }

        private TaskResult CheckString(CheckAnswer answer, WorkTask task)
        {
            string rightAnswer = NormilizeAnswer(task.Answers.First(ans => ans.IsRight).Description);
            string studentAnswer = NormilizeAnswer(answer.Values[0].ValueDescription);

            return new TaskResult()
            {
                TaskId = task.Id,
                Score = rightAnswer == studentAnswer ? 1 : 0,
                RightAnswers = task.Answers.Where((ans) => ans.IsRight)
            };
        }

        private TaskResult CheckText(CheckAnswer answer, WorkTask task, CheckRequest request)
        {
            var text1 = task.Answers.FirstOrDefault(ans => ans.IsRight).Description;
            var text2 = answer.Values[0].ValueDescription;
            var result = _analisysService.CompareTexts(text1, text2, new TextAnalisysCustomParams()
            {
                Method = MapMethodName(request.CurrentMethod),
                UseFrequency = request.UseFreequency,
                UseSynonyms = request.UseSynonyms,
                SynonymsMaxFine = request.SynonymsMaxFine
            });

            return new TaskResult()
            {
                TaskId = task.Id,
                Score = result.Similarity,
                RightAnswers = task.Answers.Where((ans) => ans.IsRight),
                GeneralTerminsInFirstText = result.GeneralTerminsInFirstText,
                GeneralTerminsInSecondText = result.GeneralTerminsInSecondText,
                ExtraTerminsInFirstText = result.ExtraTerminsInFirstText,
                ExtraTerminsInSecondText = result.ExtraTerminsInSecondText
            };
        }

        private string NormilizeAnswer(string answer)
        {
            return (new string(answer.Replace(" ", "").Where(c => Char.IsLetter(c)).ToArray())).ToLower(); 
        }

        private TextAnalisysMethod? MapMethodName(string methodName)
        {
            switch (methodName?.ToLower())
            {
                case "jaccard":
                    return TextAnalisysMethod.Jaccard;
                case "tfidf":
                    return TextAnalisysMethod.Tfidf;
                case "bert":
                    return TextAnalisysMethod.Bert;
                default:
                    return null;
            }
        }
    }
}
