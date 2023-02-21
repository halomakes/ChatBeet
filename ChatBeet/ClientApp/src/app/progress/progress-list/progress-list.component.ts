import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { tap, concatMap, of } from 'rxjs';
import { IdentityService } from 'src/app/identity.service';
import { DeleteProgressComponent } from '../delete-progress/delete-progress.component';
import { EditProgressComponent } from '../edit-progress/edit-progress.component';
import { ProgressSpan } from '../progress-span';
import { ProgressService } from '../progress.service';

@Component({
  selector: 'app-progress-list',
  templateUrl: './progress-list.component.html',
  styleUrls: ['./progress-list.component.scss']
})
export class ProgressListComponent implements OnInit {
  public spans: Array<ProgressSpan> | undefined;

  constructor(private service: ProgressService, private identity: IdentityService, private dialog: MatDialog) { }

  ngOnInit(): void {
    this.identity.guildChanges.pipe(
      concatMap(guild => guild?.id ? this.service.getSpans(guild?.id) : of(undefined)),
      tap(spans => this.spans = spans)
    ).subscribe();
  }

  public add = (): void => {
    const dialogRef = this.dialog.open(EditProgressComponent, {
      width: '600px'
    });
    this.refreshOnClose(dialogRef);
  }

  public edit = (span: ProgressSpan): void => {
    const dialogRef = this.dialog.open(EditProgressComponent, {
      width: '600px',
      data: span
    });
    this.refreshOnClose(dialogRef);
  }

  public delete = (span: ProgressSpan): void => {
    const dialogRef = this.dialog.open(DeleteProgressComponent, {
      data: span
    });
    this.refreshOnClose(dialogRef);
  }

  private refreshOnClose = <T>(dialogRef: MatDialogRef<T>): void => {
    dialogRef.afterClosed().pipe(
      concatMap(() => this.service.getSpans(this.identity.selectedGuild!.id)),
      tap(spans => this.spans = spans)
    ).subscribe();
  }
}
