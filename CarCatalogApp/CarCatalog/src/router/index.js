import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'Catalog',
      component: () => import('@/views/Catalog.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/login',
      name: 'Login',
      component: () => import('@/views/Login.vue'),
      meta: { guestOnly: true }
    },
    {
      path: '/register',
      name: 'Register',
      component: () => import('@/views/Register.vue'),
      meta: { guestOnly: true }
    },
    {
      path: '/car/:id',
      name: 'CarDetails',
      component: () => import('@/views/CarDetails.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/admin',
      name: 'AdminPanel',
      component: () => import('@/views/AdminPanel.vue'),
      meta: { requiresAuth: true, requiresAdmin: true }
    },
    {
      path: '/admin/cars/new',
      name: 'car-form-new',
      component: () => import('@/views/CarFormView.vue'),
      meta: { requiresAuth: true, requiresAdmin: true }
    },
    {
      path: '/admin/cars/:id/edit',
      name: 'car-form-edit',
      component: () => import('@/views/CarFormView.vue'),
      props: true,
      meta: { requiresAuth: true, requiresAdmin: true }
    },
    {
      path: '/profile',
      name: 'profile',
      component: () => import('@/views/ProfilePage.vue'),
      meta: { requiresAuth: true },
      props: (route) => ({ 
        tab: route.query.tab || 'personal' 
      })
    }
  ]
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
  } else if (to.meta.requiresAdmin && !authStore.isAdmin) {
    next('/')
  } else if (to.meta.guestOnly && authStore.isAuthenticated) {
    next('/')
  } else {
    next()
  }
})

export default router