import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule, MatIconRegistry } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatSelectModule } from '@angular/material/select';

import { SideDrawerComponent } from './side-drawer/side-drawer.component';
import { KarmaComponent } from './karma/karma.component';
import { DiscordContentComponent } from './discord-content/discord-content.component';
import { DefinitionsComponent } from './definitions/definitions.component';
import { HighGroundComponent } from './high-ground/high-ground.component';
import { CrewmatesComponent } from './crewmates/crewmates.component';
import { ProgressListComponent } from './progress/progress-list/progress-list.component';
import { ProgressPreviewComponent } from './progress/progress-preview/progress-preview.component';
import { EditProgressComponent } from './progress/edit-progress/edit-progress.component';
import { DeleteProgressComponent } from './progress/delete-progress/delete-progress.component';
import { CommandListComponent } from './command-list/command-list.component';
import { PreferencesComponent } from './preferences/preferences.component';
import { SetPreferenceComponent } from './set-preference/set-preference.component';
import { QuoteListComponent } from './quotes/quote-list/quote-list.component';

const MaterialComponents = [
  MatToolbarModule,
  MatButtonModule,
  MatIconModule,
  MatMenuModule,
  MatDividerModule,
  MatSidenavModule,
  MatListModule,
  MatTableModule,
  MatFormFieldModule,
  MatPaginatorModule,
  MatInputModule,
  MatSortModule,
  MatRippleModule,
  MatChipsModule,
  MatProgressBarModule,
  MatCardModule,
  MatDialogModule,
  MatDatepickerModule,
  MatNativeDateModule,
  MatSnackBarModule,
  MatExpansionModule,
  MatSelectModule
]

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    SideDrawerComponent,
    KarmaComponent,
    DiscordContentComponent,
    DefinitionsComponent,
    HighGroundComponent,
    CrewmatesComponent,
    ProgressListComponent,
    ProgressPreviewComponent,
    EditProgressComponent,
    DeleteProgressComponent,
    CommandListComponent,
    PreferencesComponent,
    SetPreferenceComponent,
    QuoteListComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'karma', component: KarmaComponent },
      { path: 'definitions', component: DefinitionsComponent },
      { path: 'high-ground', component: HighGroundComponent },
      { path: 'crewmates', component: CrewmatesComponent },
      { path: 'progress', component: ProgressListComponent },
      { path: 'commands', component: CommandListComponent },
      { path: 'preferences', component: PreferencesComponent },
      { path: 'quotes', component: QuoteListComponent },
      { path: '**', component: HomeComponent }
    ]),
    BrowserAnimationsModule,
    ...MaterialComponents
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(iconRegistry: MatIconRegistry) {
    iconRegistry.setDefaultFontSetClass('material-symbols-outlined');
  }
}
