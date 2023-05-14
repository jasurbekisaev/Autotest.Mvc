using Autotest.Mvc.Models;
using Autotest.Mvc.Repositories;
namespace Autotest.Mvc.Services;

public class UsersService
{
    private const string UserIdCookieKey = "user_id";
    public readonly TicketRepository _ticketRepository;
    public readonly UserRepository _userRepository;
    private readonly QuestionService _questionService;

    public UsersService(TicketRepository ticketRepository, UserRepository userRepository, QuestionService questionService)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
        _questionService = questionService;
    }

    public void Register(CreateUserModel createUser, HttpContext httpContext)
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = createUser.Name!,
            Password = createUser.Password!,
            Username = createUser.Username!,
            PhotoPath = SavePhoto(createUser.Photo!),
        };

        user.CurrentTicket = user.CurrentTicketIndex == null ? null
            : _ticketRepository.GetTicketById(user.CurrentTicketIndex.Value);
        user.Tickets = _ticketRepository.GetTicketList(user.Id);

        CreateUserTickets(user);

        _userRepository.AddUser(user);

        httpContext.Response.Cookies.Append(UserIdCookieKey, user.Id);
    }

    public bool LogIn(SignInUserModel signInUserModel, HttpContext httpContext)
    {
        var user = _userRepository.GetUserByUsername(signInUserModel.Username!);

        if (user == null || user.Password != signInUserModel.Password)
            return false;

        httpContext.Response.Cookies.Append(UserIdCookieKey, user.Id);

        return true;
    }

    public User? GetCurrentUser(HttpContext context)
    {
        if (context.Request.Cookies.ContainsKey(UserIdCookieKey))
        {
            var userId = context.Request.Cookies[UserIdCookieKey];
            var user = _userRepository.GetUserById(userId!);
            return user;
        }

        return null;
    }

    public bool IsLoggedIn(HttpContext context)
    {
        if (!context.Request.Cookies.ContainsKey(UserIdCookieKey)) return false;

        var userId = context.Request.Cookies[UserIdCookieKey];
        var user = _userRepository.GetUserById(userId!);

        return user != null;
    }


    public void LogOut(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(UserIdCookieKey);
    }

    private void CreateUserTickets(User user)
    {
        for (var i = 0; i < _questionService.TicketsCount; i++)
        {
            var startIndex = i * _questionService.TicketQuestionsCount + 1;
            _ticketRepository.AddTicket(new Ticket()
            {
                TicketId = i,
                UserId = user.Id,
                CurrentQuestionIndex = startIndex,
                StartIndex = startIndex,
                QuestionsCount = _questionService.TicketQuestionsCount
            });
        }

    }

    private string SavePhoto(IFormFile file)
    {
        if (!Directory.Exists("wwwroot/UserImages"))
            Directory.CreateDirectory("wwwroot/UserImages");

        var fileName = Guid.NewGuid() + ".jpg";
        var ms = new MemoryStream();
        file.CopyTo(ms);
        System.IO.File.WriteAllBytes(Path.Combine("wwwroot", "UserImages", fileName), ms.ToArray());

        return "/UserImages/" + fileName;
    }
}