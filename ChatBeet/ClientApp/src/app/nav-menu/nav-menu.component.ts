import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { tap } from 'rxjs';
import { AdministrationService } from '../administration.service';
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
  public currentGuild?: Guild;
  public availableGuilds?: Array<Guild>;
  public inviteLink?: string;

  @Output()
  public navToggled: EventEmitter<void> = new EventEmitter<void>();

  constructor(private identity: IdentityService, private admin: AdministrationService) { }

  ngOnInit(): void {
    this.identity.getCurrentUser().pipe(tap(u => {
      this.isLoggedIn = !!u?.user;
      this.currentUser = u?.user;
      this.availableGuilds = u?.guilds;
    })).subscribe();
    this.admin.getInvitation().pipe(tap(l => {
      this.inviteLink = l;
    })).subscribe();
    this.identity.guildChanges.pipe(tap(g => {
      this.currentGuild = g;
    })).subscribe();
  }

  public selectGuild = (guild: Guild): any => this.identity.selectedGuild = guild;
}
