import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { GuildId } from '../common/guild-id';
import { ProgressSpan } from './progress-span';

@Injectable({
  providedIn: 'root'
})
export class ProgressService {

  constructor(private http: HttpClient) { }

  public getSpans = (guildId: GuildId): Observable<Array<ProgressSpan>> => this.http.get<Array<ProgressSpan>>(`/api/guilds/${guildId}/fixedtimeranges`).pipe(
    map(r => r.map(v => new ProgressSpan(v)))
  );

  public createSpan = (span: ProgressSpan): Observable<any> => this.http.post(`/api/guilds/${span.guildId}/fixedtimeranges`, span);

  public getSpan = (guildId: GuildId, key: string): Observable<ProgressSpan> => this.http.get<ProgressSpan>(`/api/guilds/${guildId}/fixedtimeranges/${key}`).pipe(
    map(r => new ProgressSpan(r))
  );

  public updateSpan = (span: ProgressSpan): Observable<any> => this.http.put(`/api/guilds/${span.guildId}/fixedtimeranges/${span.key}`, span);

  public deleteSpan = (guildId: GuildId, key: string): Observable<any> => this.http.delete(`/api/guilds/${guildId}/fixedtimeranges/${key}`);
}
