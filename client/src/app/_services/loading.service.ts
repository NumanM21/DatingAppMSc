import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  requestsToLoadCount = 0;

  constructor(private serviceSpinner : NgxSpinnerService) { }


  requestCount(){
    this.requestsToLoadCount++;
    this.serviceSpinner.show(undefined, {
      type: 'pacman',
      size: 'medium',
      bdColor: 'rgba(0,0,0,0.5)', //FIXME: Can alter this if needed for dimming
      color: '#1a1a1a'
    })
  }

  idle(){
    this.requestsToLoadCount--;
    if (this.requestsToLoadCount <= 0){
      this.requestsToLoadCount = 0;
      this.serviceSpinner.hide();
    }
  }

}
