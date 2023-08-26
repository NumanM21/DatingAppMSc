import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, delay, finalize } from 'rxjs';
import { LoadingService } from '../_services/loading.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  constructor(private serviceLoading: LoadingService) {}

    // This method is called once HTTP request is on the way/ sent out 
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.serviceLoading.requestCount();// Increment count (each request we ++)
    return next.handle(request).pipe(
      delay(1000), // fake delay for now //FIXME: Remove this once not needed
      finalize(()=>{
        this.serviceLoading.idle()
      })
    )
  }
}
