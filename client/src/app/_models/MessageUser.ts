export interface MessageUser {
  messageId: number
  messageSenderId: number
  messageSenderUsername: string
  messageSenderPhotoURL: any
  messageReceivingId: number
  messageReceivingUsername: string
  messageReceivingPhotoURL: string
  messageContent: string
  messageReadAt: Date;
  messageSentAt: string
}
