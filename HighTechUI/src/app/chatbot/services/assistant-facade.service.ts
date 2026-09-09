import { Injectable } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { BehaviorSubject, Observable, filter, map, startWith } from 'rxjs';
import { ChatResponse, ChatTurn } from '../../models/chat.model';
import { ConfiguratorFacade } from '../../configurator/services/configurator-facade.service';
import { AssistantApiService } from './assistant-api.service';
import { AuthorizeService } from 'src/api-authorization/services/authorize-facade.service';

@Injectable()
export class AssistantFacade {
    private readonly sessionId: string = crypto.randomUUID();
    private turnsSubject = new BehaviorSubject<ChatTurn[]>([]);
    private isOpenSubject = new BehaviorSubject<boolean>(false);
    private isBusySubject = new BehaviorSubject<boolean>(false);

    public get turns$(): Observable<ChatTurn[]> {
        return this.turnsSubject.asObservable();
    }

    public get isOpen$(): Observable<boolean> {
        return this.isOpenSubject.asObservable();
    }

    public get isBusy$(): Observable<boolean> {
        return this.isBusySubject.asObservable();
    }

    public readonly onConfigurator$: Observable<boolean>;

    constructor(
        private assistantApi: AssistantApiService,
        private configuratorFacade: ConfiguratorFacade,
        private authorizeService: AuthorizeService,
        private router: Router
    ) {
        this.onConfigurator$ = this.router.events.pipe(
            filter(e => e instanceof NavigationEnd),
            map(() => this.isConfiguratorUrl(this.router.url)),
            startWith(this.isConfiguratorUrl(this.router.url))
        );

        this.authorizeService.authorizationChange.subscribe(() => {
            this.turnsSubject.next([]);
            this.isOpenSubject.next(false);
        });
    }

    public togglePanel(): void {
        this.isOpenSubject.next(!this.isOpenSubject.value);
    }

    public openPanel(): void {
        this.isOpenSubject.next(true);
    }

    public closePanel(): void {
        this.isOpenSubject.next(false);
    }

    public sendMessage(text: string): void {
        const trimmed = text?.trim();
        if (!trimmed || this.isBusySubject.value) return;

        const userTurn: ChatTurn = {
            id: this.makeId(),
            role: 'user',
            text: trimmed,
            timestamp: Date.now()
        };
        this.appendTurn(userTurn);

        const onConfigurator = this.isConfiguratorUrl(this.router.url);
        const currentSelection = this.configuratorFacade.getSelectionSnapshot() ?? {};

        this.isBusySubject.next(true);
        this.assistantApi
            .chat({
                sessionId: this.sessionId,
                message: trimmed,
                context: { onConfigurator, currentSelection }
            })
            .subscribe({
                next: response => {
                    this.appendTurn(this.toAssistantTurn(response));
                    this.isBusySubject.next(false);
                },
                error: () => {
                    this.appendTurn({
                        id: this.makeId(),
                        role: 'assistant',
                        text: "Sorry, I couldn't reach the assistant. Please try again.",
                        timestamp: Date.now()
                    });
                    this.isBusySubject.next(false);
                }
            });
    }

    public applyTurn(turnId: string): void {
        const turn = this.findTurn(turnId);
        if (!turn || !turn.action) return;
        if (turn.action.type !== 'pc-creation' && turn.action.type !== 'pc-modification') return;
        if (turn.appliedState === 'applied') return;

        this.configuratorFacade.applyAssistantMessage(turn.action).subscribe({
            next: () => this.updateTurn(turnId, t => ({ ...t, appliedState: 'applied' })),
            error: () => this.updateTurn(turnId, t => ({ ...t, appliedState: 'pending' }))
        });
    }

    public newChat(): void {
        this.assistantApi.newChat({ sessionId: this.sessionId }).subscribe({
            next: () => this.turnsSubject.next([]),
            error: () => this.turnsSubject.next([])
        });
    }

    private toAssistantTurn(response: ChatResponse): ChatTurn {
        const id = this.makeId();
        const timestamp = Date.now();
        const message = response.message;
        const products = response.products ?? {};

        if (message.type === 'chat') {
            return { id, role: 'assistant', text: message.text, timestamp };
        }

        if (message.type === 'pc-creation' || message.type === 'pc-modification') {
            return {
                id,
                role: 'assistant',
                action: message,
                products,
                appliedState: 'pending',
                text: message.rationale,
                timestamp
            };
        }

        // Unknown type — render rationale as plain text.
        return {
            id,
            role: 'assistant',
            text: message.rationale ?? 'Unsupported message type.',
            timestamp
        };
    }

    private appendTurn(turn: ChatTurn): void {
        this.turnsSubject.next([...this.turnsSubject.value, turn]);
    }

    private updateTurn(turnId: string, update: (t: ChatTurn) => ChatTurn): void {
        const next = this.turnsSubject.value.map(t => (t.id === turnId ? update(t) : t));
        this.turnsSubject.next(next);
    }

    private findTurn(turnId: string): ChatTurn | undefined {
        return this.turnsSubject.value.find(t => t.id === turnId);
    }

    private isConfiguratorUrl(url: string): boolean {
        return url?.startsWith('/configurator') ?? false;
    }

    private makeId(): string {
        if (typeof crypto !== 'undefined' && typeof crypto.randomUUID === 'function') {
            return crypto.randomUUID();
        }
        return `${Date.now()}-${Math.random().toString(36).slice(2)}`;
    }
}
