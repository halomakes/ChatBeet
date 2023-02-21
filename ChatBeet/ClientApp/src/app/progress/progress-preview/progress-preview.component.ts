import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { ProgressCompletion } from '../progress-completion';
import { ProgressSpan } from '../progress-span';

@Component({
  selector: 'app-progress-preview',
  templateUrl: './progress-preview.component.html',
  styleUrls: ['./progress-preview.component.scss']
})
export class ProgressPreviewComponent implements OnChanges {
  @Input() public mode?: 'bar' | 'text';
  @Input() public span: ProgressSpan | undefined;
  public completion?: ProgressCompletion;

  constructor() { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.span.currentValue !== changes.span.previousValue) {
      this.completion = changes.span.currentValue
        ? new ProgressCompletion(changes.span.currentValue)
        : undefined;
    }
  }
}
