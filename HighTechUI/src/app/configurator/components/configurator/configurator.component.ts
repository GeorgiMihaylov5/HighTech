import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { combineLatest, Observable } from 'rxjs';
import { startWith } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { ConfiguratorCategory, ConfiguratorSelection, ConfiguratorValidationResult } from '../../../models/configurator.model';
import { ConfiguratorFacade } from '../../services/configurator-facade.service';
import { OrderedProduct } from '../../../models/order.model';
import { getCategoryIcon, RuleCodes } from '../../configurator.utils';
import { AssistantFacade } from '../../../chatbot/services/assistant-facade.service';

@Component({
  selector: 'app-configurator',
  templateUrl: './configurator.component.html',
  styleUrls: ['./configurator.component.css'],
  standalone: false
})
export class ConfiguratorComponent implements OnInit {
  vm$: Observable<{
    categories: ConfiguratorCategory[];
    selection: ConfiguratorSelection;
    validation: ConfiguratorValidationResult | null;
    isBusy: boolean;
  }>;
  expandedCategory: string | null = null;
  addingToBasket = false;
  addResult: 'success' | 'partial' | null = null;

  constructor(
    private facade: ConfiguratorFacade,
    private router: Router,
    private toastr: ToastrService,
    private assistantFacade: AssistantFacade
  ) {}

  ngOnInit(): void {
    this.vm$ = combineLatest({
      categories: this.facade.categories$,
      selection: this.facade.selection$,
      validation: this.facade.validation$.pipe(startWith(null)),
      isBusy: this.facade.isBusy$
    });
    this.facade.loadCategories().subscribe(() => {
      this.facade.validate().subscribe();
    });
  }

  toggleCategory(categoryName: string): void {
    this.expandedCategory = this.expandedCategory === categoryName ? null : categoryName;
  }

  onPartSelected(categoryName: string, productId: string | null): void {
    if (productId == null) {
      this.removePart(categoryName);
      return;
    }
    this.facade.selectPart(categoryName, productId);
    this.expandedCategory = null;
  }

  removePart(categoryName: string): void {
    this.facade.clearPart(categoryName);
    if (this.expandedCategory === categoryName) {
      this.expandedCategory = null;
    }
  }

  addToBasket(): void {
    this.addingToBasket = true;
    this.addResult = null;

    this.facade.addToBasket().subscribe({
      next: result => {
        this.addingToBasket = false;
        if (result === 'unauthorized') {
          this.toastr.info('Please sign in to add this configuration to your basket.');
          this.router.navigate(['/authentication/login']);
        } else if (result === 'success') {
          this.addResult = 'success';
          this.facade.clearDraft();
          this.toastr.success('PC configuration added to basket.');
        } else {
          this.toastr.info('Complete a valid PC configuration before adding it to basket.');
        }
      },
      error: () => {
        this.addingToBasket = false;
        this.toastr.error('Could not add the PC configuration to basket.');
      }
    });
  }

  clearAll(): void {
    this.facade.clearDraft();
  }

  fixWithAi(issueMessages: string[]): void {
    const issuesText = issueMessages.length > 0
      ? `Compatibility issues:\n- ${issueMessages.join('\n- ')}`
      : '';
    const prompt = `Fix the compatibility issues in my current PC build.${issuesText ? '\n\n' + issuesText : ''}`;

    this.assistantFacade.openPanel();
    this.assistantFacade.sendMessage(prompt);
  }

  getCategoryIcon(name: string): string {
    return getCategoryIcon(name);
  }

  getCategorySubtitle(name: string): string {
    const subtitles: { [key: string]: string } = {
      'cpu': 'Processor',
      'motherboard': 'ATX, Micro-ATX, Mini-ITX',
      'gpu': 'Graphics Card',
      'ram': 'Memory',
      'storage': 'SSD, HDD',
      'case': 'ATX, Micro-ATX, Mini-ITX',
      'psu': 'Power Supply',
      'cpu cooler': 'Air or Liquid cooler',
      'assembly': 'Build service'
    };
    return subtitles[(name ?? '').trim().toLowerCase()] ?? '';
  }

  getResolvedItem(validation: ConfiguratorValidationResult | null, productId: string): OrderedProduct | undefined {
    return validation?.resolvedItems?.find(r => r.productId === productId);
  }

  getSpecsString(item: OrderedProduct | undefined): string {
    if (!item?.product?.fields?.length) return '';
    return item.product.fields.slice(0, 3).map(f => String(f.value)).join(', ');
  }

  hasCompatibilityIssue(validation: ConfiguratorValidationResult | null, categoryName: string): boolean {
    if (!validation?.issues) return false;
    const normalized = (categoryName ?? '').trim().toLowerCase();
    return validation.issues.some(
      i => i.rule !== RuleCodes.COMPLETE_BUILD && i.rule !== RuleCodes.ASSEMBLY_SERVICE &&
           ((i.categoryA ?? '').trim().toLowerCase() === normalized ||
            (i.categoryB ?? '').trim().toLowerCase() === normalized)
    );
  }
}
