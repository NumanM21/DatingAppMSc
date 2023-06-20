import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { keyframes } from '@angular/animations';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error) {
          switch (error.status) {
            case 400: // 2 cases if 400 (validation or bad request)
              if (error.error.errors) // errors is object which is returned
              {
                const modStateErrors = [];
                for (const value in error.error.errors) {
                  if (error.error.errors[value]) // build array of values from object in array
                  {
                    modStateErrors.push(error.error.errors[value])
                  }
                }
                throw modStateErrors.flat();
              }
              else 
              {
                this.toastr.error(error.error, error.status.toString())
              }
              break;
            case 401:
              this.toastr.error('unauthorised', error.status.toString())
              break;
            case 404:
              this.router.navigateByUrl('/not-found');
              break;
            case 500:
                const extraNavigation : NavigationExtras = {state: {error: error.error}}
                this.router.navigateByUrl('/server-error', extraNavigation);
                break;
            default:
              this.toastr.error('something went wrong')
              console.log(error);
              break;
          }
        }
        throw error;
      })
    )
  }
}
