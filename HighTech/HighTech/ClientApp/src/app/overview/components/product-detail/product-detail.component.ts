import { Component, Input, OnInit } from '@angular/core';
import { Product } from '../../../models/product.model';
import { ActivatedRoute } from '@angular/router';
import { State } from 'src/app/core/state.service';
import { tap } from 'rxjs';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.css']
})
export class ProductDetailComponent implements OnInit{
  public product: Product;

  constructor(private state: State) {
    this.state.selectedProduct$.subscribe(
      p => {
        console.log(p)
        this.product= p;
      }
    )
  }

  ngOnInit(): void {
  }
}
