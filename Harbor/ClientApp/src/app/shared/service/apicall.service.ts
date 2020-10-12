import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class APICallService {

  constructor(private http: HttpClient) {

  }

  PostMethod(ControllerName: string, MethodName: string, Parameters: {}) {
    let URL = '/api/' + ControllerName + '/' + MethodName;
    return this.http.post(URL, Parameters);
  }

  GetMethod(ControllerName: string, MethodName: string, Parameters: {}) {
    let URL = '/api/' + ControllerName + '/' + MethodName;
    return this.http.get(URL, Parameters);
  }

  setCookies(token: string) {
    let expireDate = new Date();
    expireDate.setDate(expireDate.getTime() + (20 * 60 * 100));
    let cookie = 'firstAttemptToken=' + token + ';expires=' + expireDate.toUTCString() + '; path=/';
    document.cookie = cookie;
  }

  getCookies(): string {
    let returnToken: string = '';
    let name = 'firstAttemptToken';
    let decodedCookies = decodeURIComponent(document.cookie);
    let cookies = decodedCookies.split(';');
    for (let i = 0; i < cookies.length; i++) {
      let cookie = cookies[i];
      while (cookie.charAt(0) == ' ') {
        cookie = cookie.substring(1);
      }
      if (cookie.indexOf(name) == 0) {
        returnToken = cookie.substring(name.length + 1, cookie.length);
        return returnToken;
      }
    }
    return returnToken;
  }
}