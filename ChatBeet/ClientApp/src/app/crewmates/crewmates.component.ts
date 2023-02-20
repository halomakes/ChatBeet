import { Component, OnInit } from '@angular/core';
import { concatMap, of, tap } from 'rxjs';
import { IdentityService } from '../identity.service';
import { CrewmatesService } from './crewmates.service';
import { SuspicionLevel } from './suspicion-level';

type OrderedLevel = { level: SuspicionLevel | undefined, order: number };

@Component({
  selector: 'app-crewmates',
  templateUrl: './crewmates.component.html',
  styleUrls: ['./crewmates.component.scss']
})
export class CrewmatesComponent implements OnInit {
  public levels: Array<OrderedLevel> | undefined;
  public defaultColor = "#555";

  constructor(private service: CrewmatesService, private identity: IdentityService) { }

  ngOnInit(): void {
    this.identity.guildChanges.pipe(
      concatMap(guild => guild?.id ? this.service.getLevels(guild?.id) : of(undefined)),
      tap(this.setLevels)
    ).subscribe();
  }

  private setLevels = (levels: Array<SuspicionLevel> | undefined): void => {
    if (levels != undefined) {
      const stagger = [5, 6, 4, 7, 3, 8, 2, 9, 1];
      var sorted = stagger
        .map((val, idx) => ({ level: levels[idx], order: val }))
        .sort((a, b) => (a.level > b.level) ? -1 : 1);
      this.levels = sorted;
      console.log(this.levels);
    }
  }
}
