namespace Insig.Common.Infrastructure.Email;

public class EmailSettings
{
    public int Port { get; set; }
    public string Host { get; set; }
    public string DisplayName { get; set; }
    public Credentials Credentials { get; set; }
}

public class Credentials
{
    public string Name { get; set; }
    public string Password { get; set; }
}