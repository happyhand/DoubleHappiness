<template>
  <b-container fluid class="sign-in-content">
    <b-row align-h="center">
      <b-col sm="3">
        <UserSignIn />
      </b-col>
    </b-row>
  </b-container>
</template>

<script>
import UserSignIn from "@/components/auth/SignInBox.vue";

export default {
  name: "SignIn",
  components: {
    UserSignIn,
  },
};
</script>

<style scoped>
.sign-in-content {
  padding-top: 15rem;
  background-color: #eeeeee;
  height: 100vh;
}
</style>
