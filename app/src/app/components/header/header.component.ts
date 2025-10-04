import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService, Usuario } from '../../services/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit, OnDestroy {
  isLoggedIn = false;
  currentUser: Usuario | null = null;
  private subscriptions = new Subscription();

  constructor(private router: Router, private authService: AuthService) {}

  ngOnInit() {

    this.subscriptions.add(
      this.authService.isLoggedIn$.subscribe(isLoggedIn => {
        this.isLoggedIn = isLoggedIn;
      })
    );

    this.subscriptions.add(
      this.authService.currentUser$.subscribe(user => {
        this.currentUser = user;
      })
    );
  }

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }

  goToPerfil() {

    this.router.navigate(['/perfil']);
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
  }

  goToNovaPublicacao() {

    console.log('Navegando para Nova Publicação');
    this.router.navigate(['/publicacoes']);
  }

  goToLaboratorio() {

    console.log('Navegando para Laboratório');
    this.router.navigate(['/laboratorio']);
  }

  goToOrcamento() {

    console.log('Navegando para Orçamento');
    this.router.navigate(['/orcamento']);
  }

  goToHome() {
    this.router.navigate(['/']);
  }

  goToInfo() {
    this.router.navigate(['/info']);
  }

  goToSobreNos() {
    this.router.navigate(['/sobre-nos']);
  }

  goToContato() {
    this.router.navigate(['/contato']);
  }

  goToRelatorios() {
    this.router.navigate(['/relatorios']);
  }
}
