import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-modal-for-roles',
  templateUrl: './modal-for-roles.component.html',
  styleUrls: ['./modal-for-roles.component.css']
})

// Pass these values to the modal, through the manage-user component, when we call the method openModalForRoles():

// manage-user.component will populate field below
export class ModalForRolesComponent implements OnInit {
  username = '';
  rolesSelected: any[] = [];
  @Input() rolesAvailableToAssign: string[] = [];


  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
  }



  // Update the rolesSelected array based on the checkbox state.
  CheckBoxUpdate(valueChecked: string) {
    // Check if the valueChecked is already in the rolesSelected array.
    const idx = this.rolesSelected.indexOf(valueChecked);

    // If the value is in the array (checkbox was previously checked and now unchecked by the user), 
    // remove it from the array.
    if (idx !== -1) {
      this.rolesSelected.splice(idx, 1);
    }

    // If the value is not in the array (so -1 idx returned) (checkbox was previously unchecked and now checked by the user), 
    // add it to the array.
    else {
      this.rolesSelected.push(valueChecked);
    }
  }





}
