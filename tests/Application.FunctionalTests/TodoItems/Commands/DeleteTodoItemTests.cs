using CleanArchitectureWebApi.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitectureWebApi.Application.TodoItems.Commands.DeleteTodoItem;
using CleanArchitectureWebApi.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitectureWebApi.Domain.Entities;

namespace CleanArchitectureWebApi.Application.FunctionalTests.TodoItems.Commands;

[Collection("Functional")]
public class DeleteTodoItemTests : TestBase
{
    [Fact]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new DeleteTodoItemCommand(99);

        await Should.ThrowAsync<NotFoundException>(() => TestApp.SendAsync(command));
    }

    [Fact]
    public async Task ShouldDeleteTodoItem()
    {
        var listId = await TestApp.SendAsync<int>(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await TestApp.SendAsync<int>(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "New Item"
        });

        await TestApp.SendAsync(new DeleteTodoItemCommand(itemId));

        var item = await TestApp.FindAsync<TodoItem>(itemId);

        item.ShouldBeNull();
    }
}
