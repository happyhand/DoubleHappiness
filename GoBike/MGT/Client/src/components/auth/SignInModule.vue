<template>
  <div class="sign-box shadow">
    <b-row>
      <b-col>
        <h1 class="text-center font-weight-bold sign-box-label">
          <font-awesome-icon :icon="['fas', 'biking']" />
          <span>Go Bike</span>
        </h1>
      </b-col>
    </b-row>
    <b-form @submit="onSubmit" @reset="onReset" v-if="show">
      <b-row class="sign-box-row">
        <b-col>
          <b-input-group>
            <b-input-group-prepend is-text>
              <font-awesome-icon
                :icon="['fas', 'user']"
                size="lg"
                class="sign-box-icon"
              />
            </b-input-group-prepend>
            <b-form-input
              id="account"
              v-model="form.account"
              required
              placeholder="帳號"
              class="text-muted sign-box-input"
            ></b-form-input>
          </b-input-group>
        </b-col>
      </b-row>
      <b-row class="sign-box-row">
        <b-col>
          <b-input-group>
            <b-input-group-prepend is-text>
              <font-awesome-icon
                :icon="['fas', 'lock']"
                size="lg"
                class="sign-box-icon"
              />
            </b-input-group-prepend>
            <b-form-input
              id="password"
              type="password"
              v-model="form.password"
              required
              placeholder="密碼"
              class="text-muted sign-box-input"
            ></b-form-input>
          </b-input-group>
        </b-col>
      </b-row>

      <b-button type="submit" block variant="info">登入</b-button>
      <b-button type="reset" block>重置</b-button>
    </b-form>
  </div>
</template>

<script>
import Vue from "vue";
import Component from "vue-class-component";
import { Inject } from "@vue-ioc/core";
import { ApiService } from "../../service/api-service";

@Component
export default class SignInModule extends Vue {
  @Inject()
  apiService;

  data() {
    return {
      form: {
        account: "",
        password: ""
      },
      show: true
    };
  }
  onSubmit(evt) {
    evt.preventDefault();
    alert(JSON.stringify(this.form));

    this.$http
      .post("http://apigobike.ddns.net:18596/api/Member/Login", {
        email: this.form.account,
        password: this.form.password
      })
      .then(response => {});
  }
  onReset(evt) {
    evt.preventDefault();
    // Reset our form values
    this.form.account = "";
    this.form.password = "";
    // Trick to reset/clear native browser form validation state
    this.show = false;
    this.$nextTick(() => {
      this.show = true;
    });
  }
}

// export default {
//   data() {
//     return {
//       form: {
//         account: "",
//         password: ""
//       },
//       show: true
//     };
//   },
//   methods: {
//     onSubmit(evt) {
//       evt.preventDefault();
//       alert(JSON.stringify(this.form));

//       this.$http
//         .post("http://apigobike.ddns.net:18596/api/Member/Login", {
//           email: this.form.account,
//           password: this.form.password
//         })
//         .then(response => {});
//     },
//     onReset(evt) {
//       evt.preventDefault();
//       // Reset our form values
//       this.form.account = "";
//       this.form.password = "";
//       // Trick to reset/clear native browser form validation state
//       this.show = false;
//       this.$nextTick(() => {
//         this.show = true;
//       });
//     }
//   }
// };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
.sign-box {
  padding: 2rem;
  border-radius: 0.5rem;
  background-color: #ffffff;
}

.sign-box-label {
  margin-bottom: 2rem;
  color: #999999;
}

.sign-box-row {
  margin-bottom: 1.5rem;
}

.sign-box-icon {
  color: #ffffff;
}

.sign-box-input {
  letter-spacing: 0.1rem;
}
</style>
