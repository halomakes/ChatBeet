import { formatDate } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { formatDateForForm } from 'src/app/common/formatDateForForm';
import { touchAllControls } from 'src/app/common/touch-controls';
import { IdentityService } from 'src/app/identity.service';
import { ProgressSpan } from '../progress-span';
import { ProgressService } from '../progress.service';

@Component({
  selector: 'app-edit-progress',
  templateUrl: './edit-progress.component.html',
  styleUrls: ['./edit-progress.component.scss']
})
export class EditProgressComponent implements OnInit {
  public form: FormGroup = new FormGroup({
    key: new FormControl('', [Validators.required, Validators.pattern(/^[\w\d]+(?:-[\w\d]+)?$/), Validators.maxLength(30)]),
    template: new FormControl('', [Validators.required, Validators.pattern(/.*\{(?:percentage|elapsed|remaining)\}.*/), Validators.maxLength(200)]),
    beforeRangeMessage: new FormControl('', [Validators.maxLength(200)]),
    afterRangeMessage: new FormControl('', [Validators.maxLength(200)]),
    startDate: new FormControl(null, [Validators.required]),
    endDate: new FormControl(null, [Validators.required])
  });

  constructor(public dialogRef: MatDialogRef<EditProgressComponent>,
    private snackbar: MatSnackBar,
    private service: ProgressService,
    private identity: IdentityService,
    @Inject(MAT_DIALOG_DATA) public data?: ProgressSpan) { }

  ngOnInit(): void {
    if (this.data?.key) {
      this.form.get('key')?.setValue(this.data.key);
      this.form.get('template')?.setValue(this.data.template);
      this.form.get('beforeRangeMessage')?.setValue(this.data.beforeRangeMessage);
      this.form.get('afterRangeMessage')?.setValue(this.data.afterRangeMessage);
      this.form.get('startDate')?.setValue(formatDateForForm(this.data.startDate));
      this.form.get('endDate')?.setValue(formatDateForForm(this.data.endDate));
    }
  }

  public save = (): void => {
    touchAllControls(this.form);
    if (this.form.valid && this.form.value.startDate < this.form.value.endDate) {
      const value = this.form.value;
      const span = new ProgressSpan({
        key: value.key,
        template: value.template,
        beforeRangeMessage: value.beforeRangeMessage || '',
        afterRangeMessage: value.afterRangeMessage || '',
        startDate: new Date(value.startDate),
        endDate: new Date(value.endDate),
        guildId: this.identity.selectedGuild?.id
      });
      this.form.disable();
      if (this.data?.key) {
        this.service.updateSpan(span).subscribe(() => {
          this.snackbar.open('Span updated.', undefined, {
            duration: 5000
          });
          this.dialogRef.close();
        }, err => {
          this.snackbar.open('Something went wrong.', undefined, {
            duration: 5000
          });
          this.form.enable();
        });
      } else {
        this.service.createSpan(span).subscribe(() => {
          this.snackbar.open('Span created.', undefined, {
            duration: 5000
          });
          this.dialogRef.close();
        }, err => {
          console.log(err);
          if (err instanceof HttpErrorResponse && err.status == 409) {
            this.snackbar.open('A span already exists with that Key.', undefined, {
              duration: 5000
            });
          } else {
            this.snackbar.open('Something went wrong.', undefined, {
              duration: 5000
            });
          }
          this.form.enable();
        });
      }
    } else {
      this.snackbar.open('Form is invalid, please address errors and try again.', undefined, {
        duration: 5000
      })
    }
  }

  public isTimeError = (): boolean => {
    const value = this.form.value;
    if (!value.startDate || !value.endDate)
      return false;
    return new Date(value.startDate).getTime() >= new Date(value.endDate).getTime();
  }
}
