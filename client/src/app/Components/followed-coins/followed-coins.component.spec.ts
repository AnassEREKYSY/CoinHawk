import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FollowedCoinsComponent } from './followed-coins.component';

describe('FollowedCoinsComponent', () => {
  let component: FollowedCoinsComponent;
  let fixture: ComponentFixture<FollowedCoinsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FollowedCoinsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FollowedCoinsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
