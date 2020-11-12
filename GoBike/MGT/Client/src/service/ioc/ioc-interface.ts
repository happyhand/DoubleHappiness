import { Token } from "@/injectable/injectable-interface";

/**
 * Ioc 依賴注入服務介面
 */
export interface IocInterface {
  addProvider<T>(token: Token<T>, provider: any): void;
  getProvider<T>(token: Token<T>): T;
}
