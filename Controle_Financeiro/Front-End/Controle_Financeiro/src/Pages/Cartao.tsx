import { Fragment, useEffect, useState, type FormEvent } from "react";
import toast from "react-hot-toast";
import axios from "axios";
import { criarCartao, consultarCartoes, alterarCartao, deletarCartao, type CartaoRequest, type CartaoResponse } from "../Services/CartaoService";

import nubank from "../assets/Cartoes/Cartao-Nubank.png";
import santander from "../assets/Cartoes/Cartao-santander.png";
import bradesco from "../assets/Cartoes/Cartao-Bradesco.png";
import inter from "../assets/Cartoes/Cartao-Inter.png";
import amazonPrime from "../assets/Cartoes/Cartao-Amazon-Prime.png";
import bancoBrasil from "../assets/Cartoes/Cartao-Banco-Brasil.png";
import caixa from "../assets/Cartoes/Cartao-Caixa.png";

const CARTOES_DISPONIVEIS = [
  { nome: "Nubank", imagem: "nubank.png", src: nubank },
  { nome: "Santander", imagem: "santander.png", src: santander },
  { nome: "Bradesco", imagem: "bradesco.png", src: bradesco },
  { nome: "Inter", imagem: "inter.png", src: inter },
  { nome: "Amazon Prime", imagem: "amazon-prime.png", src: amazonPrime },
  { nome: "Banco do Brasil", imagem: "banco-do-brasil.png", src: bancoBrasil },
  { nome: "Caixa", imagem: "caixa.png", src: caixa },
];

const ITENS_POR_PAGINA = 4;

export default function Cartao() {
  const [cartoes, setCartoes] = useState<CartaoResponse[]>([]);
  const [carregando, setCarregando] = useState(true);
  const [salvando, setSalvando] = useState(false);

  const [nome, setNome] = useState("");
  const [imagem, setImagem] = useState("");
  const [diaFechamento, setDiaFechamento] = useState("");
  const [diaVencimento, setDiaVencimento] = useState("");

  const [cartaoEditando, setCartaoEditando] = useState<CartaoResponse | null>(null);
  const [cartaoExcluindo, setCartaoExcluindo] = useState<CartaoResponse | null>(null);

  const [modalFormularioAberto, setModalFormularioAberto] = useState(false);
  const [paginaAtual, setPaginaAtual] = useState(1);

  async function carregarCartoes() {
    try {
      setCarregando(true);
      const dados = await consultarCartoes();
      setCartoes(dados);
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.status !== 401) {
        toast.error(error.response?.data?.mensagem || "Não foi possível carregar os cartões.");
      }
    } finally {
      setCarregando(false);
    }
  }

  function obterImagem(imagem: string) {
    return CARTOES_DISPONIVEIS.find((cartao) => cartao.imagem === imagem)?.src;
  }

  useEffect(() => {
    carregarCartoes();
  }, []);

  function limparFormulario() {
    setNome("");
    setImagem("");
    setDiaFechamento("");
    setDiaVencimento("");
    setCartaoEditando(null);
  }

  function abrirCadastro() {
    limparFormulario();
    setModalFormularioAberto(true);
  }

  function abrirEdicao(cartao: CartaoResponse) {
    setCartaoEditando(cartao);
    setNome(cartao.nome);
    setImagem(cartao.imagem);
    setDiaFechamento(String(cartao.diaFechamento));
    setDiaVencimento(String(cartao.diaVencimento));
    setModalFormularioAberto(true);
  }

  function fecharFormulario() {
    setModalFormularioAberto(false);
    limparFormulario();
  }

  async function salvarCartao(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const fechamento = Number(diaFechamento);
    const vencimento = Number(diaVencimento);

    if (!nome.trim()) {
      toast.error("Informe o nome do cartão.");
      return;
    }

    if (!imagem) {
      toast.error("Selecione a imagem do cartão.");
      return;
    }

    if (!Number.isInteger(fechamento) || fechamento < 1 || fechamento > 31) {
      toast.error("O dia de fechamento deve estar entre 1 e 31.");
      return;
    }

    if (!Number.isInteger(vencimento) || vencimento < 1 || vencimento > 31) {
      toast.error("O dia de vencimento deve estar entre 1 e 31.");
      return;
    }

    const dados: CartaoRequest = {
      nome: nome.trim(),
      imagem,
      diaFechamento: fechamento,
      diaVencimento: vencimento,
    };

    try {
      setSalvando(true);

      if (cartaoEditando) {
        await alterarCartao(cartaoEditando.id, dados);
        toast.success("Cartão alterado com sucesso!");
      } else {
        await criarCartao(dados);
        toast.success("Cartão cadastrado com sucesso!");
      }

      fecharFormulario();
      setPaginaAtual(1);
      await carregarCartoes();
    } catch (error) {
      if (axios.isAxiosError(error)) {
        toast.error(error.response?.data?.mensagem || "Não foi possível salvar o cartão.");
      } else {
        toast.error("Ocorreu um erro ao salvar o cartão.");
      }
    } finally {
      setSalvando(false);
    }
  }

  async function confirmarExclusao() {
    if (!cartaoExcluindo) return;

    try {
      await deletarCartao(cartaoExcluindo.id);
      toast.success("Cartão excluído com sucesso!");
      setCartaoExcluindo(null);

      const novaLista = cartoes.filter((cartao) => cartao.id !== cartaoExcluindo.id);

      setCartoes(novaLista);

      const totalPaginas = Math.max(1, Math.ceil(novaLista.length / ITENS_POR_PAGINA));

      if (paginaAtual > totalPaginas) {
        setPaginaAtual(totalPaginas);
      }
    } catch (error) {
      if (axios.isAxiosError(error)) {
        toast.error(error.response?.data?.mensagem || "Não foi possível excluir o cartão.");
      } else {
        toast.error("Ocorreu um erro ao excluir o cartão.");
      }
    }
  }

  const totalPaginas = Math.max(1, Math.ceil(cartoes.length / ITENS_POR_PAGINA));

  const cartoesPaginados = cartoes.slice((paginaAtual - 1) * ITENS_POR_PAGINA, paginaAtual * ITENS_POR_PAGINA);

  return (
    <Fragment>
      <main className="w-full px-4 py-6">
        <section className="mx-auto w-full max-w-5xl rounded-lg border border-black/5 bg-[#F5F7F6] p-6 shadow-[0_4px_14px_rgba(0,0,0,0.08)]">
          <div className="mb-6 flex flex-col justify-between gap-4 sm:flex-row sm:items-center">
            <div>
              <h1 className="mb-2 text-3xl font-semibold text-[#2F4F4F]">Cartões</h1>
              <p className="text-sm text-gray-600">Cadastre e gerencie seus cartões.</p>
            </div>

            <button
              type="button"
              onClick={abrirCadastro}
              className="rounded-lg bg-[#2F4F4F] px-5 py-3 font-medium text-white transition hover:bg-[#243d3d]"
            >
              + Novo cartão
            </button>
          </div>

          {carregando ? (
            <div className="py-10 text-center text-gray-600">Carregando cartões...</div>
          ) : cartoes.length === 0 ? (
            <div className="rounded-lg border border-dashed border-[#9DB4AB] px-4 py-10 text-center">
              <p className="font-medium text-[#2F4F4F]">Nenhum cartão cadastrado.</p>
              <p className="mt-2 text-sm text-gray-600">Clique em "Novo cartão" para cadastrar o primeiro.</p>
            </div>
          ) : (
            <div className="space-y-3">
              {cartoesPaginados.map((cartao) => (
                <article
                  key={cartao.id}
                  className="flex flex-col gap-4 rounded-lg border border-[#9DB4AB] bg-white p-4 sm:flex-row sm:items-center sm:justify-between"
                >
                  <div className="flex min-w-0 items-center gap-4">
                    <div className="flex h-20 w-32 shrink-0 items-center justify-center overflow-hidden rounded-lg bg-gray-100">
                      <img
                        src={obterImagem(cartao.imagem)}
                        alt={`Cartão ${cartao.nome}`}
                        className="h-full w-full object-contain"
                        onError={(event) => {
                          event.currentTarget.style.display = "none";
                        }}
                      />
                    </div>

                    <div className="min-w-0">
                      <h2 className="wrap-break-word text-lg font-semibold text-[#2F4F4F]">{cartao.nome}</h2>
                      <p className="mt-1 text-sm text-gray-600">Fechamento: dia {cartao.diaFechamento}</p>
                      <p className="text-sm text-gray-600">Vencimento: dia {cartao.diaVencimento}</p>
                    </div>
                  </div>

                  <div className="flex shrink-0 gap-2">
                    <button
                      type="button"
                      onClick={() => abrirEdicao(cartao)}
                      className="rounded-lg border border-[#2F4F4F] px-4 py-2 text-sm font-medium text-[#2F4F4F] transition hover:bg-[#edf2f0]"
                    >
                      Editar
                    </button>

                    <button
                      type="button"
                      onClick={() => setCartaoExcluindo(cartao)}
                      className="rounded-lg border border-red-300 px-4 py-2 text-sm font-medium text-red-600 transition hover:bg-red-50"
                    >
                      Excluir
                    </button>
                  </div>
                </article>
              ))}
            </div>
          )}

          {!carregando && cartoes.length > ITENS_POR_PAGINA && (
            <div className="mt-6 flex items-center justify-center gap-4">
              <button
                type="button"
                disabled={paginaAtual === 1}
                onClick={() => setPaginaAtual((pagina) => pagina - 1)}
                className="rounded-lg border border-[#9DB4AB] px-4 py-2 text-sm disabled:cursor-not-allowed disabled:opacity-40"
              >
                Anterior
              </button>

              <span className="text-sm text-gray-600">
                Página {paginaAtual} de {totalPaginas}
              </span>

              <button
                type="button"
                disabled={paginaAtual === totalPaginas}
                onClick={() => setPaginaAtual((pagina) => pagina + 1)}
                className="rounded-lg border border-[#9DB4AB] px-4 py-2 text-sm disabled:cursor-not-allowed disabled:opacity-40"
              >
                Próxima
              </button>
            </div>
          )}
        </section>
      </main>

      {modalFormularioAberto && (
        <div className="fixed inset-0 z-50 flex items-center justify-center overflow-y-auto bg-black/50 p-4">
          <div className="my-auto w-full max-w-lg rounded-xl bg-white p-6 shadow-xl">
            <div className="mb-6 flex items-center justify-between gap-4">
              <h2 className="text-2xl font-semibold text-[#2F4F4F]">{cartaoEditando ? "Editar cartão" : "Novo cartão"}</h2>

              <button
                type="button"
                onClick={fecharFormulario}
                aria-label="Fechar formulário"
                className="rounded-lg px-3 py-1 text-2xl text-gray-500 hover:bg-gray-100"
              >
                ×
              </button>
            </div>

            <form onSubmit={salvarCartao} className="space-y-4">
              <div>
                <label htmlFor="cartao-nome" className="mb-1 block text-sm font-medium text-gray-700">
                  Nome do cartão
                </label>
                <input
                  id="cartao-nome"
                  type="text"
                  value={nome}
                  onChange={(event) => setNome(event.target.value)}
                  maxLength={100}
                  required
                  placeholder="Ex.: Nubank"
                  className="w-full rounded-lg border border-[#9DB4AB] px-3 py-2 outline-none focus:ring-2 focus:ring-[#2F4F4F]/30"
                />
              </div>

              <div>
                <label htmlFor="cartao-imagem" className="mb-1 block text-sm font-medium text-gray-700">
                  Imagem do cartão
                </label>
                <select
                  id="cartao-imagem"
                  value={imagem}
                  onChange={(event) => {
                    setImagem(event.target.value);

                    const cartaoSelecionado = CARTOES_DISPONIVEIS.find((cartao) => cartao.imagem === event.target.value);

                    if (cartaoSelecionado && !cartaoEditando) {
                      setNome(cartaoSelecionado.nome);
                    }
                  }}
                  required
                  className="w-full rounded-lg border border-[#9DB4AB] bg-white px-3 py-2 outline-none focus:ring-2 focus:ring-[#2F4F4F]/30"
                >
                  <option value="">Selecione uma imagem</option>
                  {CARTOES_DISPONIVEIS.map((cartao) => (
                    <option key={cartao.imagem} value={cartao.imagem}>
                      {cartao.nome}
                    </option>
                  ))}
                </select>

                {imagem && (
                  <div className="mt-3 flex h-28 items-center justify-center rounded-lg bg-gray-100 p-3">
                    <img
                      src={obterImagem(imagem)}
                      alt="Prévia do cartão"
                      className="h-full max-w-full object-contain"
                      onError={(event) => {
                        event.currentTarget.style.display = "none";
                      }}
                    />
                  </div>
                )}
              </div>

              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <div>
                  <label htmlFor="cartao-fechamento" className="mb-1 block text-sm font-medium text-gray-700">
                    Dia de fechamento
                  </label>
                  <input
                    id="cartao-fechamento"
                    type="number"
                    min={1}
                    max={31}
                    value={diaFechamento}
                    onChange={(event) => setDiaFechamento(event.target.value)}
                    required
                    placeholder="Ex.: 20"
                    className="w-full rounded-lg border border-[#9DB4AB] px-3 py-2 outline-none focus:ring-2 focus:ring-[#2F4F4F]/30"
                  />
                </div>

                <div>
                  <label htmlFor="cartao-vencimento" className="mb-1 block text-sm font-medium text-gray-700">
                    Dia de vencimento
                  </label>
                  <input
                    id="cartao-vencimento"
                    type="number"
                    min={1}
                    max={31}
                    value={diaVencimento}
                    onChange={(event) => setDiaVencimento(event.target.value)}
                    required
                    placeholder="Ex.: 27"
                    className="w-full rounded-lg border border-[#9DB4AB] px-3 py-2 outline-none focus:ring-2 focus:ring-[#2F4F4F]/30"
                  />
                </div>
              </div>

              <div className="flex flex-col-reverse gap-3 pt-3 sm:flex-row sm:justify-end">
                <button
                  type="button"
                  onClick={fecharFormulario}
                  disabled={salvando}
                  className="rounded-lg border border-gray-300 px-5 py-2.5 font-medium text-gray-700 transition hover:bg-gray-100 disabled:opacity-50"
                >
                  Cancelar
                </button>

                <button
                  type="submit"
                  disabled={salvando}
                  className="rounded-lg bg-[#2F4F4F] px-5 py-2.5 font-medium text-white transition hover:bg-[#243d3d] disabled:cursor-not-allowed disabled:opacity-50"
                >
                  {salvando ? "Salvando..." : cartaoEditando ? "Salvar alterações" : "Cadastrar cartão"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {cartaoExcluindo && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
          <div className="w-full max-w-md rounded-xl bg-white p-6 shadow-xl">
            <h2 className="text-xl font-semibold text-[#2F4F4F]">Excluir cartão</h2>

            <p className="mt-3 text-gray-600">
              Tem certeza de que deseja excluir o cartão <strong>{cartaoExcluindo.nome}</strong>?
            </p>

            <div className="mt-6 flex flex-col-reverse gap-3 sm:flex-row sm:justify-end">
              <button
                type="button"
                onClick={() => setCartaoExcluindo(null)}
                className="rounded-lg border border-gray-300 px-5 py-2.5 font-medium text-gray-700 transition hover:bg-gray-100"
              >
                Cancelar
              </button>

              <button
                type="button"
                onClick={confirmarExclusao}
                className="rounded-lg bg-red-600 px-5 py-2.5 font-medium text-white transition hover:bg-red-700"
              >
                Confirmar exclusão
              </button>
            </div>
          </div>
        </div>
      )}
    </Fragment>
  );
}
