import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Preference, PreferenceType } from './preference';

@Injectable({
  providedIn: 'root'
})
export class PreferencesService {

  constructor(private http: HttpClient) { }

  public getPreferences = (): Observable<Array<Preference>> => this.http.get<Array<Preference>>('/api/preferences');

  public setPreference = (value: { preference: PreferenceType, value: string }): Observable<any> => this.http.put('/api/preferences', { preference: Number(value.preference), value: value.value });
}
