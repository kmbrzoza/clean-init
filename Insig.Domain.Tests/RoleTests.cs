using System;
using AutoFixture;
using Insig.Domain.Accesses;
using Shouldly;
using Xunit;

namespace Insig.Domain.Tests;

public class RoleTests
{
    private readonly Fixture _fixture = new Fixture();

    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    public void Role_WhenCreatingNewRoleValueWithInCorrectData_ThenExceptionIsThrown(string name)
    {
        Should.Throw<ArgumentException>(() => new Role(name));
    }

    [Fact]
    public void Role_WhenCreatingNewRoleWithCorrectData_ThenRoleIsCreated()
    {
        // given
        string name = _fixture.Create<string>();

        // when
        var role = new Role(name);

        // then
        role.ShouldNotBeNull();
        role.Name.ShouldBe(name);
    }

    [Fact]
    public void Role_WhenCreatingNewRoleWithCorrectDataAndId_ThenRoleIsCreated()
    {
        // given
        string name = _fixture.Create<string>();
        int id = _fixture.Create<int>();

        // when
        var role = new Role(id, name);

        // then
        role.ShouldNotBeNull();
        role.Id.ShouldBe(id);
        role.Name.ShouldBe(name);
    }
}
