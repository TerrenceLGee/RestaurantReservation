namespace RestaurantReservation.Application.Abstractions;

public class SmtpOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
}