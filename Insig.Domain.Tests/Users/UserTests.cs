using System;
using AutoFixture;
using Insig.Common.Auth.Lookups;
using Insig.Domain.Users;
using Shouldly;
using Xunit;

namespace Insig.Domain.Tests.Users;

public class UserTests
{
    private readonly Fixture _fixture = new Fixture();

    [Fact]
    public void User_WhenCreatingNewUserWithInvalidData_ThenExceptionIsThrown()
    {
        // given
        var sub = _fixture.Create<string>();
        var email = _fixture.Create<string>();

        // when then
        Should.Throw<ArgumentException>(() => new User(" ", email));
        Should.Throw<ArgumentException>(() => new User(null, email));
        Should.Throw<ArgumentException>(() => new User(sub, " "));
        Should.Throw<ArgumentException>(() => new User(sub, null));
    }

    [Fact]
    public void User_WhenCreatingNewUserWithCorrectData_ThenUserIsCreated()
    {
        // given
        var sub = _fixture.Create<string>();
        var email = _fixture.Create<string>();

        // when
        var user = new User(sub, email);

        // then
        user.ShouldSatisfyAllConditions(u =>
        {
            u.ShouldNotBeNull();
            u.Sub.ShouldBe(sub);
            u.Email.ShouldBe(email);
            u.RoleId.ShouldBe((int)RoleEnum.User);
        });
    }
}
