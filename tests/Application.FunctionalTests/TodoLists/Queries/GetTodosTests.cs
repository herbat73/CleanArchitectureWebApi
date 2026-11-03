using CleanArchitectureWebApi.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitectureWebApi.Application.TodoLists.Queries.GetTodos;
using CleanArchitectureWebApi.Domain.Entities;
using CleanArchitectureWebApi.Domain.ValueObjects;

namespace CleanArchitectureWebApi.Application.FunctionalTests.TodoLists.Queries;

[Collection("Functional")]
public class GetTodosTests : TestBase
{
    [Fact]
    public async Task ShouldReturnPriorityLevels()
    {
        await TestApp.RunAsDefaultUserAsync();

        var query = new GetTodosQuery();

        var result = await TestApp.SendAsync<TodosVm>(query);

        result.PriorityLevels.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task ShouldReturnAllListsAndItems()
    {
        await TestApp.RunAsDefaultUserAsync();

        await TestApp.AddAsync(new TodoList
        {
            Title = "Shopping",
            Colour = Colour.Blue,
            Items =
                {
                    new TodoItem { Title = "Apples", Done = true },
                    new TodoItem { Title = "Milk", Done = true },
                    new TodoItem { Title = "Bread", Done = true },
                    new TodoItem { Title = "Toilet paper" },
                    new TodoItem { Title = "Pasta" },
                    new TodoItem { Title = "Tissues" },
                    new TodoItem { Title = "Tuna" }
                }
        });

        var query = new GetTodosQuery();

        var result = await TestApp.SendAsync<TodosVm>(query);

        result.Lists.Count.ShouldBe(1);
        result.Lists.First().Items.Count.ShouldBe(7);
    }

    [Fact]
    public async Task ShouldServeCachedResultUntilACommandInvalidatesIt()
    {
        await TestApp.RunAsDefaultUserAsync();

        await TestApp.AddAsync(new TodoList { Title = "Shopping", Colour = Colour.Blue });

        (await TestApp.SendAsync<TodosVm>(new GetTodosQuery())).Lists.Count.ShouldBe(1);

        // Written straight to the database, bypassing the commands, so the cached result is still served.
        await TestApp.AddAsync(new TodoList { Title = "Chores", Colour = Colour.Red });

        (await TestApp.SendAsync<TodosVm>(new GetTodosQuery())).Lists.Count.ShouldBe(1);

        // Any successful todo command evicts the cache.
        await TestApp.SendAsync<int>(new CreateTodoListCommand { Title = "Work" });

        (await TestApp.SendAsync<TodosVm>(new GetTodosQuery())).Lists.Count.ShouldBe(3);
    }

    [Fact]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new GetTodosQuery();

        var action = () => TestApp.SendAsync(query);

        await Should.ThrowAsync<UnauthorizedAccessException>(action);
    }
}
