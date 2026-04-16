import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PoMenuItem, PoMenuModule, PoToolbarModule } from '@po-ui/ng-components';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    PoToolbarModule,
    PoMenuModule
  ],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  readonly menus: PoMenuItem[] = [
    {
      label: 'Produtos',
      link: '/products',
      icon: 'an an-storefront'
    }
  ];
}