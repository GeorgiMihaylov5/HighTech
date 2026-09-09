export type AssistantMessage =
  | { type: 'chat'; text: string; rationale?: string }
  | { type: 'pc-creation'; selection: { [categoryName: string]: string }; rationale?: string }
  | { type: 'pc-modification'; changes: Array<{ categoryName: string; productId: string | null }>; rationale?: string }
  | { type: 'recommendation'; categoryName: string; productIds: string[]; rationale?: string };
