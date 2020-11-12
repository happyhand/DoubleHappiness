import { Injectable } from "@/injectable/injectable";
import { domain } from "@/model/api-module";
import Axios from "axios-observable";
import { catchError, map } from "rxjs/operators";
import { AxiosResponse } from "axios";
import { Observable, of, throwError } from "rxjs";
import LogService from "../log/log-service";

/**
 * Api 服務
 */
@Injectable()
export default class ApiService {
  constructor(private readonly logService: LogService) {}
  post<T>(url: string, data: any): Observable<T> {
    this.logService.logInfo(typeof this, "post", `Request post api :: Url: ${url} Data: ${JSON.stringify(data)}`);
    return Axios.post<T>(`${domain}/${url}`, data).pipe(
      map((response: AxiosResponse<T>) => {
        return response.data;
      }),
      catchError((error: any) => {
        if (error.response) {
          this.logService.logError(typeof this, "post", "Post Error", error);
          return of(error.response.data);
        } else {
          this.logService.logError(typeof this, "post", "Post unknown Error", error);
          return throwError(error);
        }
      })
    );
  }
}
