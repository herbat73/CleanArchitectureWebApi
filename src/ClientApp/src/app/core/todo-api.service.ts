import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from './api.config';
import {
  CreateTodoItemCommand,
  CreateTodoListCommand,
  TodosVm,
  UpdateTodoItemCommand,
  UpdateTodoItemDetailCommand,
} from './api.models';

/** Typed wrapper around the TodoLists and TodoItems endpoints of the WebApi. */
@Injectable({ providedIn: 'root' })
export class TodoApiService {
  private readonly http = inject(HttpClient);

  getTodos(): Observable<TodosVm> {
    return this.http.get<TodosVm>(`${API_BASE_URL}/TodoLists`);
  }

  createList(command: CreateTodoListCommand): Observable<number> {
    return this.http.post<number>(`${API_BASE_URL}/TodoLists`, command);
  }

  deleteList(id: number): Observable<void> {
    return this.http.delete<void>(`${API_BASE_URL}/TodoLists/${id}`);
  }

  createItem(command: CreateTodoItemCommand): Observable<number> {
    return this.http.post<number>(`${API_BASE_URL}/TodoItems`, command);
  }

  updateItem(command: UpdateTodoItemCommand): Observable<void> {
    return this.http.put<void>(`${API_BASE_URL}/TodoItems/${command.id}`, command);
  }

  updateItemDetail(command: UpdateTodoItemDetailCommand): Observable<void> {
    return this.http.patch<void>(`${API_BASE_URL}/TodoItems/UpdateDetail/${command.id}`, command);
  }

  deleteItem(id: number): Observable<void> {
    return this.http.delete<void>(`${API_BASE_URL}/TodoItems/${id}`);
  }
}
