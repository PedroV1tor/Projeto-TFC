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
  mostrarModalExclusao = false;
  private subscription = new Subscription();


  perfilForm = {
    nome: '',
    sobrenome: '',
    usuario: '',
    telefone: '',
    matricula: '',
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




        if (error.status === 401) {

          this.authService.logout();
          this.router.navigate(['/login']);
        } else if (error.status === 404) {

          alert('Erro: Usuário não encontrado. Faça login novamente.');
          this.authService.logout();
          this.router.navigate(['/login']);
        } else {

          alert('Erro ao carregar perfil. Tente novamente.');
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
        cep: this.usuario.endereco?.cep || '',
        rua: this.usuario.endereco?.rua || '',
        bairro: this.usuario.endereco?.bairro || '',
        numero: this.usuario.endereco?.numero || '',
        complemento: this.usuario.endereco?.complemento || '',
        referencia: this.usuario.endereco?.referencia || ''
      };
    }
  }

  salvarPerfil() {
    // Validações obrigatórias
    if (!this.perfilForm.nome.trim()) {
      alert('O nome é obrigatório.');
      return;
    }

    if (!this.perfilForm.sobrenome.trim()) {
      alert('O sobrenome é obrigatório.');
      return;
    }

    if (!this.perfilForm.usuario.trim() || this.perfilForm.usuario.trim().length < 3) {
      alert('O nome de usuário é obrigatório e deve ter no mínimo 3 caracteres.');
      return;
    }

    if (!this.perfilForm.telefone.trim()) {
      alert('O telefone é obrigatório.');
      return;
    }

    if (!this.validarTelefone(this.perfilForm.telefone)) {
      alert('Por favor, digite um telefone válido.');
      return;
    }

    // Validações de endereço (obrigatórios)
    if (!this.perfilForm.cep.trim()) {
      alert('O CEP é obrigatório.');
      return;
    }

    if (!this.validarCEP(this.perfilForm.cep)) {
      alert('Por favor, digite um CEP válido.');
      return;
    }

    if (!this.perfilForm.rua.trim()) {
      alert('A rua é obrigatória.');
      return;
    }

    if (!this.perfilForm.bairro.trim()) {
      alert('O bairro é obrigatório.');
      return;
    }

    // Prepara dados para envio - remove campos undefined e garante formato correto
    const enderecoObj: any = {
      cep: this.perfilForm.cep.trim(),
      rua: this.perfilForm.rua.trim(),
      bairro: this.perfilForm.bairro.trim()
    };
    
    // Adiciona campos opcionais apenas se tiverem valor
    const numeroTrimmed = this.perfilForm.numero.trim();
    if (numeroTrimmed) {
      enderecoObj.numero = numeroTrimmed;
    }
    
    const complementoTrimmed = this.perfilForm.complemento.trim();
    if (complementoTrimmed) {
      enderecoObj.complemento = complementoTrimmed;
    }
    
    const referenciaTrimmed = this.perfilForm.referencia.trim();
    if (referenciaTrimmed) {
      enderecoObj.referencia = referenciaTrimmed;
    }

    const dadosAtualizacao: any = {
      nome: this.perfilForm.nome.trim(),
      sobrenome: this.perfilForm.sobrenome.trim(),
      email: this.usuario?.email || '', // Email vem do usuário logado
      nomeUsuario: this.perfilForm.usuario.trim(),
      telefone: this.perfilForm.telefone.trim(),
      endereco: enderecoObj
    };


    this.authService.updatePerfil(dadosAtualizacao).subscribe({
      next: () => {
        alert('Perfil atualizado com sucesso!');
        this.modoEdicao = false;
        this.carregarPerfil();
      },
      error: (error) => {


        
        // Tenta mostrar mensagens de erro específicas
        if (error.status === 400 && error.error) {
          let mensagemErro = 'Erro de validação:\n\n';
          
          // Se houver erros do ModelState (formato do ASP.NET Core)
          if (error.error.errors) {
            const erros = error.error.errors;
            Object.keys(erros).forEach(campo => {
              const campoNome = campo.replace(/([A-Z])/g, ' $1').replace(/^./, str => str.toUpperCase());
              mensagemErro += `• ${campoNome}: ${Array.isArray(erros[campo]) ? erros[campo].join(', ') : erros[campo]}\n`;
            });
          } else if (error.error.message) {
            mensagemErro = error.error.message;
          } else {
            mensagemErro = JSON.stringify(error.error, null, 2);
          }

          alert(mensagemErro);
        } else {
          alert('Erro ao atualizar perfil. Verifique o console para mais detalhes.');
        }
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

  getAvatarText(): string {
    if (!this.usuario) return 'U';

    if (this.usuario.tipo === 'empresa') {
      // Para empresa, pega as primeiras letras da Razão Social
      const razaoSocial = this.usuario.razaoSocial || 'E';
      const palavras = razaoSocial.split(' ');
      if (palavras.length >= 2) {
        return (palavras[0].charAt(0) + palavras[1].charAt(0)).toUpperCase();
      }
      return razaoSocial.substring(0, 2).toUpperCase();
    } else {
      // Para pessoa física, usa nome e sobrenome
      const nome = this.usuario.nome?.charAt(0) || 'U';
      const sobrenome = this.usuario.sobrenome?.charAt(0) || '';
      return (nome + sobrenome).toUpperCase();
    }
  }

  confirmarExclusao() {
    this.mostrarModalExclusao = true;
  }

  cancelarExclusao() {
    this.mostrarModalExclusao = false;
  }

  excluirPerfil() {
    if (!confirm('Tem certeza que deseja excluir seu perfil? Esta ação não pode ser desfeita.')) {
      return;
    }

    this.authService.deletePerfil().subscribe({
      next: () => {

        alert('Perfil excluído com sucesso. Você será redirecionado para a página inicial.');
        this.authService.logout();
        this.router.navigate(['/']);
      },
      error: (error) => {

        if (error.status === 404) {
          alert('Perfil não encontrado ou já foi excluído.');
        } else if (error.status === 401) {
          alert('Sessão expirada. Faça login novamente.');
          this.authService.logout();
          this.router.navigate(['/login']);
        } else {
          alert('Erro ao excluir perfil. Tente novamente.');
        }
        this.mostrarModalExclusao = false;
      }
    });
  }
}
