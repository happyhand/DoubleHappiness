import "bootstrap";
import "bootstrap/dist/css/bootstrap.min.css";
import "@fortawesome/fontawesome-free/css/all.min.css";
import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import VueAxios from "vue-axios";
import Axios from "axios";
import IocService from "./service/ioc/ioc-service";
import iocPlugin from "./plugin/ioc/ioc-plugin";

createApp(App)
  .use(store)
  .use(router)
  .use(VueAxios, Axios)
  .use(iocPlugin, new IocService())
  .mount("#app");
