using Autotest.Mvc.Models;
using Microsoft.Data.Sqlite;

namespace Autotest.Mvc.Repositories;

public class UserRepository
{
    private readonly SqliteConnection _connection;
    private readonly TicketRepository _ticketRepository;
    public UserRepository(TicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
        _connection = new SqliteConnection("Data source = autotest.db");
        _connection.Open();
        CreateUserTable();
    }

    // user table yaratish
    private void CreateUserTable()
    {
        var command = _connection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS users(" +
            "id TEXT UNIQUE, " +
            "username TEXT NOT NULL , " +
            "password TEXT , " +
            "name TEXT,  " +
            "photo_url TEXT, " +
            "current_ticket_index INTEGER DEFAULT 0)";
        command.ExecuteNonQuery();
    }

    // user qoshish
    public void AddUser(User user)
    {
        var command = _connection.CreateCommand();
        command.CommandText = "INSERT INTO  users (id , username, password , name, photo_url) " +
            "VALUES (@id, @username, @password , @name,  @photo_url)";
        command.Parameters.AddWithValue("id", user.Id);
        command.Parameters.AddWithValue("username", user.Username);
        command.Parameters.AddWithValue("password", user.Password);
        command.Parameters.AddWithValue("name", user.Name);
        command.Parameters.AddWithValue("photo_url", user.PhotoPath);
        command.Prepare();
        command.ExecuteNonQuery();
    }

    // userni id orqali aniqlash
    public User? GetUserById(string id)
    {
        return GetUser("id", id);
    }

    // userni username orqali aniqlash
    public User? GetUserByUsername(string username)
    {
        return GetUser("username", username);
    }

    // userni ozini topib beradi
    public User? GetUser(string paramName, string paramValue)
    {
        var command = _connection.CreateCommand();
        command.CommandText = $"SELECT * FROM users WHERE {paramName} = @p";
        command.Parameters.AddWithValue("p", paramValue);
        command.Prepare();

        var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var NewUser = new User()
            {
                Id = reader.GetString(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                Name = reader.GetString(3),
                PhotoPath = reader.GetString(4),
                CurrentTicketIndex = reader.GetInt32(5)
            };

            NewUser.CurrentTicket = NewUser.CurrentTicketIndex == null ? null
                : _ticketRepository.GetTicketById(NewUser.CurrentTicketIndex.Value);
            NewUser.Tickets = _ticketRepository.GetTicketList(NewUser.Id)!;

            reader.Close();
            return NewUser;
        }

        reader.Close();
        return null;
    }

    // userimini update qilish funksiyasi
    public void UpdateUser(User user)
    {
        var command = _connection.CreateCommand();
        command.CommandText = "UPDATE users SET username =@username , password = @password , name = @name , photo_url = @photo_url , current_ticket_index = @index WHERE id = @id";
        command.Parameters.AddWithValue("id", user.Id);
        command.Parameters.AddWithValue("username", user.Username);
        command.Parameters.AddWithValue("password", user.Password);
        command.Parameters.AddWithValue("name", user.Name);
        command.Parameters.AddWithValue("photo_url", user.PhotoPath);
        command.Parameters.AddWithValue("index", user.CurrentTicketIndex);
        command.Prepare();
        command.ExecuteNonQuery();
    }


    // home page dagi indexdagi userlarni toplab beradi
    public List<User>? GetUsers()
    {
        var command = _connection.CreateCommand();
        command.CommandText = " SELECT * FROM users order by id DESC LIMIT 20";
        var reader = command.ExecuteReader();

        var users = new List<User>();
        while (reader.Read())
        {
            var user = new User()
            {
                Id = reader.GetString(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                Name = reader.GetString(3),
                PhotoPath = reader.GetString(4),
                CurrentTicketIndex = reader.GetInt32(5)
            };
            user.CurrentTicket = user.CurrentTicketIndex == null ? null
                : _ticketRepository.GetTicketById(user.CurrentTicketIndex.Value);
            user.Tickets = _ticketRepository.GetTicketList(user.Id)!;

            users.Add(user);
        }
        reader.Close();
        return users;

    }
}
