<template>
  <div class="container">
    <div class="sign-box shadow">
      <div class="row">
        <div class="col">
          <h1 class="text-center font-weight-bold sign-box-logo">
            <i class="fas fa-biking"></i>
            <span>Go Bike</span>
          </h1>
        </div>
      </div>
      <form v-if="this.show" @submit="onSubmit" @reset="onReset">
        <div class="row">
          <div class="col">
            <div class="input-group">
              <div class="input-group-prepend">
                <span class="input-group-text" id="account-addon"><i class="fas fa-user sign-box-icon"></i></span>
              </div>
              <input
                id="account"
                v-model="this.form.account"
                type="text"
                class="form-control text-muted sign-box-input"
                placeholder="帳號"
                aria-label="帳號"
                aria-describedby="account-addon"
                required
              />
            </div>
          </div>
        </div>
        <div class="row">
          <div class="col">
            <div class="input-group">
              <div class="input-group-prepend">
                <span class="input-group-text" id="password-addon"><i class="fas fa-lock sign-box-icon"></i></span>
              </div>
              <input
                id="password"
                v-model="this.form.password"
                type="password"
                class="form-control text-muted sign-box-input"
                placeholder="密碼"
                aria-label="密碼"
                aria-describedby="password-addon"
                required
              />
            </div>
          </div>
        </div>

        <button type="submit" class="btn btn-info btn-block">登入</button>
        <button type="reset" class="btn btn-secondary  btn-block">重置</button>
      </form>
    </div>
  </div>
</template>

<script lang="ts">
import { Options, Vue } from "vue-class-component";
import { ApiResponse, singInUrl } from "@/model/api-module";
import ApiService from "@/service/api/api-service";
import LogService from "@/service/log/log-service";

/**
 * 登入模組
 */
@Options({
  viewInject: { apiService: ApiService },
})
export default class SignInModule extends Vue {
  apiService: ApiService | undefined;
  form: FormData = {
    account: "vincent.xie.h2@gmail.com",
    password: "a123456",
  };

  /// typescript-eslint 規則:不需要加上 boolean 型別，直接使用即可
  /// https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-inferrable-types.md
  show = true;

  onSubmit(evt: Event) {
    console.log("onSubmit:", evt);
    evt.preventDefault();
    this.apiService!.post<ApiResponse>(singInUrl, {
      email: this.form.account,
      password: this.form.password,
    }).subscribe((res) => {
      console.log("Sign In", res);
    });
  }

  onReset(evt: Event) {
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

interface FormData {
  account: string;
  password: string;
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
.sign-box {
  padding: 30px;
  border-radius: 0.5rem;
  background-color: #ffffff;
}

.sign-box-logo {
  margin-bottom: 30px;
  color: #999999;
}

.sign-box-logo i {
  margin-right: 10px;
}

.row .input-group {
  margin-bottom: 20px;
}

.sign-box-icon {
  color: #ffffff;
}

.sign-box-input {
  letter-spacing: 0.5px;
}
</style>
