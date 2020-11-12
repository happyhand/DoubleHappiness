import { Type } from "@/injectable/injectable-interface";
import IocService from "@/service/ioc/ioc-service";
import { App } from "vue";

/**
 * 依賴注入套件
 */
export default {
  install(Vue: App, iocService: IocService) {
    Vue.mixin({
      beforeCreate() {
        const { viewInject } = this.$options;
        if (viewInject) {
          const injects = viewInject;
          for (const name in injects) {
            this[name] = iocService.getProvider(injects[name] as Type<any>);
          }
        }
      },
    });
  },
};
