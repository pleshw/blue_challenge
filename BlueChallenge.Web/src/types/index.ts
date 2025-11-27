// User types
export interface IEmail {
  alias: string
  provider: string
  address: string
}

export interface IUserCredentials {
  email: IEmail
  password: string
}

export interface IUserPersonalInfo {
  firstName?: string
  lastName?: string
  phone?: string
}

export interface IUser {
  id: string
  credentials: IUserCredentials
  personalInfo?: IUserPersonalInfo
}

// Schedule types
export interface IDateRange {
  start: string
  end: string
}

export interface IHourRange {
  start: string
  end: string
}

export interface ISchedule {
  id: string
  dateRange: IDateRange
  isAllDay: boolean
  hourRange?: IHourRange
  description: string
  user: IUser
}

// Request types
export interface ILoginRequest {
  email: string
  password: string
}

export interface ILoginResponse {
  accessToken: string
  expiresAtUtc: string
}

export interface ICreateUserRequest {
  email: string
  password: string
}

export interface IUpdateUserRequest {
  email: string
  password: string
}

export interface IDateRangeRequest {
  start: string
  end: string
}

export interface IHourRangeRequest {
  start: string
  end: string
}

export interface ICreateScheduleRequest {
  dateRange: IDateRangeRequest
  isAllDay: boolean
  hourRange?: IHourRangeRequest
  description: string
  userId: string
}

export interface IUpdateScheduleRequest {
  dateRange: IDateRangeRequest
  isAllDay: boolean
  hourRange?: IHourRangeRequest
  description: string
  userId: string
}
