// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from "vue";
import App from "./App";
import router from "./router";
import store from "./store";
import { BootstrapVue } from "bootstrap-vue";
import { library } from "@fortawesome/fontawesome-svg-core";
import { faUser, faLock, faBiking } from "@fortawesome/free-solid-svg-icons";

import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import "bootstrap/dist/css/bootstrap.css";
import "bootstrap-vue/dist/bootstrap-vue.css";
import VueAxios from "vue-axios";
import Axios from "axios";
import { VueIocPlugin } from "@vue-ioc/core";

// Install BootstrapVue
Vue.use(BootstrapVue);
// Install VueAxios、Axios
Vue.use(VueAxios, Axios);
// Install VueIocPlugin
Vue.use(VueIocPlugin);

// fortawesome
library.add(faUser, faLock, faBiking);
Vue.component("font-awesome-icon", FontAwesomeIcon);

Vue.config.productionTip = false;

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount("#app");
