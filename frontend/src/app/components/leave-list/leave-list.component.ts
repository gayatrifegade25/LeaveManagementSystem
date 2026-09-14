import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LeaveService, LeaveRequest } from '../../services/leave.service';

@Component({
  selector: 'app-leave-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './leave-list.component.html'
})
export class LeaveListComponent implements OnInit {
  requests: LeaveRequest[] = [];
  loading = true;

  constructor(private leaveService: LeaveService) {}

  ngOnInit(): void {
    this.leaveService.getAll().subscribe({
      next: (data) => {
        this.requests = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  cancel(id: number): void {
    this.leaveService.cancel(id).subscribe(() => {
      this.requests = this.requests.filter(r => r.id !== id);
    });
  }
}
