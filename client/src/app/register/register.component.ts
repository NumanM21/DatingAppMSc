import { Component, EventEmitter, Input, OnInit, Output, AfterViewInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() CancelRegister = new EventEmitter();
  model: any = {}
  formRegister: FormGroup = new FormGroup({});
  maxDate: Date = new Date(); // calculate 18 years from current date to ensure user is over 18

  constructor(private accountService: AccountService, private toastr: ToastrService, private builderForm: FormBuilder) { }



  ngOnInit(): void {
    this.formIntialise();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  formIntialise() {
    this.formRegister = this.builderForm.group({
      username: ['', [
        Validators.required, 
        Validators.minLength(4)]],
      password: ['', [
        Validators.required, 
        Validators.minLength(4), 
        Validators.maxLength(8), 
        this.passwordValidator]],
      confirmPassword: ['', [
        Validators.required, 
        this.valueMatch('password')]],
      gender: ['male'],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],

    });
    this.formRegister.controls['password'].valueChanges.subscribe({
      next: () => this.formRegister.controls['confirmPassword'].updateValueAndValidity() // Typo fixed: 'confimPassword' -> 'confirmPassword'
    })
  }

  // Confirm password validator
  valueMatch(toMatch: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(toMatch)?.value ? null : { MatchNotValid: true }
    }
  }
  
  // Password Regex Validator
  passwordValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    const hasUpperCase = /[A-Z]/.test(value);
    const hasNumber = /[0-9]/.test(value);
    const hasSpecialCharacters = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/.test(value);

    if (hasUpperCase && hasNumber && hasSpecialCharacters) {
      return null;
    } else {
      return { 'complexity': 'Password must have at least one uppercase letter, one number and one special character.' };
    }
  }

  register() {
    console.log(this.formRegister?.value);
  }

  cancel() {
    this.CancelRegister.emit(false);
  }
}
