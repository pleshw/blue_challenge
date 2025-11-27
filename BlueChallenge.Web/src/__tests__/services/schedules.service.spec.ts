import { describe, it, expect, vi, beforeEach } from 'vitest'
import { schedulesService } from '@/services/schedules.service'
import { api } from '@/services/api'
import type { ISchedule, IUser } from '@/types'

vi.mock('@/services/api', () => ({
  api: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
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

describe('Schedules Service', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should get all schedules', async () => {
    vi.mocked(api.get).mockResolvedValue([mockSchedule])

    const result = await schedulesService.getAll()

    expect(api.get).toHaveBeenCalledWith('/schedules')
    expect(result).toEqual([mockSchedule])
  })

  it('should get schedule by id', async () => {
    vi.mocked(api.get).mockResolvedValue(mockSchedule)

    const result = await schedulesService.getById('schedule-1')

    expect(api.get).toHaveBeenCalledWith('/schedules/schedule-1')
    expect(result).toEqual(mockSchedule)
  })

  it('should create schedule', async () => {
    vi.mocked(api.post).mockResolvedValue(mockSchedule)

    const request = {
      dateRange: { start: '2025-11-26T00:00:00Z', end: '2025-11-27T00:00:00Z' },
      isAllDay: true,
      description: 'Test schedule',
      userId: 'user-1',
    }
    const result = await schedulesService.create(request)

    expect(api.post).toHaveBeenCalledWith('/schedules', request)
    expect(result).toEqual(mockSchedule)
  })

  it('should update schedule', async () => {
    vi.mocked(api.put).mockResolvedValue(mockSchedule)

    const request = {
      dateRange: { start: '2025-11-26T00:00:00Z', end: '2025-11-27T00:00:00Z' },
      isAllDay: true,
      description: 'Test schedule',
      userId: 'user-1',
    }
    const result = await schedulesService.update('schedule-1', request)

    expect(api.put).toHaveBeenCalledWith('/schedules/schedule-1', request)
    expect(result).toEqual(mockSchedule)
  })

  it('should delete schedule', async () => {
    vi.mocked(api.delete).mockResolvedValue(undefined)

    await schedulesService.delete('schedule-1')

    expect(api.delete).toHaveBeenCalledWith('/schedules/schedule-1')
  })
})
