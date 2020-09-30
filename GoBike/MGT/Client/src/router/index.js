import Vue from "vue";
import Error404 from "../views/Error404.vue";
import Router from "vue-router";
import Home from "../views/Home.vue";
import SignIn from "../views/auth/SignIn.vue";
import store from "../store";

Vue.use(Router);

const routes = [
  {
    path: "/signin",
    name: "SignIn",
    component: SignIn,
    meta: { requiresAuth: false }
  },
  {
    path: "/",
    name: "Home",
    component: Home,
    meta: { requiresAuth: true }
  },
  {
    path: "/404",
    name: "Error404",
    component: Error404
  },
  {
    path: "*",
    redirect: "/404"
  }
];

const router = new Router({
  mode: "history",
  base: process.env.BASE_URL,
  // scrollBehavior(to, from, savedPosition) {
  //   return { x: 0, y: 0 };
  // },
  routes: routes
});

router.beforeEach((to, from, next) => {
  // to表示要進去的那頁
  if (to.meta.requiresAuth) {
    // TODO
    if (store.getters.isLogin) {
      next();
    } else {
      next({
        path: "/signin"
      });
    }
  } else next();
});

export default router;
