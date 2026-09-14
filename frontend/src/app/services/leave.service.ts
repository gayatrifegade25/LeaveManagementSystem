import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface LeaveRequest {
  id: number;
  employeeName: string;
  startDate: string;
  endDate: string;
  totalDays: number;
  reason: string;
  status: string;
  managerComment?: string;
}

export interface CreateLeaveRequest {
  startDate: string;
  endDate: string;
  reason: string;
}

@Injectable({ providedIn: 'root' })
export class LeaveService {
  private apiUrl = 'https://localhost:5001/api/leaverequests';

  constructor(private http: HttpClient) {}

  getAll(): Observable<LeaveRequest[]> {
    return this.http.get<LeaveRequest[]>(this.apiUrl);
  }

  create(request: CreateLeaveRequest): Observable<LeaveRequest> {
    return this.http.post<LeaveRequest>(this.apiUrl, request);
  }

  review(id: number, approve: boolean, comment: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/review`, { approve, comment });
  }

  cancel(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
