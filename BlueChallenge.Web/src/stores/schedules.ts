import { defineStore } from 'pinia'
import { ref } from 'vue'
import { schedulesService } from '@/services/schedules.service'
import type { ISchedule, ICreateScheduleRequest, IUpdateScheduleRequest } from '@/types'

export const useSchedulesStore = defineStore('schedules', () => {
  const schedules = ref<ISchedule[]>([])
  const selectedSchedule = ref<ISchedule | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchSchedules() {
    loading.value = true
    error.value = null

    try {
      schedules.value = await schedulesService.getAll()
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to fetch schedules'
    } finally {
      loading.value = false
    }
  }

  async function fetchScheduleById(id: string) {
    loading.value = true
    error.value = null

    try {
      selectedSchedule.value = await schedulesService.getById(id)
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to fetch schedule'
    } finally {
      loading.value = false
    }
  }

  async function createSchedule(request: ICreateScheduleRequest) {
    loading.value = true
    error.value = null

    try {
      const newSchedule = await schedulesService.create(request)
      schedules.value.push(newSchedule)
      return newSchedule
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to create schedule'
      throw e
    } finally {
      loading.value = false
    }
  }

  async function updateSchedule(id: string, request: IUpdateScheduleRequest) {
    loading.value = true
    error.value = null

    try {
      const updatedSchedule = await schedulesService.update(id, request)
      const index = schedules.value.findIndex((s) => s.id === id)
      if (index !== -1) {
        schedules.value[index] = updatedSchedule
      }
      return updatedSchedule
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to update schedule'
      throw e
    } finally {
      loading.value = false
    }
  }

  async function deleteSchedule(id: string) {
    loading.value = true
    error.value = null

    try {
      await schedulesService.delete(id)
      schedules.value = schedules.value.filter((s) => s.id !== id)
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to delete schedule'
      throw e
    } finally {
      loading.value = false
    }
  }

  function clearError() {
    error.value = null
  }

  return {
    schedules,
    selectedSchedule,
    loading,
    error,
    fetchSchedules,
    fetchScheduleById,
    createSchedule,
    updateSchedule,
    deleteSchedule,
    clearError,
  }
})
