import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-redefinir-senha',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './redefinir-senha.component.html',
  styleUrl: './redefinir-senha.component.scss'
})
export class RedefinirSenhaComponent implements OnInit {
  novaSenha: string = '';
  confirmarSenha: string = '';
  email: string = '';
  codigo: string = '';
  isLoading: boolean = false;
  message: string = '';
  isError: boolean = false;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService
  ) {}

  ngOnInit() {

    this.route.queryParams.subscribe(params => {
      this.email = params['email'] || '';
      this.codigo = params['codigo'] || '';

      if (!this.email || !this.codigo) {

        this.router.navigate(['/recuperar-senha']);
      }
    });
  }

  onRedefinir() {

    if (!this.novaSenha || !this.confirmarSenha) {
      this.showMessage('Por favor, preencha todos os campos.', true);
      return;
    }


    if (this.novaSenha.length < 6) {
      this.showMessage('A senha deve ter pelo menos 6 caracteres.', true);
      return;
    }


    if (this.novaSenha !== this.confirmarSenha) {
      this.showMessage('As senhas não coincidem. Tente novamente.', true);
      return;
    }

    this.isLoading = true;
    this.message = '';

    this.authService.redefinirSenha(this.email, this.codigo, this.novaSenha).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.showMessage('Senha redefinida com sucesso!', false);


        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Erro ao redefinir senha:', error);

        if (error.status === 400) {
          this.showMessage('Código inválido ou expirado. Solicite um novo código.', true);
        } else {
          this.showMessage('Erro no servidor. Tente novamente mais tarde.', true);
        }
      }
    });
  }

  private showMessage(msg: string, isError: boolean) {
    this.message = msg;
    this.isError = isError;
  }

  voltarParaLogin() {
    this.router.navigate(['/login']);
  }

  mostrarSenha(campo: string) {
    const input = document.getElementById(campo) as HTMLInputElement;
    if (input.type === 'password') {
      input.type = 'text';
    } else {
      input.type = 'password';
    }
  }

  get temLetraMaiuscula(): boolean {
    return /[A-Z]/.test(this.novaSenha);
  }

  get temNumero(): boolean {
    return /[0-9]/.test(this.novaSenha);
  }
}
