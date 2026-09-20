// ==============Queries==============

export interface GetCartQueryDto {
  items: GetCartQueryDtoItem[];
  /** Lines parked with "save for later" — priced, but outside the totals and checkout. */
  savedItems: GetCartQueryDtoItem[];
  /** Number of distinct ticket lines in the cart. */
  lineCount: number;
  savedLineCount: number;
  totalQuantity: number;
  totalAmount: number;
}

export interface GetCartQueryDtoItem {
  ticketId: number;
  event: GetCartQueryDtoEvent;
  ticketType: GetCartQueryDtoTicketType;
  quantity: number;
  unitPrice: number;
  subtotal: number;
  quantityInStock: number;
  benefits: string;
  addedAtUtc: string;
  isSavedForLater: boolean;
}

export interface GetCartQueryDtoEvent {
  id: number;
  name: string;
  scheduledDate: string; 
  venueName: string;
  image: string;
}

export interface GetCartQueryDtoTicketType {
  id: number;
  name: string;
}

// ==============Commands==============

export interface AddCartItemCommand {
  ticketId: number;
  quantity: number;
}

export interface UpdateCartItemCommand {
  quantity: number;
}
