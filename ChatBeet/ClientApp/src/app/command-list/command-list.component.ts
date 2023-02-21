import { Component, OnInit } from '@angular/core';
import { CommandsService } from './commands.service';

@Component({
  selector: 'app-command-list',
  templateUrl: './command-list.component.html',
  styleUrls: ['./command-list.component.scss']
})
export class CommandListComponent implements OnInit {
  public commands: Array<any> = [];

  constructor(private service: CommandsService) { }

  ngOnInit(): void {
    this.service.getCommands().subscribe(v => {
      this.commands = v;
      console.log(this.commands);
    });
  }

  public getOptions = (command: any): Array<any> => command.options ? command.options?.filter((o: any) => o.type > 2) : [];

  public getSubCommands = (command: any): Array<any> => command.options ? command.options?.filter((o: any) => o.type <= 2): [];
}
