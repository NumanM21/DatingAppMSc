import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LearnMoreModalComponent } from './learn-more-modal.component';

describe('LearnMoreModalComponent', () => {
  let component: LearnMoreModalComponent;
  let fixture: ComponentFixture<LearnMoreModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LearnMoreModalComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LearnMoreModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
