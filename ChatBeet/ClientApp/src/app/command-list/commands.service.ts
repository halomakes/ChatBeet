import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CommandsService {

  constructor(private http: HttpClient) { }

  public getCommands = (): Observable<Array<any>> => this.http.get<Array<any>>('/api/system/commands').pipe(
    map(r => r.sort((a, b) => a.name < b.name ? -1 : 1))
  );
}
