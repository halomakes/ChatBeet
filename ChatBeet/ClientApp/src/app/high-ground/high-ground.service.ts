import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { GuildId } from '../common/guild-id';
import { HighGround } from './high-ground';

@Injectable({
  providedIn: 'root'
})
export class HighGroundService {

  constructor(private http: HttpClient) { }

  public getHighGround = (guildId: GuildId): Observable<HighGround | undefined> => this.http.get<HighGround>(`/api/guilds/${guildId}/highground`).pipe(
    map(h => h ? new HighGround(h) : undefined)
  );
}
