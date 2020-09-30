import _ from "lodash";

export default {
  isLogin: state => {
    const token = localStorage.getItem("token");
    if (!_.isNil(token) && token.length > 0) {
      state.isLogin = true;
    }
    return state.isLogin;
  }
};
