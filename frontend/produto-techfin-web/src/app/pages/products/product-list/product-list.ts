import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import {
  PoModalComponent,
  PoModalModule,
  PoNotificationService,
  PoPageModule,
  PoTableModule,
  PoButtonModule,
  PoPageAction,
  PoTableAction,
  PoTableColumn
} from '@po-ui/ng-components';
import { ProductService } from '../../../core/services/product';
import { Product } from '../../../models/product.model';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule,
    PoPageModule,
    PoTableModule,
    PoModalModule,
    PoButtonModule
  ],
  templateUrl: './product-list.html',
  styleUrls: ['./product-list.scss']
})
export class ProductList implements OnInit {

  @ViewChild('confirmDeleteModal') confirmDeleteModal!: PoModalComponent;

  products: Product[] = [];
  selectedProducts: Product[] = [];
  isLoading = false;
  productToDelete: Product | null = null;

  columns: PoTableColumn[] = [
    { property: 'name', label: 'Nome' },
    { property: 'description', label: 'Descrição' },
    {
      property: 'priceFormatted',
      label: 'Preço'
    },
    { property: 'stockQuantity', label: 'Estoque', type: 'number' },
    {
      property: 'isActive',
      label: 'Ativo',
      type: 'boolean',
      boolean: { trueLabel: 'Sim', falseLabel: 'Não' }
    },
    {
      property: 'actions',
      label: 'Ações',
      type: 'icon',
      icons: [
        {
          value: 'edit',
          icon: 'an an-pencil',
          tooltip: 'Editar',
          action: (row: Product) => this.router.navigate(['/products/edit', row.id])
        },
        {
          value: 'delete',
          icon: 'an an-trash',
          tooltip: 'Excluir',
          color: 'color-07',
          action: (row: Product) => this.confirmDelete(row)
        }
      ]
    }
  ];;

  pageActions: PoPageAction[] = [
    {
      label: 'Novo Produto',
      action: () => this.router.navigate(['/products/new']),
      icon: 'an an-plus'
    },
    {
      label: 'Excluir Selecionados',
      action: () => this.deleteBatch(),
      icon: 'an an-trash',
      disabled: () => this.selectedProducts.length === 0
    }
  ];

  tableActions: PoTableAction[] = [
    {
      label: 'Editar',
      action: (product: Product) => this.router.navigate(['/products/edit', product.id]),
      icon: 'an an-pencil'
    },
    {
      label: 'Excluir',
      action: (product: Product) => this.confirmDelete(product),
      icon: 'an an-trash',
      type: 'danger'
    }
  ];

  confirmDeleteActions = [
    {
      label: 'Confirmar',
      action: () => this.deleteProduct(),
      danger: true
    },
    {
      label: 'Cancelar',
      action: () => this.confirmDeleteModal.close()
    }
  ];

  constructor(
    private productService: ProductService,
    private router: Router,
    private notification: PoNotificationService
  ) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.isLoading = true;
    this.productService.getAll().subscribe({
      next: (products) => {
        this.products = products.map(p => ({
          ...p,
          actions: ['edit', 'delete'],
          priceFormatted: p.price.toLocaleString('pt-BR', {
            style: 'currency',
            currency: 'BRL'
          })
        }));
        this.isLoading = false;
      },
      error: () => {
        this.notification.error('Erro ao carregar produtos.');
        this.isLoading = false;
      }
    });
  }

  confirmDelete(product: Product): void {
    this.productToDelete = product;
    this.confirmDeleteModal.open();
  }

  deleteProduct(): void {
    if (!this.productToDelete) return;

    this.productService.delete(this.productToDelete.id).subscribe({
      next: () => {
        this.notification.success('Produto excluído com sucesso!');
        this.confirmDeleteModal.close();
        this.productToDelete = null;
        this.loadProducts();
      },
      error: () => {
        this.notification.error('Erro ao excluir produto.');
      }
    });
  }

  deleteBatch(): void {
    if (this.selectedProducts.length === 0) return;

    const ids = this.selectedProducts.map(p => p.id);
    this.productService.deleteBatch({ ids }).subscribe({
      next: (result) => {
        this.notification.success(
          `${result.totalDeleted} produto(s) excluído(s) com sucesso!`
        );
        this.selectedProducts = [];
        this.loadProducts();
      },
      error: () => {
        this.notification.error('Erro ao excluir produtos.');
      }
    });
  }


  onSelectionChange(event: any): void {
    if (Array.isArray(event)) {
      this.selectedProducts = event;
    } else if (event) {
      this.selectedProducts = [...this.selectedProducts, event];
    }
  }

  onUnselectionChange(event: any): void {
    if (Array.isArray(event)) {
      this.selectedProducts = event;
    } else if (event) {
      this.selectedProducts = this.selectedProducts.filter(p => p.id !== event.id);
    }
  }
}