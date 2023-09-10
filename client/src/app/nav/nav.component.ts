import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/User';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { MembersService } from '../_services/members.service';
import { Member } from '../_models/Member';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}


  constructor(public accountService: AccountService, private router: Router,private toastr: ToastrService) { }

  ngOnInit(): void {
  }



  login() {
    this.accountService.login(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl('/members')
        this.toastr.success("Logged in!")
      },
    })
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/')
  }

  // Toggle password visibility
  togglePasswordVisibility(passwordField: any): void {
    if (passwordField.type === 'text') {
      passwordField.type = 'password';
    } else {
      passwordField.type = 'text';
    }
  }

}
