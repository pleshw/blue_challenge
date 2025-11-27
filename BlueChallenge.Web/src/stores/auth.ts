import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authService } from '@/services/auth.service'
import type { ILoginRequest } from '@/types'

export const useAuthStore = defineStore('auth', () => {
  const isAuthenticated = ref(authService.isAuthenticated())
  const loading = ref(false)
  const error = ref<string | null>(null)

  const isLoggedIn = computed(() => isAuthenticated.value)

  async function login(credentials: ILoginRequest) {
    loading.value = true
    error.value = null

    try {
      await authService.login(credentials)
      isAuthenticated.value = true
      return true
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Login failed'
      return false
    } finally {
      loading.value = false
    }
  }

  function logout() {
    authService.logout()
    isAuthenticated.value = false
  }

  function checkAuth() {
    isAuthenticated.value = authService.isAuthenticated()
  }

  return {
    isAuthenticated,
    loading,
    error,
    isLoggedIn,
    login,
    logout,
    checkAuth,
  }
})
