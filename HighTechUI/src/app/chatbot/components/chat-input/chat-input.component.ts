import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
    selector: 'app-chat-input',
    templateUrl: './chat-input.component.html',
    styleUrls: ['./chat-input.component.css'],
    standalone: false
})
export class ChatInputComponent {
    @Input() disabled = false;
    @Output() send = new EventEmitter<string>();

    public draft = '';

    public submit(): void {
        const text = this.draft.trim();
        if (!text || this.disabled) return;
        this.send.emit(text);
        this.draft = '';
    }

    public onKeyDown(event: KeyboardEvent): void {
        if (event.key === 'Enter' && !event.shiftKey) {
            event.preventDefault();
            this.submit();
        }
    }
}
