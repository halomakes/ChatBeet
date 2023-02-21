import { HttpClient } from '@angular/common/http';
import { EventEmitter, Injectable } from '@angular/core';
import { catchError, map, Observable, of, startWith, tap } from 'rxjs';
import { User } from './common/user';
import { CurrentUserModel } from "./common/CurrentUserModel";
import { Guild } from "./common/Guild";

@Injectable({
  providedIn: 'root'
})
export class IdentityService {
  private static currentUser?: CurrentUserModel;
  private static currentGuild?: Guild;
  private static isLoggedIn?: boolean | null = null;
  private static guildChanges: EventEmitter<Guild | undefined> = new EventEmitter<Guild | undefined>();

  constructor(private http: HttpClient) {
    this.loadStoredGuild();
  }

  public getUser = (): Observable<User | undefined> => this.getCurrentUser().pipe(
    map(r => r?.user)
  );

  public getGuilds = (): Observable<Array<Guild> | undefined> => this.getCurrentUser().pipe(
    map(r => r?.guilds)
  );

  public get guildChanges(): Observable<Guild | undefined> {
    return IdentityService.currentGuild ?
      IdentityService.guildChanges.pipe(
        startWith(IdentityService.currentGuild)
      )
      : IdentityService.guildChanges;
  }

  public set selectedGuild(guild: Guild | undefined) {
    IdentityService.currentGuild = guild;
    localStorage.setItem('guild', JSON.stringify(guild));
    IdentityService.guildChanges.emit(guild);
  }

  public get selectedGuild(): Guild | undefined {
    return IdentityService.currentGuild;
  }

  private loadStoredGuild = (): void => {
    if (!IdentityService.currentGuild) {
      var content = localStorage.getItem('guild');
      if (content) {
        const parsed = JSON.parse(content);
        IdentityService.currentGuild = new Guild(parsed);
      }
    }
  }

  public getCurrentUser = (): Observable<CurrentUserModel | undefined> => IdentityService.isLoggedIn != undefined && IdentityService.isLoggedIn != null
    ? of(IdentityService.currentUser)
    : this.http.get<CurrentUserModel>('/api/users/@me').pipe(
      map(r => new CurrentUserModel(r)),
      tap(r => {
        IdentityService.currentUser = r;
        IdentityService.isLoggedIn = true;
        console.log('Logged in as', r);
      }),
      catchError(() => {
        IdentityService.isLoggedIn = false;
        return of(undefined)
      })
    )
}
