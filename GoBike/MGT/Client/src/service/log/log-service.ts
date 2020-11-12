import { Injectable } from "@/injectable/injectable";

/**
 * Log 服務
 */
@Injectable()
export default class LogService {
  /**
   * info log
   * @param className
   * @param functionName
   * @param message
   */
  logInfo(className: string, functionName: string, message: string): void {
    console.log(`[${className}] :: [${functionName}] :: ${message}`);
  }

  /**
   * warn log
   * @param className
   * @param functionName
   * @param message
   */
  logWarn(className: string, functionName: string, message: string): void {
    console.warn(`[${className}] :: [${functionName}] :: ${message}`);
  }

  /**
   * error log
   * @param className
   * @param functionName
   * @param message
   * @param error
   */
  logError(className: string, functionName: string, message: string, error?: any): void {
    console.error(`[${className}] :: [${functionName}] :: ${message}\nError: ${error}`);
  }
}
