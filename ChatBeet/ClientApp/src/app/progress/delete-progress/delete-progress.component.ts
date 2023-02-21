import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProgressSpan } from '../progress-span';
import { ProgressService } from '../progress.service';

@Component({
  selector: 'app-delete-progress',
  templateUrl: './delete-progress.component.html',
  styleUrls: ['./delete-progress.component.scss']
})
export class DeleteProgressComponent {

  constructor(public dialogRef: MatDialogRef<DeleteProgressComponent>,
    private snackbar: MatSnackBar,
    private service: ProgressService,
    @Inject(MAT_DIALOG_DATA) public data?: ProgressSpan) { }

  public delete = (): void => {
    if (this.data) {
      this.service.deleteSpan(this.data.guildId, this.data.key).subscribe(() => {
        this.snackbar.open('Span deleted.', undefined, {
          duration: 5000
        });
        this.dialogRef.close();
      }, err => {
        this.snackbar.open('Something went wrong.', undefined, {
          duration: 5000
        });
        this.dialogRef.close();
      });
    } else {
      this.dialogRef.close();
    }
  }
}
