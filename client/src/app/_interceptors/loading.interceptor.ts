import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, delay, finalize, identity } from 'rxjs';
import { LoadingService } from '../_services/loading.service';
import { environment } from 'src/environments/environment';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  constructor(private serviceLoading: LoadingService) {}

    // This method is called once HTTP request is on the way/ sent out 
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.serviceLoading.requestCount();// Increment count (each request we ++)
    return next.handle(request).pipe(
     (environment.production ? identity : delay(1000)) , // identity is a function that returns the same value that is passed in (so returns nothing -> we can remove delay in production, but in dev we want to see the loading spinner)
      finalize(()=>{
        this.serviceLoading.idle()
      })
    )
  }
}
