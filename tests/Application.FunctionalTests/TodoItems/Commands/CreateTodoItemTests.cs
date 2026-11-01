using CleanArchietureNoFrontEnd.Application.Common.Exceptions;
using CleanArchietureNoFrontEnd.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchietureNoFrontEnd.Application.TodoLists.Commands.CreateTodoList;
using CleanArchietureNoFrontEnd.Domain.Entities;

namespace CleanArchietureNoFrontEnd.Application.FunctionalTests.TodoItems.Commands;

[Collection("Functional")]
public class CreateTodoItemTests : TestBase
{
    [Fact]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateTodoItemCommand();

        await Should.ThrowAsync<ValidationException>(() => TestApp.SendAsync(command));
    }

    [Fact]
    public async Task ShouldCreateTodoItem()
    {
        var userId = await TestApp.RunAsDefaultUserAsync();

        var listId = await TestApp.SendAsync<int>(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var command = new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Tasks"
        };

        var itemId = await TestApp.SendAsync<int>(command);

        var item = await TestApp.FindAsync<TodoItem>(itemId);

        item.ShouldNotBeNull();
        item!.ListId.ShouldBe(command.ListId);
        item.Title.ShouldBe(command.Title);
        item.CreatedBy.ShouldBe(userId);
        item.Created.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
        item.LastModifiedBy.ShouldBe(userId);
        item.LastModified.ShouldBe(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
