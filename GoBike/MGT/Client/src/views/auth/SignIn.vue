<template>
  <div class="container-fluid sign-in-content">
    <div class="row justify-content-md-center">
      <div class="col-sm-4">
        <SignInModule />
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import SignInModule from "@/components/auth/SignInModule.vue";
import { Vue, Options } from "vue-class-component";

@Options({
  components: {
    SignInModule,
  },
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
