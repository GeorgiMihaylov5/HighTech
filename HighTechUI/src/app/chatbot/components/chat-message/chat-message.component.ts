import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ChatTurn, ProductSummary } from '../../../models/chat.model';

interface BuildLine {
    category: string;
    productId: string | null;
    label: string;
    price: number | null;
    isRemoval: boolean;
}

@Component({
    selector: 'app-chat-message',
    templateUrl: './chat-message.component.html',
    styleUrls: ['./chat-message.component.css'],
    standalone: false
})
export class ChatMessageComponent {
    @Input() turn!: ChatTurn;
    @Output() apply = new EventEmitter<string>();

    public get isUser(): boolean {
        return this.turn?.role === 'user';
    }

    public get isAction(): boolean {
        const t = this.turn?.action?.type;
        return t === 'pc-creation' || t === 'pc-modification';
    }

    public get isApplied(): boolean {
        return this.turn?.appliedState === 'applied';
    }

    public get actionLabel(): string {
        if (this.turn?.action?.type === 'pc-creation') return 'Build PC';
        if (this.turn?.action?.type === 'pc-modification') return 'Apply changes';
        return 'Apply';
    }

    public get lines(): BuildLine[] {
        const action = this.turn?.action;
        if (!action) return [];

        if (action.type === 'pc-creation') {
            return Object.entries(action.selection).map(([category, productId]) =>
                this.toLine(category, productId, false)
            );
        }

        if (action.type === 'pc-modification') {
            return action.changes.map(change =>
                this.toLine(change.categoryName, change.productId, change.productId == null)
            );
        }

        return [];
    }

    public get totalPrice(): number {
        return this.lines.reduce((sum, line) => sum + (line.price ?? 0), 0);
    }

    public get hasTotal(): boolean {
        return this.turn?.action?.type === 'pc-creation' || this.lines.some(l => l.price != null);
    }

    public onApply(): void {
        if (!this.isAction || this.isApplied) return;
        this.apply.emit(this.turn.id);
    }

    private toLine(category: string, productId: string | null, isRemoval: boolean): BuildLine {
        const product = this.lookup(productId);
        const label = isRemoval
            ? 'Remove'
            : product
                ? `${product.manufacturer} ${product.model}`
                : (productId ?? 'Unknown product');
        return {
            category,
            productId,
            label,
            price: product?.price ?? null,
            isRemoval
        };
    }

    private lookup(productId: string | null | undefined): ProductSummary | undefined {
        if (!productId) return undefined;
        return this.turn?.products?.[productId];
    }
}
