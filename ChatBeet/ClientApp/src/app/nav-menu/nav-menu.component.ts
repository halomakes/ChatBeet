import { Component, OnInit } from '@angular/core';
import { map } from 'rxjs';
import { IdentityService } from '../identity.service';
import { Guild, User } from '../models/user';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit {
  public isLoggedIn: boolean = false;
  public currentUser?: User;
  public get currentGuild(): Guild | undefined {
    return this.identity.selectedGuild;
  }
  public availableGuilds?: Array<Guild>;

  constructor(private identity: IdentityService) { }

  ngOnInit(): void {
    this.identity.getCurrentUser().pipe(map(u => {
      this.isLoggedIn = !!u?.user;
      this.currentUser = u?.user;
      this.availableGuilds = u?.guilds;
    })).subscribe();
  }

  public selectGuild = (guild: Guild): any => this.identity.selectedGuild = guild;
}
