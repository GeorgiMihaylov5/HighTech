import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ConfiguratorCategory, ConfiguratorSelection, ConfiguratorValidationResult } from '../../../models/configurator.model';
import { OrderedProduct } from '../../../models/order.model';
import { getCategoryIcon, RuleCodes } from '../../configurator.utils';

@Component({
  selector: 'app-build-summary',
  templateUrl: './build-summary.component.html',
  styleUrls: ['./build-summary.component.css'],
  standalone: false
})
export class BuildSummaryComponent {
  @Input() categories: ConfiguratorCategory[] = [];
  @Input() selection: ConfiguratorSelection | null = null;
  @Input() validation: ConfiguratorValidationResult | null = null;
  @Input() isBusy = false;
  @Input() addResult: 'success' | 'partial' | null = null;
  @Output() addToBasket = new EventEmitter<void>();
  @Output() clearAll = new EventEmitter<void>();
  @Output() selectCategory = new EventEmitter<string>();
  @Output() fixWithAi = new EventEmitter<string[]>();

  get selectedItems(): { categoryName: string; productId: string }[] {
    if (!this.selection) return [];
    return Object.entries(this.selection)
      .filter(([categoryName]) => categoryName !== 'Assembly')
      .map(([categoryName, productId]) => ({ categoryName, productId }));
  }

  get totalPrice(): number {
    if (!this.validation?.resolvedItems) return 0;
    return this.validation.resolvedItems.reduce((sum, item) => sum + (item.orderedPrice || 0), 0);
  }

  get hasSelection(): boolean {
    return this.selectedItems.length > 0;
  }

  get hasSummaryItems(): boolean {
    return this.hasSelection || this.defaultServiceItems.length > 0;
  }

  get isCompleteBuild(): boolean {
    return this.missingCategoryNames.size === 0;
  }

  get compatibilityIssues() {
    return this.validation?.issues?.filter(issue => issue.rule !== RuleCodes.COMPLETE_BUILD) ?? [];
  }

  get defaultServiceItems(): OrderedProduct[] {
    return this.validation?.resolvedItems?.filter(item => item.product?.categoryName === 'Assembly') ?? [];
  }

  get canCommit(): boolean {
    return (this.validation?.isValid === true) && this.hasSelection && !this.isBusy;
  }

  get missingCategories(): ConfiguratorCategory[] {
    if (this.validation?.issues) {
      const missingNames = this.missingCategoryNames;
      return this.categories.filter(c => missingNames.has(c.name));
    }

    if (!this.selection) return this.categories.filter(c => c.name !== 'Assembly');
    const selectedSet = new Set(Object.keys(this.selection));
    return this.categories.filter(c => c.name !== 'Assembly' && !selectedSet.has(c.name));
  }

  get integratedGraphicsLabel(): string | null {
    if (this.selection?.['GPU']) return null;
    if (!this.validation?.hasIntegratedGraphics) return null;

    const cpuItem = this.validation?.resolvedItems?.find(item => item.product?.categoryName === 'CPU');
    const value = cpuItem?.product?.fields?.find(f => f.name === 'Graphic Core')?.value;
    return value != null ? String(value).trim() || null : null;
  }

  get estimatedPower(): number {
    const powerKeywords = ['tdp', 'power', 'watt', 'tgp'];
    let total = 0;
    for (const item of this.validation?.resolvedItems ?? []) {
      if (item.product?.categoryName === 'Assembly') continue;
      for (const field of item.product?.fields ?? []) {
        if (powerKeywords.some(kw => field.name.toLowerCase().includes(kw))) {
          const val = Number(field.value);
          if (!isNaN(val) && val > 0) { total += val; break; }
        }
      }
    }
    return total;
  }

  getProductName(productId: string): string {
    const item = this.validation?.resolvedItems?.find(r => r.productId === productId);
    if (!item?.product) return productId;
    return `${item.product.manufacturer} ${item.product.model}`;
  }

  getProductPrice(productId: string): number {
    const item = this.validation?.resolvedItems?.find(r => r.productId === productId);
    return item?.orderedPrice ?? 0;
  }

  getCategoryIcon(name: string): string {
    return getCategoryIcon(name);
  }

  getMissingMessage(categoryName: string): string {
    const messages: { [key: string]: string } = {
      'CPU': 'Select a CPU to power your build',
      'Motherboard': 'Select a Motherboard for your build',
      'GPU': 'Select a GPU for graphics performance',
      'RAM': 'Select RAM for better performance',
      'Storage': 'Select Storage to complete your build',
      'Case': 'Select a Case to house your build',
      'PSU': 'Select PSU to power your system',
      'CPU Cooler': 'Select CPU Cooler to keep your system cool'
    };
    return messages[categoryName] || `Select a ${categoryName} to complete your build`;
  }

  private get missingCategoryNames(): Set<string> {
    const names = this.validation?.issues
      ?.filter(issue => issue.rule === RuleCodes.COMPLETE_BUILD && issue.categoryA)
      .map(issue => issue.categoryA) ?? [];
    return new Set(names);
  }

  onAddToBasket(): void { this.addToBasket.emit(); }
  onClearAll(): void { this.clearAll.emit(); }
  onSelectCategory(categoryName: string): void { this.selectCategory.emit(categoryName); }
  onFixWithAi(): void {
    const messages = this.compatibilityIssues
      .map(issue => issue.message)
      .filter((m): m is string => !!m);
    this.fixWithAi.emit(messages);
  }
}
