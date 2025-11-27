import { api } from './api'
import type { IUser, ICreateUserRequest, IUpdateUserRequest } from '@/types'

export const usersService = {
  async getAll(): Promise<IUser[]> {
    return api.get<IUser[]>('/users')
  },

  async getById(id: string): Promise<IUser> {
    return api.get<IUser>(`/users/${id}`)
  },

  async create(request: ICreateUserRequest): Promise<IUser> {
    return api.post<IUser>('/users', request)
  },

  async update(id: string, request: IUpdateUserRequest): Promise<IUser> {
    return api.put<IUser>(`/users/${id}`, request)
  },

  async delete(id: string): Promise<void> {
    return api.delete(`/users/${id}`)
  },
}
