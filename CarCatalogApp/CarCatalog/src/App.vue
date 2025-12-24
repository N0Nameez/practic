<script setup>
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import NavBar from '@/components/NavBar.vue'
import NotificationContainer from '@/components/NotificationContainer.vue'

const authStore = useAuthStore()
const isAuthenticated = computed(() => authStore.isAuthenticated)
</script>

<template>
  <div id="app">
    <NavBar v-if="isAuthenticated" />
    
    <main :class="{ 'with-navbar': isAuthenticated }">
      <div class="page-container">
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </div>
    </main>
    
    <NotificationContainer />
  </div>
</template>

<style scoped>
#app {
  top: 0;
  left: 0;
  min-height: 100vh;
  background: linear-gradient(135deg, var(--tts-gray-50) 0%, var(--tts-gray-100) 100%);
}

main {
  transition: padding-top 0.3s ease;
}

main.with-navbar {
  padding-top: var(--tts-navbar-height);
  min-height: calc(100vh - var(--tts-navbar-height));
}

.page-container {
  max-width: 1440px;
  margin: 0 auto;
  padding: 1.5rem;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>