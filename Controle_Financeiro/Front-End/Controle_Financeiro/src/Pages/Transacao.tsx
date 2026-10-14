import { Fragment, useEffect, useState } from "react";

import toast from "react-hot-toast";

import dayjs from "dayjs";

import { criarTransacao, listarTransacoes, deletarTransacao, alterarTransacao, type TransacaoRequest } from "../Services/TransacaoService";

import {
  listarComprasParceladas,
  criarCompraParcelada,
  atualizarCompraParcelada,
  deletarCompraParcelada,
  marcarParcelaComoPaga,
  reabrirParcela,
} from "../Services/CompraParceladaService";

import { consultarCategoria } from "../Services/CategoriaService";

import { consultarCartoes, type CartaoResponse } from "../Services/CartaoService";

import { getUsuarioLogado, type UsuarioLogadoResponse } from "../Services/UsuarioService";

import type { CategoriaResponse } from "../Types/CategoriaResponse";

import type { TransacaoResponse } from "../Types/TransacaoResponse";

import type { CompraParceladaResponse } from "../Types/CompraParceladaResponse";

import { Button } from "../Components/Button";

import filtro from "../assets/filtro.png";

type TransacaoComCartao = TransacaoResponse & {
  cartaoId?: number | null;

  cartao?: { id: number; nome: string } | null;
};

type TipoLancamento = "todos" | "normais" | "parceladas";

export default function Transacao() {
  const [descricao, setDescricao] = useState("");

  const [valor, setValor] = useState("");

  const [tipo, setTipo] = useState<number | "">("");

  const [categoriaId, setCategoriaId] = useState<number | "">("");

  const [data, setData] = useState("");

  const [cartaoId, setCartaoId] = useState<number | "">("");

  const [quantidadeParcelas, setQuantidadeParcelas] = useState("1");

  const [transacoes, setTransacoes] = useState<TransacaoResponse[]>([]);

  const [comprasParceladas, setComprasParceladas] = useState<CompraParceladaResponse[]>([]);

  const [categorias, setCategorias] = useState<CategoriaResponse[]>([]);

  const [cartoes, setCartoes] = useState<CartaoResponse[]>([]);

  const [usuario, setUsuario] = useState<UsuarioLogadoResponse | null>(null);

  const [filtroDescricao, setFiltroDescricao] = useState("");

  const [filtroTipo, setFiltroTipo] = useState<number | "">("");

  const [filtroCategoriaId, setFiltroCategoriaId] = useState<number | "">("");

  const [tipoLancamento, setTipoLancamento] = useState<TipoLancamento>("todos");

  const [comprasExpandidas, setComprasExpandidas] = useState<number[]>([]);

  const [parcelasProcessando, setParcelasProcessando] = useState<number[]>([]);

  const [showModal, setShowModal] = useState(false);

  const [transacaoSelecionada, setTransacaoSelecionada] = useState<TransacaoResponse | null>(null);
  const [compraParceladaSelecionada, setCompraParceladaSelecionada] = useState<CompraParceladaResponse | null>(null);

  const [descricaoEdit, setDescricaoEdit] = useState("");

  const [valorEdit, setValorEdit] = useState("");

  const [tipoEdit, setTipoEdit] = useState<number | "">("");

  const [categoriaIdEdit, setCategoriaIdEdit] = useState<number | "">("");

  const [dataEdit, setDataEdit] = useState("");

  const [cartaoIdEdit, setCartaoIdEdit] = useState<number | "">("");
  const [quantidadeParcelasEdit, setQuantidadeParcelasEdit] = useState("2");

  const [loadingCriar, setLoadingCriar] = useState(false);

  const [loadingEditar, setLoadingEditar] = useState(false);

  const [deletingId, setDeletingId] = useState<number | null>(null);

  const [deletingCompraId, setDeletingCompraId] = useState<number | null>(null);

  const isMenorDeIdade = usuario ? usuario.idade < 18 : false;

  const ehParcelada = quantidadeParcelas !== "1";

  const classeCampo =
    "w-full rounded-xl border border-slate-200 bg-white px-3.5 py-3 text-sm text-slate-800 shadow-sm outline-none transition placeholder:text-slate-400 hover:border-slate-300 focus:border-[#52796F] focus:ring-4 focus:ring-[#52796F]/15 disabled:cursor-not-allowed disabled:bg-slate-100 disabled:text-slate-500";

  async function carregarDados() {
    try {
      const [transacoesData, comprasData, categoriasData, usuarioData, cartoesData] = await Promise.all([
        listarTransacoes(),

        listarComprasParceladas(),

        consultarCategoria(),

        getUsuarioLogado(),

        consultarCartoes(),
      ]);

      setTransacoes(transacoesData);

      setComprasParceladas(comprasData);

      setCategorias(categoriasData);

      setUsuario(usuarioData);

      setCartoes(cartoesData);
    } catch (error) {
      console.error(error);

      toast.error("Erro ao carregar os dados financeiros.");
    }
  }

  useEffect(() => {
    carregarDados();
  }, []);

  const categoriasFiltradas = (Array.isArray(categorias) ? categorias : []).filter((categoria) => {
    if (tipo === "") return false;

    if (tipo === 1) {
      return categoria.finalidade === 2 || categoria.finalidade === 3;
    }

    return categoria.finalidade === 1 || categoria.finalidade === 3;
  });

  const categoriasFiltradasEdit = (Array.isArray(categorias) ? categorias : []).filter((categoria) => {
    if (tipoEdit === "") return false;

    if (tipoEdit === 1) {
      return categoria.finalidade === 2 || categoria.finalidade === 3;
    }

    return categoria.finalidade === 1 || categoria.finalidade === 3;
  });

  async function listarTodasTransacoes() {
    const dados = await listarTransacoes();

    setTransacoes(dados);
  }

  async function listarTodasComprasParceladas() {
    const dados = await listarComprasParceladas();

    setComprasParceladas(dados);
  }

  async function CriarTransacao() {
    if (!descricao.trim() || valor === "" || tipo === "" || !data) {
      toast.error("Preencha todos os campos obrigatórios.");

      return;
    }

    if (!usuario?.id) {
      toast.error("Usuário não encontrado.");

      return;
    }

    const valorNumerico = Number(valor.replace(",", "."));

    if (!Number.isFinite(valorNumerico) || valorNumerico <= 0) {
      toast.error("Informe um valor válido.");

      return;
    }

    if (Number(tipo) !== 2) {
      if (ehParcelada) {
        toast.error("Receitas não podem ser cadastradas como compras parceladas.");

        return;
      }

      if (categoriaId === "") {
        toast.error("Selecione uma categoria.");

        return;
      }
    }

    if (Number(tipo) === 2 && cartaoId === "") {
      toast.error("Selecione um cartão para registrar a despesa.");

      return;
    }

    try {
      setLoadingCriar(true);

      const dataFormatada = dayjs(data).format("YYYY-MM-DD");

      if (ehParcelada) {
        await criarCompraParcelada({
          descricao: descricao.trim(),

          valorTotal: valorNumerico,

          quantidadeParcelas: Number(quantidadeParcelas),

          dataCompra: dataFormatada,

          cartaoId: Number(cartaoId),
        });

        await listarTodasComprasParceladas();

        toast.success("Compra parcelada cadastrada com sucesso!");
      } else {
        if (categoriaId === "") {
          toast.error("Selecione uma categoria.");

          return;
        }

        const payload: TransacaoRequest = {
          descricao: descricao.trim(),

          valor: valorNumerico,

          tipo: Number(tipo),

          categoriaId: Number(categoriaId),

          data: dataFormatada,

          usuarioId: usuario.id,

          cartaoId: Number(tipo) === 2 && cartaoId !== "" ? Number(cartaoId) : null,
        };

        await criarTransacao(payload);

        await listarTodasTransacoes();

        toast.success("Transação cadastrada com sucesso!");
      }

      setDescricao("");

      setValor("");

      setTipo("");

      setCategoriaId("");

      setData("");

      setCartaoId("");

      setQuantidadeParcelas("1");
    } catch (error) {
      console.error(error);

      toast.error("Não foi possível cadastrar o lançamento.");
    } finally {
      setLoadingCriar(false);
    }
  }

  async function DeletarTransacao(id: number) {
    if (!window.confirm("Tem certeza que deseja excluir esta transação?")) return;

    try {
      setDeletingId(id);

      await deletarTransacao(id);

      await listarTodasTransacoes();

      toast.success("Transação excluída com sucesso!");
    } catch (error) {
      console.error(error);

      toast.error("Erro ao excluir transação.");
    } finally {
      setDeletingId(null);
    }
  }

  async function excluirCompraParcelada(compra: CompraParceladaResponse) {
    if (!window.confirm(`Deseja excluir a compra "${compra.descricao}" e todas as suas parcelas?`)) {
      return;
    }

    try {
      setDeletingCompraId(compra.id);

      await deletarCompraParcelada(compra.id);

      await listarTodasComprasParceladas();

      toast.success("Compra parcelada excluída com sucesso!");
    } catch (error) {
      console.error(error);

      toast.error("Erro ao excluir compra parcelada.");
    } finally {
      setDeletingCompraId(null);
    }
  }

  async function alternarPagamento(parcelaId: number, paga: boolean) {
    try {
      setParcelasProcessando((prev) => [...prev, parcelaId]);

      if (paga) {
        await reabrirParcela(parcelaId);
      } else {
        await marcarParcelaComoPaga(parcelaId);
      }

      await listarTodasComprasParceladas();

      toast.success(paga ? "Parcela reaberta com sucesso!" : "Parcela marcada como paga!");
    } catch (error) {
      console.error(error);

      toast.error("Não foi possível atualizar a parcela.");
    } finally {
      setParcelasProcessando((prev) => prev.filter((id) => id !== parcelaId));
    }
  }

  function alternarExpansao(compraId: number) {
    setComprasExpandidas((prev) => (prev.includes(compraId) ? prev.filter((id) => id !== compraId) : [...prev, compraId]));
  }

  function abrirModalEditar(transacao: TransacaoResponse) {
    const transacaoComCartao = transacao as TransacaoComCartao;

    setTransacaoSelecionada(transacao);
    setCompraParceladaSelecionada(null);
    setDescricaoEdit(transacao.descricao);
    setValorEdit(transacao.valor.toFixed(2).replace(".", ","));
    setTipoEdit(transacao.tipo);
    setCategoriaIdEdit(transacao.categoriaId);
    setDataEdit(transacao.data.split("T")[0]);
    setCartaoIdEdit(transacaoComCartao.cartaoId ?? "");
    setShowModal(true);
  }

  function abrirModalEditarCompra(compra: CompraParceladaResponse) {
    setTransacaoSelecionada(null);
    setCompraParceladaSelecionada(compra);
    setDescricaoEdit(compra.descricao);
    setValorEdit(compra.valorTotal.toFixed(2).replace(".", ","));
    setDataEdit(compra.dataCompra.split("T")[0]);
    setCartaoIdEdit(compra.cartaoId);
    setQuantidadeParcelasEdit(String(compra.quantidadeParcelas));
    setShowModal(true);
  }

  async function editarTransacao() {
    if (!descricaoEdit.trim() || valorEdit === "" || !dataEdit) {
      toast.error("Preencha todos os campos obrigatórios.");
      return;
    }

    const valorNumerico = Number(valorEdit.replace(",", "."));

    if (!Number.isFinite(valorNumerico) || valorNumerico <= 0) {
      toast.error("Informe um valor válido.");
      return;
    }

    if (compraParceladaSelecionada) {
      if (cartaoIdEdit === "") {
        toast.error("Selecione um cartão.");
        return;
      }

      if (Number(quantidadeParcelasEdit) < 2 || Number(quantidadeParcelasEdit) > 60) {
        toast.error("A quantidade deve estar entre 2 e 60 parcelas.");
        return;
      }

      if (compraParceladaSelecionada.parcelas.some((parcela) => parcela.paga)) {
        toast.error("Não é possível editar uma compra com parcelas pagas.");
        return;
      }

      try {
        setLoadingEditar(true);
        await atualizarCompraParcelada(compraParceladaSelecionada.id, {
          descricao: descricaoEdit.trim(),
          valorTotal: valorNumerico,
          quantidadeParcelas: Number(quantidadeParcelasEdit),
          dataCompra: dayjs(dataEdit).format("YYYY-MM-DD"),
          cartaoId: Number(cartaoIdEdit),
        });
        await listarTodasComprasParceladas();
        toast.success("Compra parcelada atualizada com sucesso!");
        setShowModal(false);
        setCompraParceladaSelecionada(null);
      } catch (error) {
        console.error(error);
        toast.error("Erro ao atualizar compra parcelada.");
      } finally {
        setLoadingEditar(false);
      }
      return;
    }

    if (!transacaoSelecionada || !usuario?.id) {
      toast.error("Usuário ou transação não encontrados.");
      return;
    }

    if (tipoEdit === "" || categoriaIdEdit === "") {
      toast.error("Preencha o tipo e a categoria.");
      return;
    }

    if (Number(tipoEdit) !== 2 && cartaoIdEdit !== "") {
      toast.error("Cartão de crédito só pode ser associado a despesas.");
      return;
    }

    try {
      setLoadingEditar(true);

      const payload: TransacaoRequest = {
        descricao: descricaoEdit.trim(),
        valor: valorNumerico,
        tipo: Number(tipoEdit),
        categoriaId: Number(categoriaIdEdit),
        data: dayjs(dataEdit).format("YYYY-MM-DD"),
        usuarioId: usuario.id,
        cartaoId: Number(tipoEdit) === 2 && cartaoIdEdit !== "" ? Number(cartaoIdEdit) : null,
      };

      await alterarTransacao(transacaoSelecionada.id, payload);
      await listarTodasTransacoes();

      toast.success("Transação atualizada com sucesso!");
      setShowModal(false);
      setTransacaoSelecionada(null);
    } catch (error) {
      console.error(error);
      toast.error("Erro ao atualizar transação.");
    } finally {
      setLoadingEditar(false);
    }
  }

  function formatarValor(valor: number, tipo: number) {
    const sinal = tipo === 1 ? "+ " : "- ";

    return (
      sinal +
      valor.toLocaleString("pt-BR", {
        style: "currency",

        currency: "BRL",
      })
    );
  }

  function formatarMoeda(valor: number) {
    return valor.toLocaleString("pt-BR", {
      style: "currency",

      currency: "BRL",
    });
  }

  function formatarData(dataISO: string) {
    const [ano, mes, dia] = dataISO.split("T")[0].split("-");

    return `${dia}/${mes}/${ano}`;
  }

  function obterNomeCartao(transacao: TransacaoResponse) {
    const transacaoComCartao = transacao as TransacaoComCartao;

    if (transacaoComCartao.cartao?.nome) {
      return transacaoComCartao.cartao.nome;
    }

    const id = transacaoComCartao.cartaoId;

    if (id == null) return "Nenhum";

    return cartoes.find((cartao) => cartao.id === id)?.nome ?? "Cartão";
  }

  const transacoesFiltradas = transacoes.filter((t) => {
    if (tipoLancamento === "parceladas") return false;

    if (filtroDescricao && !t.descricao.toLowerCase().includes(filtroDescricao.toLowerCase())) {
      return false;
    }

    if (filtroTipo !== "" && t.tipo !== Number(filtroTipo)) return false;

    if (filtroCategoriaId !== "" && t.categoriaId !== Number(filtroCategoriaId)) {
      return false;
    }

    return true;
  });

  const comprasParceladasFiltradas = comprasParceladas.filter((compra) => {
    if (tipoLancamento === "normais") return false;

    if (filtroDescricao && !compra.descricao.toLowerCase().includes(filtroDescricao.toLowerCase())) {
      return false;
    }

    if (filtroTipo !== "" && Number(filtroTipo) !== 2) return false;

    // O DTO atual de compra parcelada não contém CategoriaId.

    // Por isso, não é possível filtrar compras parceladas por categoria ainda.

    return true;
  });

  const totalParcelasPagas = comprasParceladas.reduce((total, compra) => total + compra.parcelas.filter((parcela) => parcela.paga).length, 0);

  const totalParcelas = comprasParceladas.reduce((total, compra) => total + compra.parcelas.length, 0);

  return (
    <div className="mx-auto w-full max-w-7xl space-y-8 px-3 pb-10 sm:px-5 lg:px-8">
      {/* FORMULÁRIO ÚNICO */}

      <section className="mx-auto w-full rounded-2xl border border-slate-200/80 bg-white p-5 shadow-sm sm:p-7 lg:p-8">
        <div className="mb-7 flex flex-col gap-2 border-b border-slate-100 pb-5 sm:flex-row sm:items-end sm:justify-between">
          <div>
            <p className="mb-1 text-xs font-bold uppercase tracking-[0.16em] text-[#648879]">Lançamentos</p>
            <h2 className="text-2xl font-bold tracking-tight text-slate-800 sm:text-3xl">Adicionar transação</h2>
            <p className="mt-1 text-sm text-slate-500">Registre receitas, despesas e compras parceladas.</p>
          </div>
          <span className="w-fit rounded-full bg-[#E8EFED] px-3 py-1.5 text-xs font-semibold text-[#2F4F4F]">Novo lançamento</span>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label htmlFor="descricao" className="mb-2 block text-sm font-semibold text-slate-700">
              Descrição
            </label>

            <input id="descricao" className={classeCampo} value={descricao} onChange={(e) => setDescricao(e.target.value)} />
          </div>

          <div>
            <label htmlFor="valor" className="mb-2 block text-sm font-semibold text-slate-700">
              Valor total
            </label>

            <input
              id="valor"
              inputMode="decimal"
              className={classeCampo}
              value={valor}
              onChange={(e) => setValor(e.target.value)}
              placeholder="Ex.: 1200,00"
            />
          </div>

          <div>
            <label htmlFor="data" className="mb-2 block text-sm font-semibold text-slate-700">
              Data da compra/transação
            </label>

            <input id="data" type="date" className={classeCampo} value={data} onChange={(e) => setData(e.target.value)} />
          </div>

          <div>
            <label htmlFor="tipo" className="mb-2 block text-sm font-semibold text-slate-700">
              Tipo
            </label>

            <select
              id="tipo"
              className={classeCampo}
              value={tipo}
              onChange={(e) => {
                const novoTipo = e.target.value === "" ? "" : Number(e.target.value);

                setTipo(novoTipo);

                setCategoriaId("");

                if (novoTipo !== 2) {
                  setCartaoId("");

                  setQuantidadeParcelas("1");
                }
              }}
            >
              <option value="">Selecione</option>

              {!isMenorDeIdade && <option value={1}>Receita</option>}

              <option value={2}>Despesa</option>
            </select>
          </div>

          {tipo === 2 && (
            <div>
              <label htmlFor="parcelas" className="mb-2 block text-sm font-semibold text-slate-700">
                Parcelas
              </label>

              <select id="parcelas" className={classeCampo} value={quantidadeParcelas} onChange={(e) => setQuantidadeParcelas(e.target.value)}>
                <option value="1">À vista</option>

                {Array.from({ length: 19 }, (_, i) => i + 2).map((qtd) => (
                  <option key={qtd} value={qtd}>
                    {qtd}x
                  </option>
                ))}
              </select>
            </div>
          )}

          {tipo !== 2 && (
            <div>
              <label htmlFor="categoria" className="mb-2 block text-sm font-semibold text-slate-700">
                Categoria
              </label>

              <select
                id="categoria"
                className={classeCampo}
                value={categoriaId}
                onChange={(e) => setCategoriaId(e.target.value === "" ? "" : Number(e.target.value))}
              >
                <option value="">Selecione</option>

                {categoriasFiltradas.map((categoria) => (
                  <option key={categoria.id} value={categoria.id}>
                    {categoria.descricao}
                  </option>
                ))}
              </select>
            </div>
          )}

          {tipo === 2 && (
            <>
              <div>
                <label htmlFor="categoria" className="mb-2 block text-sm font-semibold text-slate-700">
                  Categoria
                </label>

                <select
                  id="categoria"
                  className={classeCampo}
                  value={categoriaId}
                  onChange={(e) => setCategoriaId(e.target.value === "" ? "" : Number(e.target.value))}
                  disabled={ehParcelada}
                >
                  <option value="">{ehParcelada ? "Categoria será habilitada após ajuste no back-end" : "Selecione"}</option>

                  {categoriasFiltradas.map((categoria) => (
                    <option key={categoria.id} value={categoria.id}>
                      {categoria.descricao}
                    </option>
                  ))}
                </select>

                {ehParcelada && (
                  <p className="text-xs text-gray-500 mt-1">
                    Para persistir a categoria em compras parceladas, precisamos adicioná-la ao DTO e à entidade no back-end.
                  </p>
                )}
              </div>

              <div>
                <label htmlFor="cartao" className="mb-2 block text-sm font-semibold text-slate-700">
                  Cartão de crédito
                </label>

                <select
                  id="cartao"
                  className={classeCampo}
                  value={cartaoId}
                  onChange={(e) => setCartaoId(e.target.value === "" ? "" : Number(e.target.value))}
                >
                  <option value="">Selecione o cartão</option>

                  {cartoes.map((cartao) => (
                    <option key={cartao.id} value={cartao.id}>
                      {cartao.nome}
                    </option>
                  ))}
                </select>
              </div>
            </>
          )}

          {ehParcelada && tipo === 2 && valor !== "" && Number(valor.replace(",", ".")) > 0 && (
            <div className="md:col-span-2 rounded-2xl border border-[#C8D6D1] bg-linear-to-br from-[#F5F9F7] to-white p-5">
              <p className="font-semibold text-slate-800">Resumo do parcelamento</p>

              <div className="mt-3 grid grid-cols-1 gap-3 text-sm sm:grid-cols-2">
                <span className="rounded-xl bg-white px-3 py-3 text-slate-600 shadow-sm">
                  Valor total<strong className="mt-1 block text-lg text-slate-900">{formatarMoeda(Number(valor.replace(",", ".")))}</strong>
                </span>

                <span className="rounded-xl bg-white px-3 py-3 text-slate-600 shadow-sm">
                  Valor aproximado por parcela
                  <strong className="mt-1 block text-lg text-slate-900">
                    {formatarMoeda(Number(valor.replace(",", ".")) / Number(quantidadeParcelas))}
                  </strong>
                </span>
              </div>

              <p className="text-xs text-gray-500 mt-2">
                O valor exato de cada parcela será calculado pelo back-end, com os centavos distribuídos corretamente.
              </p>
            </div>
          )}

          <div className="md:col-span-2 border-t border-slate-100 pt-5">
            <Button
              onClick={CriarTransacao}
              label={loadingCriar ? "Salvando..." : ehParcelada && tipo === 2 ? "Criar Compra Parcelada" : "Criar Transação"}
              variant="primary"
              disabled={loadingCriar}
            />
          </div>
        </div>
      </section>

      {/* LISTAGEM UNIFICADA */}

      <section className="mx-auto w-full rounded-2xl border border-slate-200/80 bg-white p-5 shadow-sm sm:p-7 lg:p-8">
        <div className="mb-6 flex flex-col gap-2 border-b border-slate-100 pb-5 sm:flex-row sm:items-end sm:justify-between">
          <div>
            <p className="mb-1 text-xs font-bold uppercase tracking-[0.16em] text-[#648879]">Visão geral</p>
            <h2 className="text-2xl font-bold tracking-tight text-slate-800 sm:text-3xl">Suas transações</h2>
            <p className="mt-1 text-sm text-slate-500">Pesquise, filtre e gerencie seus lançamentos.</p>
          </div>
          <span className="w-fit rounded-full bg-slate-100 px-3 py-1.5 text-xs font-semibold text-slate-600">
            {transacoesFiltradas.length + comprasParceladasFiltradas.length} resultado(s)
          </span>
        </div>

        <div className="flex flex-wrap gap-2 mb-5">
          {[
            { value: "todos", label: "Todas" },

            { value: "normais", label: "À vista / Normais" },

            { value: "parceladas", label: "Parceladas" },
          ].map((aba) => (
            <button
              key={aba.value}
              type="button"
              onClick={() => setTipoLancamento(aba.value as TipoLancamento)}
              className={`rounded-xl px-4 py-2.5 text-sm font-semibold transition-all focus-visible:outline-none focus-visible:ring-4 focus-visible:ring-[#52796F]/20 ${
                tipoLancamento === aba.value ? "bg-[#2F4F4F] text-white" : "bg-[#E2E8E6] text-[#2F4F4F] hover:bg-[#D0DFDA]"
              }`}
            >
              {aba.label}
            </button>
          ))}
        </div>

        <div className="mb-4 flex items-center gap-2 text-sm font-bold uppercase tracking-wide text-slate-700">
          <img src={filtro} alt="Filtros" className="w-5 h-5 mr-2" />
          Filtros
        </div>

        <div className="mb-6 grid grid-cols-1 items-end gap-4 rounded-2xl bg-slate-50/80 p-4 sm:grid-cols-2 lg:grid-cols-4">
          <div>
            <label className="mb-1.5 block text-xs font-semibold uppercase tracking-wide text-slate-500">Descrição</label>

            <input
              className={classeCampo}
              value={filtroDescricao}
              onChange={(e) => setFiltroDescricao(e.target.value)}
              placeholder="Buscar por descrição"
            />
          </div>

          <div>
            <label className="mb-1.5 block text-xs font-semibold uppercase tracking-wide text-slate-500">Tipo</label>

            <select className={classeCampo} value={filtroTipo} onChange={(e) => setFiltroTipo(e.target.value === "" ? "" : Number(e.target.value))}>
              <option value="">Todos</option>

              <option value={1}>Receita</option>

              <option value={2}>Despesa</option>
            </select>
          </div>

          <div>
            <label className="mb-1.5 block text-xs font-semibold uppercase tracking-wide text-slate-500">Categoria</label>

            <select
              className={classeCampo}
              value={filtroCategoriaId}
              onChange={(e) => setFiltroCategoriaId(e.target.value === "" ? "" : Number(e.target.value))}
            >
              <option value="">Todas</option>

              {categorias.map((categoria) => (
                <option key={categoria.id} value={categoria.id}>
                  {categoria.descricao}
                </option>
              ))}
            </select>
          </div>

          <button
            type="button"
            className="rounded-xl border border-slate-200 bg-white px-4 py-3 text-sm font-semibold text-slate-600 transition hover:border-slate-300 hover:bg-slate-100 focus-visible:outline-none focus-visible:ring-4 focus-visible:ring-[#52796F]/20"
            onClick={() => {
              setFiltroDescricao("");

              setFiltroTipo("");

              setFiltroCategoriaId("");
            }}
          >
            Limpar filtros
          </button>
        </div>

        {/* TRANSAÇÕES COMUNS */}

        {transacoesFiltradas.map((t, index) => (
          <Fragment key={`transacao-${t.id}`}>
            <div
              className={`hidden md:grid grid-cols-[2fr_1fr_1fr_1fr_1fr_180px] items-center gap-3 rounded-xl border border-slate-200 px-4 py-4 mb-2 transition-colors ${
                index % 2 === 0 ? "bg-white" : "bg-[#F5F7F6]"
              } hover:bg-[#E8EFED]`}
            >
              <div className="font-medium text-[#2F4F4F] wrap-break-word">{t.descricao}</div>

              <div>{formatarValor(t.valor, t.tipo)}</div>

              <div>{formatarData(t.data)}</div>

              <div>{t.tipo === 1 ? "Receita" : "Despesa"}</div>

              <div>{obterNomeCartao(t)}</div>

              <div className="flex justify-start gap-2">
                <Button onClick={() => abrirModalEditar(t)} label="Editar" variant="edit" disabled={loadingEditar} />

                <Button
                  onClick={() => DeletarTransacao(t.id)}
                  label={deletingId === t.id ? "Excluindo..." : "Excluir"}
                  variant="delete"
                  disabled={deletingId === t.id}
                />
              </div>
            </div>

            <div className="mb-3 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm transition-shadow hover:shadow-md md:hidden">
              <div className="font-semibold text-lg text-[#2F4F4F] mb-3">{t.descricao}</div>

              <div className="space-y-2 text-[#2F4F4F]">
                <div className="flex justify-between gap-4">
                  <span>Valor</span>

                  <span>{formatarValor(t.valor, t.tipo)}</span>
                </div>

                <div className="flex justify-between gap-4">
                  <span>Data</span>

                  <span>{formatarData(t.data)}</span>
                </div>

                <div className="flex justify-between gap-4">
                  <span>Tipo</span>

                  <span>{t.tipo === 1 ? "Receita" : "Despesa"}</span>
                </div>

                <div className="flex justify-between gap-4">
                  <span>Cartão</span>

                  <span>{obterNomeCartao(t)}</span>
                </div>
              </div>

              <div className="flex gap-2 mt-4">
                <Button onClick={() => abrirModalEditar(t)} label="Editar" variant="edit" disabled={loadingEditar} />

                <Button
                  onClick={() => DeletarTransacao(t.id)}
                  label={deletingId === t.id ? "Excluindo..." : "Excluir"}
                  variant="delete"
                  disabled={deletingId === t.id}
                />
              </div>
            </div>
          </Fragment>
        ))}

        {/* COMPRAS PARCELADAS */}

        {comprasParceladasFiltradas.map((compra) => {
          const expandida = comprasExpandidas.includes(compra.id);

          const pagas = compra.parcelas.filter((parcela) => parcela.paga).length;

          const valorEmAberto = compra.parcelas.filter((parcela) => !parcela.paga).reduce((total, parcela) => total + parcela.valor, 0);

          const cartao = cartoes.find((c) => c.id === compra.cartaoId);

          return (
            <article
              key={`compra-${compra.id}`}
              className="mb-4 overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm transition-shadow hover:shadow-md"
            >
              <div className="p-5 sm:p-6">
                <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
                  <div className="min-w-0">
                    <div className="flex flex-wrap items-center gap-2">
                      <h3 className="font-semibold text-lg text-[#2F4F4F] wrap-break-word">{compra.descricao}</h3>

                      <span className="rounded-full bg-[#E8EFED] px-3 py-1 text-xs font-semibold text-[#2F4F4F]">Compra parcelada</span>
                    </div>

                    <p className="mt-1 text-sm text-gray-600">
                      Compra em {formatarData(compra.dataCompra)} · {cartao?.nome ?? "Cartão"}
                    </p>
                  </div>

                  <div className="text-left md:text-right">
                    <p className="font-semibold text-lg text-[#2F4F4F]">{formatarMoeda(compra.valorTotal)}</p>

                    <p className="text-sm text-gray-600">
                      {compra.quantidadeParcelas} parcelas de aproximadamente {formatarMoeda(compra.valorTotal / compra.quantidadeParcelas)}
                    </p>
                  </div>
                </div>

                <div className="mt-4 grid grid-cols-1 sm:grid-cols-3 gap-3 text-sm">
                  <div className="rounded-xl border border-slate-100 bg-slate-50 p-4">
                    <p className="text-gray-600">Parcelas pagas</p>

                    <p className="font-semibold text-[#2F4F4F]">
                      {pagas} de {compra.parcelas.length}
                    </p>
                  </div>

                  <div className="rounded-xl border border-slate-100 bg-slate-50 p-4">
                    <p className="text-gray-600">Parcelas em aberto</p>

                    <p className="font-semibold text-[#2F4F4F]">{compra.parcelas.length - pagas}</p>
                  </div>

                  <div className="rounded-xl border border-slate-100 bg-slate-50 p-4">
                    <p className="text-gray-600">Saldo em aberto</p>

                    <p className="font-semibold text-[#2F4F4F]">{formatarMoeda(valorEmAberto)}</p>
                  </div>
                </div>

                <div className="mt-5 flex flex-wrap gap-2 border-t border-slate-100 pt-4">
                  <button
                    type="button"
                    onClick={() => abrirModalEditarCompra(compra)}
                    disabled={loadingEditar || compra.parcelas.some((parcela) => parcela.paga)}
                    className="rounded-lg bg-[#7A9D8F] px-4 py-2 text-sm font-medium text-white hover:bg-[#648879] disabled:opacity-50"
                    title={compra.parcelas.some((parcela) => parcela.paga) ? "Não é possível editar uma compra com parcelas pagas" : "Editar compra"}
                  >
                    Editar compra
                  </button>

                  <button
                    type="button"
                    onClick={() => alternarExpansao(compra.id)}
                    className="rounded-lg bg-[#2F4F4F] px-4 py-2 text-sm font-medium text-white hover:bg-[#243d3d]"
                  >
                    {expandida ? "Ocultar parcelas" : "Ver parcelas"}
                  </button>

                  <button
                    type="button"
                    onClick={() => excluirCompraParcelada(compra)}
                    disabled={deletingCompraId === compra.id}
                    className="rounded-lg bg-[#B36A60] px-4 py-2 text-sm font-medium text-white hover:bg-[#98564D] disabled:opacity-60"
                  >
                    {deletingCompraId === compra.id ? "Excluindo..." : "Excluir compra"}
                  </button>
                </div>
              </div>

              {expandida && (
                <div className="border-t border-slate-200 bg-slate-50/80 p-4 sm:p-5">
                  <h4 className="font-semibold text-[#2F4F4F] mb-3">Parcelas</h4>

                  <div className="space-y-2">
                    {compra.parcelas

                      .slice()

                      .sort((a, b) => a.numero - b.numero)

                      .map((parcela) => (
                        <div
                          key={parcela.id}
                          className="flex flex-col gap-3 rounded-xl border border-slate-200 bg-white p-4 sm:flex-row sm:items-center sm:justify-between"
                        >
                          <div>
                            <p className="font-medium text-[#2F4F4F]">
                              Parcela {parcela.numero}/{compra.quantidadeParcelas}
                            </p>

                            <p className="text-sm text-gray-600">Vencimento: {formatarData(parcela.dataVencimento)}</p>
                          </div>

                          <div className="flex flex-wrap items-center gap-3">
                            <span className="font-semibold text-[#2F4F4F]">{formatarMoeda(parcela.valor)}</span>

                            <span
                              className={`rounded-full px-3 py-1 text-xs font-semibold ${
                                parcela.paga ? "bg-green-100 text-green-800" : "bg-amber-100 text-amber-800"
                              }`}
                            >
                              {parcela.paga ? "Paga" : "Em aberto"}
                            </span>

                            <button
                              type="button"
                              disabled={parcelasProcessando.includes(parcela.id)}
                              onClick={() => alternarPagamento(parcela.id, parcela.paga)}
                              className={`rounded-lg px-3 py-2 text-sm font-medium text-white disabled:opacity-60 ${
                                parcela.paga ? "bg-[#7A9D8F] hover:bg-[#648879]" : "bg-[#2F4F4F] hover:bg-[#243d3d]"
                              }`}
                            >
                              {parcelasProcessando.includes(parcela.id) ? "Salvando..." : parcela.paga ? "Reabrir parcela" : "Marcar como paga"}
                            </button>
                          </div>
                        </div>
                      ))}
                  </div>
                </div>
              )}
            </article>
          );
        })}

        {transacoesFiltradas.length === 0 && comprasParceladasFiltradas.length === 0 && (
          <div className="rounded-2xl border border-dashed border-slate-300 bg-slate-50 px-5 py-12 text-center">
            <div className="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-2xl bg-[#E8EFED] text-[#52796F]">
              <span className="text-xl">⌕</span>
            </div>
            <p className="font-semibold text-slate-700">Nenhum lançamento encontrado</p>
            <p className="mt-1 text-sm text-slate-500">Tente ajustar os filtros ou cadastre uma nova transação.</p>
          </div>
        )}

        {comprasParceladas.length > 0 && tipoLancamento !== "normais" && (
          <p className="mt-4 text-sm text-gray-600">
            Progresso geral das parcelas: {totalParcelasPagas} de {totalParcelas} pagas.
          </p>
        )}
      </section>

      {/* MODAL DE EDIÇÃO */}
      {showModal && (transacaoSelecionada || compraParceladaSelecionada) && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/55 p-3 backdrop-blur-sm sm:p-5">
          <div className="max-h-[92vh] w-full max-w-lg overflow-y-auto rounded-2xl border border-white/60 bg-white p-5 shadow-2xl sm:p-7">
            <h2 className="mb-2 text-2xl font-bold tracking-tight text-slate-800">
              {compraParceladaSelecionada ? "Editar Compra Parcelada" : "Editar Transação"}
            </h2>

            <div className="mb-4">
              <label htmlFor="input-descricao" className="mb-1.5 block text-sm font-semibold text-slate-700">
                Descrição
              </label>
              <input id="input-descricao" className={classeCampo} value={descricaoEdit} onChange={(e) => setDescricaoEdit(e.target.value)} />
            </div>

            <div className="mb-4">
              <label htmlFor="input-valor" className="mb-1.5 block text-sm font-semibold text-slate-700">
                {compraParceladaSelecionada ? "Valor total" : "Valor"}
              </label>
              <input id="input-valor" inputMode="decimal" className={classeCampo} value={valorEdit} onChange={(e) => setValorEdit(e.target.value)} />
            </div>

            <div className="mb-4">
              <label htmlFor="input-data" className="mb-1.5 block text-sm font-semibold text-slate-700">
                {compraParceladaSelecionada ? "Data da compra" : "Data"}
              </label>
              <input id="input-data" type="date" className={classeCampo} value={dataEdit} onChange={(e) => setDataEdit(e.target.value)} />
            </div>

            {compraParceladaSelecionada ? (
              <>
                <div className="mb-4">
                  <label htmlFor="parcelas-edit" className="mb-1.5 block text-sm font-semibold text-slate-700">
                    Quantidade de parcelas
                  </label>
                  <select
                    id="parcelas-edit"
                    className={classeCampo}
                    value={quantidadeParcelasEdit}
                    onChange={(e) => setQuantidadeParcelasEdit(e.target.value)}
                  >
                    {Array.from({ length: 59 }, (_, i) => i + 2).map((qtd) => (
                      <option key={qtd} value={qtd}>
                        {qtd}x
                      </option>
                    ))}
                  </select>
                </div>

                <div className="mb-4">
                  <label htmlFor="cartao-edit" className="mb-1.5 block text-sm font-semibold text-slate-700">
                    Cartão de crédito
                  </label>
                  <select
                    id="cartao-edit"
                    className={classeCampo}
                    value={cartaoIdEdit}
                    onChange={(e) => setCartaoIdEdit(e.target.value === "" ? "" : Number(e.target.value))}
                  >
                    <option value="">Selecione o cartão</option>
                    {cartoes.map((cartao) => (
                      <option key={cartao.id} value={cartao.id}>
                        {cartao.nome}
                      </option>
                    ))}
                  </select>
                </div>
                <p className="mb-4 text-xs text-gray-500">Compras com parcelas pagas não podem ser editadas.</p>
              </>
            ) : (
              <>
                <div className="mb-4">
                  <label htmlFor="tipo-edit" className="mb-1.5 block text-sm font-semibold text-slate-700">
                    Tipo
                  </label>
                  <select
                    id="tipo-edit"
                    className={classeCampo}
                    value={tipoEdit}
                    onChange={(e) => {
                      const novoTipo = e.target.value === "" ? "" : Number(e.target.value);
                      setTipoEdit(novoTipo);
                      setCategoriaIdEdit("");
                      if (novoTipo !== 2) setCartaoIdEdit("");
                    }}
                  >
                    <option value="">Selecione</option>
                    {!isMenorDeIdade && <option value={1}>Receita</option>}
                    <option value={2}>Despesa</option>
                    <option value={3}>Investimento</option>
                  </select>
                </div>

                <div className="mb-4">
                  <label htmlFor="categoria-edit" className="mb-1.5 block text-sm font-semibold text-slate-700">
                    Categoria
                  </label>
                  <select
                    id="categoria-edit"
                    className={classeCampo}
                    value={categoriaIdEdit}
                    onChange={(e) => setCategoriaIdEdit(e.target.value === "" ? "" : Number(e.target.value))}
                  >
                    <option value="">Selecione</option>
                    {categoriasFiltradasEdit.map((categoria) => (
                      <option key={categoria.id} value={categoria.id}>
                        {categoria.descricao}
                      </option>
                    ))}
                  </select>
                </div>

                {tipoEdit === 2 && (
                  <div className="mb-4">
                    <label htmlFor="cartao-edit" className="mb-1.5 block text-sm font-semibold text-slate-700">
                      Cartão de crédito
                    </label>
                    <select
                      id="cartao-edit"
                      className={classeCampo}
                      value={cartaoIdEdit}
                      onChange={(e) => setCartaoIdEdit(e.target.value === "" ? "" : Number(e.target.value))}
                    >
                      <option value="">Nenhum</option>
                      {cartoes.map((cartao) => (
                        <option key={cartao.id} value={cartao.id}>
                          {cartao.nome}
                        </option>
                      ))}
                    </select>
                  </div>
                )}
              </>
            )}

            <div className="flex justify-center gap-2">
              <Button onClick={editarTransacao} label={loadingEditar ? "Salvando..." : "Salvar"} variant="saveModal" disabled={loadingEditar} />
              <Button
                onClick={() => {
                  setShowModal(false);
                  setTransacaoSelecionada(null);
                  setCompraParceladaSelecionada(null);
                }}
                label="Cancelar"
                variant="cancelModal"
              />
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
