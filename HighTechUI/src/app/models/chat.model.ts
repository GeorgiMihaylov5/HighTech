import { AssistantMessage } from './assistant-message.model';

export interface ChatContext {
    onConfigurator: boolean;
    currentSelection: { [categoryName: string]: string };
}

export interface ChatRequest {
    sessionId: string;
    message: string;
    context: ChatContext;
}

export interface NewChatRequest {
    sessionId: string;
}

export interface ProductSummary {
    id: string;
    manufacturer: string;
    model: string;
    price: number;
}

export interface ChatResponse {
    message: AssistantMessage;
    products: { [productId: string]: ProductSummary };
}

export type ChatTurnAppliedState = 'pending' | 'applied';

export interface ChatTurn {
    id: string;
    role: 'user' | 'assistant';
    text?: string;
    action?: AssistantMessage;
    products?: { [productId: string]: ProductSummary };
    appliedState?: ChatTurnAppliedState;
    timestamp: number;
}
