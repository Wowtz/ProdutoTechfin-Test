export interface DeleteBatchResult {
  totalRequested: number;
  totalDeleted: number;
  notFoundIds: string[];
}