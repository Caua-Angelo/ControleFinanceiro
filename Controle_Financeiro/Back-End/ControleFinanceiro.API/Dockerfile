FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["ControleFinanceiro.API/ControleFinanceiro.API.csproj", "ControleFinanceiro.API/"]
COPY ["ControleFinanceiro.Application/ControleFinanceiro.Application.csproj", "ControleFinanceiro.Application/"]
COPY ["ControleFinanceiro.Data/ControleFinanceiro.Infra.Data.csproj", "ControleFinanceiro.Data/"]
COPY ["ControleFinanceiro.Domain/ControleFinanceiro.Domain.csproj", "ControleFinanceiro.Domain/"]

RUN dotnet restore "ControleFinanceiro.API/ControleFinanceiro.API.csproj"

COPY . .

WORKDIR "/src/ControleFinanceiro.API"
RUN dotnet build "ControleFinanceiro.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ControleFinanceiro.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "ControleFinanceiro.API.dll"]