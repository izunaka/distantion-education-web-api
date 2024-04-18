using DistantionEducationWebApi.Models;
using DistantionEducationWebApi.Repositories;
using DistantionEducationWebApi.Requests;
using DistantionEducationWebApi.Responses;
using DistantionEducationWebApi.Services;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;
using System.Reflection;
using static Community.CsharpSqlite.Sqlite3;
using System.Diagnostics;
using Newtonsoft.Json;
using DistantionEducationWebApi.DTO;
using System.Globalization;
using System.Text;

namespace DistantionEducationWebApi.Implementations
{
    public class TextAnalisysService : ITextAnalisysService
    {
        const string SettingsSectionName = "TextAnalisysSettings";
        const string ModulePathFieldName = "ModulePath";
        const string PythonPathFieldName = "PythonPath";
        const string LibsPathFieldName = "PythonLibsPath";
        const string DefaultMethodFieldName = "DefaultMethod";
        const string UseSynonymsFieldName = "UseSynonyms";
        const string UseFrequencyFieldName = "UseFrequency";
        const string SynonymsMaxFineFieldName = "SynonymsMaxFine";
        const string MappingFunctionName = "mapping";

        private readonly IConfigurationSection _settings;

        public TextAnalisysService(IConfiguration configurationManager)
        {
            _settings = configurationManager.GetSection(SettingsSectionName);
        }

        public TextAnalisysServiceResponse CompareTexts(string text1, string text2, TextAnalisysCustomParams customParams)
        {
            ModuleResponse result;

            string modulePath = _settings.GetValue<string>(ModulePathFieldName);
            string methodName = customParams?.Method?.ToString().ToLower() ?? _settings.GetValue<string>(DefaultMethodFieldName).ToLower();
            string fileName = $"{modulePath}/{methodName}Method.py";

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = _settings.GetValue<string>(PythonPathFieldName);
            start.Arguments = PrepareArgs(text1, text2, customParams, fileName);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;

            //start.StandardOutputEncoding = Encoding.GetEncoding(1251);
            start.StandardOutputEncoding = Encoding.UTF8;
            using (Process process = Process.Start(start))
            {
                process.WaitForExit();
                using (StreamReader reader = process.StandardOutput)
                {
                    var rawRes = reader.ReadToEnd();
                    result = JsonConvert.DeserializeObject<ModuleResponse>(rawRes);
                }
            }

            return new TextAnalisysServiceResponse()
            {
                Similarity = result.Similarity,
                GeneralTerminsInFirstText = result.GeneralTerminsInFirstText,
                GeneralTerminsInSecondText = result.GeneralTerminsInSecondText,
                ExtraTerminsInFirstText = result.ExtraTerminsInFirstText,
                ExtraTerminsInSecondText = result.ExtraTerminsInSecondText
            };
        }

        private string PrepareArgs(string text1, string text2, TextAnalisysCustomParams customParams, string fileName)
        {
            List<string> args = new List<string>();
            args.Add(fileName);
            args.Add($"--text1 \"{text1}\"");
            args.Add($"--text2 \"{text2}\"");
            args.Add($"--maxfine {customParams?.SynonymsMaxFine ?? _settings.GetValue<double>(SynonymsMaxFineFieldName)}");
            if (customParams?.UseSynonyms ?? _settings.GetValue<bool>(UseSynonymsFieldName)) { args.Add($"--synonyms 1"); }
            if (customParams?.UseFrequency ?? _settings.GetValue<bool>(UseFrequencyFieldName)) { args.Add($"--frequency 1"); }
            return String.Join(" ", args.ToArray());
        }

        private class ModuleResponse
        {
            public double Similarity { get; set; }

            public IEnumerable<string> GeneralTerminsInFirstText { get; set; }

            public IEnumerable<string> GeneralTerminsInSecondText { get; set; }

            public IEnumerable<string> ExtraTerminsInFirstText { get; set; }

            public IEnumerable<string> ExtraTerminsInSecondText { get; set; }
        }

        //private readonly IConfigurationSection _settings;
        //private readonly ScriptEngine _engine;
        //private readonly ScriptScope _scope;

        //public TextAnalisysService(IConfiguration configurationManager)
        //{
        //    _settings = configurationManager.GetSection(SettingsSectionName);
        //    _engine = Python.CreateEngine();
        //    _scope = _engine.CreateScope();

        //    var searchPaths = _engine.GetSearchPaths();
        //    foreach (var path in _settings.GetSection(LibsPathFieldName).Get<string[]>())
        //    {
        //        searchPaths.Add(path);
        //    }
        //    _engine.SetSearchPaths(searchPaths);
        //}

        //public double CompareTexts(string text1, string text2, TextAnalisysCustomParams customParams)
        //{
        //    string methodName = customParams?.Method?.ToString().ToLower() ?? _settings.GetValue<string>(DefaultMethodFieldName).ToLower();
        //    string fileName = $"{_settings.GetValue<string>(ModulePathFieldName)}\\{methodName}Method.py";

        //    _engine.ExecuteFile(fileName);
        //    dynamic mapping = _scope.GetVariable(MappingFunctionName);

        //    return mapping(text1, text2, new
        //    {
        //        useSynonyms = customParams?.UseSynonyms ?? _settings.GetValue<bool>(UseSynonymsFieldName),
        //        synonymsMaxFine = customParams?.SynonymsMaxFine ?? _settings.GetValue<double>(SynonymsMaxFineFieldName),
        //        useFrequency = customParams?.UseFrequency ?? _settings.GetValue<bool>(UseFrequencyFieldName)
        //    });
        //}
    }
}
