import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../core/auth.service';
import { describeError } from '../core/describe-error';
import { TodoItemDto, TodoListDto, TodosVm } from '../core/api.models';
import { TodoApiService } from '../core/todo-api.service';

@Component({
  selector: 'app-todos',
  imports: [FormsModule],
  templateUrl: './todos.html',
  styleUrl: './todos.css',
})
export class Todos implements OnInit {
  private readonly api = inject(TodoApiService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly vm = signal<TodosVm | null>(null);
  protected readonly error = signal<string | null>(null);

  protected newListTitle = '';
  protected newListColour = '';
  /** Draft title for the "add item" field of each list, keyed by list id. */
  protected readonly drafts: Record<number, string> = {};

  ngOnInit(): void {
    this.reload();
  }

  protected reload(): void {
    this.api.getTodos().subscribe({
      next: vm => {
        this.vm.set(vm);
        this.newListColour ||= vm.colours[0]?.code ?? '';
        this.error.set(null);
      },
      error: err => this.fail(err),
    });
  }

  protected createList(): void {
    const title = this.newListTitle.trim();
    if (!title) return;

    this.api.createList({ title, colour: this.newListColour }).subscribe({
      next: () => {
        this.newListTitle = '';
        this.reload();
      },
      error: err => this.fail(err),
    });
  }

  protected deleteList(list: TodoListDto): void {
    this.api.deleteList(list.id).subscribe({
      next: () => this.reload(),
      error: err => this.fail(err),
    });
  }

  protected addItem(list: TodoListDto): void {
    const title = (this.drafts[list.id] ?? '').trim();
    if (!title) return;

    this.api.createItem({ listId: list.id, title }).subscribe({
      next: () => {
        this.drafts[list.id] = '';
        this.reload();
      },
      error: err => this.fail(err),
    });
  }

  protected toggleDone(item: TodoItemDto): void {
    this.api
      .updateItem({ id: item.id, title: item.title ?? '', done: !item.done })
      .subscribe({
        next: () => this.reload(),
        error: err => this.fail(err),
      });
  }

  protected setPriority(item: TodoItemDto, priority: number): void {
    this.api
      .updateItemDetail({ id: item.id, listId: item.listId, priority, note: item.note })
      .subscribe({
        next: () => this.reload(),
        error: err => this.fail(err),
      });
  }

  protected deleteItem(item: TodoItemDto): void {
    this.api.deleteItem(item.id).subscribe({
      next: () => this.reload(),
      error: err => this.fail(err),
    });
  }

  protected signOut(): void {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }

  private fail(err: unknown): void {
    if (err instanceof HttpErrorResponse && err.status === 401) {
      this.auth.logout();
      this.router.navigateByUrl('/login');
      return;
    }
    this.error.set(describeError(err));
  }
}
