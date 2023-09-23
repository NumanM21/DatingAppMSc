import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.css']
})
export class TestErrorComponent implements OnInit {
  baseUrl = environment.apiUrl;
  validationErrors400 :string[] = [];

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
  }

  getError404() {
    this.http.get(this.baseUrl + 'bug/not-found').subscribe({
      next: response => console.log(response),
      error: error => console.log(error),
    })
  }

  getError400() {
    this.http.get(this.baseUrl + 'bug/bad-request').subscribe({
      next: response => console.log(response),
      error: error => console.log(error),
    })
  }

  getError500() {
    this.http.get(this.baseUrl + 'bug/server-error').subscribe({
      next: response => console.log(response),
      error: error => console.log(error),
    })
  }

  getError401() {
    this.http.get(this.baseUrl + 'bug/auth').subscribe({
      next: response => console.log(response),
      error: error => console.log(error),
    })
  }

  getValidationError400() {
    this.http.post(this.baseUrl + 'account/register', {}).subscribe({
      next: response => console.log(response),
      error: error => {
        console.log(error);
        this.validationErrors400 = error;
      },
    })
  }


}
