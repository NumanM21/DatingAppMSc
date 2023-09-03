import { Component, EventEmitter, Input, OnInit, Output, AfterViewInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() CancelRegister = new EventEmitter();
  model: any = {}
  formRegister: FormGroup = new FormGroup({});
  
  constructor(private accountService:AccountService, private toastr: ToastrService) { }



  ngOnInit(): void {
    this.formIntialise();
  }

  formIntialise(){
    this.formRegister = new FormGroup({
      username: new FormControl('', [Validators.required, Validators.minLength(4)]), 
      password: new FormControl('',[Validators.required ,Validators.minLength(4), Validators.maxLength(8), this.passwordValidator]),
      confirmPassword: new FormControl('',[Validators.required, this.valueMatch('password')]),
    })
    this.formRegister.controls['password'].valueChanges.subscribe({
      next: () => this.formRegister.controls['confirmPassword'].updateValueAndValidity() // Typo fixed: 'confimPassword' -> 'confirmPassword'
    })
  }

  valueMatch(toMatch: string): ValidatorFn{
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(toMatch)?.value ? null : {MatchNotValid:true}
    }
  }

  passwordValidator(control: AbstractControl): ValidationErrors | null{
    const value = control.value;
    const hasUpperCase = /[A-Z]/.test(value);
    const hasNumber = /[0-9]/.test(value);
    const hasSpecialCharacters = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/.test(value);

    if (hasUpperCase && hasNumber && hasSpecialCharacters) {
      return null;
    } else {
      return {'complexity': 'Password must have at least one uppercase letter, one number and one special character.'};
    }
  }

  register() {
    console.log(this.formRegister?.value);
    // Your existing code for registration
  }

  cancel() {
    this.CancelRegister.emit(false);
  }
}
