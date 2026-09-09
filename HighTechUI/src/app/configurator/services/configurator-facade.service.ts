import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, catchError, forkJoin, map, of, switchMap, tap } from 'rxjs';
import { ConfiguratorCategory, ConfiguratorPartOption, ConfiguratorSelection, ConfiguratorValidationResult } from '../../models/configurator.model';
import { OrderedProduct } from '../../models/order.model';
import { AssistantMessage } from '../../models/assistant-message.model';
import { ConfiguratorApiService } from './configurator-api.service';
import { OverviewFacade } from '../../overview/services/overview-facade.service';
import { AuthorizeService } from 'src/api-authorization/services/authorize-facade.service';
import { getStoredToken } from '../../core/token.util';

export type AddToBasketResult = 'success' | 'unauthorized' | 'invalid';

const STORAGE_PREFIX = 'configurator';

@Injectable()
export class ConfiguratorFacade {
    private categoriesSubject = new BehaviorSubject<ConfiguratorCategory[]>([]);
    private selectionSubject = new BehaviorSubject<ConfiguratorSelection>({});
    private candidatePartsSubject = new BehaviorSubject<{ [category: string]: ConfiguratorPartOption[] }>({});
    private validationSubject = new BehaviorSubject<ConfiguratorValidationResult | null>(null);
    private isBusySubject = new BehaviorSubject<boolean>(false);

    public get categories$(): Observable<ConfiguratorCategory[]> {
        return this.categoriesSubject.asObservable();
    }

    public get selection$(): Observable<ConfiguratorSelection> {
        return this.selectionSubject.asObservable();
    }

    public get validation$(): Observable<ConfiguratorValidationResult | null> {
        return this.validationSubject.asObservable();
    }

    public get isBusy$(): Observable<boolean> {
        return this.isBusySubject.asObservable();
    }

    public getCandidateParts(categoryName: string): Observable<ConfiguratorPartOption[]> {
        return this.candidatePartsSubject.pipe(
            map(all => all[categoryName] ?? [])
        );
    }

    constructor(
        private configuratorApi: ConfiguratorApiService,
        private overviewFacade: OverviewFacade,
        private authorizeService: AuthorizeService
    ) {
        this.loadActiveDraft();
        this.authorizeService.authorizationChange.subscribe(() => this.loadActiveDraft());
    }

    public loadCategories(): Observable<ConfiguratorCategory[]> {
        return this.invokeAsync(
            this.configuratorApi.getCategories().pipe(
                tap(cats => this.categoriesSubject.next(cats))
            )
        );
    }

    public selectPart(categoryName: string, productId: string): void {
        const selection = { ...this.selectionSubject.value, [categoryName]: productId };
        this.selectionSubject.next(selection);
        this.persistDraft(selection);
        this.validate().subscribe();
    }

    public clearPart(categoryName: string): void {
        const selection = { ...this.selectionSubject.value };
        delete selection[categoryName];
        this.selectionSubject.next(selection);
        this.persistDraft(selection);
        this.validate().subscribe();
    }

    public loadCompatibleParts(categoryName: string): Observable<ConfiguratorPartOption[]> {
        return this.loadPartOptions(categoryName, false);
    }

    /**
     * Loads parts for a category, using a single server call to getCompatibleParts for the
     * filtered case and getParts + getCompatibleParts (2 calls) when incompatible parts
     * should also be shown. This replaces the previous O(N) validate-per-part fan-out.
     */
    public loadPartOptions(categoryName: string, includeIncompatible: boolean): Observable<ConfiguratorPartOption[]> {
        const cleared = { ...this.candidatePartsSubject.value };
        delete cleared[categoryName];
        this.candidatePartsSubject.next(cleared);

        const selection = this.selectionSubject.value;

        const compatibleParts$ = this.configuratorApi.getCompatibleParts(categoryName, selection).pipe(
            map(parts => parts.map(p => ({ ...p, isCompatible: true } as ConfiguratorPartOption)))
        );

        const parts$: Observable<ConfiguratorPartOption[]> = includeIncompatible
            ? forkJoin({
                all: this.configuratorApi.getParts(categoryName),
                compatible: this.configuratorApi.getCompatibleParts(categoryName, selection)
              }).pipe(
                map(({ all, compatible }) => {
                    const compatibleIds = new Set(compatible.map(p => p.id));
                    return all.map(p => ({ ...p, isCompatible: compatibleIds.has(p.id) } as ConfiguratorPartOption));
                })
              )
            : compatibleParts$;

        return this.invokeAsync(
            parts$.pipe(
                tap(parts => {
                    const all = { ...this.candidatePartsSubject.value, [categoryName]: parts };
                    this.candidatePartsSubject.next(all);
                })
            )
        );
    }

    public validate(): Observable<ConfiguratorValidationResult> {
        const selection = this.selectionSubject.value;

        return this.invokeAsync(
            this.configuratorApi.validate(selection).pipe(
                tap(result => this.validationSubject.next(result))
            )
        );
    }

    public addToBasket(): Observable<AddToBasketResult> {
        return this.validate().pipe(
            switchMap(result => {
                if (!result.isValid) {
                    return of<AddToBasketResult>('invalid');
                }

                const username = getStoredToken()?.nameid?.trim();
                if (!username) {
                    return of<AddToBasketResult>('unauthorized');
                }

                const buildId = crypto.randomUUID();

                for (const item of result.resolvedItems) {
                    const orderedProduct: OrderedProduct = {
                        ...item,
                        buildId,
                        count: item.count ?? 1
                    };
                    this.overviewFacade.addToBasket(orderedProduct);
                }

                return of<AddToBasketResult>('success');
            })
        );
    }

    public clearDraft(): void {
        const selection: ConfiguratorSelection = {};
        this.selectionSubject.next(selection);
        this.candidatePartsSubject.next({});
        this.persistDraft(selection);
        this.validate().subscribe();
    }

    public getSelectionSnapshot(): ConfiguratorSelection {
        return { ...this.selectionSubject.value };
    }

    public applyAssistantMessage(msg: AssistantMessage): Observable<ConfiguratorValidationResult> {
        switch (msg.type) {
            case 'chat':
                return this.validate();

            case 'pc-creation': {
                const selection: ConfiguratorSelection = {};
                for (const [category, productId] of Object.entries(msg.selection)) {
                    const key = this.resolveCategoryKey(category);
                    selection[key] = productId;
                }
                this.selectionSubject.next(selection);
                this.persistDraft(selection);
                return this.validate();
            }

            case 'pc-modification': {
                const selection = { ...this.selectionSubject.value };
                for (const change of msg.changes) {
                    const key = this.resolveCategoryKey(change.categoryName);
                    if (change.productId == null) {
                        delete selection[key];
                    } else {
                        selection[key] = change.productId;
                    }
                }
                this.selectionSubject.next(selection);
                this.persistDraft(selection);
                return this.validate();
            }

            case 'recommendation':
                return this.validate();

            default:
                return this.validate();
        }
    }

    private resolveCategoryKey(name: string): string {
        if (!name) return name;
        const match = this.categoriesSubject.value.find(
            c => c.name?.toLowerCase() === name.toLowerCase()
        );
        return match?.name ?? name;
    }

    private persistDraft(selection: ConfiguratorSelection): void {
        const key = this.getDraftKey();
        if (key) {
            sessionStorage.setItem(key, JSON.stringify(selection));
        }
    }

    private loadDraft(): ConfiguratorSelection {
        const key = this.getDraftKey();
        if (!key) return {};

        const json = sessionStorage.getItem(key);
        if (!json) return {};

        try {
            return JSON.parse(json) as ConfiguratorSelection;
        } catch {
            return {};
        }
    }

    private loadActiveDraft(): void {
        this.selectionSubject.next(this.loadDraft());
        this.candidatePartsSubject.next({});
        this.validationSubject.next(null);
    }

    private getDraftKey(): string {
        const username = getStoredToken()?.nameid?.trim();
        return username ? `${STORAGE_PREFIX}:${username}` : `${STORAGE_PREFIX}:guest`;
    }

    private invokeAsync<T>(action: Observable<T>): Observable<T> {
        this.isBusySubject.next(true);
        return action.pipe(
            tap(() => this.isBusySubject.next(false)),
            catchError(err => {
                this.isBusySubject.next(false);
                throw err;
            })
        );
    }
}
