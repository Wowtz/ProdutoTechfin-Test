import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'products', pathMatch: 'full' },
  {
    path: 'products',
    loadComponent: () =>
      import('./pages/products/product-list/product-list')
        .then(m => m.ProductList)
  },
  {
    path: 'products/new',
    loadComponent: () =>
      import('./pages/products/product-form/product-form')
        .then(m => m.ProductFormComponent)
  },
  {
    path: 'products/edit/:id',
    loadComponent: () =>
      import('./pages/products/product-form/product-form')
        .then(m => m.ProductFormComponent)
  },
  { path: '**', redirectTo: 'products' }
];