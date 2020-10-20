<template>
  <div id="app">
    <router-view />
  </div>
</template>

<script>
import Vue from "vue";
import Component from "vue-class-component";
import { Module } from "@vue-ioc/core";
// @Module({
//   providers: [{ provide: ApiService, useValue: apiService }]
// })
// @Component({
//   components: { SignInModule }
// })
// export default class App extends Vue {}
// export default {
//   name: "App"
// };

@Component
export default class App extends Vue {}
</script>

<style>
#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  width: 100vw;
  height: 100vh;
}
</style>
