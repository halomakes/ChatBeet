import { Component, OnInit } from '@angular/core';
import { tap, concatMap, of } from 'rxjs';
import { IdentityService } from 'src/app/identity.service';
import { ProgressSpan } from '../progress-span';
import { ProgressService } from '../progress.service';

@Component({
  selector: 'app-progress-list',
  templateUrl: './progress-list.component.html',
  styleUrls: ['./progress-list.component.scss']
})
export class ProgressListComponent implements OnInit {
  public spans: Array<ProgressSpan> | undefined;

  constructor(private service: ProgressService, private identity: IdentityService) { }

  ngOnInit(): void {
    this.identity.guildChanges.pipe(
      concatMap(guild => guild?.id ? this.service.getSpans(guild?.id) : of(undefined)),
      tap(spans => this.spans = spans)
    ).subscribe();
  }

}
