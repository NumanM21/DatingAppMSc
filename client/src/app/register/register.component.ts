import { Component, EventEmitter, Input, OnInit, Output, AfterViewInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';


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
  ErrorValidation: string[] | undefined;

  constructor(private accountService: AccountService, private toastr: ToastrService, private builderForm: FormBuilder, private rout: Router) { }



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
      next: () => this.formRegister.controls['confirmPassword'].updateValueAndValidity() 
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
    const dateOfBirth = this.getOnlyDate(this.formRegister.controls['dateOfBirth'].value);
    /// ... spreads all the values from formRegister.value, and we choose the dob field
    const formRegisterValues = { ...this.formRegister.value, dateOfBirth: dateOfBirth }
    this.accountService.register(formRegisterValues).subscribe({
      next: () => {
        // Once user registers, they are automatically logged in
        this.rout.navigateByUrl('/members');
      },
      error: err => {
        this.ErrorValidation = err; // puts error message in array
      }
    })
  }

  cancel() {
    this.CancelRegister.emit(false);
  }

  // DatePicker gets date + local time (want to stay UTC consistent, so remove local time)

  private getOnlyDate(dateOfBirth: string | undefined) {
    if (dateOfBirth) {
      // Now we have a Date object with dateOfBirth from user input 
      let dob = new Date(dateOfBirth);
      // Use set minutes (local machine) and subtract timezone offset to get UTC time, then toString so we can slice the string to get only the date (which is up to 10 character)
      return new Date(dob.setMinutes(dob.getMinutes() - dob.getTimezoneOffset())).toISOString().slice(0, 10);
    }
    else return; // no dateOfBirth
  }
}
