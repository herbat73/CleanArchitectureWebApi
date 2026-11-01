using CleanArchitectureWebApi.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitectureWebApi.Application.TodoItems.Commands.UpdateTodoItem;
using CleanArchitectureWebApi.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitectureWebApi.Domain.Entities;

namespace CleanArchitectureWebApi.Application.FunctionalTests.TodoItems.Commands;

[Collection("Functional")]
public class UpdateTodoItemTests : TestBase
{
    [Fact]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new UpdateTodoItemCommand { Id = 99, Title = "New Title" };
        await Should.ThrowAsync<NotFoundException>(() => TestApp.SendAsync(command));
    }

    [Fact]
    public async Task ShouldUpdateTodoItem()
    {
        var userId = await TestApp.RunAsDefaultUserAsync();

        var listId = await TestApp.SendAsync<int>(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await TestApp.SendAsync<int>(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "New Item"
        });

        var command = new UpdateTodoItemCommand
        {
            Id = itemId,
            Title = "Updated Item Title"
        };

        await TestApp.SendAsync(command);

        var item = await TestApp.FindAsync<TodoItem>(itemId);

        item.ShouldNotBeNull();
        item!.Title.ShouldBe(command.Title);
        item.LastModifiedBy.ShouldNotBeNull();
        item.LastModifiedBy.ShouldBe(userId);
        item.LastModified.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
