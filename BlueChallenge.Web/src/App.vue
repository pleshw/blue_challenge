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
</style>
