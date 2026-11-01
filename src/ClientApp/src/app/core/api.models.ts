// Shapes mirror the DTOs and commands in src/Application of CleanArchitectureWebApi.
// ASP.NET Core serialises properties as camelCase.

export interface TokenResponse {
  tokenType: string;
  accessToken: string;
  expiresIn: number;
  refreshToken: string;
}

export interface LookupDto {
  id: number;
  title: string | null;
}

export interface ColourDto {
  code: string;
  name: string;
}

export interface TodoItemDto {
  id: number;
  listId: number;
  title: string | null;
  done: boolean;
  priority: number;
  note: string | null;
}

export interface TodoListDto {
  id: number;
  title: string | null;
  colour: string | null;
  items: TodoItemDto[];
}

export interface TodosVm {
  priorityLevels: LookupDto[];
  colours: ColourDto[];
  lists: TodoListDto[];
}

export interface CreateTodoListCommand {
  title: string;
  colour: string;
}

export interface CreateTodoItemCommand {
  listId: number;
  title: string;
}

export interface UpdateTodoItemCommand {
  id: number;
  title: string;
  done: boolean;
}

export interface UpdateTodoItemDetailCommand {
  id: number;
  listId: number;
  priority: number;
  note: string | null;
}
