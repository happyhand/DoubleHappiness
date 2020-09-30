import Vue from "vue";
import Vuex from "vuex";

import state from "@/store/state.js";
import getters from "@/store/getters.js";

Vue.use(Vuex);

export default new Vuex.Store({
  state,
  getters
});
