import { Component, ElementRef, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { Observable, Subscription, skip } from 'rxjs';
import { ConfiguratorPartOption, ConfiguratorValidationResult } from '../../../models/configurator.model';
import { ConfiguratorFacade } from '../../services/configurator-facade.service';
import { RuleCodes } from '../../configurator.utils';

@Component({
  selector: 'app-part-picker',
  templateUrl: './part-picker.component.html',
  styleUrls: ['./part-picker.component.css'],
  standalone: false
})
export class PartPickerComponent implements OnInit, OnChanges, OnDestroy {
  @Input() categoryName: string;
  @Input() currentSelectionId: string | null;
  @Input() validation: ConfiguratorValidationResult | null = null;
  @Output() partSelected = new EventEmitter<string | null>();

  @ViewChild('cardsContainer') cardsContainer!: ElementRef<HTMLDivElement>;

  parts$: Observable<ConfiguratorPartOption[]>;
  searchQuery = '';
  showIncompatible = false;
  private selectionSubscription: Subscription;

  constructor(private facade: ConfiguratorFacade) {}

  ngOnInit(): void {
    this.parts$ = this.facade.getCandidateParts(this.categoryName);
    this.selectionSubscription = this.facade.selection$.pipe(skip(1)).subscribe(() => {
      this.reloadParts();
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['categoryName']) {
      this.showIncompatible = false;
      this.parts$ = this.facade.getCandidateParts(this.categoryName);
      this.reloadParts();
    }
  }

  ngOnDestroy(): void {
    this.selectionSubscription?.unsubscribe();
  }

  selectPart(productId: string): void {
    this.partSelected.emit(productId);
  }

  toggleIncompatible(): void {
    this.showIncompatible = !this.showIncompatible;
    this.reloadParts();
  }

  getFilteredParts(parts: ConfiguratorPartOption[]): ConfiguratorPartOption[] {
    if (!this.searchQuery.trim()) return parts;
    const q = this.searchQuery.toLowerCase();
    return parts.filter(p =>
      (p.manufacturer + ' ' + p.model).toLowerCase().includes(q)
    );
  }

  get compatMessage(): { type: 'success' | 'warning'; text: string } | null {
    if (!this.currentSelectionId || !this.validation) return null;

    const issues = this.validation.issues?.filter(
      i => i.rule !== RuleCodes.COMPLETE_BUILD && i.rule !== RuleCodes.ASSEMBLY_SERVICE &&
           (i.categoryA === this.categoryName || i.categoryB === this.categoryName)
    ) ?? [];

    if (issues.length === 0) {
      return {
        type: 'success',
        text: `Great choice! This ${this.categoryName} is compatible with your selected components.`
      };
    }

    return { type: 'warning', text: issues[0].message };
  }

  scrollLeft(): void {
    if (!this.cardsContainer) return;
    this.cardsContainer.nativeElement.scrollBy({ left: -320, behavior: 'smooth' });
  }

  scrollRight(): void {
    if (!this.cardsContainer) return;
    this.cardsContainer.nativeElement.scrollBy({ left: 320, behavior: 'smooth' });
  }

  private reloadParts(): void {
    if (!this.categoryName) return;
    this.facade.loadPartOptions(this.categoryName, this.showIncompatible).subscribe();
  }
}
