import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, FormGroup, NgControl } from '@angular/forms';

// Using this class to allow our inputs from our register DOM be re-usable (so we don't have to repeat lines of code) --> DRY
@Component({
  selector: 'app-input-text',
  templateUrl: './input-text.component.html',
  styleUrls: ['./input-text.component.css']
})
export class InputTextComponent implements ControlValueAccessor {

  // FIXME: Should these be undefined?
  @Input() label = ''; // label for the input
  @Input() type = 'input-text';
  @Input() controlName = ''; // name of the form control
  @Input() parentForm: FormGroup | undefined; // Pass the parent FormGroup

  // @Self() decorator tells Angular to look for the dependency only from the local injector and not from the parent injector.
  // NGcontrol -> All Form Control classes inherit from this class
  constructor(@Self() public controlNG: NgControl) { 
    this.controlNG.valueAccessor = this; // this is the input-text component
  }

  // ControlValueAccessor interface methods. Register forms will pass through these methods
  // Methods controlled via the Form Control class
  writeValue(obj: any): void {

  }

  registerOnChange(fn: any): void {

  }

  registerOnTouched(fn: any): void {

  }



  get _formControl(): FormControl{
    return this.controlNG.control as FormControl; // casting controlNG into a FormControl
    // Issue with [formControl] in register.component.html -> Get around this
  }
}
