export const AI_MAX_PROMPT_LENGTH = 2000;

export interface GenerateChatReplyCommand {
  prompt: string;
}

export interface GenerateChatReplyResponse {
  reply: string;
}
