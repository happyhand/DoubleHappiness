import { createStore } from "vuex";
import Getters from "./getters";
import State from "./state";
export default createStore({
  state: State, /// 狀態參數
  getters: Getters, /// 用於呼叫並處理 state 參數
  mutations: {}, /// 用於更改 state 參數 (需用於同步操作)
  actions: {}, /// 用於呼叫 mutations 進行更改 state 參數 (可用於非同步操作)
  modules: {}, /// 用於切割各類型不同的 state
});
