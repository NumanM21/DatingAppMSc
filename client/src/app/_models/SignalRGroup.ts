

export interface SignalRGroup {
  groupName: string;
  connectionsInGroup: ConnectionsInGroup[];

}


export interface ConnectionsInGroup {
  connectionId: string;
  username: string;

}