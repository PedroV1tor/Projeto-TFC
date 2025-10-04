import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService, Usuario } from '../../services/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-perfil',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './perfil.component.html',
  styleUrl: './perfil.component.scss'
})
export class PerfilComponent implements OnInit, OnDestroy {
  usuario: Usuario | null = null;
  modoEdicao = false;
  private subscription = new Subscription();


  perfilForm = {
    nome: '',
    sobrenome: '',
    usuario: '',
    telefone: '',
    matricula: '',
    instituicao: '',
    curso: '',
    bio: '',
    cep: '',
    rua: '',
    bairro: '',
    numero: '',
    complemento: '',
    referencia: ''
  };

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {

    if (!this.authService.isLoggedIn) {
      this.router.navigate(['/login']);
      return;
    }


    this.carregarPerfil();


    this.subscription.add(
      this.authService.currentUser$.subscribe(user => {
        if (!user) {
          this.router.navigate(['/login']);
        }
      })
    );
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  carregarPerfil() {
    this.authService.getPerfil().subscribe({
      next: (usuario) => {
        this.usuario = usuario;
        this.resetarFormulario();
      },
      error: (error) => {
        console.error('Erro ao carregar perfil:', error);
        if (error.status === 401) {
          this.authService.logout();
          this.router.navigate(['/login']);
        }
      }
    });
  }

  ativarEdicao() {
    this.modoEdicao = true;
    this.resetarFormulario();
  }

  cancelarEdicao() {
    this.modoEdicao = false;
    this.resetarFormulario();
  }

  resetarFormulario() {
    if (this.usuario) {
      this.perfilForm = {
        nome: this.usuario.nome || '',
        sobrenome: this.usuario.sobrenome || '',
        usuario: this.usuario.nomeUsuario || '',
        telefone: this.usuario.telefone || '',
        matricula: this.usuario.matricula || '',
        instituicao: '',
        curso: '',
        bio: '',
        cep: this.usuario.cep || '',
        rua: this.usuario.rua || '',
        bairro: this.usuario.bairro || '',
        numero: this.usuario.numero || '',
        complemento: this.usuario.complemento || '',
        referencia: this.usuario.referencia || ''
      };
    }
  }

  salvarPerfil() {

    if (!this.perfilForm.nome.trim()) {
      alert('O nome é obrigatório.');
      return;
    }

    if (!this.perfilForm.sobrenome.trim()) {
      alert('O sobrenome é obrigatório.');
      return;
    }

    if (this.perfilForm.telefone && !this.validarTelefone(this.perfilForm.telefone)) {
      alert('Por favor, digite um telefone válido.');
      return;
    }

    if (this.perfilForm.cep && !this.validarCEP(this.perfilForm.cep)) {
      alert('Por favor, digite um CEP válido.');
      return;
    }


    this.authService.updatePerfil({
      nome: this.perfilForm.nome.trim(),
      sobrenome: this.perfilForm.sobrenome.trim(),
      nomeUsuario: this.perfilForm.usuario.trim(),
      telefone: this.perfilForm.telefone.trim(),
      matricula: this.perfilForm.matricula.trim(),
      cep: this.perfilForm.cep.trim(),
      rua: this.perfilForm.rua.trim(),
      bairro: this.perfilForm.bairro.trim(),
      numero: this.perfilForm.numero.trim(),
      complemento: this.perfilForm.complemento.trim(),
      referencia: this.perfilForm.referencia.trim()
    }).subscribe({
      next: () => {
        alert('Perfil atualizado com sucesso!');
        this.modoEdicao = false;

        this.carregarPerfil();
      },
      error: (error) => {
        console.error('Erro ao atualizar perfil:', error);
        alert('Erro ao atualizar perfil. Tente novamente.');
      }
    });
  }

    private validarTelefone(telefone: string): boolean {

    const apenasNumeros = telefone.replace(/\D/g, '');

    return apenasNumeros.length === 10 || apenasNumeros.length === 11;
  }

  private validarCEP(cep: string): boolean {

    const apenasNumeros = cep.replace(/\D/g, '');

    return apenasNumeros.length === 8;
  }

  formatarTelefone() {
    let telefone = this.perfilForm.telefone.replace(/\D/g, '');

    if (telefone.length <= 10) {
      telefone = telefone.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
    } else {
      telefone = telefone.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
    }

    this.perfilForm.telefone = telefone;
  }

  formatarCEP() {
    let cep = this.perfilForm.cep.replace(/\D/g, '');
    if (cep.length <= 8) {
      cep = cep.replace(/(\d{5})(\d{3})/, '$1-$2');
    }
    this.perfilForm.cep = cep;
  }

  formatarData(data: string): string {
    if (!data) return 'Não informado';
    return new Date(data).toLocaleDateString('pt-BR');
  }

  voltarInicio() {
    this.router.navigate(['/']);
  }
}
