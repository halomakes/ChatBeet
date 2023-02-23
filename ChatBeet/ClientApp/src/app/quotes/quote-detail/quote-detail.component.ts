import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { combineLatestWith, concatMap, tap } from 'rxjs';
import { IdentityService } from 'src/app/identity.service';
import { Quote } from '../quote';
import { QuoteMessage } from '../quote-message';
import { QuoteService } from '../quote.service';

@Component({
  selector: 'app-quote-detail',
  templateUrl: './quote-detail.component.html',
  styleUrls: ['./quote-detail.component.scss']
})
export class QuoteDetailComponent implements OnInit {
  public quote?: Quote;
  public slug?: string;
  public messages?: Array<QuoteMessage>;

  constructor(private route: ActivatedRoute, private service: QuoteService, private identity: IdentityService) { }

  ngOnInit(): void {
    this.route.params.pipe(
      tap(p => this.slug = p.slug),
      combineLatestWith(this.route.queryParams),
      concatMap(([routeParams, queryParams]) => this.service.getQuote(queryParams.guild || this.identity.selectedGuild?.id, routeParams.slug)),
      tap(r => this.messages = r.messages!.sort((a, b) => a.createdAt.getTime() - b.createdAt.getTime())),
      tap(r => this.quote = r)
    ).subscribe();
  }
}
