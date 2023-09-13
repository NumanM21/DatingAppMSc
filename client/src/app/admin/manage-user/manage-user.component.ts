import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/User';
import { AccountService } from 'src/app/_services/account.service';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-manage-user',
  templateUrl: './manage-user.component.html',
  styleUrls: ['./manage-user.component.css']
})
export class ManageUserComponent implements OnInit {
  usersToManage: User[] = [];


  constructor(public serviceAccount:AccountService ,private serviceAdmin: AdminService) { }

  ngOnInit(): void {
    this.loadUsersWithRoles();
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


