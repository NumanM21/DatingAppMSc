import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { PopUpConfirmationComponent } from '../modals/pop-up-confirmation/pop-up-confirmation.component';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PopupConfirmationService {

  refModalBs?: BsModalRef<PopUpConfirmationComponent>;

  constructor(private serviceModal: BsModalService) { }

  // This function is called from the component that wants to open the modal when we click away from edit profile after making changes -> Will return true if we click yes, false if we click no

  confirmAction(title = 'Confirm Action', message = 'Are you sure you want to do this? Unsaved changes will be LOST!',btnOkText = 'Yes',btnCancelText = 'No'): Observable<boolean> {

    // Inital states:
    const configuration = { // initialState is MUST -> how TS recognizes what we are passing in
      initialState: {
        title, message, btnOkText, btnCancelText 
      }
    }
    this.refModalBs = this.serviceModal.show(PopUpConfirmationComponent, configuration);

    return this.refModalBs.onHide!.pipe(
      map(() => {
        // We initalized .res to false, so we can use ! to over-ride TS
        return this.refModalBs!.content!.res;
      })
    )
  }








}





