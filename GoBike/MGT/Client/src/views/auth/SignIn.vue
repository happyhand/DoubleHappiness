<template>
  <b-container fluid class="sign-in-content">
    <b-row align-h="center">
      <b-col sm="3">
        <SignInModule />
      </b-col>
    </b-row>
  </b-container>
</template>

<script>
import Vue from "vue";
import Component from "vue-class-component";
import { Module } from "@vue-ioc/core";
import SignInModule from "@/components/auth/SignInModule.vue";
import { ApiService } from "../../service/api-service";

@Module({
  providers: [ApiService]
})
@Component({
  components: {
    SignInModule
  }
})
export default class SignIn extends Vue {}
</script>

<style scoped>
.sign-in-content {
  padding-top: 15rem;
  background-color: #eeeeee;
  height: 100vh;
}
</style>
