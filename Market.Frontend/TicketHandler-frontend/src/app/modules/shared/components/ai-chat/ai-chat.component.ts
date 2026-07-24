import { Component, ElementRef, ViewChild, inject } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { BaseComponent } from '../../../../core/components/base-classes/base-component';
import { AiApiService } from '../../../../api-services/ai/ai-api.service';
import { AI_MAX_PROMPT_LENGTH } from '../../../../api-services/ai/ai-api.model';
import { CurrentUserService } from '../../../../core/services/auth/current-user.service';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';

export type AiChatRole = 'user' | 'assistant';

export interface AiChatMessage {
  role: AiChatRole;
  content: string;
  timestamp: Date;
}

@Component({
  selector: 'app-ai-chat',
  standalone: false,
  templateUrl: './ai-chat.component.html',
  styleUrl: './ai-chat.component.scss',
})
export class AiChatComponent extends BaseComponent {
  private readonly api = inject(AiApiService);
  private readonly currentUserService = inject(CurrentUserService);
  private readonly auth = inject(AuthFacadeService);

  isAuthenticated = this.currentUserService.isAuthenticated;

  readonly maxPromptLength = AI_MAX_PROMPT_LENGTH;

  isOpen = false;

  prompt = '';

  messages: AiChatMessage[] = [];

  suggestions: string[] = [
    'What events are happening this weekend?',
    'How do I get a refund for a ticket?',
    'Where can I find my QR code?',
  ];

  @ViewChild('messageList') private messageList?: ElementRef<HTMLElement>;

  toggle(): void {
    this.isOpen = !this.isOpen;
    if (this.isOpen) this.scrollToBottom();
  }

  close(): void {
    this.isOpen = false;
  }

  signIn(): void {
    this.auth.redirectToLogin();
  }

  send(): void {
    const prompt = this.prompt.trim();
    if (!prompt || this.isLoading || !this.isAuthenticated()) return;

    this.prompt = '';
    this.messages.push({ role: 'user', content: prompt, timestamp: new Date() });
    this.ask(prompt);
  }

  useSuggestion(suggestion: string): void {
    if (this.isLoading || !this.isAuthenticated()) return;
    this.prompt = suggestion;
    this.send();
  }

  clearConversation(): void {
    if (this.isLoading) return;
    this.messages = [];
    this.errorMessage = null;
  }

  retry(): void {
    if (this.isLoading || !this.isAuthenticated()) return;

    const lastPrompt = [...this.messages].reverse().find(m => m.role === 'user');
    if (!lastPrompt) return;

    this.ask(lastPrompt.content);
  }

  onKeydown(event: KeyboardEvent): void {
    if (event.key !== 'Enter' || event.shiftKey) return;
    event.preventDefault();
    this.send();
  }

  trackByIndex(index: number): number {
    return index;
  }

  private ask(prompt: string): void {
    this.startLoading();
    this.scrollToBottom();

    this.api.chat({ prompt }).subscribe({
      next: ({ reply }) => {
        this.messages.push({ role: 'assistant', content: reply, timestamp: new Date() });
        this.stopLoading();
        this.scrollToBottom();
      },
      error: (error: HttpErrorResponse) => {
        this.stopLoading(this.toErrorMessage(error));
        this.scrollToBottom();
      },
    });
  }

  private toErrorMessage(error: HttpErrorResponse): string {
    switch (error.status) {
      case 401:
      case 403:
        return 'Sign in to use the assistant.';
      case 429:
        return 'Too many messages — please wait a moment and try again.';
      case 400:
        return error.error?.message ?? 'That message could not be sent.';
      default:
        return 'The assistant is unavailable right now.';
    }
  }

  private scrollToBottom(): void {
    setTimeout(() => {
      const list = this.messageList?.nativeElement;
      if (list) list.scrollTop = list.scrollHeight;
    });
  }
}
