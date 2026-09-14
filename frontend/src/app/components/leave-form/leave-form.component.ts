import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LeaveService } from '../../services/leave.service';

@Component({
  selector: 'app-leave-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './leave-form.component.html'
})
export class LeaveFormComponent {
  form: FormGroup;
  submitted = false;
  error = '';

  constructor(private fb: FormBuilder, private leaveService: LeaveService) {
    this.form = this.fb.group({
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      reason: ['', [Validators.required, Validators.maxLength(300)]]
    });
  }

  submit(): void {
    if (this.form.invalid) return;

    this.leaveService.create(this.form.value).subscribe({
      next: () => {
        this.submitted = true;
        this.form.reset();
      },
      error: (err) => {
        this.error = err.error ?? 'Something went wrong.';
      }
    });
  }
}
