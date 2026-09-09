import { Component, ElementRef, EventEmitter, HostListener, OnInit, Output } from '@angular/core';
import { AuthorizeService } from '../../services/authorize-facade.service';
import { Observable } from 'rxjs';

@Component({
	selector: 'app-auth-nav',
	templateUrl: './auth-nav.component.html',
	styleUrls: ['./auth-nav.component.css'],
	standalone: false
})
export class AuthNavComponent implements OnInit {
	public isAuthenticated?: Observable<boolean>;
	public name: Observable<string | null>;
	public isMenuOpen = false;

	@Output() navigated = new EventEmitter<void>();

	constructor(private authorizeService: AuthorizeService, private hostRef: ElementRef) {
		this.setData();
	}

	ngOnInit() {
		this.authorizeService.authorizationChange.subscribe(_ => {
			this.setData();
			this.isMenuOpen = false;
		});
	}

	setData() {
		this.isAuthenticated = this.authorizeService.isAuthenticated();
		this.name = this.authorizeService.getUserName();
	}

	toggleMenu(event: Event): void {
		event.stopPropagation();
		this.isMenuOpen = !this.isMenuOpen;
	}

	onNavigated(): void {
		this.isMenuOpen = false;
		this.navigated.emit();
	}

	@HostListener('document:click', ['$event'])
	onDocumentClick(event: MouseEvent): void {
		if (!this.hostRef.nativeElement.contains(event.target)) {
			this.isMenuOpen = false;
		}
	}
}
