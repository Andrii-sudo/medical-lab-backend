using LabAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace DbInitializer;

internal static class DataGenerator
{
    const string SourcePath = "Source.json";

    internal static void ClearDatabase(MedicalLabsContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.Migrate();
    }

    public static void ParseSource(MedicalLabsContext context)
    {
        using StreamReader reader = new StreamReader(SourcePath);

        string json = reader.ReadToEnd();
        List<SourceItem> items = JsonSerializer.Deserialize<List<SourceItem>>(json)
            ?? throw new InvalidOperationException($"{SourcePath} is empty or has invalid format");

        // Для уникнення дублуювання
        var addedAnalyses = new Dictionary<string, Analysis>();
        var addedParameters = new Dictionary<(string, string), Parameter>();
        var addedParamNorms = new HashSet<(string, string, byte, byte, string)>();

        foreach (var it in items)
        {
            if (it.Question.Contains("with the condition"))
            {
                continue;
            }

            int i = 0;
            i = MoveToNextQuote(it.Question, i);
            (i, string analysisName) = ReadQuote(it.Question, i);

            i = MoveToNextQuote(it.Question, i);
            (i, string paramUnit) = ReadQuote(it.Question, i);

            i = MoveToNextQuote(it.Question, i);
            (i, string sampleType) = ReadQuote(it.Question, i);

            i = MoveToNextQuote(it.Question, i);
            (i, string rawGender) = ReadQuote(it.Question, i);
            string gender = MapGender(rawGender);

            i = MoveToNextQuote(it.Question, i);
            (i, string ageGroup) = ReadQuote(it.Question, i);
            var (minAge, maxAge) = MapAgeGroup(ageGroup);


            string paramName = analysisName;
            if (it.Question.Contains("in the category"))
            {
                i = MoveToNextQuote(it.Question, i);
                (i, paramName) = ReadQuote(it.Question, i);
            }

            var (minValue, maxValue) = GetNormRange(it.Answer);


            Analysis? analysisEntity;
            if (!addedAnalyses.TryGetValue(analysisName, out analysisEntity))
            {
                analysisEntity = new Analysis
                {
                    Name = analysisName,
                    Price = 0,
                    SampleType = sampleType,
                    ExpiryDays = 0
                };

                context.Analyses.Add(analysisEntity);

                addedAnalyses.Add(analysisName, analysisEntity);
            }

            var paramKey = (analysisName, paramName);
            Parameter? parameterEntity;
            if (!addedParameters.TryGetValue(paramKey, out parameterEntity))
            {
                parameterEntity = new Parameter
                {
                    ParameterName = paramName,
                    Unit = paramUnit,
                    Analysis = analysisEntity
                };

                context.Parameters.Add(parameterEntity);

                addedParameters.Add(paramKey, parameterEntity);
            }

            var paramNormKey = (analysisName, paramName, minAge, maxAge, gender);
            if (!addedParamNorms.Contains(paramNormKey))
            {
                ParameterNorm parameterNorm = new ParameterNorm
                {
                    Gender = gender,
                    AgeMin = minAge,
                    AgeMax = maxAge,
                    MinValue = minValue,
                    MaxValue = maxValue,
                    Parameter = parameterEntity
                };

                context.ParameterNorms.Add(parameterNorm);

                addedParamNorms.Add(paramNormKey);
            }
        }
        context.SaveChanges(); // Ключі згенеруються тут


        static int MoveToNextQuote(string question, int i)
        {
            while (question[i] != '\'' && i < question.Length)
            {
                i++;
            }

            return ++i;
        }

        static (int, string) ReadQuote(string question, int i)
        {
            StringBuilder sb = new StringBuilder();
            while (question[i] != '\'' && i < question.Length)
            {
                sb.Append(question[i]);
                i++;
            }

            return (++i, sb.ToString());
        }

        static string MapGender(string gender)
        {
            switch (gender)
            {
                case "Male":
                    return "M";
                case "Female":
                    return "F";
                default: // 'any gender'
                    return "A";
            }
        }

        static (byte, byte) MapAgeGroup(string ageGroup)
        {
            switch (ageGroup)
            {
                case "Infant":
                    return (0, 0);
                case "Child":
                    return (1, 17);
                case "Adult":
                    return (18, 255);
                default: // any age group
                    return (0, 255);
            }
        }

        static (decimal?, decimal?) GetNormRange(string answer)
        {
            string cleanAnswer = answer.Replace("\n", "");

            decimal? minValue = null;
            decimal? maxValue = null;
            if (cleanAnswer[0] == '<')
            {
                maxValue = decimal.Parse(cleanAnswer.Trim('<'), CultureInfo.InvariantCulture);
            }
            else if (cleanAnswer[0] == '>')
            {
                minValue = decimal.Parse(cleanAnswer.Trim('>'), CultureInfo.InvariantCulture);
            }
            else if (cleanAnswer.Contains('-'))
            {
                string[] parts = cleanAnswer.Split('-');
                minValue = decimal.Parse(parts[0], CultureInfo.InvariantCulture);
                maxValue = decimal.Parse(parts[1], CultureInfo.InvariantCulture);
            }
            else
            {
                decimal exactValue = decimal.Parse(cleanAnswer, CultureInfo.InvariantCulture);
                minValue = exactValue;
                maxValue = exactValue;
            }

            return (minValue, maxValue);
        }
    }
}
