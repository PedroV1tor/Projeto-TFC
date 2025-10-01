export interface Orcamento {
  id: number;
  titulo: string;
  descricao: string;
  prazoEntrega: string;
  prazoOrcamento: string;
  valor?: number;
  cliente?: string;
  responsavel?: string;
  status: 'pendente' | 'aprovado' | 'rejeitado' | 'concluido';
  criadoEm: string;
  atualizadoEm?: string;
}
