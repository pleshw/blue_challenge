<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useSchedulesStore } from '@/stores/schedules'
import { useUsersStore } from '@/stores/users'
import Card from 'primevue/card'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Tag from 'primevue/tag'
import { useRouter } from 'vue-router'

const schedulesStore = useSchedulesStore()
const usersStore = useUsersStore()
const router = useRouter()

onMounted(() => {
  schedulesStore.fetchSchedules()
  usersStore.fetchUsers()
})

const totalSchedules = computed(() => schedulesStore.schedules.length)
const totalUsers = computed(() => usersStore.users.length)

const today = computed(() => {
  const now = new Date()
  return new Date(now.getFullYear(), now.getMonth(), now.getDate()).getTime()
})

// Agendamentos futuros e de hoje (ordenados por data crescente)
const upcomingSchedules = computed(() => {
  return schedulesStore.schedules
    .filter((s) => {
      const endDate = new Date(s.dateRange.end)
      const endDateTime = new Date(endDate.getFullYear(), endDate.getMonth(), endDate.getDate()).getTime()
      return endDateTime >= today.value
    })
    .sort((a, b) => new Date(a.dateRange.start).getTime() - new Date(b.dateRange.start).getTime())
})

// Agendamentos passados (ordenados por data decrescente - mais recentes primeiro)
const pastSchedules = computed(() => {
  return schedulesStore.schedules
    .filter((s) => {
      const endDate = new Date(s.dateRange.end)
      const endDateTime = new Date(endDate.getFullYear(), endDate.getMonth(), endDate.getDate()).getTime()
      return endDateTime < today.value
    })
    .sort((a, b) => new Date(b.dateRange.end).getTime() - new Date(a.dateRange.end).getTime())
})

function isToday(schedule: { dateRange: { start: string; end: string } }): boolean {
  const startDate = new Date(schedule.dateRange.start)
  const endDate = new Date(schedule.dateRange.end)
  const startDateTime = new Date(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()).getTime()
  const endDateTime = new Date(endDate.getFullYear(), endDate.getMonth(), endDate.getDate()).getTime()
  return startDateTime <= today.value && endDateTime >= today.value
}

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('pt-BR')
}

function getRowClass(data: { dateRange: { start: string; end: string } }): string {
  return isToday(data) ? 'today-row' : ''
}
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
    </div>

    <div class="action-wrapper">
      <Card class="action-card" @click="router.push('/schedules')">
        <template #content>
          <div class="action-content">
            <span class="action-text">Novo Agendamento</span>
          </div>
        </template>
      </Card>
    </div>

    <Card class="schedules-card">
      <template #title>
        <div class="section-header">
          <i class="pi pi-calendar-plus"></i>
          <span>Próximos Agendamentos</span>
          <Tag v-if="upcomingSchedules.length > 0" :value="upcomingSchedules.length.toString()" severity="info" />
        </div>
      </template>
      <template #content>
        <DataTable
          :value="upcomingSchedules"
          :rowClass="getRowClass"
          stripedRows
          showGridlines
          :rows="5"
          paginator
          :rowsPerPageOptions="[5, 10, 20]"
          tableStyle="min-width: 50rem"
        >
          <template #empty>
            <div class="empty-message">
              <i class="pi pi-calendar"></i>
              <p>Nenhum agendamento próximo</p>
            </div>
          </template>

          <Column header="Status" style="width: 100px">
            <template #body="{ data }">
              <Tag v-if="isToday(data)" value="Hoje" severity="warn" />
              <Tag v-else value="Futuro" severity="info" />
            </template>
          </Column>

          <Column field="description" header="Descrição" style="width: 30%"></Column>

          <Column header="Período" style="width: 25%">
            <template #body="{ data }">
              <span>{{ formatDate(data.dateRange.start) }} - {{ formatDate(data.dateRange.end) }}</span>
            </template>
          </Column>

          <Column header="Horário" style="width: 15%">
            <template #body="{ data }">
              <Tag v-if="data.isAllDay" value="Dia todo" severity="secondary" />
              <span v-else-if="data.hourRange">
                {{ data.hourRange.start.substring(0, 5) }} - {{ data.hourRange.end.substring(0, 5) }}
              </span>
            </template>
          </Column>

          <Column header="Usuário" style="width: 20%">
            <template #body="{ data }">
              <span>{{ data.user.credentials.email.address }}</span>
            </template>
          </Column>
        </DataTable>
      </template>
    </Card>

    <Card class="schedules-card past-schedules">
      <template #title>
        <div class="section-header">
          <i class="pi pi-history"></i>
          <span>Agendamentos Passados</span>
          <Tag v-if="pastSchedules.length > 0" :value="pastSchedules.length.toString()" severity="secondary" />
        </div>
      </template>
      <template #content>
        <DataTable
          :value="pastSchedules"
          stripedRows
          showGridlines
          :rows="5"
          paginator
          :rowsPerPageOptions="[5, 10, 20]"
          tableStyle="min-width: 50rem"
        >
          <template #empty>
            <div class="empty-message">
              <i class="pi pi-check-circle"></i>
              <p>Nenhum agendamento passado</p>
            </div>
          </template>

          <Column field="description" header="Descrição" style="width: 35%"></Column>

          <Column header="Período" style="width: 25%">
            <template #body="{ data }">
              <span>{{ formatDate(data.dateRange.start) }} - {{ formatDate(data.dateRange.end) }}</span>
            </template>
          </Column>

          <Column header="Horário" style="width: 15%">
            <template #body="{ data }">
              <Tag v-if="data.isAllDay" value="Dia todo" severity="secondary" />
              <span v-else-if="data.hourRange">
                {{ data.hourRange.start.substring(0, 5) }} - {{ data.hourRange.end.substring(0, 5) }}
              </span>
            </template>
          </Column>

          <Column header="Usuário" style="width: 25%">
            <template #body="{ data }">
              <span>{{ data.user.credentials.email.address }}</span>
            </template>
          </Column>
        </DataTable>
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

.schedules-card {
  margin-bottom: 1.5rem;
}

.section-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.section-header i {
  color: var(--p-primary-500);
}

.past-schedules {
  opacity: 0.85;
}

.empty-message {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  color: var(--p-text-muted-color);
}

.empty-message i {
  font-size: 2rem;
  margin-bottom: 0.5rem;
}

:deep(.today-row) {
  background-color: var(--p-highlight-background) !important;
  font-weight: 600;
}

:deep(.today-row td) {
  background-color: var(--p-highlight-background) !important;
}

.action-wrapper {
  display: flex;
  justify-content: center;
  margin-bottom: 2rem;
}

.action-card {
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
  background: linear-gradient(135deg, var(--p-yellow-500) 0%, var(--p-orange-500) 50%, var(--p-red-400) 100%) !important;
  border: 3px solid var(--p-surface-0) !important;
  box-shadow: 0 4px 15px color-mix(in srgb, var(--p-yellow-500) 40%, transparent);
  min-width: 300px;
}

.action-card:hover {
  transform: translateY(-4px) scale(1.02);
  box-shadow: 0 8px 30px color-mix(in srgb, var(--p-orange-500) 50%, transparent);
}

.action-content {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 60px;
}

.action-text {
  font-size: 1.5rem;
  font-weight: 700;
  color: white;
  text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.2);
}
</style>
