namespace Communication.Infrastructure.Configuration;

public class MessageBusSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";

    public string Uri => $"amqp://{Username}:{Password}@{Host}:{Port}/{VirtualHost}";
} 