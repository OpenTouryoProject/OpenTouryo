FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 53017
EXPOSE 44399

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["OnDocker/OnDocker.csproj", "OnDocker/"]
RUN dotnet restore "OnDocker/OnDocker.csproj"
COPY . .
WORKDIR "/src/OnDocker"
RUN dotnet build "OnDocker.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "OnDocker.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OnDocker.dll"]