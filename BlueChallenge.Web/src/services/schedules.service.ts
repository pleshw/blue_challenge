import { api } from './api'
import type { ISchedule, ICreateScheduleRequest, IUpdateScheduleRequest } from '@/types'

export const schedulesService = {
  async getAll(): Promise<ISchedule[]> {
    return api.get<ISchedule[]>('/schedules')
  },

  async getById(id: string): Promise<ISchedule> {
    return api.get<ISchedule>(`/schedules/${id}`)
  },

  async create(request: ICreateScheduleRequest): Promise<ISchedule> {
    return api.post<ISchedule>('/schedules', request)
  },

  async update(id: string, request: IUpdateScheduleRequest): Promise<ISchedule> {
    return api.put<ISchedule>(`/schedules/${id}`, request)
  },

  async delete(id: string): Promise<void> {
    return api.delete(`/schedules/${id}`)
  },
}
