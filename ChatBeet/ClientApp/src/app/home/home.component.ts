import { Component, OnInit } from '@angular/core';
import { tap } from 'rxjs';
import { ServerService } from '../server.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  public version?: string;
  constructor(private server: ServerService) { }

  ngOnInit(): void {
    this.server.getVersion().pipe(
      tap(v => this.version = v)
    ).subscribe();
  }
}
