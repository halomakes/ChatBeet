import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { toHTML } from 'discord-markdown';
import { tap } from 'rxjs';
import { MentionService } from './mention.service';

@Component({
  selector: 'app-discord-content',
  templateUrl: './discord-content.component.html',
  styleUrls: ['./discord-content.component.scss']
})
export class DiscordContentComponent implements OnChanges {
  @Input() content?: string;
  public htmlContent?: string;
  private static users: { [key: string]: string } = {};
  private static channels: { [key: string]: string } = {};
  private static roles: { [key: string]: string } = {};

  constructor(private mentions: MentionService) { }

  ngOnChanges(changes: SimpleChanges): void {
    this.refreshContent(changes.content.currentValue);
  }

  private refreshContent = (content?: string): void => {
    if (!content) {
      this.htmlContent = '';
      return;
    }
    this.htmlContent = toHTML(content, {
      discordCallback: {
        user: node => this.loadUser(node.id),
        channel: node => this.loadChannel(node.id),
        role: node => this.loadRole(node.id)
      }
    });
  }

  private loadUser = (id: string): string => {
    if (DiscordContentComponent.users[id])
      return `@${DiscordContentComponent.users[id]}`;
    else {
      this.mentions.getUser(id).pipe(
        tap(m => DiscordContentComponent.users[id] = m)
      ).subscribe(() => this.refreshContent(this.content));
      return `@${id}`;
    }
  }

  private loadChannel = (id: string): string => {
    if (DiscordContentComponent.channels[id])
      return `#${DiscordContentComponent.channels[id]}`;
    else {
      this.mentions.getChannel(id).pipe(
        tap(m => DiscordContentComponent.channels[id] = m)
      ).subscribe(() => this.refreshContent(this.content));
      return `#${id}`;
    }
  }

  private loadRole = (id: string): string => {
    if (DiscordContentComponent.roles[id])
      return `@${DiscordContentComponent.roles[id]}`;
    else {
      this.mentions.getRole(id).pipe(
        tap(m => DiscordContentComponent.roles[id] = m)
      ).subscribe(() => this.refreshContent(this.content));
      return `@${id}`;
    }
  }
}
