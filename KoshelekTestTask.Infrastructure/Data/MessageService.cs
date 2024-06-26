﻿using KoshelekTestTask.Core.Entities;
using KoshelekTestTask.Core.Interfaces;
using KoshelekTestTask.Core.Models;
using Npgsql;

namespace KoshelekTestTask.Infrastructure.Data
{
    public class MessageService : IMessageService
    {
        private static readonly string ConnectionString =
            $"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};" +
            $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};" +
            $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")};" +
            $"Database={Environment.GetEnvironmentVariable("POSTGRES_DB")}";

        public async Task SendMessageAsync(Message message)
        {
            await using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();

            await using var command = new NpgsqlCommand
            {
                Connection = connection,
                CommandText =
                    $"INSERT INTO koshelek (serial_number, text, time_of_sending) VALUES ({message.SerialNumber}, '{message.Text}', '{message.TimeOfSending.ToUniversalTime():O}'::timestamp);"
            };

            command.ExecuteNonQuery();
        }

        public async Task<List<Message>> FindMessagesOverPeriodOfTimeAsync(Interval interval)
        {
            await using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();

            await using var command = new NpgsqlCommand
            {
                Connection = connection,
                CommandText =
                    "SELECT serial_number, text, time_of_sending FROM koshelek " +
                    $"WHERE time_of_sending BETWEEN '{interval.Beginning.ToUniversalTime():O}'::timestamp AND '{interval.End.ToUniversalTime():O}'::timestamp;"
            };

            await using var dataReader = await command.ExecuteReaderAsync();
            var messages = new List<Message>();
            while (dataReader.Read())
                messages.Add(new Message
                {
                    SerialNumber = dataReader.GetInt32(0),
                    Text = dataReader.GetString(1),
                    TimeOfSending = (DateTime)dataReader.GetDateTime(2)
                });
            return messages;
        }
    }
}