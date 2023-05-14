﻿using Autotest.Mvc.Models;
using Microsoft.Data.Sqlite;

namespace Autotest.Mvc.Repositories;

public class TicketRepository
{
    private readonly SqliteConnection _connection;
    public TicketRepository()
    {
        _connection = new SqliteConnection("Data source = autotest.db");
        _connection.Open();
        CreateTicketTable();
    }

    private void CreateTicketTable()
    {
        var command = _connection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS tickets(" +
            "id INTEGER PRIMARY KEY AUTOINCREMENT," +
            "user_id TEXT , " +
            "ticket_id INTEGER, " +
            "questions_count INTEGER, " +
            "start_index INTEGER , " +
            "current_question_index INTEGER , " +
            "date BIGINT)";
        command.ExecuteNonQuery();

        var command1 = _connection.CreateCommand();
        command1.CommandText = "CREATE TABLE IF NOT EXISTS ticket_answers (" +
            "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "ticket_id INTEGER, " +
            "question_index INTEGER, " +
            "choice_index INTEGER, " +
            "correct_index INTEGER )";
        command1.ExecuteNonQuery();
    }

    public void AddTicket(Ticket ticket)
    {
        var command = _connection.CreateCommand();
        command.CommandText = "INSERT INTO tickets(user_id," +
            " ticket_id, questions_count, " +
            "start_index, current_question_index," +
            " date)" +
            " VALUES(@user_id," +
            " @ticket_id," +
            " @questions_count," +
            " @start_index, " +
            "@current_question_index," +
            " @date)";

        command.Parameters.AddWithValue("user_id", ticket.UserId);
        command.Parameters.AddWithValue("ticket_id", ticket.TicketId);
        command.Parameters.AddWithValue("questions_count", ticket.QuestionsCount);
        command.Parameters.AddWithValue("start_index", ticket.StartIndex);
        command.Parameters.AddWithValue("current_question_index", ticket.CurrentQuestionIndex);
        command.Parameters.AddWithValue("date", ticket.Date.Ticks);
        command.Prepare();
        command.ExecuteNonQuery();
    }

    public void UpdateTicket(Ticket ticket)
    {

        var command = _connection.CreateCommand();
        command.CommandText = "UPDATE tickets SET current_question_index = @i , date = @d WHERE id =@id";
        command.Parameters.AddWithValue("i", ticket.CurrentQuestionIndex);
        command.Parameters.AddWithValue("d", ticket.Date.Ticks);
        command.Parameters.AddWithValue("id", ticket.Id);
        command.Prepare();
        command.ExecuteNonQuery();
    }

    public Ticket? GetTicketById(int ticketid)
    {
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT * FROM tickets WHERE id = @id";
        command.Parameters.AddWithValue("id", ticketid);
        command.Prepare();
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var NewTicket = new Ticket()
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetString(1),
                TicketId = reader.GetInt32(2),
                QuestionsCount = reader.GetInt32(3),
                StartIndex = reader.GetInt32(4),
                CurrentQuestionIndex = reader.GetInt32(5),
                Date = DateTime.FromFileTime(reader.GetInt64(6)),
            };
            NewTicket.Answers = GetTicketAnswers(ticketid);
            reader.Close();
            return NewTicket;
        }
        reader.Close();
        return null;
    }

    public void DeleteTicketAnswers(int ticketid)
    {
        var command = _connection.CreateCommand();
        command.CommandText = "DELETE FROM ticket_answers WHERE ticket_id=@id";
        command.Parameters.AddWithValue("id", ticketid);
        command.Prepare();
        command.ExecuteNonQuery();
    }

    public List<Ticket>? GetTicketList(string userId)
    {
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT * FROM tickets WHERE user_id = @id";
        command.Parameters.AddWithValue("id", userId);
        command.Prepare();

        var reader = command.ExecuteReader();

        var tickets = new List<Ticket>();

        while (reader.Read())
        {
            tickets.Add(new Ticket()
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetString(1),
                TicketId = reader.GetInt32(2),
                QuestionsCount = (int)reader.GetInt32(3),
                StartIndex = reader.GetInt32(4),
                CurrentQuestionIndex = reader.GetInt32(5),
                Date = DateTime.FromFileTime(reader.GetInt64(6))
            });
        }
        reader.Close();
        return tickets;
    }

    public void AddTicketAnswer(TicketQuestionAnswer answer)
    {
        var command = _connection.CreateCommand();
        command.CommandText = "INSERT INTO ticket_answers (" +
            "ticket_id , " +
            "question_index," +
            "choice_index, " +
            "correct_index)" +
            "VALUES (@ticket_id , @question_index , @choice_index , @correct_index);";

        command.Parameters.AddWithValue("ticket_id", answer.TicketId);
        command.Parameters.AddWithValue("question_index", answer.QuestionIndex);
        command.Parameters.AddWithValue("choice_index", answer.ChoiceIndex);
        command.Parameters.AddWithValue("correct_index", answer.CorrectIndex);
        command.Prepare();
        command.ExecuteNonQuery();

    }

    public List<TicketQuestionAnswer> GetTicketAnswers(int ticketid)
    {
        var command = _connection.CreateCommand();
        command.CommandText = "SELECT * FROM ticket_answers WHERE ticket_id = @id";
        command.Parameters.AddWithValue("id", ticketid);
        command.Prepare();

        var reader = command.ExecuteReader();

        var ticketAnswers = new List<TicketQuestionAnswer>();

        while (reader.Read())
        {
            ticketAnswers.Add(new TicketQuestionAnswer()
            {
                Id = reader.GetInt32(0),
                TicketId = reader.GetInt32(1),
                QuestionIndex = (int)reader.GetInt32(2),
                ChoiceIndex = reader.GetInt32(3),
                CorrectIndex = reader.GetInt32(4),
            });
        }
        reader.Close();
        return ticketAnswers;
    }

}
