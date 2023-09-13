import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/User';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-manage-user',
  templateUrl: './manage-user.component.html',
  styleUrls: ['./manage-user.component.css']
})
export class ManageUserComponent implements OnInit {
  usersToManage: User[] = [];


  constructor(private serviceAdmin: AdminService) { }

  ngOnInit(): void {
    this.loadUsersWithRoles();
  }

  
  loadUsersWithRoles() {
    this.serviceAdmin.loadUsersWithRoles().subscribe({
      next: resUser => {
        this.usersToManage = resUser;
      }
      });
    }
  }

  
