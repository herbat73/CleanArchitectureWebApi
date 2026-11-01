using CleanArchietureNoFrontEnd.Application.TodoLists.Commands.CreateTodoList;
using CleanArchietureNoFrontEnd.Application.TodoLists.Commands.DeleteTodoList;
using CleanArchietureNoFrontEnd.Domain.Entities;

namespace CleanArchietureNoFrontEnd.Application.FunctionalTests.TodoLists.Commands;

[Collection("Functional")]
public class DeleteTodoListTests : TestBase
{
    [Fact]
    public async Task ShouldRequireValidTodoListId()
    {
        var command = new DeleteTodoListCommand(99);
        await Should.ThrowAsync<NotFoundException>(() => TestApp.SendAsync(command));
    }

    [Fact]
    public async Task ShouldDeleteTodoList()
    {
        var listId = await TestApp.SendAsync<int>(new CreateTodoListCommand
        {
            Title = "New List"
        });

        await TestApp.SendAsync(new DeleteTodoListCommand(listId));

        var list = await TestApp.FindAsync<TodoList>(listId);

        list.ShouldBeNull();
    }
}
