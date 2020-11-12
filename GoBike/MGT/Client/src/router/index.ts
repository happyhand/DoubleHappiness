import store from "@/store";
import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import Home from "../views/Home.vue";
import Error404 from "../views/Error404.vue";
import SignIn from "../views/auth/SignIn.vue";

const routes: Array<RouteRecordRaw> = [
  //#region Example
  // {
  //   path: "/",
  //   name: "Home",
  //   component: Home,
  // },
  // {
  //   path: "/about",
  //   name: "About",
  //   // route level code-splitting
  //   // this generates a separate chunk (about.[hash].js) for this route
  //   // which is lazy-loaded when the route is visited.
  //   component: () =>
  //     import(/* webpackChunkName: "about" */ "../views/About.vue"),
  // },
  //#endregion
  {
    path: "/signin",
    name: "SignIn",
    component: SignIn,
    meta: { requiresAuth: false },
  },
  {
    path: "/",
    name: "Home",
    component: Home,
    meta: { requiresAuth: true },
  },
  {
    path: "/404",
    name: "Error404",
    component: Error404,
  },
  {
    path: "/:catchAll(.*)",
    name: "Error404",
    component: Error404,
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

router.beforeEach((to, from, next) => {
  // to表示要進去的那頁
  if (to.meta.requiresAuth) {
    if (store.state.isLogin) {
      next();
    } else {
      next({
        path: "/signin",
      });
    }
  } else next();
});

export default router;
