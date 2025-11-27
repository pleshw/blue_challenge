<script setup lang="ts">
import { computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import Button from 'primevue/button'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

interface MenuItem {
  label: string
  icon: string
  to: string
}

const menuItems: MenuItem[] = [
  { label: 'Dashboard', icon: 'pi pi-home', to: '/dashboard' },
  { label: 'Usuários', icon: 'pi pi-users', to: '/users' },
  { label: 'Agendamentos', icon: 'pi pi-calendar', to: '/schedules' },
]

const isActive = (path: string) => computed(() => route.path === path)

function navigateTo(path: string) {
  router.push(path)
}

function handleLogout() {
  authStore.logout()
  router.push('/login')
}
</script>

<template>
  <aside class="sidebar">
    <div class="sidebar-header">
      <i class="pi pi-calendar-plus text-3xl"></i>
      <span class="sidebar-title">Blue Challenge</span>
    </div>

    <nav class="sidebar-nav">
      <ul>
        <li v-for="item in menuItems" :key="item.to">
          <button
            class="nav-item"
            :class="{ active: isActive(item.to).value }"
            @click="navigateTo(item.to)"
          >
            <i :class="item.icon"></i>
            <span>{{ item.label }}</span>
          </button>
        </li>
      </ul>
    </nav>

    <div class="sidebar-footer">
      <Button
        label="Sair"
        icon="pi pi-sign-out"
        severity="secondary"
        text
        class="logout-btn"
        @click="handleLogout"
      />
    </div>
  </aside>
</template>

<style scoped>
.sidebar {
  width: 250px;
  height: 100vh;
  background: var(--p-surface-0);
  border-right: 1px solid var(--p-surface-200);
  display: flex;
  flex-direction: column;
  position: fixed;
  left: 0;
  top: 0;
}

.sidebar-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1.5rem;
  border-bottom: 1px solid var(--p-surface-200);
  color: var(--p-primary-color);
}

.sidebar-title {
  font-size: 1.25rem;
  font-weight: 600;
}

.sidebar-nav {
  flex: 1;
  padding: 1rem 0;
  overflow-y: auto;
}

.sidebar-nav ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  width: 100%;
  padding: 0.875rem 1.5rem;
  border: none;
  background: transparent;
  cursor: pointer;
  color: var(--p-text-color);
  font-size: 0.95rem;
  transition: all 0.2s;
  text-align: left;
}

.nav-item:hover {
  background: var(--p-surface-100);
}

.nav-item.active {
  background: var(--p-primary-100);
  color: var(--p-primary-color);
  font-weight: 500;
  border-left: 3px solid var(--p-primary-color);
}

.nav-item i {
  font-size: 1.1rem;
  width: 1.5rem;
}

.sidebar-footer {
  padding: 1rem;
  border-top: 1px solid var(--p-surface-200);
}

.logout-btn {
  width: 100%;
  justify-content: flex-start;
}
</style>
