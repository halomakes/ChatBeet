import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { tap } from 'rxjs';
import { CurrentUserModel } from '../common/CurrentUserModel';
import { IdentityService } from '../identity.service';
import { PreferenceType } from '../preferences/preference';
import { PreferencesService } from '../preferences/preferences.service';

@Component({
  selector: 'app-set-preference',
  templateUrl: './set-preference.component.html',
  styleUrls: ['./set-preference.component.scss']
})
export class SetPreferenceComponent implements OnInit {
  public validationMessage?: string;
  public types: Array<number> = <Array<number>><unknown>Object.keys(PreferenceType).filter((item) => !isNaN(Number(item)));
  public preferenceType = PreferenceType;
  private user?: CurrentUserModel;
  public selectedType?: PreferenceType;
  public newValue?: string;

  constructor(public dialogRef: MatDialogRef<SetPreferenceComponent>, private service: PreferencesService, private snackbar: MatSnackBar, private identity: IdentityService) { }

  ngOnInit(): void {
    this.identity.getCurrentUser().pipe(
      tap(u => this.user = u)
    ).subscribe();
  }

  public save = (): void => {
    if (this.selectedType && this.newValue) {
      this.service.setPreference({ preference: this.selectedType, value: this.newValue }).subscribe(
        () => {
          this.snackbar.open('Preference saved.', undefined, {
            duration: 5000
          });
          this.dialogRef.close();
        },
        (err: any) => {
          if (err instanceof HttpErrorResponse && err.status === 400) {
            this.validationMessage = err.error;
          } else {
            this.validationMessage = undefined;
            this.snackbar.open('Something went wrong.', undefined, {
              duration: 5000
            });
          }
        }
      )
    }
  }
}
