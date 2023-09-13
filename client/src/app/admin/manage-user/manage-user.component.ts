import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/User';
import { AccountService } from 'src/app/_services/account.service';
import { AdminService } from 'src/app/_services/admin.service';
import { ModalForRolesComponent } from 'src/app/modal/modal-for-roles/modal-for-roles.component';

@Component({
  selector: 'app-manage-user',
  templateUrl: './manage-user.component.html',
  styleUrls: ['./manage-user.component.css']
})
export class ManageUserComponent implements OnInit {
  usersToManage: User[] = [];
  refModalBs: BsModalRef<ModalForRolesComponent> = new BsModalRef<ModalForRolesComponent>(); // this is the modal reference
  rolesAvailable = ['Member', 'Admin', 'Moderator'];


  constructor(private serviceModal: BsModalService, public serviceAccount: AccountService, private serviceAdmin: AdminService) { }

  ngOnInit(): void {
    this.loadUsersWithRoles();
  }

  // Method to open a modal for managing roles of a user

  openModalForRoles(appUser: User) {

    // Define the configuration for the modal
    // This includes the CSS class for the modal and its initial state
    const modalConfiguration = {
      // Set the modal to be vertically centered on the screen
      class: 'modal-dialog-centered',

      // Define the initial state of the modal
      // This state will be passed to the modal component when it's instantiated

      initialState: {
        
        // Pass the username of the appUser to the modal
        username: appUser.username,

        // Pass the list of all available roles to the modal
        rolesAvailable: this.rolesAvailable,

        // Pass the roles of the appUser to the modal.
        // Use the spread operator to create a new array from appUser.roles 
        // to ensure we're not modifying the original roles array directly.
        rolesSelected: [...appUser.roles]
      }
    }

    // Use the modal service to show the ModalForRolesComponent with the specified configuration.
    // The result (a reference to the modal) is stored in this.refModalBs.
    this.refModalBs = this.serviceModal.show(ModalForRolesComponent, modalConfiguration);
  }



  loadUsersWithRoles() {
    this.serviceAdmin.loadUsersWithRoles().subscribe({
      next: resUser => {
        this.usersToManage = resUser;
        console.log("Retrieved users in manage-user:", this.usersToManage);
      },
      error: err => {
        console.error("Error retrieving users (err):", err);
      }
    });
  }
}


