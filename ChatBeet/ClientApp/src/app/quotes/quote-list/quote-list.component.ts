import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { concatMap, of, tap } from 'rxjs';
import { IdentityService } from 'src/app/identity.service';
import { Quote } from '../quote';
import { QuoteService } from '../quote.service';

@Component({
  selector: 'app-quote-list',
  templateUrl: './quote-list.component.html',
  styleUrls: ['./quote-list.component.scss']
})
export class QuoteListComponent implements OnInit {
  displayedColumns: string[] = ['createdAt', 'channelName', 'savedBy', 'slug'];
  dataSource: MatTableDataSource<Quote>;

  @ViewChild(MatPaginator) paginator?: MatPaginator;
  @ViewChild(MatSort) sort?: MatSort;

  constructor(private service: QuoteService, private identity: IdentityService) {
    this.dataSource = new MatTableDataSource();
  }

  ngOnInit(): void {
    this.identity.guildChanges.pipe(
      concatMap(guild => guild?.id ? this.service.getQuotes(guild?.id) : of([])),
      tap(quotes => this.dataSource.data = quotes)
    ).subscribe();
  }
}
