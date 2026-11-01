import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';
import { Login } from './login/login';
import { Todos } from './todos/todos';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: '', component: Todos, canActivate: [authGuard] },
  { path: '**', redirectTo: '' },
];
