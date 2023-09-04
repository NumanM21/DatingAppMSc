import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-pick-date',
  templateUrl: './pick-date.component.html',
  styleUrls: ['./pick-date.component.css']
})
export class PickDateComponent implements ControlValueAccessor {
  @Input() label = ''; // label for the input
  @Input() maxDate: Date | undefined; // min date for user to be over 18
  bsConfig: Partial<BsDatepickerConfig> | undefined // partial means we don't have to pass in all the properties of the BsDatepickerConfig 


  constructor(@Self() public controlNG: NgControl) {
    this.controlNG.valueAccessor = this; // setting this to our date picker component
    this.bsConfig = {
      containerClass: 'theme-default',
      dateInputFormat: 'DD MMMM YYYY'
    }
  }
  

  writeValue(obj: any): void {

  }
  registerOnChange(fn: any): void {

  }
  registerOnTouched(fn: any): void {

  }


  get _formControl(): FormControl {
    return this.controlNG.control as FormControl; // casting controlNG into a FormControl
    // Issue with [formControl] in register.component.html -> Get around this
  }




}
