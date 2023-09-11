import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MemberMessagetabComponent } from './member-messagetab.component';

describe('MemberMessagetabComponent', () => {
  let component: MemberMessagetabComponent;
  let fixture: ComponentFixture<MemberMessagetabComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MemberMessagetabComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MemberMessagetabComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
