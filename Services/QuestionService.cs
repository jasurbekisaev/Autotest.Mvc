using Autotest.Mvc.Models;
using Newtonsoft.Json;

namespace Autotest.Mvc.Services;

public class QuestionService
{

    public List<QuestionModel> Questions;

    public int TicketQuestionsCount => 10;
    public int TicketsCount => Questions.Count / TicketQuestionsCount;

    public QuestionService()
    {
        LoadJson("uz");
    }

    public void LoadJson(string language)
    {
        var jsonPath = "uzlotin.json";

        switch (language)
        {
            case "uz": jsonPath = "uzlotin.json"; break;
            case "uzc": jsonPath = "uzkiril.json"; break;
            case "ru": jsonPath = "rus.json"; break;
        }

        var path = Path.Combine("JsonData", jsonPath);

        if (File.Exists(path))
        {
            var json = System.IO.File.ReadAllText(path);
            Questions = JsonConvert.DeserializeObject<List<QuestionModel>>(json)!;
        }
    }
}