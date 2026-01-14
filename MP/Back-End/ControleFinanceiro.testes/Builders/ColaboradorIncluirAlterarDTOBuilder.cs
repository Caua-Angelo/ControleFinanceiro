using ControleFerias.Application.DTO;

namespace ControleFerias.testes.Builders
{
    public class ColaboradorIncluirAlterarDTOBuilder
    {
        private string _nome = "Fulano Teste";
        private int _equipeId = 1;

        public static ColaboradorIncluirAlterarDTOBuilder Novo() => new ColaboradorIncluirAlterarDTOBuilder();

        public ColaboradorIncluirAlterarDTOBuilder ComNome(string nome)
        {
            _nome = nome;
            return this;
        }

        public ColaboradorIncluirAlterarDTOBuilder ComEquipeId(int equipeId)
        {
            _equipeId = equipeId;
            return this;
        }

        public ColaboradorIncluirDTO Build() =>
            new ColaboradorIncluirDTO
            {
                sNome = _nome,
                EquipeId = _equipeId
            };
    }
}
