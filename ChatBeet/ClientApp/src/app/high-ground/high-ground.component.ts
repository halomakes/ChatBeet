import { Component, OnInit } from '@angular/core';
import { concatMap, of, tap } from 'rxjs';
import { Guild } from '../common/Guild';
import { IdentityService } from '../identity.service';
import { HighGround } from './high-ground';
import { HighGroundService } from './high-ground.service';

@Component({
  selector: 'app-high-ground',
  templateUrl: './high-ground.component.html',
  styleUrls: ['./high-ground.component.scss']
})
export class HighGroundComponent implements OnInit {
  public ground: HighGround | undefined;
  public guild: Guild | undefined;

  constructor(private mustafar: HighGroundService, private identity: IdentityService) { }

  ngOnInit(): void {
    this.identity.guildChanges.pipe(
      tap(g => this.guild = g),
      concatMap(guild => guild?.id ? this.mustafar.getHighGround(guild?.id) : of(undefined)),
      tap(ground => this.ground = ground)
    ).subscribe();
  }
}
