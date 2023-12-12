import { Injectable } from "@angular/core";
import { Product } from "../models/product.model";
import { BehaviorSubject, Observable, filter } from "rxjs";
import { Field } from "../models/field.model";
import { Category } from "../models/category.model";
import { CreateOptions } from "../manage/models/options.model";

@Injectable({
	providedIn: "root",
})

export class State {
    private products$ = new BehaviorSubject<Product[]>(null);
    private _selectedProduct$ = new BehaviorSubject<Product>(null);
    public overviewLoaded = false;

	public isEdit: boolean = false; 
	public editObj: Product | Category | Field;
	public option: CreateOptions;

    public getProduct$(): Observable<Product[]> {
		return this.products$.pipe(filter((products: Product[]) => products !== null));
	}

	public setProducts$(products: Product[]): void {
		this.products$.next(products);
	}

    public removeProduct(id: string): void {
		const items = this.products$.getValue();
		const newItems = items.filter(product => product.id !== id);
		this.products$.next(newItems);
	}

	public set selectedProduct(product: Product) {
		this._selectedProduct$.next(product);
	}

    public get selectedProduct$(): Observable<Product> {
		return this._selectedProduct$;
	}
}
	