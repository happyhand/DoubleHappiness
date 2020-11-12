import { INJECTED } from "@/injectable/injectable";
import { Token, Type } from "@/injectable/injectable-interface";
import { IocInterface } from "./ioc-interface";

/**
 * Ioc 依賴注入服務
 */
export default class IocService implements IocInterface {
  /// 服務對照表
  private providerMap: Map<Token<any>, any> = new Map<Token<any>, any>();

  /**
   * 新增依賴注入
   * @param token
   * @param provider
   */
  addProvider(token: Token<any>, provider: any) {
    this.providerMap.set(token, provider);
  }

  /**
   * 取得依賴注入
   * @param token
   */
  getProvider<T>(token: Token<T>): T {
    if (this.providerMap.has(token)) {
      return this.providerMap.get(token);
    } else {
      try {
        const instance = this.getInstanceFromClass(token as Type<any>);
        this.addProvider(token, instance);
        return instance;
      } catch (error) {
        throw new Error(`${token} is a normal string that cannot be instantiated`);
      }
    }
  }

  /**
   * 實作依賴單例
   * @param provider
   */
  private getInstanceFromClass<T>(provider: Type<T>): T {
    const target = provider;
    if (target[INJECTED]) {
      const injects = target[INJECTED]!.map((childToken) => this.getProvider(childToken));
      return new target(...injects);
    } else {
      if (target.length) {
        throw new Error(`Injection error.${target.name} has dependancy injection, but no @Injectable() decorate it`);
      }
      return new target();
    }
  }
}
