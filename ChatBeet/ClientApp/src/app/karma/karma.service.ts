import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { KarmaLevels } from './karma';

@Injectable({
  providedIn: 'root'
})
export class KarmaService {

  constructor(private http: HttpClient) { }

  public getKarmaLevels = (guildId: string): Observable<KarmaLevels> => this.http.get<KarmaLevels>(`/api/guilds/${guildId}/karma`);
}
