export interface Agendamento {
  id: number;
  titulo: string;
  data: string;
  descricao: string;
  usuario?: string;
  status: 'ativo' | 'cancelado' | 'concluido';
  criadoEm: string;
  atualizadoEm?: string;
}
