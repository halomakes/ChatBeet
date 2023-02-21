import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ServerService {
  private static readme: string;

  constructor(private http: HttpClient) { }

  public getVersion = (): Observable<string> => this.http.get<any>('/api/system/status').pipe(
    map(r => r.version)
  );

  public getReadme = (): Observable<string> => ServerService.readme ? of(ServerService.readme) : this.http.get('https://raw.githubusercontent.com/halomademeapc/ChatBeet/develop/README.md', { responseType: 'text' }).pipe(
    tap(r => ServerService.readme = r)
  )
}
