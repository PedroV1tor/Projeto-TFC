import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  email: string = '';
  password: string = '';

  constructor(private router: Router, private authService: AuthService) {}

  onSubmit() {
    if (!this.email || !this.password) {
      alert('Por favor, preencha todos os campos.');
      return;
    }

    // Validação básica de e-mail
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(this.email)) {
      alert('Por favor, digite um e-mail válido.');
      return;
    }

    // Tenta fazer login usando o serviço
    this.authService.login(this.email, this.password).subscribe({
      next: (response) => {
        this.router.navigate(['/']);
      },
      error: (error) => {
        console.error('Erro no login:', error);
        if (error.status === 401) {
          alert('Credenciais inválidas. Tente novamente.');
        } else {
          alert('Erro no servidor. Tente novamente mais tarde.');
        }
      }
    });
  }

  goToHome() {
    this.router.navigate(['/']);
  }

  goToCadastro() {
    this.router.navigate(['/cadastro']);
  }

  esqueceuSenha() {
    // Redireciona para a página de recuperar senha
    this.router.navigate(['/recuperar-senha']);
  }
}
