import { describe, it, expect, vi, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useSchedulesStore } from '@/stores/schedules'
import { schedulesService } from '@/services/schedules.service'
import type { ISchedule, IUser } from '@/types'

vi.mock('@/services/schedules.service', () => ({
  schedulesService: {
    getAll: vi.fn(),
    getById: vi.fn(),
    create: vi.fn(),
    update: vi.fn(),
    delete: vi.fn(),
  },
}))

const mockUser: IUser = {
  id: 'user-1',
  credentials: { email: { alias: 'test', provider: 'test.com', address: 'test@test.com' }, password: 'hash' },
}

const mockSchedule: ISchedule = {
  id: 'schedule-1',
  dateRange: { start: '2025-11-26T00:00:00Z', end: '2025-11-27T00:00:00Z' },
  isAllDay: true,
  description: 'Test schedule',
  user: mockUser,
}

const mockSchedule2: ISchedule = {
  id: 'schedule-2',
  dateRange: { start: '2025-11-28T00:00:00Z', end: '2025-11-29T00:00:00Z' },
  isAllDay: false,
  hourRange: { start: '09:00:00', end: '17:00:00' },
  description: 'Test schedule 2',
  user: mockUser,
}

describe('Schedules Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('should fetch schedules successfully', async () => {
    vi.mocked(schedulesService.getAll).mockResolvedValue([mockSchedule, mockSchedule2])

    const store = useSchedulesStore()
    await store.fetchSchedules()

    expect(store.schedules).toHaveLength(2)
    expect(store.schedules[0].id).toBe('schedule-1')
    expect(store.loading).toBe(false)
    expect(store.error).toBeNull()
  })

  it('should handle fetch schedules error', async () => {
    vi.mocked(schedulesService.getAll).mockRejectedValue(new Error('Network error'))

    const store = useSchedulesStore()
    await store.fetchSchedules()

    expect(store.schedules).toHaveLength(0)
    expect(store.error).toBe('Network error')
  })

  it('should fetch schedule by id', async () => {
    vi.mocked(schedulesService.getById).mockResolvedValue(mockSchedule)

    const store = useSchedulesStore()
    await store.fetchScheduleById('schedule-1')

    expect(store.selectedSchedule).toEqual(mockSchedule)
  })

  it('should create schedule', async () => {
    vi.mocked(schedulesService.create).mockResolvedValue(mockSchedule)

    const store = useSchedulesStore()
    const result = await store.createSchedule({
      dateRange: { start: '2025-11-26T00:00:00Z', end: '2025-11-27T00:00:00Z' },
      isAllDay: true,
      description: 'Test schedule',
      userId: 'user-1',
    })

    expect(result).toEqual(mockSchedule)
    expect(store.schedules).toContainEqual(mockSchedule)
  })

  it('should update schedule', async () => {
    const updatedSchedule = { ...mockSchedule, description: 'Updated schedule' }
    vi.mocked(schedulesService.update).mockResolvedValue(updatedSchedule)

    const store = useSchedulesStore()
    store.schedules = [mockSchedule]

    const result = await store.updateSchedule('schedule-1', {
      dateRange: { start: '2025-11-26T00:00:00Z', end: '2025-11-27T00:00:00Z' },
      isAllDay: true,
      description: 'Updated schedule',
      userId: 'user-1',
    })

    expect(result).toEqual(updatedSchedule)
    expect(store.schedules[0].description).toBe('Updated schedule')
  })

  it('should delete schedule', async () => {
    vi.mocked(schedulesService.delete).mockResolvedValue()

    const store = useSchedulesStore()
    store.schedules = [mockSchedule, mockSchedule2]

    await store.deleteSchedule('schedule-1')

    expect(store.schedules).toHaveLength(1)
    expect(store.schedules[0].id).toBe('schedule-2')
  })

  it('should clear error', () => {
    const store = useSchedulesStore()
    store.error = 'Some error'

    store.clearError()

    expect(store.error).toBeNull()
  })
})
