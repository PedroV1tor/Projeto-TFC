import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { CadastroComponent } from './components/cadastro/cadastro.component';
import { RecuperarSenhaComponent } from './components/recuperar-senha/recuperar-senha.component';
import { VerificarCodigoComponent } from './components/verificar-codigo/verificar-codigo.component';
import { RedefinirSenhaComponent } from './components/redefinir-senha/redefinir-senha.component';
import { LaboratorioComponent } from './components/laboratorio/laboratorio.component';
import { PublicacoesComponent } from './components/publicacoes/publicacoes.component';
import { OrcamentoComponent } from './components/orcamento/orcamento.component';
import { PerfilComponent } from './components/perfil/perfil.component';
import { InfoComponent } from './components/info/info.component';
import { SobreNosComponent } from './components/sobre-nos/sobre-nos.component';
import { ContatoComponent } from './components/contato/contato.component';
import { RelatoriosComponent } from './components/relatorios/relatorios.component';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'info', component: InfoComponent },
  { path: 'sobre-nos', component: SobreNosComponent },
  { path: 'contato', component: ContatoComponent },
  { path: 'login', component: LoginComponent },
  { path: 'cadastro', component: CadastroComponent },
  { path: 'recuperar-senha', component: RecuperarSenhaComponent },
  { path: 'verificar-codigo', component: VerificarCodigoComponent },
  { path: 'redefinir-senha', component: RedefinirSenhaComponent },
  { path: 'laboratorio', component: LaboratorioComponent, canActivate: [AuthGuard] },
  { path: 'publicacoes', component: PublicacoesComponent, canActivate: [AuthGuard] },
  { path: 'orcamento', component: OrcamentoComponent, canActivate: [AuthGuard] },
  { path: 'relatorios', component: RelatoriosComponent, canActivate: [AuthGuard] },
  { path: 'perfil', component: PerfilComponent, canActivate: [AuthGuard] },
];
