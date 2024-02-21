using AutoFixture;
using Insig.Domain.Users;

namespace Insig.Domain.Tests.Builder;

public class UserBuilder
{
    private string _sub, _email;
    private readonly Fixture _fixture = new Fixture();

    public UserBuilder()
    {
        _sub = _fixture.Create<string>();
        _email = _fixture.Create<string>();
    }

    public static UserBuilder CreateNew()
    {
        return new UserBuilder();
    }

    public UserBuilder WithSub(string sub)
    {
        _sub = sub;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public User Build()
    {
        var user = new User(_sub, _email);

        return user;
    }
}
