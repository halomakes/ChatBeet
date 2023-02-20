import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { concatMap, of, tap } from 'rxjs';
import { IdentityService } from '../identity.service';
import { Definition } from './definition';
import { DefinitionsService } from './definitions.service';

@Component({
  selector: 'app-definitions',
  templateUrl: './definitions.component.html',
  styleUrls: ['./definitions.component.scss']
})
export class DefinitionsComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['key', 'authorName', 'updatedAt'];
  dataSource: MatTableDataSource<Definition> = new MatTableDataSource();

  @ViewChild(MatPaginator) paginator?: MatPaginator;
  @ViewChild(MatSort) sort?: MatSort;

  constructor(private defs: DefinitionsService, private identity: IdentityService) { }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator!;
    this.dataSource.sort = this.sort!;
  }

  ngOnInit(): void {
    this.identity.guildChanges.pipe(
      concatMap(guild => guild?.id ? this.defs.getDefinitions(guild?.id) : of(undefined)),
      tap(defs => {
        if (defs) {
          this.dataSource.data = defs;
        }
      })
    ).subscribe();
  }

  public applyFilter = (event: Event): void => {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }
}
