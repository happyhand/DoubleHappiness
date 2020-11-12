import { State } from "./state";

export default {
  login(state: State, token: string): void {
    state.isLogin = true;
    localStorage.setItem("token", token);
  },
};
