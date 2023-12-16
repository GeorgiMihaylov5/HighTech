import { Component, Input, OnInit } from '@angular/core';
import { Product } from '../../../models/product.model';
import { Router } from '@angular/router';
import { OverviewFacade } from '../../services/overview-facade.service';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css']
})
export class ProductComponent implements OnInit{
  @Input() public product: Product;

  constructor(private facade: OverviewFacade) {
    
  }
  ngOnInit(): void {
  }

  public routeToDetail() {
    this.facade.detailProduct(this.product);
  }

  public get description() {
    return this.product.fields.sort((a, b) => a.name < b.name ? -1 : a.name > b.name ? 1 : 0).slice(0,3);
  }
}
