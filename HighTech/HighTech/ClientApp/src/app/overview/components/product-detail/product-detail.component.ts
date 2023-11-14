import { Component, Input, OnInit } from '@angular/core';
import { Product } from '../../../models/product.model';
import { ActivatedRoute } from '@angular/router';
import { State } from 'src/app/core/state.service';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { DetailFacade } from '../../services/detail-facade.service';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.css']
})
export class ProductDetailComponent implements OnInit {
  constructor(private detailFacade: DetailFacade) {
  }

  ngOnInit(): void {
  }

  public get product() {
    return this.detailFacade.product;
  }
}
