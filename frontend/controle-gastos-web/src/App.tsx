import { useEffect, useState } from "react";
import "./App.css";

const API_URL = "http://localhost:5000";

type Pessoa = {
  id: number;
  nome: string;
  idade: number;
};

type TipoTransacao = "Despesa" | "Receita";

type Transacao = {
  id: number;
  descricao: string;
  valor: number;
  tipo: TipoTransacao;
  pessoaId: number;
  pessoaNome: string;
};

type TotalPessoa = {
  pessoaId: number;
  nome: string;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
};

type TotalGeral = {
  totalReceitas: number;
  totalDespesas: number;
  saldoLiquido: number;
};

type TotaisResponse = {
  pessoas: TotalPessoa[];
  totalGeral: TotalGeral;
};

function App() {
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [transacoes, setTransacoes] = useState<Transacao[]>([]);
  const [totais, setTotais] = useState<TotaisResponse | null>(null);

  const [nome, setNome] = useState("");
  const [idade, setIdade] = useState("");

  const [descricao, setDescricao] = useState("");
  const [valor, setValor] = useState("");
  const [tipo, setTipo] = useState<TipoTransacao>("Despesa");
  const [pessoaId, setPessoaId] = useState("");

  const [mensagemErro, setMensagemErro] = useState("");

  useEffect(() => {
    carregarDados();
  }, []);

  async function carregarDados() {
    await Promise.all([
      carregarPessoas(),
      carregarTransacoes(),
      carregarTotais(),
    ]);
  }

  async function carregarPessoas() {
    const resposta = await fetch(`${API_URL}/pessoas`);
    const dados = await resposta.json();
    setPessoas(dados);
  }

  async function carregarTransacoes() {
    const resposta = await fetch(`${API_URL}/transacoes`);
    const dados = await resposta.json();
    setTransacoes(dados);
  }

  async function carregarTotais() {
    const resposta = await fetch(`${API_URL}/totais`);
    const dados = await resposta.json();
    setTotais(dados);
  }

  function formatarMoeda(valor: number) {
    return valor.toLocaleString("pt-BR", {
      style: "currency",
      currency: "BRL",
    });
  }

  function classeSaldo(valor: number) {
    if (valor > 0) return "valor positivo";
    if (valor < 0) return "valor negativo";
    return "valor neutro";
  }

  async function cadastrarPessoa(event: React.FormEvent) {
    event.preventDefault();
    setMensagemErro("");

    const resposta = await fetch(`${API_URL}/pessoas`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        nome,
        idade: Number(idade),
      }),
    });

    if (!resposta.ok) {
      const erro = await resposta.text();
      setMensagemErro(erro);
      return;
    }

    setNome("");
    setIdade("");
    await carregarDados();
  }

  async function deletarPessoa(id: number) {
    const confirmar = window.confirm(
      "Deseja deletar esta pessoa? Todas as transações dela também serão apagadas."
    );

    if (!confirmar) return;

    const resposta = await fetch(`${API_URL}/pessoas/${id}`, {
      method: "DELETE",
    });

    if (!resposta.ok) {
      const erro = await resposta.text();
      setMensagemErro(erro);
      return;
    }

    await carregarDados();
  }

  async function cadastrarTransacao(event: React.FormEvent) {
    event.preventDefault();
    setMensagemErro("");

    const resposta = await fetch(`${API_URL}/transacoes`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        descricao,
        valor: Number(valor),
        tipo,
        pessoaId: Number(pessoaId),
      }),
    });

    if (!resposta.ok) {
      const erro = await resposta.text();
      setMensagemErro(erro);
      return;
    }

    setDescricao("");
    setValor("");
    setTipo("Despesa");
    setPessoaId("");
    await carregarDados();
  }

  return (
    <main className="pagina">
      <header className="cabecalho">
        <div>
          <h1>Controle de Gastos</h1>
        </div>
      </header>

      {mensagemErro && (
        <div className="mensagem-erro">
          <strong>Atenção:</strong> {mensagemErro}
        </div>
      )}

      <section className="cards-resumo">
        <div className="card-resumo">
          <span>Total de Receitas</span>
          <strong className="valor positivo">
            {formatarMoeda(totais?.totalGeral.totalReceitas ?? 0)}
          </strong>
        </div>

        <div className="card-resumo">
          <span>Total de Despesas</span>
          <strong className="valor negativo">
            {formatarMoeda(totais?.totalGeral.totalDespesas ?? 0)}
          </strong>
        </div>

        <div className="card-resumo">
          <span>Saldo Líquido</span>
          <strong className={classeSaldo(totais?.totalGeral.saldoLiquido ?? 0)}>
            {formatarMoeda(totais?.totalGeral.saldoLiquido ?? 0)}
          </strong>
        </div>
      </section>

      <section className="grid-cadastros">
        <div className="card">
          <div className="titulo-card">
            <h2>Cadastrar Pessoa</h2>
          </div>

          <form onSubmit={cadastrarPessoa} className="formulario">
            <label>
              Nome
              <input
                type="text"
                placeholder="Ex: João"
                value={nome}
                onChange={(e) => setNome(e.target.value)}
              />
            </label>

            <label>
              Idade
              <input
                type="number"
                placeholder="Ex: 25"
                value={idade}
                onChange={(e) => setIdade(e.target.value)}
              />
            </label>

            <button type="submit">Cadastrar Pessoa</button>
          </form>
        </div>

        <div className="card">
          <div className="titulo-card">
            <h2>Cadastrar Transação</h2>
          </div>

          <form onSubmit={cadastrarTransacao} className="formulario">
            <label>
              Descrição
              <input
                type="text"
                placeholder="Ex: Conta de luz"
                value={descricao}
                onChange={(e) => setDescricao(e.target.value)}
              />
            </label>

            <label>
              Valor
              <input
                type="number"
                step="0.01"
                placeholder="Ex: 150.00"
                value={valor}
                onChange={(e) => setValor(e.target.value)}
              />
            </label>

            <label>
              Tipo
              <select
                value={tipo}
                onChange={(e) => setTipo(e.target.value as TipoTransacao)}
              >
                <option value="Despesa">Despesa</option>
                <option value="Receita">Receita</option>
              </select>
            </label>

            <label>
              Pessoa
              <select
                value={pessoaId}
                onChange={(e) => setPessoaId(e.target.value)}
              >
                <option value="">Selecione uma pessoa</option>

                {pessoas.map((pessoa) => (
                  <option key={pessoa.id} value={pessoa.id}>
                    {pessoa.nome} - {pessoa.idade} anos
                  </option>
                ))}
              </select>
            </label>

            <button type="submit">Cadastrar Transação</button>
          </form>
        </div>
      </section>

      <section className="grid-listagens">
        <div className="card">
          <div className="titulo-card">
            <h2>Pessoas Cadastradas</h2>
            <p>{pessoas.length} pessoa(s) cadastrada(s).</p>
          </div>

          {pessoas.length === 0 ? (
            <p className="texto-vazio">Nenhuma pessoa cadastrada.</p>
          ) : (
            <div className="tabela-container">
              <table>
                <thead>
                  <tr>
                    <th>Nome</th>
                    <th>Idade</th>
                    <th>Ação</th>
                  </tr>
                </thead>

                <tbody>
                  {pessoas.map((pessoa) => (
                    <tr key={pessoa.id}>
                      <td>{pessoa.nome}</td>
                      <td>{pessoa.idade} anos</td>
                      <td>
                        <button
                          className="botao-excluir"
                          onClick={() => deletarPessoa(pessoa.id)}
                        >
                          Deletar
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>

        <div className="card">
          <div className="titulo-card">
            <h2>Transações Cadastradas</h2>
            <p>{transacoes.length} transação(ões) cadastrada(s).</p>
          </div>

          {transacoes.length === 0 ? (
            <p className="texto-vazio">Nenhuma transação cadastrada.</p>
          ) : (
            <div className="tabela-container">
              <table>
                <thead>
                  <tr>
                    <th>Descrição</th>
                    <th>Valor</th>
                    <th>Tipo</th>
                    <th>Pessoa</th>
                  </tr>
                </thead>

                <tbody>
                  {transacoes.map((transacao) => (
                    <tr key={transacao.id}>
                      <td>{transacao.descricao}</td>
                      <td>{formatarMoeda(transacao.valor)}</td>
                      <td>
                        <span
                          className={
                            transacao.tipo === "Receita"
                              ? "tag receita"
                              : "tag despesa"
                          }
                        >
                          {transacao.tipo}
                        </span>
                      </td>
                      <td>{transacao.pessoaNome}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </section>

      <section className="card">
        <div className="titulo-card">
          <h2>Totais por Pessoa</h2>
        </div>

        {!totais || totais.pessoas.length === 0 ? (
          <p className="texto-vazio">Nenhum total disponível.</p>
        ) : (
          <div className="tabela-container">
            <table>
              <thead>
                <tr>
                  <th>Pessoa</th>
                  <th>Total Receitas</th>
                  <th>Total Despesas</th>
                  <th>Saldo</th>
                </tr>
              </thead>

              <tbody>
                {totais.pessoas.map((pessoa) => (
                  <tr key={pessoa.pessoaId}>
                    <td>{pessoa.nome}</td>
                    <td className="positivo">
                      {formatarMoeda(pessoa.totalReceitas)}
                    </td>
                    <td className="negativo">
                      {formatarMoeda(pessoa.totalDespesas)}
                    </td>
                    <td className={classeSaldo(pessoa.saldo)}>
                      {formatarMoeda(pessoa.saldo)}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </main>
  );
}

export default App;