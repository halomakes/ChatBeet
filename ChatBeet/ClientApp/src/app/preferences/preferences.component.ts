import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { tap } from 'rxjs';
import { CurrentUserModel } from '../common/CurrentUserModel';
import { IdentityService } from '../identity.service';
import { SetPreferenceComponent } from '../set-preference/set-preference.component';
import { Preference, PreferenceType } from './preference';
import { PreferencesService } from './preferences.service';

@Component({
  selector: 'app-preferences',
  templateUrl: './preferences.component.html',
  styleUrls: ['./preferences.component.scss']
})
export class PreferencesComponent implements OnInit {
  public user?: CurrentUserModel;
  public preferences: Array<Preference> = [];
  public types = PreferenceType;

  constructor(private identity: IdentityService, private service: PreferencesService, private dialog: MatDialog) { }

  ngOnInit(): void {
    this.identity.getCurrentUser().pipe(
      tap(u => this.user = u)
    ).subscribe();
    this.service.getPreferences().pipe(
      tap(p => this.preferences = p)
    ).subscribe();
  }

  public change = (): void => {
    var ref = this.dialog.open(SetPreferenceComponent);
    ref.afterClosed().subscribe(() => {
      this.ngOnInit();
    });
  }
}
