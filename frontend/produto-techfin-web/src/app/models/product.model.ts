export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  currency: string;
  stockQuantity: number;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}