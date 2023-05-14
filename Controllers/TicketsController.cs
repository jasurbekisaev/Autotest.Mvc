using Autotest.Mvc.Models;
using Autotest.Mvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace Autotest.Mvc.Controllers;

public class TicketsController : Controller
{

    private readonly QuestionService _questionService;
    private readonly UsersService _usersService;

    public TicketsController(QuestionService questionService, UsersService usersService)
    {
        _questionService = questionService;
        _usersService = usersService;
    }

    public IActionResult Index()
    {
        var user = _usersService.GetCurrentUser(HttpContext);

        if (user == null)
            return RedirectToAction("SignIn", "Users");
        // user.Tickets = UsersService._ticketRepository.GetTicketList(user.Id)!;
        return View(user);
    }

    public IActionResult StartTicket(int ticketIndex)
    {
        var user = _usersService.GetCurrentUser(HttpContext);

        if (user == null)
            return RedirectToAction("SignIn", "Users");

        var tickets = _usersService._ticketRepository.GetTicketList(user.Id);

        if (_questionService.TicketsCount <= ticketIndex)
            return View("NotFound");

        user.CurrentTicketIndex = ticketIndex;
        user.CurrentTicket = user.CurrentTicketIndex == null ? null
            : _usersService._ticketRepository.GetTicketById(user.CurrentTicketIndex.Value);
        _usersService._userRepository.UpdateUser(user);

        return RedirectToAction("Questions", new { id = user.CurrentTicket?.StartIndex });
    }

    public IActionResult Questions(int id, int? choiceIndex = null)
    {
        User? user = _usersService.GetCurrentUser(HttpContext);
        if (user == null)
            return RedirectToAction("SignIn", "Users");

        if (user.CurrentTicketIndex == null)
            return RedirectToAction("Index");

        if (id > user.CurrentTicketIndex * 10 + 10)
        {
            return RedirectToAction(nameof(Result));
        }

        var question = _questionService.Questions?.FirstOrDefault(x => x.Id == id);

        if (question == null)
            return View("NotFound");

        ViewBag.Question = question;
        ViewBag.IsAnswer = choiceIndex != null;

        if (choiceIndex != null)
        {
            var answer = new TicketQuestionAnswer()
            {
                TicketId = user.CurrentTicketIndex.Value,
                ChoiceIndex = choiceIndex.Value,
                QuestionIndex = id,
                CorrectIndex = question.Choices.IndexOf(question.Choices.First(c => c.Answer))
            };

            // user.CurrentTicket!.Answers.Add(answer);

            _usersService._ticketRepository.AddTicketAnswer(answer);

            ViewBag.Answer = answer;
        }
        ViewBag.TicketAnswers = _usersService._ticketRepository.GetTicketAnswers((int)user.CurrentTicketIndex);
        return View(user);
    }

    public IActionResult Result()
    {
        var user = _usersService.GetCurrentUser(HttpContext);

        if (user == null)
            return RedirectToAction("SignIn", "Users");

        return View(user);
    }
}