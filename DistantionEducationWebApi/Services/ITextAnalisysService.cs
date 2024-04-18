using DistantionEducationWebApi.DTO;
using DistantionEducationWebApi.Models;
using DistantionEducationWebApi.Requests;
using DistantionEducationWebApi.Responses;

namespace DistantionEducationWebApi.Services
{
    public interface ITextAnalisysService
    {
        public TextAnalisysServiceResponse CompareTexts(string text1, string text2, TextAnalisysCustomParams customParams);
    }
}
