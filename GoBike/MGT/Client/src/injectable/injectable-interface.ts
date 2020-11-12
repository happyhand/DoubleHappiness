import { INJECTED } from "./injectable";

export interface Type<T> extends Function {
  [INJECTED]?: Type<any>[]; // 在3.3实现@Injectable()用到
  new (...args: any[]): T;
}

export type Token<T> = string | Type<T>;
