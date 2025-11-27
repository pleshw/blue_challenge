<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import AppLayout from '@/components/AppLayout.vue'
import Toast from 'primevue/toast'

const route = useRoute()

const showLayout = computed(() => route.path !== '/login')
</script>

<template>
  <Toast />
  <AppLayout v-if="showLayout">
    <RouterView />
  </AppLayout>
  <RouterView v-else />
</template>

<style>
@import 'primeicons/primeicons.css';

* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: var(--p-font-family);
  background: var(--p-surface-50);
  color: var(--p-text-color);
}

.w-full {
  width: 100%;
}

/* Botões quadrados */
.p-button {
  border-radius: 0 !important;
}

.p-card {
  border-radius: 0 !important;
}

.p-datatable .p-datatable-thead > tr > th,
.p-datatable .p-datatable-tbody > tr > td {
  border-radius: 0 !important;
}

.p-dialog {
  border-radius: 0 !important;
}

.p-dialog .p-dialog-header {
  border-radius: 0 !important;
}

.p-dialog .p-dialog-content {
  border-radius: 0 !important;
}

.p-dialog .p-dialog-footer {
  border-radius: 0 !important;
}

.p-inputtext,
.p-select,
.p-textarea,
.p-datepicker {
  border-radius: 0 !important;
}

.p-tag {
  border-radius: 0 !important;
}

/* Paginators */
.p-paginator {
  border-radius: 0 !important;
}

.p-paginator .p-paginator-first,
.p-paginator .p-paginator-prev,
.p-paginator .p-paginator-next,
.p-paginator .p-paginator-last,
.p-paginator .p-paginator-page {
  border-radius: 0 !important;
}

.p-paginator .p-dropdown {
  border-radius: 0 !important;
}

.p-paginator .p-inputtext {
  border-radius: 0 !important;
}
</style>
