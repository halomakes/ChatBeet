import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MentionService {
  constructor(private http: HttpClient) { }

  public getUser = (id: string): Observable<string> => this.http.get<string>(`/api/mentions/users/${id}`);

  public getChannel = (id: string): Observable<string> => this.http.get<string>(`/api/mentions/channels/${id}`);

  public getRole = (id: string): Observable<string> => this.http.get<string>(`/api/mentions/roles/${id}`);
}
