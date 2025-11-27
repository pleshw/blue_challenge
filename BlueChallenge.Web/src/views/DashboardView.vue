<script setup lang="ts">
import { computed } from 'vue'
import { useSchedulesStore } from '@/stores/schedules'
import { useUsersStore } from '@/stores/users'
import Card from 'primevue/card'

const schedulesStore = useSchedulesStore()
const usersStore = useUsersStore()

const totalSchedules = computed(() => schedulesStore.schedules.length)
const totalUsers = computed(() => usersStore.users.length)
const todaySchedules = computed(() => {
  const today: string = new Date().toISOString().split('T')[0] ?? ''
  return schedulesStore.schedules.filter((s) => {
    const startDate: string = s.dateRange.start.split('T')[0] ?? ''
    const endDate: string = s.dateRange.end.split('T')[0] ?? ''
    return startDate <= today && endDate >= today
  }).length
})
</script>

<template>
  <div class="dashboard">
    <h1>Dashboard</h1>

    <div class="stats-grid">
      <Card class="stat-card">
        <template #content>
          <div class="stat-content">
            <i class="pi pi-users stat-icon"></i>
            <div class="stat-info">
              <span class="stat-value">{{ totalUsers }}</span>
              <span class="stat-label">Usuários</span>
            </div>
          </div>
        </template>
      </Card>

      <Card class="stat-card">
        <template #content>
          <div class="stat-content">
            <i class="pi pi-calendar stat-icon"></i>
            <div class="stat-info">
              <span class="stat-value">{{ totalSchedules }}</span>
              <span class="stat-label">Agendamentos</span>
            </div>
          </div>
        </template>
      </Card>

      <Card class="stat-card">
        <template #content>
          <div class="stat-content">
            <i class="pi pi-clock stat-icon"></i>
            <div class="stat-info">
              <span class="stat-value">{{ todaySchedules }}</span>
              <span class="stat-label">Hoje</span>
            </div>
          </div>
        </template>
      </Card>
    </div>

    <Card class="welcome-card">
      <template #title>Bem-vindo ao Blue Challenge</template>
      <template #content>
        <p>
          Este sistema permite gerenciar usuários e agendamentos de forma simples e eficiente.
          Utilize o menu lateral para navegar entre as funcionalidades disponíveis.
        </p>
        <ul>
          <li><strong>Usuários:</strong> Gerencie os usuários do sistema</li>
          <li><strong>Agendamentos:</strong> Crie e gerencie agendamentos</li>
        </ul>
      </template>
    </Card>
  </div>
</template>

<style scoped>
.dashboard {
  padding: 1rem;
}

.dashboard h1 {
  margin-bottom: 1.5rem;
  color: var(--p-text-color);
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.stat-card {
  background: linear-gradient(135deg, var(--p-primary-500) 0%, var(--p-primary-700) 100%);
  color: white;
}

.stat-content {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.stat-icon {
  font-size: 2.5rem;
  opacity: 0.8;
}

.stat-info {
  display: flex;
  flex-direction: column;
}

.stat-value {
  font-size: 2rem;
  font-weight: bold;
}

.stat-label {
  font-size: 0.875rem;
  opacity: 0.9;
}

.welcome-card ul {
  margin-top: 1rem;
  padding-left: 1.5rem;
}

.welcome-card li {
  margin-bottom: 0.5rem;
}
</style>
