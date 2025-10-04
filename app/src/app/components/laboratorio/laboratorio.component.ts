import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AgendamentoService } from '../../services/agendamento.service';
import { AuthService } from '../../services/auth.service';
import { Agendamento } from '../../models/agendamento.model';

@Component({
  selector: 'app-laboratorio',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './laboratorio.component.html',
  styleUrl: './laboratorio.component.scss'
})
export class LaboratorioComponent implements OnInit {
  agendamentos: Agendamento[] = [];
  mostrarFormulario = false;
  agendamentoEditando: Agendamento | null = null;


  novoAgendamento = {
    titulo: '',
    data: '',
    descricao: ''
  };

  constructor(
    private agendamentoService: AgendamentoService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.carregarAgendamentos();
  }

  carregarAgendamentos() {
    this.agendamentoService.getAgendamentos().subscribe({
      next: (agendamentos) => this.agendamentos = agendamentos,
      error: (error) => console.error('Erro ao carregar agendamentos:', error)
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


    const dataAgendamento = new Date(this.novoAgendamento.data);
    const hoje = new Date();
    hoje.setHours(0, 0, 0, 0);

    if (dataAgendamento < hoje) {
      alert('A data do agendamento não pode ser no passado.');
      return;
    }

    try {
      if (this.agendamentoEditando) {

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
    this.novoAgendamento = {
      titulo: agendamento.titulo,
      data: agendamento.data,
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

  alterarStatus(id: number, novoStatus: 'pendente' | 'aprovado' | 'reprovado') {
    this.agendamentoService.alterarStatus(id, novoStatus).subscribe({
      next: () => {
        alert(`Status alterado para ${this.getStatusText(novoStatus)}!`);
        this.carregarAgendamentos();
      },
      error: (error) => {
        alert('Erro ao alterar status.');
        console.error(error);
      }
    });
  }

  formatarData(data: string): string {
    return new Date(data + 'T00:00:00').toLocaleDateString('pt-BR');
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'pendente': return 'status-pendente';
      case 'aprovado': return 'status-aprovado';
      case 'reprovado': return 'status-reprovado';
      default: return '';
    }
  }

  getStatusText(status: string): string {
    switch (status) {
      case 'pendente': return 'Pendente';
      case 'aprovado': return 'Aprovado';
      case 'reprovado': return 'Reprovado';
      default: return status;
    }
  }
}
