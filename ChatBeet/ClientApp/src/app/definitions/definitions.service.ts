import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { GuildId } from '../common/guild-id';
import { Definition } from './definition';

@Injectable({
  providedIn: 'root'
})
export class DefinitionsService {

  constructor(private http: HttpClient) { }

  public getDefinitions = (id: GuildId): Observable<Array<Definition>> => this.http.get<Array<Definition>>(`/api/guilds/${id}/definitions`).pipe
    (map(r => r.map(v => new Definition(v))));
}
