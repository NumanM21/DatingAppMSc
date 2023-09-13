import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalForRolesComponent } from './modal-for-roles.component';

describe('ModalForRolesComponent', () => {
  let component: ModalForRolesComponent;
  let fixture: ComponentFixture<ModalForRolesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModalForRolesComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ModalForRolesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
