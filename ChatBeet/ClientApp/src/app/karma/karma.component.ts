import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { concatMap, of, tap } from 'rxjs';
import { IdentityService } from '../identity.service';
import { KarmaLevel, KarmaLevels } from './karma';
import { KarmaService } from './karma.service';

@Component({
  selector: 'app-karma',
  templateUrl: './karma.component.html',
  styleUrls: ['./karma.component.scss']
})
export class KarmaComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['key', 'level'];
  dataSource: MatTableDataSource<KarmaLevel>;

  @ViewChild(MatPaginator) paginator?: MatPaginator;
  @ViewChild(MatSort) sort?: MatSort;

  constructor(private karma: KarmaService, private identity: IdentityService) {
    this.dataSource = new MatTableDataSource();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator!;
    this.dataSource.sort = this.sort!;
  }

  ngOnInit(): void {
    this.identity.guildChanges.pipe(
      concatMap(guild => guild?.id ? this.karma.getKarmaLevels(guild?.id) : of(undefined)),
      tap(levels => {
        if (levels) {
          this.dataSource.data = Object.keys(levels).map(k => new KarmaLevel(k, levels[k]));
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
