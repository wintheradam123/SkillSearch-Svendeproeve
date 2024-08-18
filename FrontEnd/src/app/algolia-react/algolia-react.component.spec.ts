import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AlgoliaReactComponent } from './algolia-react.component';

describe('AlgoliaReactComponent', () => {
  let component: AlgoliaReactComponent;
  let fixture: ComponentFixture<AlgoliaReactComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AlgoliaReactComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AlgoliaReactComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
