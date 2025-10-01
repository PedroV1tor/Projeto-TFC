export interface Publicacao {
  id: number;
  titulo: string;
  resumo: string;
  descricao: string;
  autor?: string;
  status: 'ativa' | 'rascunho' | 'arquivada';
  criadoEm: string;
  atualizadoEm?: string;
  visualizacoes?: number;
  curtidas?: number;
}
