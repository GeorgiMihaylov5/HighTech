import { AfterViewChecked, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';
import { ChatTurn } from '../../../models/chat.model';
import { AssistantFacade } from '../../services/assistant-facade.service';
import { AuthorizeService } from 'src/api-authorization/services/authorize-facade.service';

@Component({
    selector: 'app-chat-panel',
    templateUrl: './chat-panel.component.html',
    styleUrls: ['./chat-panel.component.css'],
    standalone: false
})
export class ChatPanelComponent implements OnInit, AfterViewChecked {
    @ViewChild('scrollArea') private scrollArea?: ElementRef<HTMLDivElement>;

    public turns$: Observable<ChatTurn[]>;
    public isOpen$: Observable<boolean>;
    public isBusy$: Observable<boolean>;
    public isAuthenticated: Observable<boolean>;
    public showFabLabel = true;

    constructor(
        public assistantFacade: AssistantFacade,
        private authorizeService: AuthorizeService
    ) {
        this.turns$ = this.assistantFacade.turns$;
        this.isOpen$ = this.assistantFacade.isOpen$;
        this.isBusy$ = this.assistantFacade.isBusy$;
        this.isAuthenticated = this.authorizeService.isAuthenticated();
    }

    ngOnInit(): void {}

    ngAfterViewChecked(): void {
        if (!this.scrollArea) return;
        const el = this.scrollArea.nativeElement;
        if (el.scrollHeight > el.clientHeight) {
            el.scrollTop = el.scrollHeight;
        }
    }

    public trackTurn(_: number, turn: ChatTurn): string {
        return turn.id;
    }

    public onSend(text: string): void {
        this.assistantFacade.sendMessage(text);
    }

    public onApply(turnId: string): void {
        this.assistantFacade.applyTurn(turnId);
    }

    public onNewChat(): void {
        this.assistantFacade.newChat();
    }

    public onClose(): void {
        this.assistantFacade.closePanel();
    }

    public onToggle(): void {
        this.assistantFacade.togglePanel();
    }

    public dismissFabLabel(): void {
        this.showFabLabel = false;
    }
}
