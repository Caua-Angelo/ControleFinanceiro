namespace ControleFinanceiro.Application.Common
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<CampoErro> Erros { get; set; } = new();

        public static ApiResponse<T> Sucesso(string mensagem, T data, int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Mensagem = mensagem,
                Data = data
            };
        }
        public static ApiResponse<T> Criado(string mensagem, T data, int statusCode = 201)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Mensagem = mensagem,
                Data = data
            };
        }
        public static ApiResponse<T> Erro(string mensagem, T data = default!, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Mensagem = mensagem,
                Data = data
            };
        }

        public static ApiResponse<T> FalhaCampo(string campo, string mensagemErro, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Mensagem = "Um ou mais campos estão ausentes ou inválidos.",
                Erros = new List<CampoErro>
                {
                    new CampoErro
                    {
                        Campo = campo,
                        Mensagens = new List<string> { mensagemErro }
                    }
                }
            };
        }
    }

    public class CampoErro
    {
        public string Campo { get; set; } = string.Empty;
        public List<string> Mensagens { get; set; } = new();
    }
}
