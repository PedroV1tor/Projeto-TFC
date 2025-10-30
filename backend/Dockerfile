# Etapa base para runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Railway injeta a porta em $PORT. Bind dinâmico para escutar corretamente.
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
# Opcional: porta local padrão se PORT não existir
ENV PORT=8080
EXPOSE 8080
#testeup
# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copia o csproj e restaura dependências
COPY ["InovalabAPI.csproj", "/src/"]
RUN dotnet restore "/src/InovalabAPI.csproj"
# Copia o restante do código
COPY . /src
RUN dotnet build "/src/InovalabAPI.csproj" -c Release -o /app/build

# Etapa de publicação
FROM build AS publish
RUN dotnet publish "/src/InovalabAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagem final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InovalabAPI.dll"]