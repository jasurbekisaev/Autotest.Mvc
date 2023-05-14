namespace Autotest.Mvc.Models;

public class QuestionModel
{
    public int Id { get; set; }
    public string Question { get; set; } = null!;
    public string Description { get; set; } = null!;
    public QuestionMedia Media { get; set; } = null!;
    public List<QuestionChoice> Choices { get; set; } = null!;
}