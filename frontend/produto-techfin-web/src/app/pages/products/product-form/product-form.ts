import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import {
  PoNotificationService,
  PoPageModule,
  PoModule,
  PoDividerModule
} from '@po-ui/ng-components';
import { ProductService } from '../../../core/services/product';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PoModule,
    PoDividerModule
  ],
  templateUrl: './product-form.html',
  styleUrls: ['./product-form.scss']
})
export class ProductFormComponent implements OnInit {

  form!: FormGroup;
  isEditMode = false;
  isLoading = false;
  productId: string | null = null;
  pageTitle = 'Novo Produto';
  priceDisplay = 'R$ 0,00';

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private notification: PoNotificationService
  ) {}

  ngOnInit(): void {
    this.buildForm();
    this.productId = this.route.snapshot.paramMap.get('id');

    if (this.productId) {
      this.isEditMode = true;
      this.pageTitle = 'Editar Produto';
      this.loadProduct(this.productId);
    }
  }

  buildForm(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.maxLength(1000)]],
      price: [null, [Validators.required, Validators.min(0)]],
      stockQuantity: [null, [Validators.required, Validators.min(0)]]
    });
  }

  formatPrice(value: any): void {
    const raw = String(value).replace(/\D/g, '');
    const numeric = parseFloat(raw) / 100 || 0;
    this.priceDisplay = numeric.toLocaleString('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    });
    this.form.get('price')?.setValue(numeric, { emitEvent: false });
  }

  loadProduct(id: string): void {
    this.isLoading = true;
    this.productService.getById(id).subscribe({
      next: (product) => {
        this.priceDisplay = product.price.toLocaleString('pt-BR', {
          style: 'currency',
          currency: 'BRL'
        });
        this.form.patchValue({
          name: product.name,
          description: product.description,
          price: product.price,
          stockQuantity: product.stockQuantity
        });
        this.isLoading = false;
      },
      error: () => {
        this.notification.error('Erro ao carregar produto.');
        this.isLoading = false;
        this.router.navigate(['/products']);
      }
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notification.warning('Preencha todos os campos obrigatórios.');
      return;
    }

    this.isLoading = true;

    const formValue = this.form.value
    const request = {
      ...formValue,
      price: Number(formValue.price)
    };
    
    const operation$ = this.isEditMode
      ? this.productService.update(this.productId!, request)
      : this.productService.create(request);

    operation$.subscribe({
      next: () => {
        this.notification.success(
          this.isEditMode
            ? 'Produto atualizado com sucesso!'
            : 'Produto criado com sucesso!'
        );
        this.router.navigate(['/products']);
      },
      error: (err) => {
        const message = err?.error?.title ?? 'Erro ao salvar produto.';
        this.notification.error(message);
        this.isLoading = false;
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/products']);
  }

  isFieldInvalid(field: string): boolean {
    const control = this.form.get(field);
    return !!control && control.invalid && control.touched;
  }
}