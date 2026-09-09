import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatPanelComponent } from './components/chat-panel/chat-panel.component';
import { ChatMessageComponent } from './components/chat-message/chat-message.component';
import { ChatInputComponent } from './components/chat-input/chat-input.component';
import { AssistantApiService } from './services/assistant-api.service';
import { AssistantFacade } from './services/assistant-facade.service';

@NgModule({
    declarations: [
        ChatPanelComponent,
        ChatMessageComponent,
        ChatInputComponent
    ],
    imports: [
        CommonModule,
        FormsModule
    ],
    exports: [
        ChatPanelComponent
    ],
    providers: [
        AssistantApiService,
        AssistantFacade
    ]
})
export class ChatbotModule {}
