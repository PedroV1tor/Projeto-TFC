import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AgendamentoService } from '../../services/agendamento.service';
import { AuthService } from '../../services/auth.service';
import { PublicacaoService } from '../../services/publicacao.service';
import { Agendamento } from '../../models/agendamento.model';
import { Publicacao } from '../../models/publicacao.model';

@Component({
  selector: 'app-laboratorio',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './laboratorio.component.html',
  styleUrl: './laboratorio.component.scss'
})
export class LaboratorioComponent implements OnInit {
  agendamentos: Agendamento[] = [];
  publicacoes: Publicacao[] = [];
  mostrarFormulario = false;
  agendamentoEditando: Agendamento | null = null;
  isAdmin = false;


  novoAgendamento = {
    titulo: '',
    data: '',
    descricao: ''
  };

  constructor(
    private agendamentoService: AgendamentoService,
    private authService: AuthService,
    private publicacaoService: PublicacaoService,
    private router: Router
  ) {}

  ngOnInit() {
    this.isAdmin = this.authService.isAdmin;
    this.carregarAgendamentos();

    if (this.isAdmin) {
      this.carregarPublicacoes();
    }
  }

  carregarAgendamentos() {
    // Admin vê todos os agendamentos, usuários normais veem apenas os seus
    const observable = this.isAdmin
      ? this.agendamentoService.getTodosAgendamentos()
      : this.agendamentoService.getAgendamentos();

    observable.subscribe({
      next: (agendamentos: Agendamento[]) => {
        this.agendamentos = agendamentos;
        console.log(`Carregados ${agendamentos.length} agendamento(s) ${this.isAdmin ? '(admin - todos)' : '(usuário - apenas seus)'}`);
      },
      error: (error: any) => console.error('Erro ao carregar agendamentos:', error)
    });
  }

  carregarPublicacoes() {
    this.publicacaoService.getPublicacoes().subscribe({
      next: (publicacoes) => this.publicacoes = publicacoes,
      error: (error) => console.error('Erro ao carregar publicações:', error)
    });
  }

  abrirFormulario() {
    this.mostrarFormulario = true;
    this.agendamentoEditando = null;
    this.resetarFormulario();
  }

  fecharFormulario() {
    this.mostrarFormulario = false;
    this.agendamentoEditando = null;
    this.resetarFormulario();
  }

  resetarFormulario() {
    this.novoAgendamento = {
      titulo: '',
      data: '',
      descricao: ''
    };
  }

  salvarAgendamento() {
    if (!this.novoAgendamento.titulo || !this.novoAgendamento.data || !this.novoAgendamento.descricao) {
      alert('Por favor, preencha todos os campos obrigatórios.');
      return;
    }

    // Validação de data: não permitir datas no passado ou no mesmo dia
    const dataAgendamento = new Date(this.novoAgendamento.data + 'T00:00:00');
    const hoje = new Date();
    hoje.setHours(0, 0, 0, 0);

    const amanha = new Date(hoje);
    amanha.setDate(amanha.getDate() + 1);

    if (dataAgendamento.getTime() < amanha.getTime()) {
      alert('O agendamento deve ser feito com pelo menos 1 dia de antecedência. Por favor, escolha uma data a partir de amanhã.');
      return;
    }

    try {
      if (this.agendamentoEditando) {
        // Atualizar agendamento existente
        this.agendamentoService.atualizarAgendamento(
          this.agendamentoEditando.id,
          this.novoAgendamento
        ).subscribe({
          next: () => {
            alert('Agendamento atualizado com sucesso!');
            this.carregarAgendamentos();
            this.fecharFormulario();
          },
          error: (error) => {
            alert('Erro ao atualizar agendamento.');
            console.error(error);
          }
        });
      } else {
        // Criar novo agendamento
        this.agendamentoService.criarAgendamento(this.novoAgendamento).subscribe({
          next: () => {
            alert('Agendamento criado com sucesso!');
            this.carregarAgendamentos();
            this.fecharFormulario();
          },
          error: (error) => {
            alert('Erro ao criar agendamento.');
            console.error(error);
          }
        });
      }
    } catch (error: any) {
      if (error.message.includes('logado')) {
        alert('Você precisa estar logado para realizar esta ação.');
        this.router.navigate(['/login']);
      } else {
        alert('Erro ao salvar agendamento: ' + error.message);
      }
    }
  }

  editarAgendamento(agendamento: Agendamento) {
    this.agendamentoEditando = agendamento;

    // Formatar a data corretamente para o input do tipo date (YYYY-MM-DD)
    let dataFormatada = agendamento.data;
    if (agendamento.data.includes('T')) {
      dataFormatada = agendamento.data.split('T')[0];
    }

    this.novoAgendamento = {
      titulo: agendamento.titulo,
      data: dataFormatada,
      descricao: agendamento.descricao
    };
    this.mostrarFormulario = true;
  }

  excluirAgendamento(id: number) {
    if (confirm('Tem certeza que deseja excluir este agendamento?')) {
      this.agendamentoService.excluirAgendamento(id).subscribe({
        next: () => {
          alert('Agendamento excluído com sucesso!');
          this.carregarAgendamentos();
        },
        error: (error) => {
          alert('Erro ao excluir agendamento.');
          console.error(error);
        }
      });
    }
  }

  getDataMinima(): string {
    // Retorna a data de amanhã no formato YYYY-MM-DD para o atributo min do input date
    const amanha = new Date();
    amanha.setDate(amanha.getDate() + 1);

    const ano = amanha.getFullYear();
    const mes = String(amanha.getMonth() + 1).padStart(2, '0');
    const dia = String(amanha.getDate()).padStart(2, '0');

    return `${ano}-${mes}-${dia}`;
  }

  formatarData(data: string): string {
    if (!data) return 'Data não informada';

    // Se a data já contém o horário (ISO string), usar diretamente
    if (data.includes('T')) {
      return new Date(data).toLocaleDateString('pt-BR');
    }

    // Se é apenas a data (YYYY-MM-DD), adicionar horário para evitar problemas de timezone
    return new Date(data + 'T12:00:00').toLocaleDateString('pt-BR');
  }

  getStatusClass(status: string): string {
    if (!status) return 'status-pendente';

    switch (status.toLowerCase()) {
      case 'aprovado':
        return 'status-aprovado';
      case 'reprovado':
        return 'status-reprovado';
      case 'pendente':
      default:
        return 'status-pendente';
    }
  }

  getStatusText(status: string): string {
    if (!status) return 'Pendente';

    switch (status.toLowerCase()) {
      case 'aprovado':
        return 'Aprovado';
      case 'reprovado':
        return 'Reprovado';
      case 'pendente':
      default:
        return 'Pendente';
    }
  }

  alterarStatusAgendamento(agendamento: Agendamento, novoStatus: 'pendente' | 'aprovado' | 'reprovado') {
    if (confirm(`Deseja alterar o status deste agendamento para "${this.getStatusText(novoStatus)}"?`)) {
      this.agendamentoService.alterarStatus(agendamento.id, novoStatus).subscribe({
        next: () => {
          alert('Status do agendamento alterado com sucesso!');
          this.carregarAgendamentos();
        },
        error: (error) => {
          alert('Erro ao alterar status do agendamento.');
          console.error(error);
        }
      });
    }
  }

  // Métodos para gerenciar publicações (apenas para admin)
  getStatusClassPublicacao(status: string): string {
    switch (status) {
      case 'ativa':
        return 'status-ativa';
      case 'rascunho':
        return 'status-rascunho';
      case 'arquivada':
        return 'status-arquivada';
      default:
        return 'status-rascunho';
    }
  }

  getStatusTextPublicacao(status: string): string {
    switch (status) {
      case 'ativa':
        return 'Ativa';
      case 'rascunho':
        return 'Rascunho';
      case 'arquivada':
        return 'Arquivada';
      default:
        return 'Rascunho';
    }
  }

  alterarStatusPublicacao(publicacao: Publicacao, novoStatus: 'ativa' | 'rascunho' | 'arquivada') {
    if (confirm(`Deseja alterar o status desta publicação para "${this.getStatusTextPublicacao(novoStatus)}"?`)) {
      this.publicacaoService.alterarStatus(publicacao.id, novoStatus).subscribe({
        next: () => {
          alert('Status da publicação alterado com sucesso!');
          this.carregarPublicacoes();
        },
        error: (error) => {
          alert('Erro ao alterar status da publicação.');
          console.error(error);
        }
      });
    }
  }

  excluirPublicacao(id: number) {
    if (confirm('Tem certeza que deseja excluir esta publicação?')) {
      this.publicacaoService.excluirPublicacao(id).subscribe({
        next: () => {
          alert('Publicação excluída com sucesso!');
          this.carregarPublicacoes();
        },
        error: (error) => {
          alert('Erro ao excluir publicação.');
          console.error(error);
        }
      });
    }
  }

  formatarDataPublicacao(data: string): string {
    if (!data) return 'Data não informada';
    return new Date(data).toLocaleDateString('pt-BR');
  }
}
