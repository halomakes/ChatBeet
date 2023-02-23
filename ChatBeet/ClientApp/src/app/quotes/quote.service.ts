import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { GuildId } from '../common/guild-id';
import { Quote } from './quote';

@Injectable({
  providedIn: 'root'
})
export class QuoteService {

  constructor(private http: HttpClient) { }

  public getQuotes = (guildId: GuildId): Observable<Array<Quote>> => this.http.get<Array<Quote>>(`/api/guilds/${guildId}/quotes`).pipe(
    map(r => r.map(v => new Quote(v)))
  );

  public getQuote = (guildId: GuildId, slug: string): Observable<Quote> => this.http.get<Quote>(`/api/guilds/${guildId}/quotes/${slug}`).pipe(
    map(r => new Quote(r))
  );
}
