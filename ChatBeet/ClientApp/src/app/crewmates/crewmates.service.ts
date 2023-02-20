import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { GuildId } from '../common/guild-id';
import { SuspicionLevel } from './suspicion-level';

@Injectable({
  providedIn: 'root'
})
export class CrewmatesService {

  constructor(private http: HttpClient) { }

  public getLevels = (guildId: GuildId): Observable<Array<SuspicionLevel>> => this.http.get<Array<SuspicionLevel>>(`/api/guilds/${guildId}/suspicions`).pipe(
    map(r => r.map(v => new SuspicionLevel(v)))
  );
}
