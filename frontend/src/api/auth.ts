import http from './http';
import type { AuthResponse, LoginRequest, RegisterRequest } from '../types/types';

export const login = (data: LoginRequest) =>
  http.post<AuthResponse>('/api/auth/login', data).then((r) => r.data);

export const register = (data: RegisterRequest) =>
  http.post<AuthResponse>('/api/auth/register', data).then((r) => r.data);
